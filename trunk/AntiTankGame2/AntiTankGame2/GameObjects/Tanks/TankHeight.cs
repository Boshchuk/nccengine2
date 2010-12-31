using AntiTankGame2.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NccEngine2;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.NccInput;

namespace AntiTankGame2.GameObjects.Tanks
{
    /// <summary>
    /// This tank can be placed on Height Map
    /// </summary>
    sealed class TankHeight : LittleTank, IAcceptNccInput
    {
        #region Constants

        // This constant controls how quickly the tank can move forward and backward
        const float TankVelocity = 0.5f;

        // The radius of the tank's wheels. This is used when we calculate how fast they
        // should be rotating as the tank moves.
        const float TankWheelRadius = 18;

        // controls how quickly the tank can turn from side to side.
        const float TankTurnSpeed = .025f;


        #endregion

        #region Properties

        /// <summary>
        /// The direction that the tank is facing, in radians. This value will be used
        /// to position and and aim the camera.
        /// </summary>
        public float FacingDirection { get; set; }

        #endregion

        #region Fields

        // how is the tank oriented? We'll calculate this based on the user's input and
        // the heightmap's normals, and then use it when drawing.

        Matrix orientation = Matrix.Identity;

        // we'll use this value when making the wheels roll. It's calculated based on 
        // the distance moved.
        Matrix wheelRollMatrix = Matrix.Identity;
             
        private readonly HeightMapInfo heightMapInfo;

        
       #endregion
      
        #region construtcors
        // ReSharper disable AccessToStaticMemberViaDerivedType
        public TankHeight(HeightMapInfo heightMapInfoParam, Vector3 newPosition)
        {
            Position = newPosition;
            Scale = new Vector3(0.01f, 0.01f, 0.1f);
            heightMapInfo = heightMapInfoParam;

            const string tempModelName = "tempHeightModel";

            var tempModel = new NccModel
                                {
                                    BaseModel = EngineManager.ContentManager.Load<Model>(ContentConstants.Terrain)
                                };
            ModelManager.AddModel(tempModel,tempModelName );
         
            heightMapInfo = ModelManager.GetModel(tempModelName).BaseModel.Tag as HeightMapInfo;
            
        }
        #endregion
      

        /// <summary>
        /// This function is called when the game is Updating in response to user input.
        /// It'll move the tank around the heightmap, and update all of the tank's 
        /// necessary state.
        /// </summary>
        public void HandleInput(GameTime gameTime, Input input)
        {
            var currentGamePadState = input.CurrentGamePadState;
            var currentKeyboardState = input.CurrentKeyboardState;

            var part = 25 / BaseEngine.DebugSystem.FpsCounter.Fps;

            var turnSpeed = 4*part;
            var moveSpeed = 40*part;

            // First, we want to check to see if the tank should turn. turnAmount will 
            // be an accumulation of all the different possible inputs.
            var turnAmount = -currentGamePadState.ThumbSticks.Left.X;
            if (currentKeyboardState.IsKeyDown(Keys.NumPad4) || currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                turnAmount += turnSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.NumPad6) || currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                turnAmount -= turnSpeed;
            }

            // clamp the turn amount between -1 and 1, and then use the finished
            // value to turn the tank.
            //turnAmount = MathHelper.Clamp(turnAmount, -1, 1);
            FacingDirection += turnAmount * TankTurnSpeed;


            // Next, we want to move the tank forward or back. to do this, 
            // we'll create a Vector3 and modify use the user's input to modify the Z
            // component, which corresponds to the forward direction.
            var movement = Vector3.Zero;
            movement.Z = -currentGamePadState.ThumbSticks.Left.Y;

            if (currentKeyboardState.IsKeyDown(Keys.NumPad8) || currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                movement.Z = -moveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.NumPad5) || currentGamePadState.DPad.Down == ButtonState.Pressed)
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
            if (heightMapInfo.IsOnHeightmap(newPosition))
            {
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
            
        }

        public override void Draw(GameTime gameTime)
        {
            if (!ReadyToRender) return;
            BaseEngine.Device.DepthStencilState = new DepthStencilState {DepthBufferEnable = true};
                
            Matrix worldMatrix = orientation * Matrix.CreateTranslation(Position);
                

            var model = ModelManager.GetModel(ModelName);
            if (model != null && model.ReadyToRender)
            {
                var bonesTransforms = new Matrix[model.BaseModel.Bones.Count];
                model.BaseModel.CopyAbsoluteBoneTransformsTo(bonesTransforms);

                foreach (var mesh in model.BaseModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {

                        effect.World = bonesTransforms[mesh.ParentBone.Index]*worldMatrix;
                        effect.View = CameraManager.ActiveCamera.View;
                        effect.Projection = CameraManager.ActiveCamera.Projection;

                        effect.EnableDefaultLighting();
                    }
                    mesh.Draw();
                }
            }
        }

        public override void DrawCulling(GameTime gameTime)
        {
            Occluded = false;
            if (!ReadyToRender || Culled) return;
            Query.Begin();
            var model = ModelManager.GetModel(ModelName);
            if (model != null && model.ReadyToRender)
            {
                var transforms = new Matrix[model.BaseModel.Bones.Count];
                model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (var mesh in model.BaseModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = World;
                        effect.View = CameraManager.ActiveCamera.View;
                        effect.Projection = CameraManager.ActiveCamera.Projection;
                    }
                    mesh.Draw();
                }
            }
            Query.End();

            while (!Query.IsComplete)
            {

            }

            if (Query.IsComplete && Query.PixelCount == 0)
            {
                Occluded = true;
            }
        }
    }
}