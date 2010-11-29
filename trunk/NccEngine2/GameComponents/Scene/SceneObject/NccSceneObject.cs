using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Physics;

namespace NccEngine2.GameComponents.Scene.SceneObject
{
    public class NccSceneObject : INccSceneObject
    {
        /// <summary>
        /// Is this object ready to render?
        /// </summary>
        public bool ReadyToRender { get; set; }

        public virtual Matrix World
        {
            get
            {
                if (this is INccPhysics)
                {
                    var physicsObject = (INccPhysics)this;
                    if (physicsObject.Body != null && physicsObject.Body.CollisionSkin != null)
                    {
                        return Matrix.CreateScale(Scale) * physicsObject.Body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation * physicsObject.Body.Orientation * Matrix.CreateTranslation(physicsObject.Body.Position);
                    }
                    /*else*/
                    if (physicsObject.Body != null)
                    {
                        return Matrix.CreateScale(Scale) * physicsObject.Body.Orientation * Matrix.CreateTranslation(physicsObject.Body.Position);
                    }
                }
                return Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(position);
            }
        }

        public Material Material { get; set; }

        public string ModelName { get; set; }

        /// <summary>
        /// The absolute center of the model in 3d space.
        /// </summary>
        public Vector3 ModelCenter { get; set; }

        private float modelRadius;
        /// <summary>
        /// The absolute distance from center to outer edge of the model.
        /// </summary>
        public float ModelRadius
        {
            get { return modelRadius; }
            set { modelRadius = value; }
        }

        private Vector3 position = Vector3.Zero;
        /// <summary>
        /// The position of this object in 3d space.
        /// </summary>
        public virtual Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Scale of the object.
        /// </summary>
        public Vector3 Scale { get; set; }

        public NccSceneObject()
        {
            Rotation = Quaternion.Identity;
            Scale = Vector3.One;
            ModelCenter = Vector3.Zero;
            Material = new Material();
            ReadyToRender = false;
        }

        /// <summary>
        /// Yaw, pitch and roll of the object.
        /// </summary>
        public Quaternion Rotation { get; set; }

        public float Distance
        {
            get
            {
                if (ReadyToRender)
                    return Vector3.Distance(CameraManager.ActiveCamera.Position, World.Translation);
                return 0.0f;
            }

        }

        /// <summary>
        /// Whenever a new model is selected, we examine it to see how big
        /// it is and where it is centered. This lets us automatically zoom
        /// the display, so we can correctly handle models of any scale.
        /// </summary>
        public void MeasureModel()
        {
            if (ReadyToRender)
            {
                INccModel model = ModelManager.GetModel(ModelName);
                if (model != null && model.ReadyToRender)
                {


                    // Look up the absolute bone transforms for this model.
                    var transforms = new Matrix[model.BaseModel.Bones.Count];
                    model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

                    // Compute an (approximate) model center position by
                    // averaging the center of each mesh bounding sphere.
                    ModelCenter = Vector3.Zero;

                    foreach (var mesh in model.BaseModel.Meshes)
                    {
                        var meshBounds = mesh.BoundingSphere;
                        var transform = transforms[mesh.ParentBone.Index];
                        Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);

                        ModelCenter += meshCenter;
                    }

                    ModelCenter /= model.BaseModel.Meshes.Count;

                    // Now we know the center point, we can compute the model radius
                    // by examining the radius of each mesh bounding sphere.
                    modelRadius = 0;

                    foreach (ModelMesh mesh in model.BaseModel.Meshes)
                    {
                        BoundingSphere meshBounds = mesh.BoundingSphere;
                        Matrix transform = transforms[mesh.ParentBone.Index];
                        Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);

                        float transformScale = transform.Forward.Length();

                        float meshRadius = (meshCenter - ModelCenter).Length() + (meshBounds.Radius * transformScale);

                        modelRadius = Math.Max(modelRadius, meshRadius);
                    }
                }
            }
        }

        public virtual void Draw(GameTime gameTime)
        {

            if (ReadyToRender)
            {
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

                            //effect.DiffuseColor = new Vector3(0.5f, 1.0f, 0.5f);
                            // TODO Сделать так , что бы свет был от солнца позиции
                            //  effect.SpecularPower = 1000.0f;
                            //  effect.SpecularColor = new Vector3(1.0f, 0.5f, 0.5f);

                            BaseEngine.Device.DepthStencilState = new DepthStencilState {DepthBufferEnable = true};

                            //BaseEngine.Device.RasterizerState = new RasterizerState {FillMode = FillMode.WireFrame};

                            effect.World = transforms[mesh.ParentBone.Index] * World;
                            effect.View = CameraManager.ActiveCamera.View;
                            effect.Projection = CameraManager.ActiveCamera.Projection;
                        }
                        mesh.Draw();
                    }
                }
            }
        }

        public virtual void DrawCulling(GameTime gameTime)
        {

        }

        public override string ToString()
        {
            return "SceneObject";
        }
    }   

}