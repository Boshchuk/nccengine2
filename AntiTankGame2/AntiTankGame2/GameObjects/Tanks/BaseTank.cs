#region using statesment

using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.SceneObject;
#endregion

namespace AntiTankGame2.GameObjects.Tanks
{
    /// <summary>
    /// Base Tank we konw how draw and update
    /// </summary>
    public class BaseTank : BaseNccSceneObject
    {
        public override void Update(GameTime gameTime)
        {
            var model = ModelManager.GetModel(ModelName);
            if (model == null || !model.ReadyToRender || ReadyToRender) return;
            var transforms = new Matrix[model.BaseModel.Bones.Count];
            model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

            BoundingBox = new BoundingBox();

            foreach (var mesh in model.BaseModel.Meshes.Where(mesh => !BoundingBoxCreated))
            {
                BoundingBox = BoundingBox.CreateMerged(BoundingBox, BoundingBox.CreateFromSphere(mesh.BoundingSphere));
            }
            BoundingBoxCreated = true;

            var min = BoundingBox.Min;
            var max = BoundingBox.Max;

            BoundingBox = new BoundingBox(min, max);

            ReadyToRender = true;
        }

        public override void Draw(GameTime gameTime)
        {
            if (!ReadyToRender) return;
            BaseEngine.Device.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };

            var model = ModelManager.GetModel(ModelName);
            if (model == null || !model.ReadyToRender) return;
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