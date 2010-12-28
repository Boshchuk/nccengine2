using Microsoft.Xna.Framework;

namespace AntiTankGame2.GameLogic
{
    public static class RocketHelper
    {
        public static Vector3 RocketPos(Vector3 rocketPos,
             Vector3 targetPos, Vector3 lastTargetPos)
        {
            const float rocketSpeed = 0.09f;

            var distanceTarget =  InRange(rocketPos, targetPos, lastTargetPos);

            return Vector3.SmoothStep(rocketPos, distanceTarget, rocketSpeed);
        }

        /// <summary>
        /// if roket can turn in this direction in simple step
        /// </summary>
        /// <param name="rocketPos"></param>
        /// <param name="targetPos"></param>
        /// <param name="lastTargetPos"></param>
        /// <returns></returns>
        private static Vector3 InRange(Vector3 rocketPos, Vector3 targetPos, Vector3 lastTargetPos)
        {
            //радиус сектора макс поворота
            const float rad = 10f;

            var rocketToTargetDistanse = Vector3.Distance(rocketPos, targetPos);

            var oldsectrCentr = Vector3.SmoothStep(rocketPos, lastTargetPos, rocketToTargetDistanse);


            //максимальный радиус

            var okruznost = Vector3.SmoothStep(oldsectrCentr, targetPos, rad);


            return Vector3.Distance(targetPos,oldsectrCentr) < rad ? targetPos : okruznost;
        }

    }
}