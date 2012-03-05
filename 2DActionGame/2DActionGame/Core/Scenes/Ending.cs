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
		private readonly int defDrawTime;// = 810;// 6480 / 8 = 810
        private readonly int musicTime = 6480;
        //1:48 = 108 * 60 = 6480frame
        private Texture2D[] textures = new Texture2D[textureNum];
        private bool fromGame;
		private float[] timings = { 0, 18.5f, 37, 47, 60, 75, 84, 93, 102, 120 };
		private void WriteScore()
		{
		}

		public Ending(Scene privousScene, bool fromGame)
			: base(privousScene)
        {
			Load();
            defDrawTime = musicTime / textureNum;
            this.fromGame = fromGame;
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
            if (counter > defDrawTime * textureNum || JoyStick.IsOnKeyDown(1)) {
				SoundControl.Stop();
				SoundControl.IniMusic("Audio\\BGM\\menu_new");
				//BackScene(5);
                if (fromGame) BackScene(8);
                else isEndScene = true;
            }
            counter++;
        }
		public override void Draw(SpriteBatch spriteBatch)
        {
			if (textures[0] != null) {
                for (int i = 0; i < textureNum; i++)
					if (counter >= timings[i] * 60 && counter < timings[i + 1] * 60)
					//if (counter >= defDrawTime * i && counter < defDrawTime * (i + 1))
						spriteBatch.Draw(textures[i], Vector2.Zero, Color.White);
			}
        }
    }
}
