using Microsoft.Xna.Framework;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.NccInput;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace NccEngine2.GameComponents.Scene.Graph
{
    class SceneObjectNode : Node
    {
        // public NccSceneObject SceneObject { get; set; }

        public INccSceneObject SceneObject { get; set; }

        public SceneObjectNode(INccSceneObject newObject)
        {
            SceneObject = newObject;
        }
        public override void HandleInput(GameTime gameTime, Input input)
        {
            if (SceneObject is IAcceptNccInput)
                ((IAcceptNccInput)SceneObject).HandleInput(gameTime, input);
        }

        public override void Update(GameTime gameTime)
        {
            if (SceneObject is INccUpdateable)
                ((INccUpdateable)SceneObject).Update(gameTime);
        }

        public override void UnloadContent()
        {
            if (SceneObject is INccLoadable)
                ((INccLoadable)SceneObject).UnloadContent();
        }

        public override void LoadContent()
        {
            if (SceneObject is INccLoadable)
                ((INccLoadable)SceneObject).LoadContent();
        }

        public override void DrawCulling(GameTime gameTime)
        {
            if (SceneObject is INccCullable)
            {
                ((INccCullable)SceneObject).Culled = false;
                if (CameraManager.ActiveCamera.Frustum.Contains(((INccCullable)SceneObject).GetBoundingBoxTransformed()) == ContainmentType.Disjoint)
                {
                    ((INccCullable)SceneObject).Culled = true;
                }
                else
                {
                    SceneObject.DrawCulling(gameTime);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (SceneObject is INccCullable && ((INccCullable)SceneObject).Culled)
            {
                SceneGraphManager.Culled++;
            }
            else if (SceneObject is INccOcclusion && ((INccOcclusion)SceneObject).Occluded)
            {
                SceneGraphManager.Occluded++;
            }
            else
            {
                SceneObject.Draw(gameTime);
            }
        }
    }
}