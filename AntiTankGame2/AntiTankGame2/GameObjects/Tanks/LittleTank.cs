#region using statsment
using System;
using AntiTankGame2.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;
#endregion

namespace AntiTankGame2.GameObjects.Tanks
{
    public class LittleTank : BaseTank, INccLoadable
    {
        #region bones

        // Shortcut references to the bones that we are going to animate.
        // We could just look these up inside the Draw method, but it is more
        // efficient to do the lookups while loading and cache the results.
        protected ModelBone TurretBone;
        private ModelBone bodyBone;
        #endregion

        #region Transform Matrixes
        // Store the original transform matrix for each animating bone.
        protected Matrix TurretTransform;
// ReSharper disable UnaccessedField.Local
        private Matrix bodyTransform;
// ReSharper restore UnaccessedField.Local

        // Array holding all the bone transform matrices for the entire model.
        // We could just allocate this locally inside the Draw method, but it
        // is more efficient to reuse a single array, as this avoids creating
        // unnecessary garbage.
        protected Matrix[] BoneTransforms;

        #endregion

        #region Rotation

        // Current animation positions.
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
        protected float TurretRotation { get; private set; }
// ReSharper restore UnusedAutoPropertyAccessor.Local
// ReSharper restore UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
        public float BodyRotation { get; set; }
// ReSharper restore UnusedMember.Global

        #endregion

        #region INccLoadable members

        public void LoadContent()
        {
            var model = new NccModel(ContentConstants.TankHeightModel);
            ModelManager.AddModel(model, ContentConstants.TankModelName);
            ModelName = ContentConstants.TankModelName;
            OcclusionModelName = ContentConstants.TankModelName;

            // Look up shortcut references to the bones we are going to animate.

            TurretBone = model.BaseModel.Bones["turret"];
            bodyBone = model.BaseModel.Bones["body"];

            TurretTransform = TurretBone.Transform;
            bodyTransform = bodyBone.Transform;

            BoneTransforms = new Matrix[model.BaseModel.Bones.Count];
        }

        public void UnloadContent()
        {
            GC.Collect();
        }

        #endregion
    }
}