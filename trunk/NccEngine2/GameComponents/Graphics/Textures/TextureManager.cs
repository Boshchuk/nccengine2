using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;

namespace NccEngine2.GameComponents.Graphics.Textures
{
    public class TextureManager : GameComponent
    {
        private static Dictionary<string, INccTexture> textures = new Dictionary<string, INccTexture>();

        /// <summary>
        /// Is the TextureManagers Initialized, used for test cases and setup of Effects.
        /// </summary>
        private static bool Initialized { get; set; }

        /// <summary>
        /// The number of textures that are currently loaded.
        /// Use this for user loading bar feedback.
        /// </summary>
        private static int TexturesLoaded { get; set; }

        /// <summary>
        /// Create the shader Managers.
        /// </summary>
        /// <param name="game"></param>
        public TextureManager(Game game)
            : base(game)
        {
        }

        static TextureManager()
        {
            TexturesLoaded = 0;
            Initialized = false;
        }

        /// <summary>
        /// Add a Texture of type NccTexture.
        /// </summary>
        /// <param name="newTexture"></param>
        /// <param name="textureName"></param>
        public static void AddTexture(INccTexture newTexture, string textureName)
        {
            if (textureName != null && !textures.ContainsKey(textureName))
            {
                textures.Add(textureName, newTexture);
                if (Initialized)
                {
                    newTexture.LoadContent();
                    TexturesLoaded++;
                }
            }
        }

        public static void RemoveTexture(string textureName)
        {
            if (textureName != null && textures.ContainsKey(textureName))
            {
                if (Initialized)
                {
                    ThreadStart threadStarter = delegate
                    {
                        textures[textureName].UnloadContent();
                        textures.Remove(textureName);
                        TexturesLoaded--;
                    };
                    var loadingThread = new Thread(threadStarter);
                    loadingThread.Start();
                }
            }
        }

        /// <summary>
        /// Get a texture
        /// </summary>
        /// <param name="textureName"></param>
        /// <returns></returns>
        public static INccTexture GetTexture(string textureName)
        {
            if (textureName != null && textures.ContainsKey(textureName))
            {
                return textures[textureName];
            }
            return null;
        }

        /// <summary>
        /// Create the shaders.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            ThreadStart threadStarter = delegate
            {
                foreach (var texture in textures.Values.Where(texture => !texture.ReadyToRender))
                {
                    texture.LoadContent();
                    TexturesLoaded++;
                }
            };
            var loadingThread = new Thread(threadStarter);
            loadingThread.Start();

            Initialized = true;
        }
    }
}