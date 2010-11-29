using Microsoft.Xna.Framework;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace NccEngine2.GameComponents.Scene.Graph.Interfaces
{
    /// <summary>
    /// Every NccSceneObject that implements this interface will be culled using its bounding object.
    /// </summary>
    public interface INccCullable : INccSceneObject
    {

        bool Culled { get; set; }

        bool BoundingBoxCreated { get; }

        BoundingBox BoundingBox { get; }

        BoundingBox GetBoundingBoxTransformed();
    }

}