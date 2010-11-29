using System.Collections.Generic;

namespace NccEngine2.GameComponents.Scene.SceneObject
{
    public class Material
    {
        public int Technique { get; set; }

        public string Shader { get; set; }

        public List<string> TextureList { get; set; }

        public Material()
        {
            TextureList = new List<string>();
        }
    }
}