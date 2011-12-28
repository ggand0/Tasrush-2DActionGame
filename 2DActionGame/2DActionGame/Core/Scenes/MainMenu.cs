using System;
using System.IO;
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
    public class MainMenu : SelectScene
    {
		public MainMenu(Scene privousScene)
			: base(privousScene)
		{
			buttonNum = 3;
			button = new Button[buttonNum];
            for(int i = 0; i < buttonNum; i++)
                button[i].color = Color.Blue;

            Load();
        }

        public override void Load()
        {
			base.Load();

            for (int i = 0; i < buttonNum; i++) {
                button[i].texture = content.Load<Texture2D>("General\\Menu\\MenuButton" + i);
            }
			game.stageNum = 1;
        }

		protected override void ButtonUpdate()
        {
			if (JoyStick.IsOnKeyDown(2)) {
				isEndScene = true;
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
				SoundControl.Stop();
				SoundControl.IniMusic("Audio\\BGM\\menu_new");
			}

            if (button[0].isSelected && JoyStick.IsOnKeyDown(3)) {
				PushScene(new LvlSelect(this));
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
            }
			if (button[1].isSelected && JoyStick.IsOnKeyDown(3)) {
				PushScene(new Tutorial(this));
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
            }
			if (button[2].isSelected && JoyStick.IsOnKeyDown(3)) {
				PushScene(new Option(this));
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
			spriteBatch.Draw(backGround, Vector2.Zero, Color.White);
			for (int i = 0; i < buttonNum; i++) {
                if (button[i].isSelected)
					spriteBatch.Draw(button[i].texture, Vector2.Zero, Color.White);
			}
        }
    }
}
