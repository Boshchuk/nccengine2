using System;
using AntiTankGame2.Localization;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;

namespace AntiTankGame2.GameObjects.Tanks
{
// ReSharper disable UnusedMember.Global
    public class BigTank : BaseTank, INccLoadable

    {
        public void LoadContent()
        {
            var model = new NccModel(ContentConstants.TankHeightModel);
            ModelManager.AddModel(model, ContentConstants.TankHeightModel);
            ModelName = ContentConstants.TankHeightModel;
            OcclusionModelName = ContentConstants.TankHeightModel;
        }

        public void UnloadContent()
        {
            GC.Collect();
        }
    }
    // ReSharper restore UnusedMember.Global
}