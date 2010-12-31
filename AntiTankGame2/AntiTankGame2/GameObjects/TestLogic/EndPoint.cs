using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2.GameComponents.Audio;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.Graph;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace AntiTankGame2.GameObjects.TestLogic
{
    public sealed class EndPoint : NccSceneObject, INccLoadable, INccUpdateable, INccOcclusion, IAudioEmitter
    {
        #region fields

        public BoundingBox BoundingBox { get; private set; }

        public string OcclusionModelName { get; set; }

        public bool BoundingBoxCreated { get; private set; }


        public bool Occluded { get; set; }
#if HIDEF
        private OcclusionQuery query= new OcclusionQuery(BaseEngine.Device);
#endif
#pragma warning disable 649
        private OcclusionQuery query;// = new OcclusionQuery(BaseEngine.Device);
#pragma warning restore 649
        public OcclusionQuery Query
        {
            get { return query; }
        }

        public bool Culled { get; set; }

        #endregion


        #region construtcors
        public EndPoint()
        {
            Position = Vector3.Zero;
        }

        public EndPoint(Vector3 newPosition)
        {
            Position = newPosition;
            Scale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        #endregion

        #region Load and Unload
        public void LoadContent()
        {
            var tankModel = new NccModel("Content/Models/box");
            ModelManager.AddModel(tankModel, "rocket");
            ModelName = "rocket";
            OcclusionModelName = "rocket";
        }

        public void UnloadContent()
        {

        }

        #endregion

        #region Draw and Update

        public override void DrawCulling(GameTime gameTime)
        {
            Occluded = false;
            if (!ReadyToRender || Culled) return;
            query.Begin();
            var model = ModelManager.GetModel(OcclusionModelName);
            if (model != null && model.ReadyToRender)
            {
                var transforms = new Matrix[model.BaseModel.Bones.Count];
                model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (var mesh in model.BaseModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.World = World;
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

        public void Update(GameTime gameTime)
        {
            //var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

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

                Vector3 min, max;
                min = BoundingBox.Min;
                max = BoundingBox.Max;

                BoundingBox = new BoundingBox(min, max);

                ReadyToRender = true;
            }
        }

        #endregion

        #region methds

        public bool DrawBoundingBox
        {
            get;
            set;
        }

        public BoundingBox GetBoundingBoxTransformed()
        {
            //Vector3 min, max;
            //min = BoundingBox.Min;
            //max = BoundingBox.Max;


            var min = Vector3.Transform(BoundingBox.Min, World);
            var max = Vector3.Transform(BoundingBox.Max, World);

            return new BoundingBox(min, max);
        }

        #endregion

// ReSharper disable UnusedAutoPropertyAccessor.Local
        public Vector3 Forward { get; private set; }


        public Vector3 Up{ get; private set; }

        public Vector3 Velocity{ get; private set; }
 // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}