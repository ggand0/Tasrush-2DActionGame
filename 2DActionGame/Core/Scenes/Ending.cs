using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
    public class Ending : Scene
    {
		private const int textureNum = 8;
		private readonly int defDrawTime = 1125;// 9000 / 8 = 1125

        private Texture2D[] textures = new Texture2D[textureNum];

		public Ending(Scene privousScene)
			: base(privousScene)
        {
			Load();
        }

        public override void Load() 
        {
			for (int i = 0; i < 8; i++) {
				textures[i] = content.Load<Texture2D>("General\\Credit\\SC0" + (i + 1).ToString());
			}

			SoundControl.IniMusic("Audio\\BGM\\ending_medley_newnew");
			if (!game.isMuted) SoundControl.Play();
        }
		public override void Update(double dt) 
        {
            if (counter > defDrawTime * textureNum || JoyStick.IsOnKeyDown(3)) {
				SoundControl.Stop();
				SoundControl.IniMusic("Audio\\BGM\\menu_new");
				BackScene(5);
            }
            counter++;
        }
		public override void Draw(SpriteBatch spriteBatch)
        {
			if (textures[0] != null) {
				for (int i = 0; i < textureNum; i++)
					if (counter >= defDrawTime * i && counter < defDrawTime * (i + 1))
						spriteBatch.Draw(textures[i], Vector2.Zero, Color.White);
			}
        }
    }
}
