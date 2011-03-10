using System;
using AntiTankGame2.Localization;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace AntiTankGame2.GameObjects.Trees
{
    public class BaseTree : BaseNccSceneObject, INccLoadable
    {
        //protected NccModel Model;
        public void LoadContent()
        {
            Model = new NccModel(ContentConstants.Tree3);
            ModelManager.AddModel(Model, ContentConstants.Tree3);
            ModelName = ContentConstants.Tree3;
            OcclusionModelName = ContentConstants.Tree3;
        }

        public void UnloadContent()
        {
            GC.Collect();
        }
    }
}