#region Using Statements

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.ComponentModel;

#endregion

namespace NccHeightMapPipeline
{
    // ReSharper disable UnusedMember.Global
    /// <summary>
    /// Custom content processor for creating terrain meshes. Given an
    /// input heightfield texture, this processor uses the MeshBuilder
    /// class to programatically generate terrain geometry.
    /// </summary>
    [ContentProcessor]
    public class TerrainProcessor : ContentProcessor<Texture2DContent, ModelContent>
    {
        // This region defines the parameters that this processor accepts. This feature
        // is new to XNA Game Studio 2.0, and is used to further customize how your
        // assets are built. We'll use these parameters to control how our heightmap
        // image is converted into a mesh. The parameters use DefaultValue, Description,
        // and DisplayName attributes to control how they are displayed in the UI.
        #region Processor Parameters

        /// <summary>
        /// Controls the scale of the terrain. This will be the distance between
        /// vertices in the finished terrain mesh.
        /// </summary>
        [DefaultValue(30.0f)]
        [Description("The distance between vertices in the finished terrain mesh.")]
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        private float scale = 30.0f;


        /// <summary>
        /// Controls the height of the terrain. The heights of the vertices in the
        /// finished mesh will vary between 0 and -Bumpiness.
        /// </summary>
        [DefaultValue(640.0f)]
        [Description("Controls the height of the terrain.")]
        public float Bumpiness
        {
            get { return bumpiness; }
            set { bumpiness = value; }
        }
        private float bumpiness = 640.0f;


        /// <summary>
        /// Controls the how often the texture applied to the terrain will be repeated.
        /// </summary>
        [DefaultValue(.1f)]
        [Description("Controls how often the texture will be repeated across the terrain.")]
        private float TexCoordScale
        {
            get { return texCoordScale; }
// ReSharper disable UnusedMember.Local
            set { texCoordScale = value; }
// ReSharper restore UnusedMember.Local
        }
        private float texCoordScale = 0.1f;


        /// <summary>
        /// Controls the texture that will be applied to the terrain. If no value is
        /// supplied, a texture will not be applied.
        /// </summary>
        //[DefaultValue("rocks.bmp")]
        [DefaultValue("Textures/sand.jpg")]
        [Description("Controls the texture that will be applied to the terrain. If no value is supplied, a texture will not be applied.")]
        [DisplayName("Terrain Texture")]
        private string TerrainTexture
        {
            get { return terrainTexture; }
// ReSharper disable UnusedMember.Local
            set { terrainTexture = value; }
// ReSharper restore UnusedMember.Local
        }
        private string terrainTexture = "Textures/ground.png";

        #endregion


        /// <summary>
        /// Generates a terrain mesh from an input heightfield texture.
        /// </summary>
        public override ModelContent Process(Texture2DContent input, ContentProcessorContext context)
        {
            var builder = MeshBuilder.StartMesh("terrain");

            // Convert the input texture to float format, for ease of processing.
            input.ConvertBitmapType(typeof(PixelBitmapContent<float>));


            var heightfield = (PixelBitmapContent<float>)input.Mipmaps[0];

            // Create the terrain vertices.
            for (var y = 0; y < heightfield.Height; y++)
            {
                for (var x = 0; x < heightfield.Width; x++)
                {
                    Vector3 position;

                    // position the vertices so that the heightfield is centered
                    // around x=0,z=0
                    position.X = scale * (x - ((heightfield.Width - 1) / 2.0f));
                    position.Z = scale * (y - ((heightfield.Height - 1) / 2.0f));

                    position.Y = (heightfield.GetPixel(x, y) - 1) * bumpiness;

                    builder.CreatePosition(position);
                }
            }

            // Create a material, and point it at our terrain texture.
            var material = new BasicMaterialContent {SpecularColor = new Vector3(.4f, .4f, .4f)};

            if (!string.IsNullOrEmpty(TerrainTexture))
            {
                var directory = Path.GetDirectoryName(input.Identity.SourceFilename);
// ReSharper disable AssignNullToNotNullAttribute
                var texture = Path.Combine(directory, TerrainTexture);
// ReSharper restore AssignNullToNotNullAttribute

                material.Texture = new ExternalReference<TextureContent>(texture);
            }

            builder.SetMaterial(material);

            // Create a vertex channel for holding texture coordinates.
            var texCoordId = builder.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));

            // Create the individual triangles that make up our terrain.
            for (var y = 0; y < heightfield.Height - 1; y++)
            {
                for (var x = 0; x < heightfield.Width - 1; x++)
                {
                    AddVertex(builder, texCoordId, heightfield.Width, x, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y + 1);

                    AddVertex(builder, texCoordId, heightfield.Width, x, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y + 1);
                    AddVertex(builder, texCoordId, heightfield.Width, x, y + 1);
                }
            }

            // Chain to the ModelProcessor so it can convert the mesh we just generated.
            var terrainMesh = builder.FinishMesh();


            var model = context.Convert<MeshContent, ModelContent>(terrainMesh,"ModelProcessor");

            // generate information about the height map, and attach it to the finished
            // model's tag.
            model.Tag = new HeightMapInfoContent(terrainMesh, scale,heightfield.Width, heightfield.Height);

            return model;
        }


        /// <summary>
        /// Helper for adding a new triangle vertex to a MeshBuilder,
        /// along with an associated texture coordinate value.
        /// </summary>
        void AddVertex(MeshBuilder builder, int texCoordId, int w, int x, int y)
        {
            builder.SetVertexChannelData(texCoordId, new Vector2(x, y) * TexCoordScale);

            builder.AddTriangleVertex(x + y * w);
        }
    }
    // ReSharper restore UnusedMember.Global
}