using AntiTankGame2.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace AntiTankGame2.GameObjects.Tanks
{
    /// <summary>
    /// Ёкземпл€ры этого класса могут тусоватьс€ на карте высот
    /// </summary>
// ReSharper disable UnusedMember.Global
    public class HeightBaseObject : NccSceneObject
// ReSharper restore UnusedMember.Global
    {
        #region Properties

        /// <summary>
        /// The direction that the object is facing, in radians. This value will be used
        /// to position and and aim the camera.
        /// </summary>
// ReSharper disable UnusedMember.Global
        public float FacingDirection { get; set; }
// ReSharper restore UnusedMember.Global

        // how is the tank oriented? We'll calculate this based on the user's input and
        // the heightmap's normals, and then use it when drawing.

        readonly Matrix orientation = Matrix.Identity;

// ReSharper disable UnaccessedField.Local
        private readonly HeightMapInfo heightMapInfo;
// ReSharper restore UnaccessedField.Local

        #endregion

        #region construtcors
        // ReSharper disable AccessToStaticMemberViaDerivedType
        public HeightBaseObject(HeightMapInfo heightMapInfoParam, Vector3 newPosition)
        {
            Position = newPosition;

            Scale = new Vector3(0.01f, 0.01f, 0.1f);
            heightMapInfo = heightMapInfoParam;

            const string tempModelName = "tempHeightModel";

            var tempModel = new NccModel
                                {
                                    BaseModel = EngineManager.ContentManager.Load<Model>(ContentConstants.Terrain)
                                };
            ModelManager.AddModel(tempModel,tempModelName );
         
            heightMapInfo = ModelManager.GetModel(tempModelName).BaseModel.Tag as HeightMapInfo;
            
        }
        #endregion

        readonly DepthStencilState depthStencilState = new DepthStencilState { DepthBufferEnable = true };

        public override void Draw(GameTime gameTime)
        {
            if (!ReadyToRender) return;
            BaseEngine.Device.DepthStencilState = depthStencilState;

            var worldMatrix = orientation * Matrix.CreateTranslation(Position);


            var model = ModelManager.GetModel(ModelName);
            if (model == null || !model.ReadyToRender) return;
            var bonesTransforms = new Matrix[model.BaseModel.Bones.Count];
            model.BaseModel.CopyAbsoluteBoneTransformsTo(bonesTransforms);

            foreach (var mesh in model.BaseModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {

                    effect.World = bonesTransforms[mesh.ParentBone.Index] * worldMatrix;
                    effect.View = CameraManager.ActiveCamera.View;
                    effect.Projection = CameraManager.ActiveCamera.Projection;

                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}