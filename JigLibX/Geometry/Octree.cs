#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using JigLibX.Geometry.Primitives;
using Microsoft.Xna.Framework;
using JigLibX.Math;
#endregion

namespace JigLibX.Geometry
{
    /// <summary>
    /// Stores world collision data in an octree structure for quick ray testing
    /// during CVolumeNavRegion processing.
    /// </summary>
    public class Octree
    {

        #region Octree Cell
        /// <summary>
        /// Internally we don't store pointers but store indices into a single contiguous
        /// array of cells and triangles owned by Octree (so that the vectors can get resized).
        ///
        /// Each cell will either contain children OR contain triangles. 
        /// </summary>
        struct Cell
        {
            /// <summary>
            /// endices into the children - P means "plus" and M means "minus" and the
            /// letters are xyz. So PPM means +ve x, +ve y, -ve z
            /// </summary>
            internal enum EChild
            {
                PPP,
                PPM,
                PMP,
                PMM,
                MPP,
                MPM,
                MMP,
                MMM,
                NumChildren
            }

            /// indices of the children (if not leaf). Will be -1 if there is no child
            internal int[] mChildCellIndices;

            /// indices of the triangles (if leaf)
            internal List<int> mTriangleIndices;

            /// Bounding box for the space we own
            internal AABox mAABox;

            /// <summary>
            /// constructor clears everything
            /// </summary>
            /// <param name="aabb"></param>
            public Cell(AABox aabb)
            {
                mAABox = aabb;
                mTriangleIndices = new List<int>();
                mChildCellIndices = new int[NumChildren];

                Clear();
            }

            /// <summary>
            /// Sets all child indices to -1 and clears the triangle indices.
            /// </summary>
            public void Clear()
            {
                for (int i = 0; i < NumChildren; i++)
                    mChildCellIndices[i] = -1;

                mTriangleIndices.Clear();

            }

            /// <summary>
            /// constructor clears everything
            /// </summary>
            public bool IsLeaf
            {
                get { return mChildCellIndices[0] == -1; }
            }


        }
        #endregion

        #region private fields

        private const int NumChildren = (int)Cell.EChild.NumChildren;

        /// All our cells. The only thing guaranteed about this is that m_cell[0] (if
        /// it exists) is the root cell.
        private List<Octree.Cell> cells;

        /// the vertices
        private List<Vector3> vertices;
        /// All our triangles.
        private List<IndexedTriangle> triangles;

        private AABox boundingBox = new AABox();

        /// During intersection testing we keep a stack of cells to test (rather than recursing) - 
        /// to avoid excessive memory allocation we don't free the memory between calls unless
        /// the user calls FreeTemporaryMemory();
        private Stack<int> mCellsToTest;

        /// Counter used to prevent multiple tests when triangles are contained in more than
        /// one cell
        private int testCounter;

        #endregion

        /// <summary>
        /// On creation the extents are defined - if anything is subsequently added
        /// that lies entirely outside this bbox it will not get added.
        /// </summary>
        public Octree()
        {
            cells = new List<Cell>();
            vertices = new List<Vector3>();
            triangles = new List<IndexedTriangle>();
            mCellsToTest = new Stack<int>();
        }

        /// <summary>
        /// Clears triangles and cells. If freeMemory is set to true, the
        /// triangle index array will be freed, otherwise it will be reset
        /// preserving the allocated memory.
        /// </summary>
        /// <param name="freeMemory"></param>
        public void Clear(bool freeMemory)
        {
            cells.Clear();
            vertices.Clear();
            triangles.Clear();
        }

        public AABox BoundingBox
        {
            get { return this.boundingBox; }
        }

        /// <summary>
        /// Add the triangles - doesn't actually build the octree
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="numVertices"></param>
        /// <param name="triangleVertexIndices"></param>
        /// <param name="numTriangles"></param>
        public void AddTriangles(List<Vector3> vertices, List<TriangleVertexIndices> triangleVertexIndices)
        {
            this.vertices.Clear();
            this.triangles.Clear();
            this.cells.Clear();

            int numTriangles = triangleVertexIndices.Count;

            this.vertices = vertices;

            for (int iTriangle = 0; iTriangle < numTriangles; iTriangle++)
            {
                int i0 = triangleVertexIndices[iTriangle].I0;
                int i1 = triangleVertexIndices[iTriangle].I1;
                int i2 = triangleVertexIndices[iTriangle].I2;

                //Assert(i0 < numVertices);
                //Assert(i1 < numVertices);
                //Assert(i2 < numVertices);

                Vector3 dr1 = vertices[i1] - vertices[i0];
                Vector3 dr2 = vertices[i2] - vertices[i0];
                Vector3 N = Vector3.Cross(dr1, dr2);

                float NLen = N.Length();

                // only add if it's not degenerate. Note that this could be a problem it we use connectivity info
                // since we're actually making a hole in the mesh...
                if (NLen > JiggleMath.Epsilon)
                {
                    IndexedTriangle tri = new IndexedTriangle();
                    tri.SetVertexIndices(i0, i1, i2, vertices);

                    triangles.Add(tri);

                    //mTriangles.back().SetVertexIndices(i0, i1, i2, vertices);
                }
            }
        }

        /// <summary>
        /// Builds the octree from scratch (not incrementally) - deleting
        /// any previous tree.  Building the octree will involve placing
        /// all triangles into the root cell.  Then this cell gets pushed
        /// onto a stack of cells to examine. This stack will get parsed
        /// and every cell containing more than maxTrianglesPerCell will
        /// get split into 8 children, and all the original triangles in
        /// that cell will get partitioned between the children. A
        /// triangle can end up in multiple cells (possibly a lot!) if it
        /// straddles a boundary. Therefore when intersection tests are
        /// done tIndexedTriangle::m_counter can be set/tested using a
        /// counter to avoid properly testing the triangle multiple times
        /// (the counter _might_ wrap around, so when it wraps ALL the
        /// triangle flags should be cleared! Could do this
        /// incrementally...).
        /// </summary>
        /// <param name="maxTrianglesPerCell"></param>
        /// <param name="minCellSize"></param>
        public void BuildOctree(int maxTrianglesPerCell, float minCellSize)
        {
            boundingBox.Clear();

            for (int i = 0; i < vertices.Count; i++)
                boundingBox.AddPoint(vertices[i]);

            // clear any existing cells
            cells.Clear();

            // set up the root

            Octree.Cell rootCell = new Octree.Cell(boundingBox);

            cells.Add(rootCell);
            int numTriangles = triangles.Count;

            //rootCell.mTriangleIndices.resize(numTriangles);

            for (int i = 0; i < numTriangles; i++)
                rootCell.mTriangleIndices.Add(i);

            // rather than doing things recursively, use a stack of cells that need
            // to be processed - for each cell if it contains too many triangles we 
            // create child cells and move the triangles down into them (then we
            // clear the parent triangles).
            Stack<int> cellsToProcess = new Stack<int>();
            cellsToProcess.Push(0);

            // bear in mind during this that any time a new cell gets created any pointer
            // or reference to an existing cell may get invalidated - so use indexing.
            while (cellsToProcess.Count != 0)
            {
                int cellIndex = cellsToProcess.Pop();

                if ((cells[cellIndex].mTriangleIndices.Count <= maxTrianglesPerCell) ||
                     (cells[cellIndex].mAABox.GetRadiusAboutCentre() < minCellSize))
                    continue;

                // we need to put these triangles into the children
                for (int iChild = 0; iChild < NumChildren; iChild++)
                {
                    cells[cellIndex].mChildCellIndices[iChild] = cells.Count;
                    cellsToProcess.Push(cells.Count);

                    Octree.Cell childCell = new Octree.Cell(CreateAABox(cells[cellIndex].mAABox, (Octree.Cell.EChild)iChild));

                    cells.Add(childCell);

                    int numTris = cells[cellIndex].mTriangleIndices.Count;

                    for (int i = 0; i < numTris; i++)
                    {
                        int iTri = cells[cellIndex].mTriangleIndices[i];
                        IndexedTriangle tri = triangles[iTri];

                        if (DoesTriangleIntersectCell(tri, childCell))
                        {
                            childCell.mTriangleIndices.Add(iTri);
                        }
                    }
                }

                // the children handle all the triangles now - we no longer need them
                cells[cellIndex].mTriangleIndices.Clear();
            }
        }

        /// <summary>
        /// Create a bounding box appropriate for a child, based on a parents AABox
        /// </summary>
        /// <param name="aabb"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        private AABox CreateAABox(AABox aabb, Octree.Cell.EChild child)
        {
            Vector3 dims = 0.5f * (aabb.MaxPos - aabb.MinPos);
            Vector3 offset= new Vector3();

            switch (child)
            {
                case Octree.Cell.EChild.PPP: offset = new Vector3(1, 1, 1); break;
                case Octree.Cell.EChild.PPM: offset = new Vector3(1, 1, 0); break;
                case Octree.Cell.EChild.PMP: offset = new Vector3(1, 0, 1); break;
                case Octree.Cell.EChild.PMM: offset = new Vector3(1, 0, 0); break;
                case Octree.Cell.EChild.MPP: offset = new Vector3(0, 1, 1); break;
                case Octree.Cell.EChild.MPM: offset = new Vector3(0, 1, 0); break;
                case Octree.Cell.EChild.MMP: offset = new Vector3(0, 0, 1); break;
                case Octree.Cell.EChild.MMM: offset = new Vector3(0, 0, 0); break;

                default:
                    System.Diagnostics.Debug.WriteLine("Octree.CreateAABox  got impossible child");
                    //TRACE("tOctree::CreateAABox Got impossible child: %d", child);
                    //offset.Set(0, 0, 0);
                    break;
            }

            AABox result = new AABox();
            result.MinPos = (aabb.MinPos + new Vector3(offset.X * dims.X, offset.Y * dims.Y, offset.Z * dims.Z));
            result.MaxPos = (result.MinPos + dims);

            // expand it just a tiny bit just to be safe!
            float extra = 0.00001f;

            result.MinPos = (result.MinPos - extra * dims);
            result.MaxPos = (result.MaxPos + extra * dims);

            return result;
        }

        public int GetTrianglesIntersectingtAABox(List<int> triangles, AABox aabb)
        {
            if (cells.Count == 0)
                return 0;

            triangles.Clear();
            mCellsToTest.Clear();
            mCellsToTest.Push(0);

            IncrementTestCounter();

            while (mCellsToTest.Count != 0) // while it is not empty
            {
                int cellIndex = mCellsToTest.Pop();
                //mCellsToTest.pop_back();

                Octree.Cell cell = cells[cellIndex];

                if (!AABox.OverlapTest(aabb, cell.mAABox))
                    continue;

                if (cell.IsLeaf)
                {
                    int nTris = cell.mTriangleIndices.Count;

                    for (int i = 0; i < nTris; i++)
                    {
                        IndexedTriangle triangle = GetTriangle(cell.mTriangleIndices[i]);

                        if (triangle.counter != testCounter)
                        {
                            triangle.counter = testCounter;

                            if (AABox.OverlapTest(aabb, triangle.BoundingBox))
                                triangles.Add(cell.mTriangleIndices[i]);
                        }
                    }
                }
                else
                {
                    // if non-leaf, just add the children to check
                    for (int iChild = 0; iChild < Octree.NumChildren; iChild++)
                    {
                        int childIndex = cell.mChildCellIndices[iChild];
                        mCellsToTest.Push(childIndex);
                    }
                }
            }
            return triangles.Count;
        }

        private bool DoesTriangleIntersectCell(IndexedTriangle triangle, Octree.Cell cell)
        {
            if (!AABox.OverlapTest(triangle.BoundingBox, cell.mAABox))
                return false;

            // quick test
            if (cell.mAABox.IsPointInside(GetVertex(triangle.GetVertexIndex(0))) ||
                cell.mAABox.IsPointInside(GetVertex(triangle.GetVertexIndex(1))) ||
                cell.mAABox.IsPointInside(GetVertex(triangle.GetVertexIndex(2))))
                return true;

            // all points are outside... so if there is intersection it must be due to the
            // box edges and the triangle...
            Triangle tri = new Triangle(GetVertex(triangle.GetVertexIndex(0)), GetVertex(triangle.GetVertexIndex(1)), GetVertex(triangle.GetVertexIndex(2)));

            Box box = new Box(cell.mAABox.MinPos, Matrix.Identity, cell.mAABox.GetSideLengths());
            Vector3[] pts;// = new Vector3[8];

            box.GetCornerPoints(out pts);
            Box.Edge[] edges;
            box.GetEdges(out edges);

            for (int i = 0; i < 12; i++)
            {
                Box.Edge edge = edges[i];

                Segment seg = new Segment(pts[(int)edge.Ind0], pts[(int)edge.Ind1] - pts[(int)edge.Ind0]);

                if (Overlap.SegmentTriangleOverlap(seg, tri))
                    return true;
            }
            // Unless it's the triangle edges and the box
            //Vector3 pos, n;

            // now each edge of the triangle with the box
            for (int iEdge = 0; iEdge < 3; ++iEdge)
            {
                Vector3 pt0 = tri.GetPoint(iEdge);
                Vector3 pt1 = tri.GetPoint((iEdge + 1) % 3);

                if (Overlap.SegmentAABoxOverlap(new Segment(pt0, pt1 - pt0), cell.mAABox))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Increment our test counter, wrapping around if necessary and zapping the 
        /// triangle counters.
        /// </summary>
        private void IncrementTestCounter()
        {
            ++testCounter;

            if (testCounter == 0)
            {
                // wrap around - clear all the triangle counters
                int numTriangles = triangles.Count;

                for (int i = 0; i < numTriangles; ++i)
                    triangles[i].counter = 0;

                testCounter = 1;
            }
        }

        /// <summary>
        /// Get a triangle
        /// </summary>
        /// <param name="iTriangle"></param>
        /// <returns></returns>
        public IndexedTriangle GetTriangle(int iTriangle)
        {
            return triangles[iTriangle];
        }

        /// <summary>
        /// Get a vertex
        /// </summary>
        /// <param name="iVertex"></param>
        /// <returns></returns>
        public Vector3 GetVertex(int iVertex)
        {
            return vertices[iVertex];
        }

        public void GetVertex(int iVertex,out Vector3 result)
        {
            result = vertices[iVertex];
        }

        /// <summary>
        /// Gets the number of triangles
        /// </summary>
        public int NumTriangles
        {
            get { return triangles.Count; }
        }



    }
}
