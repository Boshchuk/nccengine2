using System;
using AntiTankGame2.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.Graph;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace AntiTankGame2.GameObjects.Tanks
{
    public class BaseTank : NccSceneObject, INccLoadable, INccUpdateable, INccOcclusion
    {
        #region fields

        public BoundingBox BoundingBox { get; private set; }

        public string OcclusionModelName { get; set; }

        public bool BoundingBoxCreated { get; private set; }

        public bool Occluded { get; set; }

        // ReSharper disable AccessToStaticMemberViaDerivedType
        OcclusionQuery query; //= new OcclusionQuery(EngineManager.Device);
        // ReSharper restore AccessToStaticMemberViaDerivedType
        public OcclusionQuery Query
        {
            get
            {
                if (GraphicsAdapter.DefaultAdapter.IsProfileSupported(GraphicsProfile.Reach))
                {
                    if (query == null)
                    {
                        query = new OcclusionQuery(EngineManager.Device);
                    }
                    else
                    {
                        return query;
                    }
                }
                return null;
            }
        }

        public bool Culled { get; set; }

        #endregion


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

       

        public void Update(GameTime gameTime)
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


        public BoundingBox GetBoundingBoxTransformed()
        {
            var min = Vector3.Transform(BoundingBox.Min, World);
            var max = Vector3.Transform(BoundingBox.Max, World);

            return new BoundingBox(min, max);
        }

    }

}