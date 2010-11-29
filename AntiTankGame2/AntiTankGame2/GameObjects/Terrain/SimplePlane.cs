using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.Graph;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace AntiTankGame2.GameObjects.Terrain
{
    /// <summary>
    /// The simle plane from fbx file
    /// </summary>
    public class SimplePlane : NccSceneObject, INccLoadable, INccUpdateable, INccOcclusion
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

        public override void DrawCulling(GameTime gameTime)
        {
            //Occluded = false;
            //var lastState = EngineManager.Device.RasterizerState.FillMode;
            if (ReadyToRender && !Culled)
            {
                //  query.Begin();
                var model = ModelManager.GetModel(ModelName);
                if (model != null && model.ReadyToRender)
                {
                    var transforms = new Matrix[model.BaseModel.Bones.Count];
                    model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

                    foreach (var mesh in model.BaseModel.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            //effect.EnableDefaultLighting();
                            //effect.PreferPerPixelLighting = true;
                            effect.World = World;
                            effect.View = CameraManager.ActiveCamera.View;
                            effect.Projection = CameraManager.ActiveCamera.Projection;
                        }
                        mesh.Draw();
                    }
                }
                //  query.End();

                //  while (!query.IsComplete)
                {

                }

                //  if (query.IsComplete && query.PixelCount == 0)
                {
                    //Occluded = true;
                }
            }
           // EngineManager.Device.RasterizerState = new RasterizerState { FillMode = lastState };

        }

        public  void Update(GameTime gameTime)
        {
            // var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var model = ModelManager.GetModel(ModelName);
            if (model != null && model.ReadyToRender && !ReadyToRender)
            {
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

        private OcclusionQuery query;//= new OcclusionQuery(EngineManager.Device);
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

        public bool Occluded { get; set; }
    }

}