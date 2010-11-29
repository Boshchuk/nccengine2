using Microsoft.Xna.Framework;

namespace NccEngine2.GameComponents.Scene.SceneObject
{
    public interface INccSceneObject
    {
        Matrix World { get; }

        float Distance { get; }

        Vector3 Position { get; set; }

        void DrawCulling(GameTime gameTime);

        void Draw(GameTime gameTime);
    }
}