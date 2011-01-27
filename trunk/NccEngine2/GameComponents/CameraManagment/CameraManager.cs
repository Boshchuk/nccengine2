#region Using stastment

using System.Collections;
using Microsoft.Xna.Framework;

#endregion
namespace NccEngine2.GameComponents.CameraManagment
{
    public class CameraManager : GameComponent
    {
        public enum CameraNumber
        {
            /// <summary>
            /// Used as active camera
            /// </summary>
            Default = 1,
            Dolly = 2,
            ThreeNumber = 3,
            FourNumber = 4,
            FiveNumber = 5,
            SixNumber = 6,
            SevenNumber = 7,
            EightNumber = 8,
            NineNumber = 9,
            TenNumber = 10
        }

        private static readonly Hashtable Cameras = new Hashtable();

        /// <summary>
        /// Is the CameraManagers Initialized, used for test cases and setup of Effects.
        /// </summary>
        private static bool Initialized { get; set; }

        /// <summary>
        /// The camera where all the action takes place.
        /// </summary>
        public static Camera ActiveCamera { get; private set; }

        /// <summary>
        /// Create the camera Managers.
        /// </summary>
        /// <param name="game"></param>
        public CameraManager(Game game)
            : base(game)
        {
            Enabled = true;
        }

        static CameraManager()
        {
            Initialized = false;
        }

        /// <summary>
        /// Create the cameras.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            AddCamera(new FirstPersonCamera(), CameraNumber.Default);
            SetActiveCamera(CameraNumber.Default);
            //Note AddCamera(new FirstPersonCamera(), CameraNumber.Dolly);

            Initialized = true;
        }

        /// <summary>
        /// Update the active camera.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            ActiveCamera.Update(gameTime);
        }

        /// <summary>
        /// Adds a new camera to the CameraManagers
        /// </summary>
        /// <param name="newCamera"></param>
        /// <param name="cameraNumber"></param>
        public static void AddCamera(Camera newCamera, CameraNumber cameraNumber)
        {
            if (!Cameras.Contains(cameraNumber))
            {
                Cameras.Add(cameraNumber, newCamera);
            }
        }

        public static void SetCamerasFrustum(float near, float far, float aspectRatio)
        {
            foreach (Camera camera in Cameras.Values)
            {
                camera.Projection = Matrix.CreatePerspectiveFieldOfView(camera.FieldOfView, aspectRatio, near, far);
            }
        }

        /// <summary>
        /// Change the projection matrix of all cameras.
        /// </summary>
        /// <param name="aspectRatio"></param>
        public static void SetAllCamerasProjectionMatrix(float aspectRatio)
        {
            foreach (Camera camera in Cameras.Values)
            {
                camera.Projection = Matrix.CreatePerspectiveFieldOfView(camera.FieldOfView, aspectRatio, camera.NearPlane, camera.FarPlane);
            }
        }

        /// <summary>
        /// Changes the active camera by label
        /// </summary>
        /// <param name="cameraNumber"></param>
        public static void SetActiveCamera(CameraNumber cameraNumber)
        {
            if (Cameras.ContainsKey(cameraNumber))
            {
                ActiveCamera = Cameras[cameraNumber] as Camera;
            }
        }
    }
}