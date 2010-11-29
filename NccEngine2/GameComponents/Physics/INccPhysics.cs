using JigLibX.Collision;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace NccEngine2.GameComponents.Physics
{
    /// <summary>
    /// Every NccSceneObject that implements this interface will be added to the physics manager
    /// </summary>
    public interface INccPhysics : INccSceneObject
    {
        Body Body { get; }

        CollisionSkin CollisionSkin { get; }

        Vector3 SetMass(float mass);
    }
}