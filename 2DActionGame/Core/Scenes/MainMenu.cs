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
			buttonNum = 5;
			button = new Button[buttonNum];
            for(int i = 0; i < buttonNum; i++)
                button[i].color = Color.Blue;

            Load();
        }

        public override void Load()
        {
			base.Load();

			for (int i = 0; i < buttonNum - 2; i++)	// 最後がデバッグ用なので-1
				button[i].texture = content.Load<Texture2D>("General\\Menu\\MenuButton" + i);
			button[3].name = "StageSelect";			// 4debug
			button[4].name = "Ranking";

			game.stageNum = 1;
			
        }

		protected override void ButtonUpdate()
        {
			base.ButtonUpdate();

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
			if (button[3].isSelected && JoyStick.IsOnKeyDown(3)) {
				PushScene(new StageSelect(this));
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
            }
			if (button[4].isSelected && JoyStick.IsOnKeyDown(3)) {
				PushScene(new Ranking(this));
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
			spriteBatch.Draw(backGround, Vector2.Zero, Color.White);
			for (int i = 0; i < buttonNum; i++) {
				if (i >= 3) {
					if (button[i].isSelected) spriteBatch.DrawString(game.pumpDemi, button[i].name, new Vector2(200, 150 + i * 50), button[i].color);
				} else if (button[i].isSelected)
					spriteBatch.Draw(button[i].texture, Vector2.Zero, Color.White);
			}
        }
    }
}
