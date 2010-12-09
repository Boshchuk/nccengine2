using System;
using Microsoft.Xna.Framework;

namespace AntiTankGame2.GameLogic
{
    public class RocketHelper
    {
        public static Vector3 RocketPos(GameTime gameTime, Vector3 launcherPos,
              Vector3 endPoitPosition, 
            float  horisontAngle, float verticalAngle)
        {

            //single movment part
            //when target position stable
            
            //if( RocketHelper.InRange(endop))


            //Vector3.CatmullRom()

            return new Vector3();
        }

        /// <summary>
        /// Step beetwen to points on radiys
        /// </summary>
       public static Vector3  LinerStep(Vector3 startPoint, Vector3 targetPoint, float speed)
        {
            return  Vector3.SmoothStep(startPoint, targetPoint, speed);
        }

        private static bool InRange(Vector3 rocketPos, Vector3 targetPos, Vector3 lastTargetPos)
        {
            const float rad = 100f;
            
            if (Vector3.Distance(targetPos,lastTargetPos) < rad)
            {
                return true;
            }

            return false;
        }

    }
}