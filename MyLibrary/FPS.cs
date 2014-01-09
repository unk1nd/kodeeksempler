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
    public partial class FPS : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private float fps;
        private float updateInterval = 1.0f;
        private float timeSinceLastUpdate = 0.0f;
        private float framecount = 0;

        public FPS(Game game)
            : this(game, false, false, game.TargetElapsedTime) { }

        public FPS(Game game, bool synchWithVerticalRetrace,
                   bool isFixedTimeStep, TimeSpan targetElapsedTime)
            : base(game)
        {
            GraphicsDeviceManager graphics =
                (GraphicsDeviceManager)Game.Services.GetService(
                typeof(IGraphicsDeviceManager));

            graphics.SynchronizeWithVerticalRetrace =
            synchWithVerticalRetrace;

            Game.IsFixedTimeStep = isFixedTimeStep;
            Game.TargetElapsedTime = targetElapsedTime;
        }

        /// <summary>
        /// Allows the game component to perform any initialization 
        /// it needs to before starting to run.  This is where it 
        /// can query for
        /// any required services and load content.
        /// </summary>
        public sealed override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides snapshot of timing
        /// values.</param>
        public sealed override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public sealed override void Draw(GameTime gameTime)
        {
            //Henter ut forl�pt tid siden siste kall p� Draw():
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Teller opp antall frames:
            framecount++;
            //Vi beregner her antall rammmer per sekund (updateInterval = 1).
            //Holder rede p� tiden som er g�tt siden siste beregning av frameraten.
            timeSinceLastUpdate += elapsed;

            //N�r det er g�tt mer enn et sekund (updateInterval = 1) beregnes antall
            //frames for siste sekund:
            if (timeSinceLastUpdate > updateInterval)
            {
                //Beregner fps (framecount vil normalt v�re 60 mens timSinceLastUpdate vil v�re ca. 1):
                fps = framecount / timeSinceLastUpdate;
                Game.Window.Title = "FPS: " + fps.ToString() + " EGT: " +
                    gameTime.ElapsedGameTime.TotalSeconds.ToString();
                //Nullstiller fps:
                framecount = 0;
                //Nullstiller (nesten) timeSinceLastUpdate: 
                timeSinceLastUpdate -= updateInterval;
            }
            base.Draw(gameTime);
        }
    }
}