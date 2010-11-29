#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using JigLibX.Geometry.Primitives;
using Microsoft.Xna.Framework;
#endregion

namespace JigLibX.Geometry
{
    /// <summary>
    /// Support for an indexed triangle - assumes ownership by something that 
    /// has an array of vertices and an array of tIndexedTriangle
    /// </summary>
    public class IndexedTriangle
    {
        // used when traversing to stop us visiting the same triangle twice
        internal int counter;
        /// indices into our owner's array of vertices 
        private int[] vertexIndices;
        private int convexFlags;
        private Plane plane;
        private AABox boundingBox;

        public IndexedTriangle()
        {
            vertexIndices = new int[3];

            vertexIndices[0] = vertexIndices[1] = vertexIndices[2] = -1;

            convexFlags = unchecked((ushort)~0); // TODO check this
            plane = new Plane(new Vector3(0.0f, 0.0f, 0.0f), 0.0f);

            boundingBox = new AABox();
        }

        /// <summary>
        /// Set the indices into the relevant vertex array for this
        /// triangle. Also sets the plane and bounding box
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="vertexArray"></param>
        public void SetVertexIndices(int i0, int i1, int i2, List<Vector3> vertexArray)
        {
            vertexIndices[0] = i0;
            vertexIndices[1] = i1;
            vertexIndices[2] = i2;

            plane = new Plane( vertexArray[i0], vertexArray[i1], vertexArray[i2]);
            
            boundingBox.Clear();
            boundingBox.AddPoint(vertexArray[i0]);
            boundingBox.AddPoint(vertexArray[i1]);
            boundingBox.AddPoint(vertexArray[i2]);
        }

        /// <summary>
        /// Get the indices into the relevant vertex array for this triangle.
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        public void GetVertexIndices(out int i0, out int i1, out int i2)
        {
            i0 = vertexIndices[0];
            i1 = vertexIndices[1];
            i2 = vertexIndices[2];
        }

        /// Get the vertex index association with iCorner (which should be
        /// 0, 1 or 2)
        public int GetVertexIndex(int iCorner)
        {
            return vertexIndices[iCorner];
        }

        /// Get the triangle plane
        public Plane Plane
        {
            get { return plane; }
        }

        /// Has the edge been marked as convex. Same convention for edge
        /// numbering as in tTriangle.
        public bool IsEdgeConvex(int iEdge)
        {
            return 0 != (convexFlags & (1 << iEdge));
        }

        public void SetEdgeConvex(int iEdge, bool convex)
        {
            if (convex)
                convexFlags |= (ushort)(1 << iEdge);
            else
                convexFlags &= (ushort)~(1 << iEdge);
        }

        /// Has the point been marked as convex. Same convention for point
        /// numbering as in tTriangle
        public bool IsPointConvex(int iPoint)
        {
            return 0 != (convexFlags & (1 << (iPoint + 3)));
        }

        public void SetPointConvex(int iPoint, bool convex)
        {
            if (convex)
                convexFlags |= (ushort)(1 << (iPoint + 3));
            else
                convexFlags &= (ushort)~(1 << (iPoint + 3));
        }

        public AABox BoundingBox
        {
            get { return boundingBox; }
        }

    }

    /// <summary>
    /// structure used to set up the mesh
    /// </summary>
    public struct TriangleVertexIndices
    {
        public int I0, I1, I2;

        public TriangleVertexIndices(int i0, int i1, int i2) 
        {
            this.I0 = i0; 
            this.I1 = i1; 
            this.I2 = i2;
        }

        public void Set(int i0, int i1, int i2)
        {
            I0 = i0; I1 = i1; I2 = i2;
        }
    }

}
