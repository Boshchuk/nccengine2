using Microsoft.Xna.Framework.Graphics;

namespace NccEngine2.GameComponents.Models
{
    /// <summary>
    /// Describes a model for the NccEngine.
    /// </summary>
    public interface INccModel
    {
        string FileName { get; set; }

        Model BaseModel { get; }

        bool ReadyToRender { get; }

        void LoadContent();
    }
}