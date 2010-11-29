using System;
using Microsoft.Xna.Framework.Graphics;

namespace NccEngine2.GameComponents.Graphics.Textures
{
    public class NccTexture : INccTexture
    {
        /// <summary>
        /// The file name of the asset.
        /// </summary>
        public string FileName { get; set; }

        ///<summary>
        ///Gets the underlying Effect.
        ///</summary>
        public Texture BaseTexture { get; private set; }

        ///<summary>
        ///Is the texture ready to be rendered.
        ///</summary>
        public bool ReadyToRender { get; private set; }

        /// <summary>
        /// Construct a new NccTexture.
        /// </summary>
        public NccTexture()
        {
            ReadyToRender = false;
        }

        /// <summary>
        /// Construct a new NccTexture.
        /// </summary>
        /// <param name="fileName">The asset file name.</param>
        public NccTexture(string fileName)
        {
            ReadyToRender = false;
            FileName = fileName;
        }

        public void LoadContent()
        {
            if (String.IsNullOrEmpty(FileName)) return;
            BaseTexture = BaseEngine.ContentManager.Load<Texture>(FileName);
            ReadyToRender = true;
        }

        public void UnloadContent()
        {
            BaseTexture.Dispose();
        }
    }

}