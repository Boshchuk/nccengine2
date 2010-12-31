using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace AntiTankGame2.GameObjects.Terrain
{
    /// <summary>
    /// The simle plane from fbx file
    /// </summary>
    public class SimplePlane : BaseNccSceneObject, INccLoadable
    {
        public void LoadContent()
        {
            var planeModel = new NccModel("Content/Models/Land/plane");
            ModelManager.AddModel(planeModel, "simlePlane");
            ModelName = "simlePlane";
            OcclusionModelName = "simlePlane";
        }

        public void UnloadContent()
        {
            System.GC.Collect();
        }

        public override void Draw(GameTime gameTime)
        {
            if (!ReadyToRender) return;
            BaseEngine.Device.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };

            var model = ModelManager.GetModel(ModelName);
            if (model != null && model.ReadyToRender)
            {
                var transforms = new Matrix[model.BaseModel.Bones.Count];
                model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (var mesh in model.BaseModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = transforms[mesh.ParentBone.Index] * World;
                        effect.View = CameraManager.ActiveCamera.View;
                        effect.Projection = CameraManager.ActiveCamera.Projection;

                        // effect.EnableDefaultLighting();
                        effect.FogColor = new Vector3(255,255,255);
                        effect.FogStart = 10;
                        effect.FogEnd = 100;
                        //effect.TextureEnabled = false;

                    }
                    mesh.Draw();
                }
            }
        }
    }
}