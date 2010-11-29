#region File Description
//-----------------------------------------------------------------------------
// DebugManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace NccEngine2.GameDebugTools
{
    /// <summary>
    /// DebugManager class that holds graphics resources for debug
    /// </summary>
    public class DebugManager : DrawableGameComponent
    {
        // the name of the font to load
        private string debugFont;

        #region Properties

        /// <summary>
        /// Gets a sprite batch for debug.
        /// </summary>
        public SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// Gets white texture.
        /// </summary>
        public Texture2D WhiteTexture { get; private set; }

        /// <summary>
        /// Gets SpriteFont for debug.
        /// </summary>
        public SpriteFont DebugFont { get; private set; }

        #endregion

        #region Initialize

        public DebugManager(Game game, string debugFont)
            : base(game)
        {
            // Added as a Service.
            Game.Services.AddService(typeof(DebugManager), this);
            this.debugFont = debugFont;

            // This component doesn't need be call neither update nor draw.
            Enabled = false;
            Visible = false;
        }

        protected override void LoadContent()
        {
            // Load debug content.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            DebugFont = Game.Content.Load<SpriteFont>(debugFont);

            // Create white texture.
            WhiteTexture = new Texture2D(GraphicsDevice, 1, 1);
            var whitePixels = new[] { Color.White };
            WhiteTexture.SetData(whitePixels);

            base.LoadContent();
        }

        #endregion
    }
}