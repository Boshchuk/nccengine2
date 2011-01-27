using System;
using Microsoft.Xna.Framework;

namespace NccEngine2.GameComponents.CameraManagment
{
    public class Camera
    {
        #region fields

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

        private Matrix view;

        /// <summary>
        /// Matrix containing coordinates of the camera.
        /// </summary>
        public Matrix View
        {
            get
            {

                //// Start with our regular position and target
                //Vector3 position =  Position;
                //Vector3 target = Vector3.Zero; //Target;

                //// If we're shaking, add our offset to our position and target
                //if (shaking)
                //{
                //    position += shakeOffset;
                //    target += shakeOffset;
                //}

                //// Return the matrix using our modified position and target
                //return Matrix.CreateLookAt(position, target, view.Up);
                return view;
            }
            set { view = value; }
        }

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

        #endregion


        #region Virtual

        public virtual void Update(GameTime gameTime)
        {
            // If we're shaking...
            if (shaking)
            {
                // Move our timer ahead based on the elapsed time
                shakeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // If we're at the max duration, we're not going to be shaking anymore
                if (shakeTimer >= shakeDuration)
                {
                    shaking = false;
                    shakeTimer = shakeDuration;
                }

                // Compute our progress in a [0, 1] range
                float progress = shakeTimer / shakeDuration;

                // Compute our magnitude based on our maximum value and our progress. This causes
                // the shake to reduce in magnitude as time moves on, giving us a smooth transition
                // back to being stationary. We use progress * progress to have a non-linear fall 
                // off of our magnitude. We could switch that with just progress if we want a linear 
                // fall off.
                float magnitude = shakeMagnitude * (1f - (progress * progress));

                // Generate a new offset vector with three random values and our magnitude
                shakeOffset = new Vector3(NextFloat(), NextFloat(), NextFloat()) * magnitude;
            }
        }

        public virtual void SetPosition(Vector3 newPosition)
        { }

        public virtual void SetReference(Vector3 newReference)
        { }

        public virtual void Translate(Vector3 move)
        { }

        /// <summary>
        /// Horisonral Rotation
        /// </summary>
        /// <param name="angle">angle to rotate</param>
        public virtual void RotateX(float angle)
        { }
        /// <summary>
        /// Rotate Vertical
        /// </summary>
        /// <param name="angle">angle to rotate</param>
        public virtual void RotateY(float angle)
        { }

        public virtual void RotateZ(float angle)
        { }

        public virtual void Reset()
        { }

        #endregion

        private bool shaking;

        private float shakeMagnitude;
        private float shakeDuration;
        private float shakeTimer;
        private Vector3 shakeOffset;

        // We only need one Random object no matter how many Cameras we have
        private static readonly Random random = new Random();

        /// <summary>
        /// Shakes the camera with a specific magnitude and duration.
        /// </summary>
        /// <param name="magnitude">The largest magnitude to apply to the shake.</param>
        /// <param name="duration">The length of time (in seconds) for which the shake should occur.</param>
        public void Shake(float magnitude, float duration)
        {
            // We're now shaking
            shaking = true;

            // Store our magnitude and duration
            shakeMagnitude = magnitude;
            shakeDuration = duration;

            // Reset our timer
            shakeTimer = 0f;
        }

        /// <summary>
        /// Helper to generate a random float in the range of [-1, 1].
        /// </summary>
        private float NextFloat()
        {
            return (float)random.NextDouble() * 2f - 1f;
        }
    }
}