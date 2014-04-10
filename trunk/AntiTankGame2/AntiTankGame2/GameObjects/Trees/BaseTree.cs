using System;
using AntiTankGame2.Localization;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace AntiTankGame2.GameObjects.Trees
{
    using Microsoft.Xna.Framework.Graphics;

    public class BaseTree : BaseNccSceneObject, INccLoadable
    {
        protected NccModel Model;
        public void LoadContent()
        {
            Model = new NccModel(ContentConstants.TankHeightModel);
            ModelManager.AddModel(Model, ContentConstants.TankHeightModel);
            ModelName = ContentConstants.TankHeightModel;
            OcclusionModelName = ContentConstants.TankHeightModel;
        }

        public void UnloadContent()
        {
            GC.Collect();
        }
    }
}