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
        protected ModelBone turretBone;
        protected ModelBone bodyBone;
        #endregion

        #region Transform Matrixes
        // Store the original transform matrix for each animating bone.
        protected Matrix turretTransform;
        protected Matrix bodyTransform;

        // Array holding all the bone transform matrices for the entire model.
        // We could just allocate this locally inside the Draw method, but it
        // is more efficient to reuse a single array, as this avoids creating
        // unnecessary garbage.
        protected Matrix[] boneTransforms;

        #endregion

        #region Rotation

        // Current animation positions.
        public float TurretRotation { get; set; }
        public float BodyRotation { get; set; }

        #endregion

        #region INccLoadable members

        public void LoadContent()
        {
            var model = new NccModel(ContentConstants.TankHeightModel);
            ModelManager.AddModel(model, ContentConstants.TankModelName);
            ModelName = ContentConstants.TankModelName;
            OcclusionModelName = ContentConstants.TankModelName;

            // Look up shortcut references to the bones we are going to animate.

            turretBone = model.BaseModel.Bones["turret"];
            bodyBone = model.BaseModel.Bones["body"];

            turretTransform = turretBone.Transform;
            bodyTransform = bodyBone.Transform;

            boneTransforms = new Matrix[model.BaseModel.Bones.Count];
        }

        public void UnloadContent()
        {
            GC.Collect();
        }

        #endregion
    }
}