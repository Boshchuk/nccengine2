using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace NccHeightMapPipeline
{
    // ReSharper disable UnusedMember.Global
    /// <summary>
    /// A TypeWriter for HeightMapInfo, which tells the content pipeline how to save the
    /// data in HeightMapInfo. This class should match HeightMapInfoReader: whatever the
    /// writer writes, the reader should read.
    /// </summary>
    [ContentTypeWriter]
    public class HeightMapInfoWriter : ContentTypeWriter<HeightMapInfoContent>
    {
        protected override void Write(ContentWriter output, HeightMapInfoContent value)
        {
            output.Write(value.TerrainScale);

            output.Write(value.Height.GetLength(0));
            output.Write(value.Height.GetLength(1));
            foreach (var height in value.Height)
            {
                output.Write(height);
            }
            foreach (var normal in value.Normals)
            {
                output.Write(normal);
            }
        }

        /// <summary>
        /// Tells the content pipeline what CLR type the sky
        /// data will be loaded into at runtime.
        /// </summary>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return  "NccEngine2.HeightMapInfo, NccEngine2, Version=1.0.0.0, Culture=neutral";
        }


        /// <summary>
        /// Tells the content pipeline what worker type
        /// will be used to load the sky data.
        /// </summary>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "NccEngine2.HeightMapInfoReader, NccEngine2, Version=1.0.0.0, Culture=neutral";
        }
    }
    // ReSharper restore UnusedMember.Global
}