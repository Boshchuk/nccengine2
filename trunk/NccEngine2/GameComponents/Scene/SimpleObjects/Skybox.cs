using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NccEngine2.GameComponents.Graphics.Textures;
using NccEngine2.Helpers;

namespace NccEngine2.GameComponents.Scene.SimpleObjects
{
    public class Skybox : GameComponent//, IDrawable
    {
        #region fields

        Texture2D[] textures = new Texture2D[6];
        private Effect effect;

        private VertexBuffer vertuces;
        private IndexBuffer indices;
        private VertexDeclaration vertexDecl;

        public Vector3 CameraDirection { get; set; }

        private Vector3 vCameraPosition;
        public Vector3 CameraPosition
        {
            get { return vCameraPosition; }
            set 
            {
                vCameraPosition = value;

                worldMatrix = Matrix.CreateTranslation(vCameraPosition);
            }
        }


        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }
        private Matrix worldMatrix;

        public ContentManager ContentManager { get; set; }
         
        #endregion


        public Skybox(Game game) : base(game)
        {
        }

        public override void  Initialize()
        {
 	        base.Initialize();
             TextureManager.AddTexture(new NccTexture
                 ( ConstantNames.SkyboxContentPath+ ConstantNames.SkyboxTextureNameBack),
                 ConstantNames.SkyboxTextureNameBack);

             TextureManager.AddTexture(new NccTexture
                 ( ConstantNames.SkyboxContentPath+ ConstantNames.SkyboxTextureNameFront),
                 ConstantNames.SkyboxTextureNameFront);

             TextureManager.AddTexture(new NccTexture
                 ( ConstantNames.SkyboxContentPath+ ConstantNames.SkyboxTextureNameBottom),
                 ConstantNames.SkyboxTextureNameBottom);

             TextureManager.AddTexture(new NccTexture
                 ( ConstantNames.SkyboxContentPath+ ConstantNames.SkyboxTextureNameTop),
                 ConstantNames.SkyboxTextureNameTop);

             TextureManager.AddTexture(new NccTexture
                 ( ConstantNames.SkyboxContentPath+ ConstantNames.SkyboxTextureNameLeft),
                 ConstantNames.SkyboxTextureNameLeft);

             TextureManager.AddTexture(new NccTexture
                 ( ConstantNames.SkyboxContentPath+ ConstantNames.SkyboxTextureNameRight),
                 ConstantNames.SkyboxTextureNameRight);
            

            textures[0] = TextureManager.GetTexture(ConstantNames.SkyboxTextureNameBack) as Texture2D;
            textures[1] = TextureManager.GetTexture(ConstantNames.SkyboxTextureNameFront) as Texture2D;

            textures[2] = TextureManager.GetTexture(ConstantNames.SkyboxTextureNameBottom) as Texture2D;
            textures[3] = TextureManager.GetTexture(ConstantNames.SkyboxTextureNameTop) as Texture2D;
            textures[4] = TextureManager.GetTexture(ConstantNames.SkyboxTextureNameLeft) as Texture2D;
            textures[5] = TextureManager.GetTexture(ConstantNames.SkyboxTextureNameRight) as Texture2D;;
            
            effect = BaseEngine.ContentManager.Load<Effect>(ConstantNames.SkyboxContentPath+ConstantNames.SkyboxEffectName);

            IGraphicsDeviceService graphicsDeviceService =
                (IGraphicsDeviceService) Game.Services.GetService(typeof (IGraphicsDeviceService));

            //vertexDecl = new VertexDeclaration(graphicsDeviceService.GraphicsDevice, new VertexElement[]{
            //        new VertexElement{0,VertexElementFormat.Vector3, VertexElementUsage.Position,0),}
            //        new VertexElement(0,VertexElementFormat.Vector2,VertexElementUsage.TextureCoordinate,0)});
        }

        public void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public bool Visible
        {
            get { throw new NotImplementedException(); }
        }

        public int DrawOrder
        {
            get { throw new NotImplementedException(); }
        }

        //public event EventHandler VisibleChanged;
        //public event EventHandler DrawOrderChanged;
    }
}