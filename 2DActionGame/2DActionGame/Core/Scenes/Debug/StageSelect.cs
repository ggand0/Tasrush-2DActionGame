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
	public class StageSelect : SelectScene
	{

		public StageSelect(Scene privousScene)
			: base(privousScene)
		{
			buttonNum = 4;
			button = new Button[buttonNum];

			for (int i = 0; i < button.Length; i++)
				button[i].color = Color.Blue;

			Load();
		}

		protected override void ButtonUpdate()
		{
			if (button[0].isSelected && JoyStick.IsOnKeyDown(3)) {
				game.stageNum = 1;
				PushScene(new LvlSelect(this));

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[1].isSelected && JoyStick.IsOnKeyDown(3)) {
				game.stageNum = 2;
				PushScene(new LvlSelect(this));

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[2].isSelected && JoyStick.IsOnKeyDown(3)) {
				game.stageNum = 3;
				PushScene(new LvlSelect(this));

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[3].isSelected && JoyStick.IsOnKeyDown(3)) {
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
				isEndScene = true;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(backGround, Vector2.Zero, Color.White);
			spriteBatch.DrawString(game.Arial, "Stage Select", new Vector2(250, 50), Color.Orange);
			spriteBatch.DrawString(game.Arial2, "choose stage ", new Vector2(200, 100), Color.Orange);

			//sprite.DrawString(game.Arial, "Stage0(4debug)", new Vector2(200, 100), button[0].color);
			spriteBatch.DrawString(game.Arial, "Stage 1", new Vector2(200, 140), button[0].color);
			spriteBatch.DrawString(game.Arial, "Stage 2", new Vector2(200, 180), button[1].color);
			spriteBatch.DrawString(game.Arial, "Stage 3", new Vector2(200, 220), button[2].color);
			spriteBatch.DrawString(game.Arial, "Back", new Vector2(200, 360), button[3].color);
		}
	}
}
