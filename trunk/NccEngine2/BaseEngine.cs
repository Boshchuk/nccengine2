#region Using stastment

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NccEngine.GameComponents;
using NccEngine2.GameComponents.CameraManagment;
using NccEngine2.GameComponents.Graphics.FX.Bloom;
using NccEngine2.GameComponents.Graphics.Screens;
using NccEngine2.GameComponents.Graphics.Textures;
using NccEngine2.GameComponents.Models;
using NccEngine2.GameComponents.Physics;
using NccEngine2.GameComponents.Scene;
using NccEngine2.GameDebugTools;

#endregion

namespace NccEngine2
{
    /// <summary>
    /// Base Game Class
    /// </summary>
    public class BaseEngine : Game
    {
        #region Fields

        /// <summary>
        /// Width of visible render area.
        /// </summary>
        public static int Width { get; private set; }

        /// <summary>
        /// Height of visible render area.
        /// </summary>
        public static int Height { get; private set; }

        /// <summary>
        /// Aspect ratio of render area.
        /// </summary>
        public static float AspectRatio { get; private set; }

        /// <summary>
        /// Color used to redraw the background scene.
        /// </summary>
        public static Color BackgroundColor { get; private set; }
      
        /// <summary>
        /// Window title for test cases.
        /// </summary>
        public static string WindowTitle { get; private set; }

        /// <summary>
        /// Is the application active.
        /// </summary>
        public static bool IsAppActive { get; set; }

        protected static GraphicsDeviceManager GraphicsDeviceManager;
        /// <summary>
        /// The graphics device, used to render.
        /// </summary>
        public static GraphicsDevice Device
        {
            get { return GraphicsDeviceManager.GraphicsDevice; }
        }

        /// <summary>
        /// Content Managers
        /// </summary>
        public static ContentManager ContentManager { get; private set; }

        public static CameraManager CameraManagers;

        //private static ShaderManager shaderManagers;

        private static TextureManager textureManagers;

        private static ScreenManager screenManagers;

        private static SceneGraphManager sceneGraphManager;

        private static ModelManager modelManager;

        private static PhysicsManager physicsManager;

        //private static LensFlareComponent LensFlareComponent;

        /// <summary>
        /// Bloom Component to manage settings
        /// </summary>
        public static BloomComponent Bloom;

        

        /// <summary>
        /// The input helper for menus, gamepads, keyboard and mouse.
        /// </summary>
        public static Input Input { get; private set; }

        //Note Dont forget Restore
        //private static bool checkedGraphicsOptions;

        private static bool applyDeviceChanges;

        // Our debug system. We can keep this reference or use the DebugSystem.Instance
        // property once we've called DebugSystem.Initialize.
        DebugSystem debugSystem;

        // Position for debug command test.
        Vector2 debugPos = new Vector2(100, 100);

        SpriteBatch spriteBatch;
        SpriteFont font;

        // a blank 1x1 texture
        Texture2D blank;

        // Stopwatch for TimeRuler test.
        Stopwatch stopwatch = new Stopwatch();

        #endregion

        #region constructors
        /// <summary>
        /// Create NccEngine
        /// </summary>
        /// <param name="windowsTitle">Window Title</param>
        protected BaseEngine(string windowsTitle)
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            //TODO: Make correct choosing
            // GraphicsDeviceManager.PreferMultiSampling = true;
//Note Restore multisampling
            //GraphicsDeviceManager.  MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
            //GraphicsDeviceManager.MinimumVertexShaderProfile = ShaderProfile.VS_2_0;

            

            GraphicsDeviceManager.PreparingDeviceSettings += GraphicsDeviceManagersPreparingDeviceSettings;

            GraphicsDeviceManager.ApplyChanges();

            //GameSettings.Initialize();

            ApplyResolutionChange();

            WindowTitle = windowsTitle;

#if DEBUG
            // Disable vertical retrace to get highest framerates possible for testing performance.
            GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
#endif
            // Demand to update as fast as possible, do not use fixed time steps.
            // The whole game is designed this way, if you remove this line
            // the game will not behave normal any longer!
            IsFixedTimeStep = false;

            // Init content Managers
            ContentManager = new ContentManager(Services);

            // Init the InputHelper
            Input = new Input(this);
            Components.Add(Input);

           

            // Init camera Managers
            CameraManagers = new CameraManager(this);
            Components.Add(CameraManagers);

            // Init shader Managers
            //shaderManagers = new ShaderManager(this);
            //Components.Add(shaderManagers);

            // Init texture Managers
            textureManagers = new TextureManager(this);
            Components.Add(textureManagers);

            // Init screen Managers
            screenManagers = new ScreenManager(this);
            Components.Add(screenManagers);

            // Init SceneGraph Managers
            sceneGraphManager = new SceneGraphManager(this);
            Components.Add(sceneGraphManager);

            // Init Model Managers
            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            // Init Physics Managers
            physicsManager = new PhysicsManager(this);
            Components.Add(physicsManager);

            //LensFlareComponent = new LensFlareComponent(this);
            //Components.Add(LensFlareComponent);

            Bloom = new BloomComponent(this);
            Components.Add(Bloom);

            //TODO include other inits here!
        }

        static BaseEngine()
        {
            Input = null;
            ContentManager = null;
            IsAppActive = false;
            WindowTitle = "";
            BackgroundColor = Color.LightBlue;
            AspectRatio = 1.0f;

            //SetAlphaBlendingEnabled(true);
        }

        /// <summary>
        /// Create NccEngine
        /// </summary>
        protected BaseEngine() : this("Game") { }

        #endregion

        /// <summary>
        /// Prepare the graphics device.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event args</param>
        static void GraphicsDeviceManagersPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                var presentParams = e.GraphicsDeviceInformation.PresentationParameters;
                if (GraphicsDeviceManager.PreferredBackBufferHeight == 720)
                {

//NOTE chek multisampleType
                    //presentParams.MultiSampleType =   ltiSampleType.FourSamples;
#if !DEBUG
                    presentParams.PresentationInterval = PresentInterval.One;
#endif
                }
                else
                {
                    presentParams.MultiSampleCount = 2;
#if !DEBUG
                    presentParams.PresentationInterval = PresentInterval.Two;
#endif
                }

                // Add support for NVidia PerfHUD.
                foreach (var currentAdapter in GraphicsAdapter.Adapters)
                {
                    if (!currentAdapter.Description.Contains("Perf")) continue;
                    e.GraphicsDeviceInformation.Adapter = currentAdapter;

                    // e.GraphicsDeviceInformation.Adapter.DeviceName.  DeviceType = DeviceType.Reference;

                    break;
                }
            }
        }

        public static void CheckOptionsAndPsVersion()
        {
            if (Device == null)
            {
                throw new InvalidOperationException("Graphics Device is not created yet!");
            }

            //Note Don't forget restore
            //checkedGraphicsOptions = true;
        }

        public static void ApplyResolutionChange()
        {
            var resolutionWidth = 800;// 640;// GameSettings.Default.ResolutionWidth;
            var resolutionHeight = 600; // 480;//GameSettings.Default.ResolutionHeight;

            if (resolutionWidth <= 0 || resolutionWidth <= 0)
            {
                resolutionWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                resolutionHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
#if XBOX360
            // Xbox 360 graphics settings are fixed
            graphicsDeviceManagers.IsFullScreen = true;
            graphicsDeviceManagers.PreferredBackBufferWidth =
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphicsDeviceManagers.PreferredBackBufferHeight =
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#else
            GraphicsDeviceManager.PreferredBackBufferWidth = resolutionWidth;
            GraphicsDeviceManager.PreferredBackBufferHeight = resolutionHeight;
            //GraphicsDeviceManager.IsFullScreen = GameSettings.Default.Fullscreen;

            applyDeviceChanges = true;
#endif
        }

        public static void ApplyResolutionChange(int width, int height)
        {
            var resolutionWidth = width;
            int resolutionHeight = height;

            if (resolutionWidth <= 0 || resolutionWidth <= 0)
            {
                resolutionWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                resolutionHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }

            GraphicsDeviceManager.PreferredBackBufferWidth = resolutionWidth;
            GraphicsDeviceManager.PreferredBackBufferHeight = resolutionHeight;
           // GraphicsDeviceManager.IsFullScreen = GameSettings.Default.Fullscreen;

            applyDeviceChanges = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GraphicsDeviceManager.DeviceReset += GraphicsDeviceManagersDeviceReset;
            GraphicsDeviceManagersDeviceReset(null, EventArgs.Empty);

            // initialize the debug system with the game and the name of the font 
            // we want to use for the debugging
            debugSystem = DebugSystem.Initialize(this, "Content/Font");

            // register a new command that lets us move a sprite on the screen
            debugSystem.DebugCommandUI.RegisterCommand(
                "pos",              // Name of command
                "set position",     // Description of command
                PosCommand          // Command execution delegate
                );

            SetAlphaBlendingEnabled(true);

            base.Initialize();
        }
    


        static void GraphicsDeviceManagersDeviceReset(object sender, EventArgs e)
        {
            // Update width and height
            Width = GraphicsDeviceManager.GraphicsDevice.Viewport.Width;
            Height = GraphicsDeviceManager.GraphicsDevice.Viewport.Height;
            AspectRatio = Width / Height;
            CameraManager.SetAllCamerasProjectionMatrix(AspectRatio);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = ContentManager.Load<SpriteFont>("Content/Font");

            

            // create our blank texture
            blank = new Texture2D(GraphicsDevice, 1, 1);
            blank.SetData(new[] { Color.White });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // tell the TimeRuler that we're starting a new frame. you always want
            // to call this at the start of Update
          //  debugSystem.TimeRuler.StartFrame();

            // Start measuring time for "Update".
           // debugSystem.TimeRuler.BeginMark("Update", Color.Blue);

            //HandleInput();
            //HandleTouchInput();

            // Simulate game update by doing a busy loop for 1ms
           // stopwatch.Reset();
           // stopwatch.Start();
           // while (stopwatch.ElapsedMilliseconds < 1) ;

            // Update other components.
            UpdateZoomyText(gameTime);

            base.Update(gameTime);

            // Stop measuring time for "Update".
          //  debugSystem.TimeRuler.EndMark("Update");
        }

       

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
          //  debugSystem.TimeRuler.BeginMark("Draw", Color.Yellow);

            Device.Clear(BackgroundColor);

            //GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            ScaleMatrix = Matrix.CreateScale(GraphicsDeviceManager.PreferredBackBufferWidth / 480f, GraphicsDeviceManager.PreferredBackBufferHeight / 800f, 1);

            base.Draw(gameTime);


            DrawZoomyText();

            // Apply device changes
            if (!applyDeviceChanges) return;
            GraphicsDeviceManager.ApplyChanges();
            applyDeviceChanges = false;

            
             // Start measuring time for "Draw".
            

            

            //spriteBatch.Begin();

            // Show usage.
            //string message =
            //    "A Button, A key: Show/Hide FPS Counter\n" +
             //   "B Button, B key: Show/Hide Time Ruler\n" +
              //  "X Button, X key: Show/Hide Time Ruler Log\n" +
             //   "Tab key, flick down: Open debug command UI\n" +
             //   "Tab key, flick up: Close debug command UI\n" +
 //               "Tap: Show keyboard input panel";

   //         Vector2 size = font.MeasureString(message);
    //        Layout layout = new Layout(GraphicsDevice.Viewport);

      //      float margin = font.LineSpacing;
       //     Rectangle rc = new Rectangle(0, 0,
         //                           (int)(size.X + margin),
           //                         (int)(size.Y + margin));

            // Compute boarder size, position.
          //  rc = layout.Place(rc, 0.01f, 0.01f, Alignment.TopRight);
          //  spriteBatch.Draw(blank, rc, Color.Black * .5f);

            // Draw usage message text.
            //layout.ClientArea = rc;
            //Vector2 pos = layout.Place(size, 0, 0, Alignment.Center);
//            spriteBatch.DrawString(font, message, pos, Color.White);

            // Draw debug command test sprite.

  //          spriteBatch.End();

            // Draw other components.
            base.Draw(gameTime);

            // Stop measuring time for "Draw".
    //        debugSystem.TimeRuler.EndMark("Draw");
            
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);
            IsAppActive = true;
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);
            IsAppActive = false;
        }

        #region Render states
        /// <summary>
        /// Alpha blending
        /// </summary>
        public static void SetAlphaBlendingEnabled(bool value)
        {
            var dss = new DepthStencilState();
            var bs = new BlendState();
            if (value)
            {
                
                bs.AlphaSourceBlend = Blend.SourceAlpha;
                bs.ColorSourceBlend = Blend.SourceAlpha;
                bs.ColorDestinationBlend = Blend.InverseSourceAlpha;
                bs.AlphaDestinationBlend = Blend.InverseSourceAlpha;

                dss.DepthBufferEnable = true;

                Device.BlendState = bs;
                Device.DepthStencilState = dss;

            }
            else
            {
                 bs.AlphaSourceBlend = Blend.SourceAlpha;
                 bs.ColorSourceBlend = Blend.SourceAlpha;
                 bs.ColorDestinationBlend = Blend.InverseSourceAlpha;
                 bs.AlphaDestinationBlend = Blend.InverseSourceAlpha;

                 Device.BlendState = bs;
                 Device.DepthStencilState = dss;
            }
        }

        /// <summary>
        /// Alpha modes
        /// </summary>
        public enum AlphaMode
        {
            /// <summary>
            /// Disable alpha blending for this (even if the texture has alpha)
            /// </summary>
            DisableAlpha,
            /// <summary>
            /// Default alpha mode: SourceAlpha and InvSourceAlpha, which does
            /// nothing if the texture does not have alpha, else it just displays
            /// it as it is (with transparent pixels).
            /// </summary>
            Default,
            /// <summary>
            /// Use source alpha one mode, this is the default mode for lighting
            /// effects.
            /// </summary>
            SourceAlphaOne,
            /// <summary>
            /// One one alpha mode.
            /// </summary>
            OneOne,
        }

        /// <summary>
        /// Current alpha mode
        /// </summary>
        public static void SetCurrentAlphaMode(AlphaMode value)
        {
            switch (value)
            {
                case AlphaMode.DisableAlpha:
                    Device.BlendState.AlphaSourceBlend = Blend.Zero;
                    Device.BlendState.AlphaDestinationBlend = Blend.One;
                    break;
                case AlphaMode.Default:
                    Device.BlendState.AlphaSourceBlend = Blend.SourceAlpha;
                    Device.BlendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
                    break;
                case AlphaMode.SourceAlphaOne:
                    Device.BlendState.AlphaSourceBlend = Blend.SourceAlpha;
                    Device.BlendState.AlphaDestinationBlend = Blend.One;
                    break;
                case AlphaMode.OneOne:
                    Device.BlendState.AlphaSourceBlend = Blend.One;
                    Device.BlendState.AlphaDestinationBlend = Blend.One;
                    break;
            }
        }
        #endregion


        #region Debug command test code.

        /// <summary>
        /// This method is called from DebugCommandHost when the user types the 'pos'
        /// command into the command prompt. This is registered with the command prompt
        /// through the DebugCommandUI.RegisterCommand method we called in Initialize.
        /// </summary>
        void PosCommand(IDebugCommandHost host, string command, IList<string> arguments)
        {
            // if we got two arguments from the command
            if (arguments.Count == 2)
            {
                // process text "pos xPos yPos" by parsing our two arguments
                debugPos.X = Single.Parse(arguments[0], CultureInfo.InvariantCulture);
                debugPos.Y = Single.Parse(arguments[1], CultureInfo.InvariantCulture);
            }
            else
            {
                // if we didn't get two arguments, we echo the current position of the cat
                host.Echo(String.Format("Pos={0},{1}", debugPos.X, debugPos.Y));
            }
        }

        #endregion

        /// <summary>
        /// Creates a new zoomy text menu item selection effect.
        /// </summary>
        public static void SpawnZoomyText(string text, Vector2 position)
        {
            zoomyTexts.Add(new ZoomyText { Text = text, Position = position });
        }


        const float ZoomyTextLifespan = 0.75f;

        static List<ZoomyText> zoomyTexts = new List<ZoomyText>();
        public SpriteBatch SpriteBatch { get; private set; } //TODO Group wuth ather fields
        public Matrix ScaleMatrix { get; private set; }
        public SpriteFont Font { get; private set; }
        public SpriteFont BigFont { get; private set; }

        // Zoomy text provides visual feedback when selecting menu items.
        // This is implemented by the main game, rather than any individual menu
        // screen, because the zoomy effect from selecting a menu item needs to
        // display across the transition while that menu makes way for a new one.
        class ZoomyText
        {
            public string Text;
            public Vector2 Position;
            public float Age;
        }

        /// <summary>
        /// Updates the zoomy text animations.
        /// </summary>
        static void UpdateZoomyText(GameTime gameTime)
        {
            int i = 0;

            while (i < zoomyTexts.Count)
            {
                zoomyTexts[i].Age += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (zoomyTexts[i].Age >= ZoomyTextLifespan)
                    zoomyTexts.RemoveAt(i);
                else
                    i++;
            }
        }

        /// <summary>
        /// Draws the zoomy text animations.
        /// </summary>
        void DrawZoomyText()
        {
            if (zoomyTexts.Count <= 0)
                return;

            SpriteBatch.Begin(0, null, null, null, null, null, ScaleMatrix);

            foreach (ZoomyText zoomyText in zoomyTexts)
            {
                Vector2 pos = zoomyText.Position + Font.MeasureString(zoomyText.Text) / 2;

                float age = zoomyText.Age / ZoomyTextLifespan;
                float sqrtAge = (float)Math.Sqrt(age);

                float scale = 0.333f + sqrtAge * 2f;

                float alpha = 1 - age;

                SpriteFont font = BigFont;

                // Our BigFont only contains characters a-z, so if the text
                // contains any numbers, we have to use the other font instead.
                foreach (char ch in zoomyText.Text)
                {
                    if (char.IsDigit(ch))
                    {
                        font = Font;
                        scale *= 2;
                        break;
                    }
                }

                Vector2 origin = font.MeasureString(zoomyText.Text) / 2;

                SpriteBatch.DrawString(font, zoomyText.Text, pos, Color.Lerp(new Color(64, 64, 255), Color.White, sqrtAge) * alpha, 0, origin, scale, 0, 0);
            }

            SpriteBatch.End();
        }
    }
}
