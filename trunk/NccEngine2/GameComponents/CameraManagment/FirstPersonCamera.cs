using Microsoft.Xna.Framework;

namespace NccEngine2.GameComponents.CameraManagment
{
    public class FirstPersonCamera : Camera
    {
        /// <summary>
        /// The spot in 3d space where the camera is looking.
        /// </summary>
        public Vector3 CameraReference { get; private set; }

        public FirstPersonCamera()
        {
            CameraReference = new Vector3(0, 0, 10);
        }

        /// <summary>
        /// Set the position in 3d space.
        /// </summary>
        /// <param name="newPosition"></param>
        public override void SetPosition(Vector3 newPosition)
        {
            Position = newPosition;
        }

        /// <summary>
        /// Set the point in 3d space where the camera is looking.
        /// </summary>
        /// <param name="newReference"></param>
        public override void SetReference(Vector3 newReference)
        {
            CameraReference = newReference;
        }

        /// <summary>
        /// Move the camera in 3d space.
        /// </summary>
        /// <param name="move"></param>
        public override void Translate(Vector3 move)
        {
            var forwardMovement = Matrix.CreateRotationY(Yaw);
            var v = Vector3.Transform(move, forwardMovement);

            Position += new Vector3(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Rotate around the Y, Default Up axis.  Usually called Yaw.
        /// </summary>
        /// <param name="angle">Angle in degrees to rotate the camera.</param>
        public override void RotateY(float angle)
        {
            angle = MathHelper.ToRadians(angle);
            if (Yaw >= MathHelper.Pi * 2)
                Yaw = MathHelper.ToRadians(0.0f);
            else if (Yaw <= -MathHelper.Pi * 2)
                Yaw = MathHelper.ToRadians(0.0f);
            Yaw += angle;
        }

        /// <summary>
        /// Rotate the camera around the X axis.  Usually called pitch.
        /// </summary>
        /// <param name="angle">Angle in degrees to rotate the camera.</param>
        public override void RotateX(float angle)
        {
            angle = MathHelper.ToRadians(angle);
            Pitch += angle;
            if (Pitch >= MathHelper.ToRadians(75))
                Pitch = MathHelper.ToRadians(75);
            else if (Pitch <= MathHelper.ToRadians(-75))
                Pitch = MathHelper.ToRadians(-75);
        }

        public override void Update(GameTime gameTime)
        {
            var cameraPosition = Position;
            var rotationMatrix = Matrix.CreateRotationY(Yaw);
            var pitchMatrix = Matrix.Multiply(Matrix.CreateRotationX(Pitch), rotationMatrix);
            var transformedReference = Vector3.Transform(CameraReference, pitchMatrix);
            var cameraLookat = cameraPosition + transformedReference;

            View = Matrix.CreateLookAt(cameraPosition, cameraLookat, Vector3.Up);

            Frustum = new BoundingFrustum(Matrix.Multiply(View, Projection));
            ReflectedFrustum = new BoundingFrustum(Matrix.Multiply(ReflectedView, Projection));
        }
    }
}
