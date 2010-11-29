using JigLibX.Collision;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace NccEngine2.GameComponents.Physics
{
    internal class PhysicsManager : GameComponent
    {
        private static PhysicsSystem physicsSystem;
        private static int collisionIndex;

        public static bool UseSweepAndPrune { get; set; }

        public static bool DrawDebugger { get; set; }

        /// <summary>
        /// Is the PhysicsManagers Initialized, used for test cases and setup of Effects.
        /// </summary>
        public static bool Initialized { get; private set; }

        /// <summary>
        /// Create the Physics Managers.
        /// </summary>
        /// <param name="game"></param>
        public PhysicsManager(Game game)
            : base(game)
        {
            Enabled = false;
        }

        static PhysicsManager()
        {
            Initialized = false;
        }

        public override void Initialize()
        {
            base.Initialize();

            physicsSystem = new PhysicsSystem
            {
                CollisionSystem = new CollisionSystemGrid(32, 32, 4, 13, 13, 13),
                Gravity = new Vector3(0, -5.0f, 0.0f),
                EnableFreezing = true,
                SolverType = PhysicsSystem.Solver.Accumulated
            };
            physicsSystem.CollisionSystem.UseSweepTests = true;

            physicsSystem.NumCollisionIterations = 2;
            physicsSystem.NumContactIterations = 5;
            physicsSystem.NumPenetrationRelaxtionTimesteps = 20;

            Initialized = true;
        }

        new public static void Update(GameTime gameTime)
        {
            physicsSystem.Integrate(1.0f / 60.0f);
        }

        public static void UnloadContent()
        {
            for (var i = 0; i < physicsSystem.Controllers.Count; i++)
            {
                physicsSystem.Controllers[0].DisableController();
                physicsSystem.RemoveConstraint(physicsSystem.Constraints[0]);
            }
        }

        public static void RemoveObject(INccPhysics physicsObject)
        {
            physicsObject.Body.DisableBody();
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.RemoveCollisionSkin(physicsObject.CollisionSkin);

            foreach (var body in PhysicsSystem.CurrentPhysicsSystem.Bodies)
            {
                body.SetActive();
            }
        }
    }
}