using System;
using AntiTankGame2.Localization;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace AntiTankGame2.GameObjects
{
    public class NccSkySphere : BaseNccSceneObject, INccLoadable
    {
        private Effect skySphereEffect;
        private Model skySphere;

        public void LoadContent()
        {
            skySphereEffect = BaseEngine.ContentManager.Load<Effect>(ContentConstants.SkySphereEffectPath);
            var skyboxTexture = BaseEngine.ContentManager.Load<TextureCube>(ContentConstants.SkyboxTexturePath);
            skySphere = BaseEngine.ContentManager.Load<Model>(ContentConstants.SkySpherePath);

            // Set the parameters of the effect
            skySphereEffect.Parameters["ViewMatrix"].SetValue(CameraManager.ActiveCamera.View);
            skySphereEffect.Parameters["ProjectionMatrix"].SetValue(CameraManager.ActiveCamera.Projection);
            skySphereEffect.Parameters["SkyboxTexture"].SetValue(skyboxTexture);
            // Set the Skysphere Effect to each part of the Skysphere model
            foreach (ModelMesh mesh in skySphere.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = skySphereEffect;
                }
            }
        }

        public void UnloadContent()
        {
            GC.Collect();   
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {

            // Set the View and Projection matrix for the effect
            skySphereEffect.Parameters["ViewMatrix"].SetValue(CameraManager.ActiveCamera.View);
            skySphereEffect.Parameters["ProjectionMatrix"].SetValue(CameraManager.ActiveCamera.Projection);
            // Draw the sphere model that the effect projects onto
            foreach (ModelMesh mesh in skySphere.Meshes)
            {
                mesh.Draw();
            }

            // Undo the renderstate settings from the shader
            //            GraphicsDevice.RenderState.CullMode =
            //                CullMode.CullCounterClockwiseFace;
            //            GraphicsDevice.RenderState.DepthBufferWriteEnable = true;

            base.Draw(gameTime);
        }
    }
}