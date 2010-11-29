using Microsoft.Xna.Framework;

namespace NccEngine2.GameComponents.CameraManagment
{
    public class Camera
    {
        /// <summary>
        /// Postition of the camera.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The viewable angle.
        /// </summary>
        public float FieldOfView { get; set; }

        public float Yaw { get; set; }

        public float Pitch { get; set; }

        /// <summary>
        /// The near plane used to determine the viewable area.
        /// </summary>
        public float NearPlane { get; set; }

        /// <summary>
        /// The far plane used to determine the viewable area.
        /// </summary>
        public float FarPlane { get; set; }

        /// <summary>
        /// Slightly smaller viewable field of view for culling.
        /// </summary>
        public float ViewableFieldOfView
        {
            get { return FieldOfView / 1.125f; }
        }

        /// <summary>
        /// Matrix containing coordinates of the camera.
        /// </summary>
        public Matrix View { get; set; }

        /// <summary>
        /// Reflected View matrix around an arbitrary plane.
        /// </summary>
        public Matrix ReflectedView { get; set; }

        /// <summary>
        /// The projection matrix, what can be seen.
        /// </summary>
        public Matrix Projection { get; set; }

        /// <summary>
        /// The trapezoid that contains everything that the camera can see.
        /// </summary>
        public BoundingFrustum Frustum { get; set; }

        public Camera()
        {
            FarPlane = 3500.0f;
            NearPlane = 0.1f;
            FieldOfView = MathHelper.Pi / 3.0f;
            Position = Vector3.Zero;
        }

        /// <summary>
        /// The trapezoid that contains everything that the camera can see if it was reflected.
        /// </summary>
        public BoundingFrustum ReflectedFrustum { get; set; }

        public virtual void Update(GameTime gameTime)
        { }

        public virtual void SetPosition(Vector3 newPosition)
        { }

        public virtual void SetReference(Vector3 newReference)
        { }

        public virtual void Translate(Vector3 move)
        { }

        public virtual void RotateX(float angle)
        { }

        public virtual void RotateY(float angle)
        { }

        public virtual void RotateZ(float angle)
        { }

        public virtual void Reset()
        { }
    }
}