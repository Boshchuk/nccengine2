#region Using Statements

using System;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework;

#endregion
namespace NccHeightMapPipeline 
{
    /// <summary>
    /// HeightMapInfoContent contains information about a size and heights of a
    /// heightmap. When the game is being built, it is constructed by the
    /// TerrainProcessor, and attached to the finished terrain's Tag. When the game is
    /// run, it will be read in as a HeightMapInfo.
    /// </summary>
    public class HeightMapInfoContent
    {
        /// <summary>
        /// This propery is a 2D array of floats, and tells us the height that each
        /// position in the heightmap is.
        /// </summary>
        public float[,] Height { get; private set; }

        /// <summary>
        /// This property is a 2D array of Vector3s, and tells us the normal that each
        /// position in the heightmap is.
        /// </summary>
        public Vector3[,] Normals
        {
            get { return normals; }
            set { normals = value; }
        }
        private Vector3[,] normals;


        /// <summary>
        /// TerrainScale is the distance between each entry in the Height property.
        /// For example, if TerrainScale is 30, Height[0,0] and Height[1,0] are 30
        /// units apart.
        /// </summary>
        public float TerrainScale
        {
            get { return terrainScale; }
        }
        private float terrainScale;

        /// <summary>
        /// This constructor will initialize the height array from the values in the 
        /// bitmap. Each pixel in the bitmap corresponds to one entry in the height
        /// array.
        /// </summary>
        public HeightMapInfoContent(MeshContent terrainMesh, float terrainScale,
            int terrainWidth, int terrainLength)
        {
            // validate the parameters
            if (terrainMesh == null)
            {
                throw new ArgumentNullException("terrainMesh");
            }
            if (terrainWidth <= 0)
            {
                throw new ArgumentOutOfRangeException("terrainWidth");
            }
            if (terrainLength <= 0)
            {
                throw new ArgumentOutOfRangeException("terrainLength");
            }

            this.terrainScale = terrainScale;

            // create new arrays of the requested size.
            Height = new float[terrainWidth, terrainLength];
            normals = new Vector3[terrainWidth, terrainLength];

            // to fill those arrays, we'll look at the position and normal data
            // contained in the terrainMesh.
            GeometryContent geometry = terrainMesh.Geometry[0];
            // we'll go through each vertex....
            for (int i = 0; i < geometry.Vertices.VertexCount; i++)
            {
                // ... and look up its position and normal.
                Vector3 position = geometry.Vertices.Positions[i];
                var normal = (Vector3)geometry.Vertices.Channels
                    [VertexChannelNames.Normal()][i];

                // from the position's X and Z value, we can tell what X and Y
                // coordinate of the arrays to put the height and normal into.
                var arrayX = (int)
                    ((position.X / terrainScale) + (terrainWidth - 1) / 2.0f);
                var arrayY = (int)
                    ((position.Z / terrainScale) + (terrainLength - 1) / 2.0f);

                Height[arrayX, arrayY] = position.Y;
                normals[arrayX, arrayY] = normal;
            }
        }
    }
}