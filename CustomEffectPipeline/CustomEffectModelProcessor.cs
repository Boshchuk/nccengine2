#region File Description
//-----------------------------------------------------------------------------
// CustomEffectModelProcessor.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System.ComponentModel;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
#endregion

namespace CustomEffectPipeline
{
    /// <summary>
    /// Overloaded model processor that calls our new material processor.
    /// </summary>
    [ContentProcessor]
    public class CustomEffectModelProcessor : ModelProcessor
    {
        [DisplayName("Custom Effect")]
        [Description("The custom effect applied to the model.")]
        public string CustomEffect
        {
            get { return customEffect; }
            set { customEffect = value; }
        }
        private string customEffect;

        /// <summary>
        /// Use the CustomEffectMaterialProcessor for all of the materials in the model.
        /// We pass the processor parameter along to the material processor for the 
        /// effect file name.
        /// </summary>
        protected override MaterialContent ConvertMaterial(MaterialContent material, 
                                                        ContentProcessorContext context)
        {
            OpaqueDataDictionary processorParameters = new OpaqueDataDictionary();

            processorParameters.Add("CustomEffect", customEffect);
            processorParameters["ColorKeyColor"] = ColorKeyColor;
            processorParameters["ColorKeyEnabled"] = ColorKeyEnabled;
            processorParameters["TextureFormat"] = TextureFormat;
            processorParameters["GenerateMipmaps"] = GenerateMipmaps;
            processorParameters["ResizeTexturesToPowerOfTwo"] = ResizeTexturesToPowerOfTwo;

            return context.Convert<MaterialContent, MaterialContent>(material, 
                                  "CustomEffectMaterialProcessor", processorParameters);
        }
    }
}