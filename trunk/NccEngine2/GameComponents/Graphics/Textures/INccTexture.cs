using Microsoft.Xna.Framework.Graphics;

namespace NccEngine2.GameComponents.Graphics.Textures
{
    /// <summary>
    /// A texture object for the NccEngine
    /// </summary>
    public interface INccTexture
    {
        /// <summary>
        /// ��� �����, � �������
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// ��������, ������� ������ ����� ������,
        /// </summary>
        Texture BaseTexture { get; }
        /// <summary>
        /// ���������� � ���������
        /// </summary>
        bool ReadyToRender { get; }

        void LoadContent();

        void UnloadContent();
    }
}