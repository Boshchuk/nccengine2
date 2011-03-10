using AntiTankGame2.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Models;

namespace AntiTankGame2.GameObjects.Trees
{
// ReSharper disable UnusedMember.Global
    public class HeightTree : BaseTree
    {
        protected Matrix[] BoneTransforms;
        // This constant controls how quickly the tank can move forward and backward
        const float TankVelocity = 0.5f;

        // The radius of the tank's wheels. This is used when we calculate how fast they
        // should be rotating as the tank moves.
        const float TankWheelRadius = 18;

        // controls how quickly the tank can turn from side to side.
        const float TankTurnSpeed = .025f;

        /// <summary>
        /// The direction that the tank is facing, in radians. This value will be used
        /// to position and and aim the camera.
        /// </summary>
        // ReSharper disable MemberCanBePrivate.Global
        public float FacingDirection { get; set; }
        // ReSharper restore MemberCanBePrivate.Global
        readonly DepthStencilState depthStencilState = new DepthStencilState { DepthBufferEnable = true };

        // how is the tank oriented? We'll calculate this based on the user's input and
        // the heightmap's normals, and then use it when drawing.

        Matrix orientation = Matrix.Identity;

        // we'll use this value when making the wheels roll. It's calculated based on 
        // the distance moved.
        Matrix wheelRollMatrix = Matrix.Identity;

        
        private readonly HeightMapInfo heightMapInfo;
        public HeightTree(HeightMapInfo heightMapInfoParam, Vector3 newPosition)
        {
            Position = newPosition;
            Scale = new Vector3(20, 20, 20);
            heightMapInfo = heightMapInfoParam;

            const string tempModelName = "tempHeightModelTree";

            var tempModel = new NccModel
                                {
                                    BaseModel = BaseEngine.ContentManager.Load<Model>(ContentConstants.Terrain)
                                };
            ModelManager.AddModel(tempModel,tempModelName );
         
            heightMapInfo = ModelManager.GetModel(tempModelName).BaseModel.Tag as HeightMapInfo;

           
        }

        public override void Update(GameTime gameTime)
        {
            //var currentGamePadState = input.CurrentGamePadState;
            //var currentKeyboardState = input.CurrentKeyboardState;

            //var part = 25 / BaseEngine.DebugSystem.FpsCounter.Fps;
            var part = gameTime.ElapsedGameTime.Milliseconds;

            var turnSpeed = (float)0.04 * part;
            var moveSpeed = (float)0.80 * part;

            // First, we want to check to see if the tank should turn. turnAmount will 
            // be an accumulation of all the different possible inputs.
            //var turnAmount = -currentGamePadState.ThumbSticks.Left.X;
            //if (currentKeyboardState.IsKeyDown(Keys.NumPad4) || currentGamePadState.DPad.Left == ButtonState.Pressed)
            //{
            //    turnAmount += turnSpeed;
            //}

            //if (currentKeyboardState.IsKeyDown(Keys.NumPad6) || currentGamePadState.DPad.Right == ButtonState.Pressed)
            //{
            //    turnAmount -= turnSpeed;
            //}

            // clamp the turn amount between -1 and 1, and then use the finished
            // value to turn the tank.
            //turnAmount = MathHelper.Clamp(turnAmount, -1, 1);
            //FacingDirection += turnAmount * TankTurnSpeed;


            // Next, we want to move the tank forward or back. to do this, 
            // we'll create a Vector3 and modify use the user's input to modify the Z
            // component, which corresponds to the forward direction.
            var movement = Vector3.Zero;
            //movement.Z = -currentGamePadState.ThumbSticks.Left.Y;

            //if (currentKeyboardState.IsKeyDown(Keys.NumPad8) || currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                movement.Z = -moveSpeed;
            }
            //if (currentKeyboardState.IsKeyDown(Keys.NumPad5) || currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                movement.Z = moveSpeed;
            }

            // next, we'll create a rotation matrix from the direction the tank is 
            // facing, and use it to transform the vector.
            orientation = Matrix.CreateRotationY(FacingDirection);
            var velocity = Vector3.Transform(movement, orientation);
            velocity *= TankVelocity;

            // Now we know how much the user wants to move. We'll construct a temporary
            // vector, newPosition, which will represent where the user wants to go. If
            // that value is on the heightmap, we'll allow the move.
            var newPosition = Position + velocity;
            if (!heightMapInfo.IsOnHeightmap(newPosition)) return;
            // now that we know we're on the heightmap, we need to know the correct
            // height and normal at this position.
            Vector3 normal;
            heightMapInfo.GetHeightAndNormal(newPosition, out newPosition.Y, out normal);


            // As discussed in the doc, we'll use the normal of the heightmap
            // and our desired forward direction to recalculate our orientation
            // matrix. It's important to normalize, as well.
            orientation.Up = normal;

            orientation.Right = Vector3.Cross(orientation.Forward, orientation.Up);
            orientation.Right = Vector3.Normalize(orientation.Right);

            orientation.Forward = Vector3.Cross(orientation.Up, orientation.Right);
            orientation.Forward = Vector3.Normalize(orientation.Forward);

            // now we need to roll the tank's wheels "forward." to do this, we'll
            // calculate how far they have rolled, and from there calculate how much
            // they must have rotated.
            var distanceMoved = Vector3.Distance(Position, newPosition);
            var theta = distanceMoved / TankWheelRadius;
            var rollDirection = movement.Z > 0 ? 1 : -1;

            wheelRollMatrix *= Matrix.CreateRotationX(theta * rollDirection);

            // once we've finished all computations, we can set our position to the
            // new position that we calculated.
            Position = newPosition;
        }

        public override void Draw(GameTime gameTime)
        {
            //if (!ReadyToRender) return;
            BaseEngine.Device.DepthStencilState = depthStencilState;

            var worldMatrix = orientation * Matrix.CreateTranslation(Position);

            var model = ModelManager.GetModel(ModelName);
            if (model == null || !model.ReadyToRender) return;
            //var turretRotation = Matrix.CreateRotationY(TurretRotation);
            BoneTransforms = new Matrix[model.BaseModel.Bones.Count];
            //TurretBone.Transform = turretRotation * TurretTransform;
            //var bonesTransforms = new Matrix[model.BaseModel.Bones.Count];
            model.BaseModel.CopyAbsoluteBoneTransformsTo(BoneTransforms);

            foreach (var mesh in model.BaseModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = BoneTransforms[mesh.ParentBone.Index] * worldMatrix;
                    effect.View = CameraManager.ActiveCamera.View;
                    effect.Projection = CameraManager.ActiveCamera.Projection;

                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
    // ReSharper restore UnusedMember.Global
}