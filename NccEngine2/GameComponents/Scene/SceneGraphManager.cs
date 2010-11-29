using System;
using System.Threading;
using Microsoft.Xna.Framework;
using NccEngine.GameComponents;
using NccEngine2.GameComponents.Physics;
using NccEngine2.GameComponents.Scene.Graph;
using NccEngine2.GameComponents.Scene.SceneObject;

namespace NccEngine2.GameComponents.Scene
{
    public class SceneGraphManager : GameComponent
    {
        public static bool Paused { get; set; }

        public static int Occluded;
        public static int Culled;

        /// <summary>
        /// The root of the scene graph
        /// </summary>
        public static Node Root { get; private set; }

        public static bool UseSweepAndPrune
        {
            get { return PhysicsManager.UseSweepAndPrune; }
            set { PhysicsManager.UseSweepAndPrune = value; }
        }

        public static bool DrawDebugger
        {
            get { return PhysicsManager.DrawDebugger; }
            set { PhysicsManager.DrawDebugger = value; }
        }

        /// <summary>
        /// Create the scenegraph Managers.
        /// </summary>
        /// <param name="game"></param>
        public SceneGraphManager(Game game)
            : base(game)
        {
            Enabled = true;
            Root = new Node();
        }

        /// <summary>
        /// Draw objects
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Draw(GameTime gameTime)
        {
            Root.Draw(gameTime);
        }

        /// <summary>
        /// Draw occlusion culling objects and test frustum culling
        /// </summary>
        /// <param name="gameTime"></param>
        public static void DrawCulling(GameTime gameTime)
        {
            Root.Sort();
            Culled = 0;
            Occluded = 0;
            Root.DrawCulling(gameTime);
        }

        /// <summary>
        /// Handle inupt from the user.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="input"></param>
        public static void HandleInput(GameTime gameTime, Input input)
        {
            Paused = false;
            Root.HandleInput(gameTime, input);
        }

        /// <summary>
        /// Update game objects.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //base.Update(gameTime);
            ThreadStart physicsThreadStart = delegate
            {
                if (!Paused)
                {
                    PhysicsManager.Update(gameTime);
                }
            };
            var physicsThread = new Thread(physicsThreadStart);
            physicsThread.Start();

            Root.Update(gameTime);

            physicsThread.Join();
        }

        /// <summary>
        /// Load the content of all the objects in the scenegraph
        /// </summary>
        public static void LoadContent()
        {
            Root.LoadContent();
        }

        /// <summary>
        /// Unload the content of all the objects in the scenegraph
        /// </summary>
        public static void UnloadContent()
        {
            Root.UnloadContent();
        }

        /// <summary>
        /// Add an object to the scenegraph.
        /// </summary>
        /// <param name="newObject"></param>
        public static void AddObject(NccSceneObject newObject)
        {
            var node = new SceneObjectNode(newObject);
            Root.AddNode(node);
        }

        public static void AddObject(INccSceneObject newObject)
        {
            var node = new SceneObjectNode(newObject);
            Root.AddNode(node);
        }

        public static Node GetObject(string key)
        {
            return Root.GetNode(key);
        }

        public static Node GetObject(Guid key)
        {
            return Root.GetNode(key);
        }

        public static void ClearAllObjects()
        {
            Root.UnloadContent();
            PhysicsManager.UnloadContent();
            GC.Collect();
        }

        public static void ClearSelectedObject(NccSceneObject obj)
        {
            Root.UnloadContent(obj);
            GC.Collect();
        }

        public static void ClearSelectedObject(INccSceneObject obj)
        {
            Root.UnloadContent(obj);
            GC.Collect();
        }
    }

}