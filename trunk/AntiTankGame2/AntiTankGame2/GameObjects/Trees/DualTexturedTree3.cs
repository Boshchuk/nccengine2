using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.CameraManagment;

namespace AntiTankGame2.GameObjects.Trees
{
    using NccEngine2.GameComponents.Models;

    /// <summary>
    /// Special kind of tree)
    /// </summary>
// ReSharper disable UnusedMember.Global
    public class DualTexturedTree3 : BaseTree
// ReSharper restore UnusedMember.Global
    {
        /// <summary>
        /// Draws the DualTextureEffect demo.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Compute camera matrices.
            //var time = (float)gameTime.TotalGameTime.TotalSeconds;
            
            var transforms = new Matrix[ Model.BaseModel.Bones.Count];

            Model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

            BaseEngine.Device.BlendState = BlendState.Opaque;
            BaseEngine.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            BaseEngine.Device.DepthStencilState = DepthStencilState.Default;
            BaseEngine.Device.SamplerStates[0] = SamplerState.LinearWrap;

            foreach (var mesh in Model.BaseModel.Meshes)
            {
                var textures = new List<Texture2D>();

                foreach (DualTextureEffect effect in mesh.Effects)
                {
                    var world = transforms[mesh.ParentBone.Index];

                    effect.World = world;
                    effect.View = CameraManager.ActiveCamera.View;
                    effect.Projection = CameraManager.ActiveCamera.Projection;

                    effect.DiffuseColor = new Vector3(0.75f);
                    
                    textures.Add(effect.Texture);
                    textures.Add(effect.Texture2);
                }

                // Draw the mesh.
                mesh.Draw();

                // Restore the original textures.
                var i = 0;

                foreach (DualTextureEffect effect in mesh.Effects)
                {
                    effect.Texture = textures[i++];
                    effect.Texture2 = textures[i++];
                }
            }
            //base.Draw(gameTime);
        }
    }
}