using Microsoft.Xna.Framework.Graphics;

namespace NccEngine2.GameComponents.Graphics.Textures
{
    /// <summary>
    /// A texture object for the NccEngine
    /// </summary>
    public interface INccTexture
    {
        /// <summary>
        /// Имя файля, с которым
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Текстура, которую должен иметь объект,
        /// </summary>
        Texture BaseTexture { get; }
        /// <summary>
        /// Готовность к отрисовке
        /// </summary>
        bool ReadyToRender { get; }

        void LoadContent();

        void UnloadContent();
    }
}