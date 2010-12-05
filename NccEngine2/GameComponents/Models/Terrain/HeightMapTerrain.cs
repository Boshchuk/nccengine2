using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Scene.Graph;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;
using NccEngine2.GameComponents.Scene.SceneObject;
using NccEngine2.Helpers;

namespace NccEngine2.GameComponents.Models.Terrain
{
    public sealed class HeightMapTerrain : NccSceneObject, INccLoadable, INccUpdateable, INccOcclusion // OccluderSceneObject, INccPhysics
    {
        public HeightMapTerrain()
        {
            Position = Vector3.Zero;
        }

        public HeightMapTerrain(Vector3 newPosition)
        {
            Position = newPosition;
        }

        public Vector3 SetMass(float mass)
        {
            return Vector3.Zero;
        }

        public void Update(GameTime gameTime)
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

        public override void DrawCulling(GameTime gameTime)
        {
            Occluded = false;
            var lastFillModeState = EngineManager.Device.RasterizerState.FillMode; /*   RenderState.FillMode;*/
            //  if (ReadyToRender && !Culled)
            {
                //query.Begin();
                var model = ModelManager.GetModel(ModelName);
                // if (model != null && model.ReadyToRender)
                {
                    var transforms = new Matrix[model.BaseModel.Bones.Count];
                    model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

                    foreach (var mesh in model.BaseModel.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {

                            //effect.EnableDefaultLighting();
                            //effect.PreferPerPixelLighting = true;
                            effect.World = transforms[mesh.ParentBone.Index];
                            effect.View = CameraManager.ActiveCamera.View;
                            effect.Projection = CameraManager.ActiveCamera.Projection;

                            //TODO ERROR HERE

                            //EngineManager.Device.RenderState.FillMode = FillMode.WireFrame;
                            //effect.FogEnabled = true;
                            //   effect.FogColor = new Vector3(Color.LightBlue.R,Color.LightBlue.G,Color.LightBlue.B);
                            //  effect.FogStart = 1000;
                            //  effect.FogEnd = 3200;

                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            // Set the fog to match the black background color
                            //effect.FogEnabled = true;
                            //effect.FogColor = Vector3.Zero;
                            //effect.FogStart = 1000;
                            //effect.FogEnd = 3200;
                        }
                        mesh.Draw();
                    }
                }
                //query.End();

                /* while (!query.IsComplete)
                 {

                 }*/

                //  if (query.IsComplete && query.PixelCount == 0)
               // {
                    // Occluded = true;
               // }
            }
            //var rState = new RasterizerState { FillMode = lastFillModeState };
           // BaseEngine.Device.RasterizerState = rState;
        }

        public override void Draw(GameTime gameTime)
        {

           // BaseEngine.RestorSamplerState();

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

                            // BaseEngine.Device.RasterizerState = new RasterizerState {CullMode = CullMode.None};
                            // BaseEngine.Device.RasterizerState = new RasterizerState {FillMode = FillMode.WireFrame};

                            effect.World = transforms[mesh.ParentBone.Index] ;
                            effect.View = CameraManager.ActiveCamera.View;
                            effect.Projection = CameraManager.ActiveCamera.Projection;

                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            // Set the fog to match the black background color
                            //effect.FogEnabled = true;
                            //effect.FogColor = Vector3.Zero;
                            //effect.FogStart = 1000;
                            //effect.FogEnd = 3200;
                        }
                        mesh.Draw();
                    }
                }
            }
        }

        public void LoadContent()
        {
            var terrainModel = new NccModel(ConstantNames.TerrainModelPath);

            ModelManager.AddModel(terrainModel, ConstantNames.TerrainModelName);

            ModelName = ConstantNames.TerrainModelName;
            OcclusionModelName = ConstantNames.TerrainModelOcclusionName;
        }

        public void UnloadContent()
        {

        }

        public override string ToString()
        {
            return string.Format(ConstantNames.HeightMapTerrainToStringMessage, ModelName, OcclusionModelName);
        }

        public bool Culled
        {
            get;
            set;
        }

        public bool BoundingBoxCreated
        {
            get;
            set;
        }

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
                        query = new OcclusionQuery(BaseEngine.Device);
                    }
                    else
                    {
                        return query;
                    }
                }
                return null;
            }
        }

        public bool Occluded
        {
            get;
            set;
        }
    }
}