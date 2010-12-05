using Microsoft.Xna.Framework;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace NccEngine2.GameComponents.NccInput
{
    /// <summary>
    /// Allows a NccSceneObject to handle input.
    /// </summary>
    public interface IAcceptNccInput : INccSceneObject
    {
        void HandleInput(GameTime gameTime, Input input);
    }

}