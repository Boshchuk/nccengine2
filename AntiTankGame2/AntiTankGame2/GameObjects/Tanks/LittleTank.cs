using System;
using AntiTankGame2.Localization;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;

namespace AntiTankGame2.GameObjects.Tanks
{
    public class LittleTank : BaseTank, INccLoadable
    {
        public void LoadContent()
        {
            var model = new NccModel(ContentConstants.TankHeightModel);
            ModelManager.AddModel(model, ContentConstants.TankModelName);
            ModelName = ContentConstants.TankModelName;
            OcclusionModelName = ContentConstants.TankModelName;
        }

        public void UnloadContent()
        {
            GC.Collect();
        }
    }
}