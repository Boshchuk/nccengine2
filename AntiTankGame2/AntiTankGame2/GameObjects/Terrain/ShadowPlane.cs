using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace AntiTankGame2.GameObjects.Terrain
{
    public class ShadowPlane : BaseNccSceneObject, INccLoadable
    {
        private readonly DepthStencilState depthStencilState = new DepthStencilState { DepthBufferEnable = true };

        private Texture2D ground;

        private const string Name = "simlePlane";

        public void LoadContent()
        {
            ground = BaseEngine.ContentManager.Load<Texture2D>("Content/Models/Land/shadow/ground");
            var planeModel = new NccModel("Content/Models/Land/shadow/dualtextureplane");
            ModelManager.AddModel(planeModel, Name);
            ModelName = Name;
            OcclusionModelName = Name;
            
        }

        public void UnloadContent()
        {
            System.GC.Collect();
        }

        private readonly RenderTarget2D lightmap;

         //Matrix proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,BaseEngine.Device.Viewport.AspectRatio, 1.0f, 100.0f);
         //Matrix view = Matrix.CreateLookAt(new Vector3(0, 23, 20), Vector3.Zero, Vector3.Up);

        public ShadowPlane(RenderTarget2D lightM)
        {
            lightmap = lightM;
        }

        public override void Draw(GameTime gameTime)
        {
            if (!ReadyToRender) return;
            BaseEngine.Device.DepthStencilState = depthStencilState;

            var model = ModelManager.GetModel(ModelName);
            if (model == null || !model.ReadyToRender) return;
            var transforms = new Matrix[model.BaseModel.Bones.Count];
            model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

            /* foreach (var mesh in model.BaseModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = transforms[mesh.ParentBone.Index] * World;
                        effect.View = CameraManager.ActiveCamera.View;
                        effect.Projection = CameraManager.ActiveCamera.Projection;

                        // effect.EnableDefaultLighting();
                        effect.FogColor = new Vector3(255, 255, 255);
                        effect.FogStart = 10;
                        effect.FogEnd = 100;
                        //effect.TextureEnabled = false;

                    }
                    mesh.Draw();
                }*/
            foreach (var mesh in model.BaseModel.Meshes)
            {
                mesh.Draw();
                foreach (DualTextureEffect de in mesh.Effects)
                {
                    de.Texture = ground;
                    de.Texture2 = lightmap;
                }
                  
            }
            //model.BaseModel.Draw(Matrix.CreateTranslation(0, -20, 0), CameraManager.ActiveCamera.View, CameraManager.ActiveCamera.Projection);
        }
    }
}