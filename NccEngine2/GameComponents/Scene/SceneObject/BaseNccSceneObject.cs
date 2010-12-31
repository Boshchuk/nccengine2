#define RICH

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.Graph;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;

namespace NccEngine2.GameComponents.Scene.SceneObject
{
    public class BaseNccSceneObject : NccSceneObject, INccUpdateable, INccOcclusion
    {
        public override void DrawCulling(GameTime gameTime)
        {
            Occluded = false;
            if (ReadyToRender && !Culled)
            {
                query.Begin();
                var model = ModelManager.GetModel(ModelName);
                if (model != null && model.ReadyToRender)
                {
                    var transforms = new Matrix[model.BaseModel.Bones.Count];
                    model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

                    foreach (var mesh in model.BaseModel.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.World =World;
                            effect.View = CameraManager.ActiveCamera.View;
                            effect.Projection = CameraManager.ActiveCamera.Projection;
                        }
                        mesh.Draw();
                    }
                }
                query.End();

                while (!query.IsComplete)
                {

                }

                if (query.IsComplete && query.PixelCount == 0)
                {
                    Occluded = true;
                }
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            var model = ModelManager.GetModel(ModelName);
            if (model == null || !model.ReadyToRender || ReadyToRender) return;
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

        public bool Culled { get; set; }

        public bool BoundingBoxCreated { get; set; }

        public BoundingBox BoundingBox { get; set; }

        public BoundingBox GetBoundingBoxTransformed()
        {
            var min = Vector3.Transform(BoundingBox.Min, World);
            var max = Vector3.Transform(BoundingBox.Max, World);

            return new BoundingBox(min, max);
        }

        public string OcclusionModelName { get; set; }

#if RICH
        private OcclusionQuery query;// = new OcclusionQuery(BaseEngine.Device);
#endif

#if HIDEF
        private OcclusionQuery query = new OcclusionQuery(BaseEngine.Device);
#endif
        public OcclusionQuery Query
        {
            get
            {
#if RICH
                if (GraphicsAdapter.DefaultAdapter.IsProfileSupported(GraphicsProfile.Reach))
                {
                    if (query == null)
                    {
                        query = new OcclusionQuery(BaseEngine.Device);
                    }
                    else
                    {
                        return query;
                    }
                }

                return null;
#endif
#if HIDEF
                return query;
#endif
            }
        }

        public bool Occluded { get; set; }
    }
}