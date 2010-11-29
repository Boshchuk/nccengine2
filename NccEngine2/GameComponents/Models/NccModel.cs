using System;
using Microsoft.Xna.Framework.Graphics;

namespace NccEngine2.GameComponents.Models
{
    public class NccModel : INccModel
    {
        /// <summary>
        /// The file name of the asset.
        /// </summary>
        public string FileName { get; set; }

        ///<summary>
        ///Gets the underlying Effect.
        ///</summary>
        public Model BaseModel { get; set; }

        ///<summary>
        ///Is the texture ready to be rendered.
        ///</summary>
        public bool ReadyToRender { get; private set; }

        /// <summary>
        /// Construct a new NccModel.
        /// </summary>
        public NccModel()
        {

        }

        /// <summary>
        /// Construct a new NccModel.
        /// </summary>
        /// <param name="fileName">The asset file name.</param>
        public NccModel(string fileName)
        {
            FileName = fileName;
        }

        public void LoadContent()
        {
            if (String.IsNullOrEmpty(FileName)) return;
            BaseModel = BaseEngine.ContentManager.Load<Model>(FileName);
            ReadyToRender = true;
        }


    }

}