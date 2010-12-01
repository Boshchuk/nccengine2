using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Scene.Graph;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace AntiTankGame2.GameObjects.Terrain
{
    /// <summary>
    /// The simle plane from fbx file
    /// </summary>
    public class SimplePlane : BaseNccSceneObject, INccLoadable //NccSceneObject, INccLoadable, INccUpdateable, INccOcclusion
    {
        public void LoadContent()
        {
            var planeModel = new NccModel("Content/Models/Land/plane");
            ModelManager.AddModel(planeModel, "simlePlane");
            ModelName = "simlePlane";
            OcclusionModelName = "simlePlane";
        }

        public void UnloadContent()
        {
            System.GC.Collect();
        }

       
    }

}