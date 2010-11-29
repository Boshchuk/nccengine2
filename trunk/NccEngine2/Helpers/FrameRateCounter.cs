using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NccEngine2.Helpers
{
    /// <summary>
    /// A basic DrawableGameComponent that displays the frame rate to the screen.
    /// </summary>
    public class FrameRateCounter : DrawableGameComponent
    {
        private ContentManager content;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        private readonly Vector2 fpsScreenLocation = new Vector2(32, 32);
        private int frameRate;
        private int frameCounter;
        private float elapsedTime;
        private string fpsString = "fps: ??";

      

        public FrameRateCounter(Game game)
            : base(game)
        {
            content = BaseEngine.ContentManager; 
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = content.Load<SpriteFont>("Content/Fonts/menufont");
            //content.RootDirectory = lastRoot;
        }

        protected override void UnloadContent()
        {
            content.Unload();
        }

        public override void Draw(GameTime gameTime)
        {
            frameCounter++;
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedTime >= 1f)
            {
                elapsedTime -= 1f;
                frameRate = frameCounter;
                frameCounter = 0;

                float averageFrameLength = 1000f / frameRate;
                fpsString = string.Format("fps: {0} ({1} ms)", frameRate, averageFrameLength);
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, fpsString, fpsScreenLocation, Color.White);
            spriteBatch.End();
        }
    }
}