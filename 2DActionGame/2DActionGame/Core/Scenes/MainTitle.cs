using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
    public class MainTitle : Scene
    {
		private static readonly int logoDisplayTime = 220;//120;

        private Texture2D[] texture = new Texture2D[4];
        private float dColor, e;
		private Random rnd = new Random();

		public MainTitle(Scene privousScene)
			: base(privousScene)
        {
			Load();
        }
        public override void Load()
        {
            texture[0] = content.Load<Texture2D>("General\\Menu\\TitleBG");
            texture[1] = content.Load<Texture2D>("General\\OP\\OP0");
			string t = rnd.Next(1, 2).ToString();
            texture[2] = content.Load<Texture2D>("General\\OP\\OP" + t);
			if (game.random.Next(1, 20) == 1) texture[2] = content.Load<Texture2D>("General\\OP\\OP3");
			texture[3] = content.Load<Texture2D>("General\\Menu\\PushStart");

			SoundControl.IniMusic("Audio\\BGM\\menu_new");
			KeyConfig.LoadXML("KeyConfig", "Xml\\KeyConfig.xml");
        }

		public override void Update(double dt)
        {
			if (counter < logoDisplayTime) {
				if (JoyStick.IsOnKeyDown(8) || JoyStick.IsOnKeyDown(1))
					SkipLogo();
			} else {
				if (counter == logoDisplayTime) if (!game.isMuted) SoundControl.Play(true);

#if DEBUG
				Debug();
#endif
                if (JoyStick.IsOnKeyDown(8) || JoyStick.IsOnKeyDown(1)) {
					PushScene(new MainMenu(this));
					if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
					counter = 0;
				}
			}
			counter++;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // 文字列幅or高さの取得はfontArial.MeasureString 
			if (counter < logoDisplayTime) {
				if (counter < logoDisplayTime / 3.0)// 40
					if (counter % 2 == 0)
						dColor += .05f;
				if (counter >= logoDisplayTime / 3.0 && counter < logoDisplayTime * 2 / 3.0)//65
					dColor = 1f; //d = 180;
				if (counter >= logoDisplayTime * 3 / 4.0 && counter < logoDisplayTime)// 80
					if (counter % 2 == 0)
						dColor += -.05f;

				spriteBatch.Draw(texture[1], Vector2.Zero, Color.White);
				spriteBatch.Draw(texture[2], Vector2.Zero, Color.White * dColor);//new Color(255, 255, 255, dColor
			} else {
                if (JoyStick.IsOnKeyDown(2) || JoyStick.IsOnKeyDown(3)) BackToLogo();

				if (counter % 5 == 0) e += .02f;//if (d >= 360) e = 0;
				dColor = (float)Math.Sin(e * 8) / 2.0f + 0.5f;

				spriteBatch.Draw(texture[0], Vector2.Zero, Color.White);
				spriteBatch.Draw(texture[3], Vector2.Zero, Color.White * dColor);
			}
        }

        private void SkipLogo()
        {
            counter = logoDisplayTime - 1;
        }
        private void BackToLogo()
        {
            counter = 0;
            SoundControl.Stop();
        }
    }
}
