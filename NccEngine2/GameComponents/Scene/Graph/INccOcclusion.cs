using Microsoft.Xna.Framework.Graphics;
using NccEngine2.GameComponents.Scene.Graph.Interfaces;

namespace NccEngine2.GameComponents.Scene.Graph
{
    /// <summary>
    /// Test if an object is occluded in the scene.  This interface also implements INccCullable
    /// </summary>
    public interface INccOcclusion : INccCullable
    {
        string OcclusionModelName { get; set; }
        //NOTE Dont suported in reach profile(
        OcclusionQuery Query { get; }

        bool Occluded { get; set; }
    }

}