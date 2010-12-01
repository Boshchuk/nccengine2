using System;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace AntiTankGame2.GameObjects
{
    public class SimpleSkybox : BaseNccSceneObject, INccLoadable
    {
        public void LoadContent()
        {
            var planeModel = new NccModel("Content/Skybox/skybox2");
            ModelManager.AddModel(planeModel, "skybox2");
            ModelName = "skybox2";
            OcclusionModelName = "skybox2";
        }

        public void UnloadContent()
        {
            GC.Collect();
        }
    }
}