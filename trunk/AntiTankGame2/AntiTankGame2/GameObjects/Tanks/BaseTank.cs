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
    public class BaseTank : BaseNccSceneObject, INccLoadable
    {
        
        #region Load and Unload
        public void LoadContent()
        {
            var model = new NccModel(ContentConstants.TankHeightModel);
            ModelManager.AddModel(model, ContentConstants.TankModelName);
            ModelName = ContentConstants.TankModelName;
            OcclusionModelName = ContentConstants.TankModelName;
        }

        public void UnloadContent()
        {
            GC.Collect();
        }
        #endregion

       

        public override void Update(GameTime gameTime)
        {
            //var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var model = ModelManager.GetModel(ModelName);
            if (model != null && model.ReadyToRender && !ReadyToRender)
            {

                //TODO ROTARATE TANK

                var transforms = new Matrix[model.BaseModel.Bones.Count];
                model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

                BoundingBox = new BoundingBox();

                foreach (var mesh in model.BaseModel.Meshes)
                {
                    if (!BoundingBoxCreated)
                    {
                        BoundingBox = BoundingBox.CreateMerged(BoundingBox, BoundingBox.CreateFromSphere(mesh.BoundingSphere));
                    }
                }
                BoundingBoxCreated = true;

                var min = BoundingBox.Min;
                var max = BoundingBox.Max;

                BoundingBox = new BoundingBox(min, max);

                ReadyToRender = true;
            }
        }


        public override void Draw(GameTime gameTime)
        {
            if (ReadyToRender)
            {
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