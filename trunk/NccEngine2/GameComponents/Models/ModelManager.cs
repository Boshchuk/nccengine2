using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;

namespace NccEngine2.GameComponents.Models
{
    public class ModelManager : GameComponent
    {
        private static Dictionary<string, INccModel> models = new Dictionary<string, INccModel>();

        /// <summary>
        /// Is the ModelManagers Initialized, used for test cases and setup of Effects.
        /// </summary>
        public static bool Initialized { get; private set; }

        /// <summary>
        /// The number of textures that are currently loaded.
        /// Use this for user loading bar feedback.
        /// </summary>
        public static int ModelsLoaded { get; private set; }

        /// <summary>
        /// Create the shader Managers.
        /// </summary>
        /// <param name="game"></param>
        public ModelManager(Game game)
            : base(game)
        {
        }

        static ModelManager()
        {
            ModelsLoaded = 0;
            Initialized = false;
        }

        /// <summary>
        /// Add a shader of type NccModel.
        /// </summary>
        /// <param name="newModel"></param>
        /// <param name="modelName" />
        public static void AddModel(INccModel newModel, string modelName)
        {
            if (modelName != null && !models.ContainsKey(modelName))
            {
                models.Add(modelName, newModel);
                if (Initialized)
                {
                    newModel.LoadContent();
                    ModelsLoaded++;
                }
            }
        }

        /// <summary>
        /// Get a texture
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static INccModel GetModel(string modelName)
        {
            if (modelName != null && models.ContainsKey(modelName))
            {
                return models[modelName];
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
                foreach (var model in models.Values)
                {
                    if (!model.ReadyToRender)
                    {
                        model.LoadContent();
                        ModelsLoaded++;
                    }
                }
            };
            var loadingThread = new Thread(threadStarter);
            loadingThread.Start();

            Initialized = true;
        }
    }
}