using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
    public class Ending : Scene
    {
		private const int textureNum = 9;
        private Timer timer;
		private readonly int defDrawTime;// = 810;// 6480 / 8 = 810
        private readonly int musicTime = 6480;
        //1:48 = 108 * 60 = 6480frame
        private Texture2D[] textures = new Texture2D[textureNum];
        private bool fromGame;
		private float[] timings = { 0, 18.5f, 37.4f, 46.7f, 60.5f, 74.75f, 84.25f, 93.5f, 104, 120 };
		private void WriteScore()
		{
		}

		public Ending(Scene privousScene, bool fromGame)
			: base(privousScene)
        {
			Load();
            defDrawTime = musicTime / textureNum;
            this.fromGame = fromGame;
            this.timer = new Timer();
        }

        public override void Load() 
        {
			for (int i = 0; i < textureNum; i++) {
				textures[i] = content.Load<Texture2D>("General\\Credit\\SC0" + (i + 1).ToString());
			}

			SoundControl.IniMusic("Audio\\BGM\\ending_medley_newnew");
			if (!game.isMuted) SoundControl.Play(false);
        }
		public override void Update(double dt) 
        {
            if (!timer.IsStarted) {
                timer.Start();
            }
            if (timer.TotalTime() > (float)timings.Last() || JoyStick.IsOnKeyDown(1)) {
				SoundControl.Stop();
				SoundControl.IniMusic("Audio\\BGM\\menu_new");
				//BackScene(5);
                if (fromGame) {
                    game.InitializeStack();
                    //BackScene(8);
                } else isEndScene = true;
            }
            
        }
		public override void Draw(SpriteBatch spriteBatch)
        {
			if (textures[0] != null) {
                for (int i = 0; i < textureNum; i++)
					if (timer.TotalTime() >= timings[i] && timer.TotalTime() < timings[i + 1])
					//if (counter >= defDrawTime * i && counter < defDrawTime * (i + 1))
						spriteBatch.Draw(textures[i], Vector2.Zero, Color.White);
			}
        }
    }
}
