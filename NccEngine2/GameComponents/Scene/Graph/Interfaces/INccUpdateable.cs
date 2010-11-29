using Microsoft.Xna.Framework;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace NccEngine2.GameComponents.Scene.Graph.Interfaces
{
    /// <summary>
    /// Allows an NccSceneObject to be Updated
    /// </summary>
    public interface INccUpdateable : INccSceneObject
    {
        void Update(GameTime gameTime);
    }
}