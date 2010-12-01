using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NccEngine2.GameComponents.Graphics.Screens.Menu
{
    public class FloatMenuEntry : MenuEntry
    {
        // Properties.
        public float Value { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public FloatMenuEntry()
        {
            IsDraggable = true;
        }


        /// <summary>
        /// Drag handler changes the slider position.
        /// </summary>
        public override void OnDragged(float delta)
        {
            const float speed = 1f / 300;

            Value = MathHelper.Clamp(Value + delta * speed, 0, 1);
        }


        /// <summary>
        /// Custom draw function displays a slider bar in addition to the item text.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D blankTexture)
        {
            base.Draw(spriteBatch, font, blankTexture);

            Vector2 size = font.MeasureString(Text);
            size.Y /= 2;

            Vector2 pos = Position + size;

            pos.X += 8;
            pos.Y += (Height - font.LineSpacing) / 2;

            float w = 480 - Border - pos.X;

            spriteBatch.Draw(blankTexture, new Rectangle((int)pos.X, (int)pos.Y - 3, (int)(w * Value), 6), Color);
        }
    }
}