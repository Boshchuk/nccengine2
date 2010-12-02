using System;
using AntiTankGame2.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace AntiTankGame2.GameObjects.Tanks
{
    public class BloomTank : BaseNccSceneObject, INccLoadable
    {
        #region Load and Unload
        public void LoadContent()
        {
            var model = new NccModel(ContentConstants.BloomTankModel);
            ModelManager.AddModel(model, ContentConstants.BloomTankName);
            ModelName = ContentConstants.BloomTankName;
            OcclusionModelName = ContentConstants.BloomTankName;
        }

        public void UnloadContent()
        {
            GC.Collect();
        }
        #endregion

        public override void DrawCulling(GameTime gameTime)
        {
            EngineManager.Device.DepthStencilState = DepthStencilState.Default;


            if (ReadyToRender)
            {
                var time = (float)gameTime.TotalGameTime.TotalSeconds;

                var model = ModelManager.GetModel(ModelName);
                if (model != null && model.ReadyToRender)
                {
                    var transforms = new Matrix[model.BaseModel.Bones.Count];
                    model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

                    var world = Matrix.CreateRotationY(time * 0.42f);


                    foreach (var mesh in model.BaseModel.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.World = transforms[mesh.ParentBone.Index] * world;
                            effect.View = CameraManager.ActiveCamera.View;
                            effect.Projection = CameraManager.ActiveCamera.Projection;

                            effect.EnableDefaultLighting();

                            effect.SpecularColor = Vector3.One;
                        }
                        mesh.Draw();
                    }
                }
            }
        }
        
        public override void Draw(GameTime gameTime)
        {
            EngineManager.Device.DepthStencilState = DepthStencilState.Default;


            if (ReadyToRender)
            {
                var time = (float) gameTime.TotalGameTime.TotalSeconds;

                var model = ModelManager.GetModel(ModelName);
                if (model != null && model.ReadyToRender)
                {
                    var transforms = new Matrix[model.BaseModel.Bones.Count];
                    model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

                    var world = Matrix.CreateRotationY(time * 0.42f);
                    

                    foreach (var mesh in model.BaseModel.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.World = transforms[mesh.ParentBone.Index] * world ;
                            effect.View = CameraManager.ActiveCamera.View;
                            effect.Projection = CameraManager.ActiveCamera.Projection;

                            effect.EnableDefaultLighting();

                            effect.SpecularColor = Vector3.One;
                        }
                        mesh.Draw();
                    }
                }
            }
        }
    }
}