using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace MyLibrary
{
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        //En referanse til input-komponenten:
        protected IInputHandler input;      //protected for FirstPersonCamra.

        private GraphicsDeviceManager graphics;

        private Matrix projection;
        private Matrix view;

        private Vector3 cameraPosition = new Vector3(0.0f, 3.0f, 8.5f);
        private Vector3 cameraTarget = Vector3.Zero;
        private Vector3 cameraUpVector = Vector3.Up;

        public Vector3 CameraUpVector
        {
            get { return cameraUpVector; }
            set { cameraUpVector = value; }
        }

        private Vector3 cameraReference = new Vector3(0.0f, 0.0f, -1.0f);

        private float cameraYaw = 0.0f;
        private float cameraPitch = 0.0f;

        private const float spinRate = 40.0f;

        private const float moveRate = 40.0f;      // for FirstPersonCamra.
        protected Vector3 movement = Vector3.Zero; // for FirstPersonCamera.

        //cameraPosition og cameraTarget er tilgjengelig via properties:
        public Vector3 CameraPosition
        {
            get { return cameraPosition; }
            set
            {
                cameraPosition = value;
                cameraReference = ((-1.0f) * cameraPosition);
                //cameraReference.Normalize();

            }
        }
        public Vector3 CameraTarget
        {
            get { return cameraTarget; }
            set { cameraTarget = value; }
        }

        //view og projection-matrisene er tilgjengelig via properties:
        public Matrix View
        {
            get { return view; }
            set { view = value; }
        }

        public Matrix Projection
        {
            get { return projection; }
            set { projection = value; }
        }

        public Camera(Game game)
            : base(game)
        {
            graphics = (GraphicsDeviceManager)Game.Services.GetService
 (typeof(IGraphicsDeviceManager));
            input = (IInputHandler)game.Services.GetService
 (typeof(IInputHandler));
        }
        public override void Initialize()
        {
            base.Initialize();
            this.InitializeCamera();
        }

        private void InitializeCamera()
        {
            float aspectRatio =
(float)graphics.GraphicsDevice.Viewport.Width /
                    (float)graphics.GraphicsDevice.Viewport.Height;

            Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
            aspectRatio, 1.0f, 1000.0f, out projection);

            Matrix.CreateLookAt(ref cameraPosition, ref cameraTarget,
            ref cameraUpVector, out view);

            cameraReference = ((-1.0f) * cameraPosition);
        }

        public override void Update(GameTime gameTime)
        {
            //timeDelta = tiden mellom to kall på Update
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Ved kort tid mellom hver frame vil timeDelta være liten og 
            //dermed gi et mindre bidrag til cameraYaw.
            //Dette betyr at kameraet beveger seg like fort uavhengig av 
            //frame rate.
            if (input.KeyboardState.IsKeyDown(Keys.Left))
                cameraYaw = cameraYaw + (spinRate * timeDelta);

            if (input.KeyboardState.IsKeyDown(Keys.Right))
                cameraYaw = cameraYaw - (spinRate * timeDelta);

            if (cameraYaw > 360)
                cameraYaw -= 360;
            else if (cameraYaw < 0)
                cameraYaw += 360;

            //OPP/NED (PITCH):
            if (input.KeyboardState.IsKeyDown(Keys.Down))
                cameraPitch = cameraPitch - (spinRate * timeDelta);

            if (input.KeyboardState.IsKeyDown(Keys.Up))
                cameraPitch = cameraPitch + (spinRate * timeDelta);

            if (cameraPitch > 30)
                cameraPitch = 30;

            else if (cameraPitch < -30)
                cameraPitch = -30;

            Matrix rotationMatrix;
            //Rotasjonsmatrise om Y-aksen:
            Matrix.CreateRotationY(MathHelper.ToRadians(cameraYaw), out rotationMatrix);
            //Legger til pitch dvs. rotasjon om X-aksen:
            rotationMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(cameraPitch)) * rotationMatrix;

            // Oppretter en hjelpevektor som peker i retninga kameraet 'ser':
            Vector3 transformedReference;
            Vector3.Transform(ref cameraReference, ref rotationMatrix, out transformedReference);

            // Beregner hva kameraet ser på (cameraTarget) vha. 
            // nåværende posisjonsvektor og retningsvektoren:

            //FirstPersonCamera, endrer kameraets posisjon:
            movement *= (moveRate * timeDelta);
            if (movement != Vector3.Zero)
            {
                Vector3.Transform(ref movement, ref rotationMatrix, out movement);
                cameraPosition += movement;
            }

            Vector3.Add(ref cameraPosition, ref transformedReference,out cameraTarget);

            //Oppdaterer view-matrisa vha. posisjons, kameramål og opp-vektorene:
            Matrix.CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector, out view);
            base.Update(gameTime);
        }
    }
}