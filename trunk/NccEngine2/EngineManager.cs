using Microsoft.Xna.Framework;

namespace NccEngine2
{
    public class EngineManager : BaseEngine
    {
        /// <summary>
        /// The XNA game.
        /// </summary>
        public static Game Game { get; set; }

        /// <summary>
        /// Create the engine.
        /// </summary>
        /// <param name="unitTestName">Used for testing</param>
        
        public EngineManager(string unitTestName)
            : base(unitTestName)
        { }
    }
}