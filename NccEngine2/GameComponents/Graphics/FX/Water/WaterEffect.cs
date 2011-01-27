using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NccEngine2.GameComponents.Graphics.FX.Water
{
    public class WaterEffect
    {
        private int waterWidth;
        private int waterHeight;

        private Effect effect;

        private TextureCube skyboxTexture;

        Vector4 waveSpeeds = new Vector4(1,2,0.5f,1.5f);
        Vector4 waveHeight = new Vector4(0.3f, 0.4f, 0.2f,0.3f);
        Vector4 waveLengths = new Vector4(10,5,15,7);

        Vector2[] waveDirs = new Vector2[4];
        
        private void Init()
        {
            waveDirs[0] = new Vector2(-1,0);
            waveDirs[1] = new Vector2(-1, 0.5f);
            waveDirs[2] = new Vector2(-1, 0.7f);
            waveDirs[3] = new Vector2(-1, -0.5f);

            for (int i = 0; i < 4; i++)
            {
                waveDirs[i].Normalize();
            }

            effect.CurrentTechnique = effect.Techniques["OceanWater"];
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xView"].SetValue(CameraManagment.CameraManager.ActiveCamera.View);
         //FIX   effect.Parameters["xBumpMap"].SetValue(waterBumps);
            effect.Parameters["xProjection"].SetValue(CameraManagment.CameraManager.ActiveCamera.Projection);
            effect.Parameters["xBumpStrength"].SetValue(0.5f);

            effect.Parameters["xCubeMap"].SetValue(skyboxTexture);
            effect.Parameters["xTexStretch"].SetValue(4.0f);
            effect.Parameters["xCameraPos"].SetValue(CameraManagment.CameraManager.ActiveCamera.Position);
        //FIX    effect.Parameters["xTime"].SetValue(time);

         //FIX   effect.Parameters["xWaveSpeeds"].SetValue(waveFreqs);
            effect.Parameters["xWaveHeight"].SetValue(waveHeight);
            effect.Parameters["xWaveLenghs"].SetValue(waveLengths);

            effect.Parameters["xWaveDir0"].SetValue(waveDirs[0]);
            effect.Parameters["xWaveDir1"].SetValue(waveDirs[1]);
            effect.Parameters["xWaveDir2"].SetValue(waveDirs[2]);
            effect.Parameters["xWaveDir3"].SetValue(waveDirs[3]);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                //BaseEngine.Device.GetVertexBuffers().SetValue(waterVertexBuffer,0,Ve);
                
            }
        }

        private VertexPositionTexture[] CrateWaterVerices()
         {
             var waterVertices = new VertexPositionTexture[waterWidth*waterHeight];

            var i = 0;
            for (var z = 0; z <waterHeight;z++)
            {
                for (var x = 0; x < waterWidth; x++)
                {
                    var position = new Vector3(x,0,-z);
                    var texCoord = new Vector2((float)x/30.0f,(float)z/30.0f);
                    waterVertices[i++] = new VertexPositionTexture(position,texCoord);
                }
            }
            return waterVertices;
         }
    }
}