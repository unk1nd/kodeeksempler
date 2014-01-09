using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using MyLibrary;

namespace Light2CubeShader
{
    /// <summary>
    /// Tegner en belyst kube vha. en shader.
    /// </summary>
    public class CubeLightShader1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private ContentManager content;
        private GraphicsDevice device;      //Representerer skjermkortet

        private Effect effect;
        private BasicEffect effectCoord;
 
        //Liste med vertekser:
        private MyVertexPositionNormalColored[] cubeVertices;
        private VertexPositionColor[] mVertCoord = new VertexPositionColor[6];

        //WVP-matrisene:
        private Matrix world;

        //Lysretning:
        //private Vector3 lightDirection0 = new Vector3(2.0f, 0.5f, -4.0f); 
        private Vector3 lightDirection0 = new Vector3(-1.0f, 0.0f, 0.0f); //Peker langs/mot -X
        private Vector3 lightDirection1 = new Vector3(0.0f, 1.0f, 0.0f);
        private Vector3 lightDirection2 = new Vector3(1.0f, 0.0f, 0.0f);

        SpriteBatch spriteBatch;

        private const float COORDBOUNDARY = 500.0f;

        private InputHandler input;
        private FirstPersonCamera camera;

        private SpriteFont spriteFont;

        //Ny vertekstype:
        public struct MyVertexPositionNormalColored
        {
            public Vector3 Position;
            public Color Color;
            public Vector3 Normal;

            public MyVertexPositionNormalColored(Vector3 position, Color color, Vector3 normal)
            {
                this.Position = position;
                this.Color = color;
                this.Normal = normal;
            }

            public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
                    (
                        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                        new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                        new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
                    );
        }

        /// <summary>
        /// Konstruktør. Henter ut et graphics-objekt.
        /// </summary>
        public CubeLightShader1()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(this.Services);

            //Oppretter og tar i bruk input-handleren:
            //NB! Må opprette InputHandler før Camera!
            input = new InputHandler(this);
            this.Components.Add(input);
            //Camera:
            camera = new FirstPersonCamera(this);
            this.Components.Add(camera);

            //Gjør at musepekeren er synlig over vinduet:
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Her legger man initialiseringskode.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            InitDevice();
            InitVertices();
            world = Matrix.CreateTranslation(0f, 0f, 0f);
            spriteFont = Content.Load<SpriteFont>(@"Content\Fonts\Arial");

            //Setter kameraets utgangsposisjon:
            camera.CameraPosition = new Vector3(3.0f, 3.0f, 8.5f);

            InitLights();
        }

        /// <summary>
        /// Diverse initilaliseringer. 
        /// Henter ut device-objektet.
        /// </summary>
        private void InitDevice()
        {
            device = graphics.GraphicsDevice;

            //Setter størrelse på framebuffer:
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            Window.Title = "En belyst kube - bruk av shader ...";

            effect = Content.Load<Effect>("Content/lighteffect");
            effectCoord = new BasicEffect(graphics.GraphicsDevice);
        }


        private Vector3 leftNormal;
        /// <summary>
        /// Vertekser for kuben. Må ha fire vertekser for hver side slik at vi kan sette normalvektorer for hver verteks.
        /// </summary>
        private void InitVertices()
        {
            cubeVertices = new MyVertexPositionNormalColored[36];

            Vector3 topLeftFront = new Vector3(-1.0f, 1.0f, 1.0f);
            Vector3 bottomLeftFront = new Vector3(-1.0f, -1.0f, 1.0f);
            Vector3 topRightFront = new Vector3(1.0f, 1.0f, 1.0f);
            Vector3 bottomRightFront = new Vector3(1.0f, -1.0f, 1.0f);
            Vector3 topLeftBack = new Vector3(-1.0f, 1.0f, -1.0f);
            Vector3 topRightBack = new Vector3(1.0f, 1.0f, -1.0f);
            Vector3 bottomLeftBack = new Vector3(-1.0f, -1.0f, -1.0f);
            Vector3 bottomRightBack = new Vector3(1.0f, -1.0f, -1.0f);

            Vector3 frontNormal = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 backNormal = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 topNormal = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 bottomNormal = new Vector3(0.0f, -1.0f, 0.0f);
            leftNormal = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f);

            Color frontColor = Color.Red;
            Color backColor = Color.Blue;
            Color topColor = Color.Yellow;
            Color bottomColor = Color.SandyBrown;
            Color leftColor = Color.Yellow;
            Color rightColor = Color.Gray;

            // Front face.
            cubeVertices[0] =
                new MyVertexPositionNormalColored(
                topLeftFront, frontColor, frontNormal);
            cubeVertices[1] =
                new MyVertexPositionNormalColored(
                bottomLeftFront, frontColor, frontNormal);
            cubeVertices[2] =
                new MyVertexPositionNormalColored(
                topRightFront, frontColor, frontNormal);
            cubeVertices[3] =
                new MyVertexPositionNormalColored(
                bottomLeftFront, frontColor, frontNormal);
            cubeVertices[4] =
                new MyVertexPositionNormalColored(
                bottomRightFront, frontColor, frontNormal);
            cubeVertices[5] =
                new MyVertexPositionNormalColored(
                topRightFront, frontColor, frontNormal);

            // Back face.
            cubeVertices[6] =
                new MyVertexPositionNormalColored(
                topLeftBack, backColor, backNormal);
            cubeVertices[7] =
                new MyVertexPositionNormalColored(
                topRightBack, backColor, backNormal);
            cubeVertices[8] =
                new MyVertexPositionNormalColored(
                bottomLeftBack, backColor, backNormal);
            cubeVertices[9] =
                new MyVertexPositionNormalColored(
                bottomLeftBack, backColor, backNormal);
            cubeVertices[10] =
                new MyVertexPositionNormalColored(
                topRightBack, backColor, backNormal);
            cubeVertices[11] =
                new MyVertexPositionNormalColored(
                bottomRightBack, backColor, backNormal);

            // Top face.
            cubeVertices[12] =
                new MyVertexPositionNormalColored(
                topLeftFront, topColor, topNormal);
            cubeVertices[13] =
                new MyVertexPositionNormalColored(
                topRightBack, topColor, topNormal);
            cubeVertices[14] =
                new MyVertexPositionNormalColored(
                topLeftBack, topColor, topNormal);
            cubeVertices[15] =
                new MyVertexPositionNormalColored(
                topLeftFront, topColor, topNormal);
            cubeVertices[16] =
                new MyVertexPositionNormalColored(
                topRightFront, topColor, topNormal);
            cubeVertices[17] =
                new MyVertexPositionNormalColored(
                topRightBack, topColor, topNormal);

            // Bottom face. 
            cubeVertices[18] =
                new MyVertexPositionNormalColored(
                bottomLeftFront, bottomColor, bottomNormal);
            cubeVertices[19] =
                new MyVertexPositionNormalColored(
                bottomLeftBack, bottomColor, bottomNormal);
            cubeVertices[20] =
                new MyVertexPositionNormalColored(
                bottomRightBack, bottomColor, bottomNormal);
            cubeVertices[21] =
                new MyVertexPositionNormalColored(
                bottomLeftFront, bottomColor, bottomNormal);
            cubeVertices[22] =
                new MyVertexPositionNormalColored(
                bottomRightBack, bottomColor, bottomNormal);
            cubeVertices[23] =
                new MyVertexPositionNormalColored(
                bottomRightFront, bottomColor, bottomNormal);

            // Left face.
            cubeVertices[24] =
                new MyVertexPositionNormalColored(
                topLeftFront, leftColor, leftNormal);
            cubeVertices[25] =
                new MyVertexPositionNormalColored(
                bottomLeftBack, leftColor, leftNormal);
            cubeVertices[26] =
                new MyVertexPositionNormalColored(
                bottomLeftFront, leftColor, leftNormal);
            cubeVertices[27] =
                new MyVertexPositionNormalColored(
                topLeftBack, leftColor, leftNormal);
            cubeVertices[28] =
                new MyVertexPositionNormalColored(
                bottomLeftBack, leftColor, leftNormal);
            cubeVertices[29] =
                new MyVertexPositionNormalColored(
                topLeftFront, leftColor, leftNormal);

            // Right face. 
            cubeVertices[30] =
                new MyVertexPositionNormalColored(
                topRightFront, rightColor, rightNormal);
            cubeVertices[31] =
                new MyVertexPositionNormalColored(
                bottomRightFront, rightColor, rightNormal);
            cubeVertices[32] =
                new MyVertexPositionNormalColored(
                bottomRightBack, rightColor, rightNormal);
            cubeVertices[33] =
                new MyVertexPositionNormalColored(
                topRightBack, rightColor, rightNormal);
            cubeVertices[34] =
                new MyVertexPositionNormalColored(
                topRightFront, rightColor, rightNormal);
            cubeVertices[35] =
                new MyVertexPositionNormalColored(
                bottomRightBack, rightColor, rightNormal);

            this.InitCoord();
        }

        private void InitCoord()
        {
            Vector3 pos = new Vector3();
            //X-aksen:

            pos.X = -COORDBOUNDARY; pos.Y = 0.0f; pos.Z = 0.0f;
            mVertCoord[0] = new VertexPositionColor(pos, Color.Black);
            pos.X = COORDBOUNDARY; pos.Y = 0.0f; pos.Z = 0.0f;
            mVertCoord[1] = new VertexPositionColor(pos, Color.Black);

            //Y-aksen:
            pos.X = 0.0f; pos.Y = -COORDBOUNDARY; pos.Z = 0.0f;
            mVertCoord[2] = new VertexPositionColor(pos, Color.Black);
            pos.X = 0.0f; pos.Y = COORDBOUNDARY; pos.Z = 0.0f;
            mVertCoord[3] = new VertexPositionColor(pos, Color.Black);

            //Z-aksen:
            pos.X = 0.0f; pos.Y = 0.0f; pos.Z = -COORDBOUNDARY;
            mVertCoord[4] = new VertexPositionColor(pos, Color.Black);
            pos.X = 0.0f; pos.Y = 0.0f; pos.Z = COORDBOUNDARY;
            mVertCoord[5] = new VertexPositionColor(pos, Color.CornflowerBlue);
        }

        private void InitLights()
        {
            effect.Parameters["xEnableLighting"].SetValue(true);
            lightDirection0.Normalize();
            effect.Parameters["xLightDirection"].SetValue(lightDirection0);
            effect.Parameters["xAmbient"].SetValue(0.3f);
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            MakeYRot(gameTime);

            base.Update(gameTime);
        }

        private void DrawCoordSystem()
        {
            foreach (EffectPass pass in effectCoord.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives(PrimitiveType.LineList, mVertCoord, 0, 3);
            }
        }

        private void DrawCube()
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives(PrimitiveType.TriangleList, 
                    cubeVertices, 0, 12, MyVertexPositionNormalColored.VertexDeclaration);
            }
        }

        private float yRot = 0.0f;

        void MakeYRot(GameTime gameTime)
        {
            yRot += (float)gameTime.ElapsedGameTime.Milliseconds / 50.0f;
            if (yRot > 360f)
                yRot = 0;
        }

        protected override void Draw(GameTime gameTime)
        {
            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.None;
            rasterizerState1.FillMode = FillMode.Solid;
            device.RasterizerState = rasterizerState1;

            device.Clear(Color.PowderBlue);

            effectCoord.Projection = camera.Projection;
            effectCoord.View = camera.View;
            effectCoord.World = Matrix.Identity;
            this.DrawCoordSystem();
            
            world = Matrix.CreateRotationY(MathHelper.ToRadians(yRot));
            effect.Parameters["xView"].SetValue(camera.View);
            effect.Parameters["xProjection"].SetValue(camera.Projection);
            effect.Parameters["xWorld"].SetValue(world);
            effect.Parameters["xInvTransWorld"].SetValue(Matrix.Transpose(Matrix.Invert(world)));

            effect.CurrentTechnique = effect.Techniques["Belysning"];

            this.DrawCube();
            base.Draw(gameTime);
        }

        private void DrawOverlayText(string text, int x, int y)
        {
            spriteBatch.Begin();

            // Draw the string twice to create a drop shadow, first colored black
            // and offset one pixel to the bottom right, then again in white at the
            // intended position. This makes text easier to read over the background.
            spriteBatch.DrawString(spriteFont, text, new Vector2(x, y), Color.Black);
            spriteBatch.DrawString(spriteFont, text, new Vector2(x - 1, y - 1), Color.White);

            spriteBatch.End();
        }

        private String VectorToString(Vector3 v)
        {
            return "\n" + String.Format(
                "[{0:f2} {1:f2} {2:f2}]",
                v.X, v.Y, v.Z);
        }

        private String MatrixToString(Matrix m, String _name)
        {
            return "\n" + String.Format(_name +
                "\n|{0:f1} {1:f1} {2:f1} {3:f1}|" +
                "\n|{4:f1} {5:f1} {6:f1} {7:f1}|" +
                "\n|{8:f1} {9:f1} {10:f1} {11:f1}|" +
                "\n|{12:f1} {13:f1} {14:f1} {15:f1}|\n",
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44);
        }
    } 
}
