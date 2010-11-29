using NccEngine2.GameComponents.Scene.SceneObject;

namespace NccEngine2.GameComponents.Scene.Graph.Interfaces
{
    /// <summary>
    /// Allows an NccSceneObject to be loaded.
    /// </summary>
    public interface INccLoadable : INccSceneObject
    {
        void LoadContent();
        void UnloadContent();
    }
}