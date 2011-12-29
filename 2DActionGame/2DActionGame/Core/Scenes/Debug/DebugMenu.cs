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
	public class DebugMenu : SelectScene
	{
		private Stage stage;

		public DebugMenu(PauseMenu privousScene, Stage stage)
			: base(privousScene)
		{
			sceneTitle = "Debug Menu";
			this.stage = stage;

			buttonNum = 10;
			button = new Button[buttonNum];
			menuString = new string[] { 
				"Back",
				"change on/off",
				"inDebugMode",
				"isMuted",
				"visibleSword",
				"score",
				"syoryuMode",
				"empty",
				"empty",
				"empty",
				"empty",
			};
			for (int i = 0; i < buttonNum; i++) {
				button[i].color = Color.Blue;
				button[i].name = menuString[i];
			}
			Load();
		}

		protected override void UpdateTexts()
		{
			base.UpdateTexts();

		}
		protected override void ButtonUpdate()
		{
			if (button[0].isSelected && JoyStick.IsOnKeyDown(3)) {
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
				isEndScene = true;
			}
			if (button[1].isSelected && JoyStick.IsOnKeyDown(3)) {
				if (!game.inDebugMode) game.inDebugMode = true;
				else game.inDebugMode = false;

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[2].isSelected && JoyStick.IsOnKeyDown(3)) {
				if (!game.isMuted) game.isMuted = true;
				else game.isMuted = false;

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[3].isSelected && JoyStick.IsOnKeyDown(3)) {
				if (!game.visibleSword) game.visibleSword = true;
				else game.visibleSword = false;

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}

			if (button[4].isSelected && JoyStick.IsOnKeyDown(3)) {
				if (!game.visibleScore) game.visibleScore = true;
				else game.visibleScore = false;

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}

			if (button[5].isSelected && JoyStick.IsOnKeyDown(3)) {
				if (!stage.player.syouryuuMode) stage.player.syouryuuMode = true;
				else stage.player.syouryuuMode = false;


				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}

			if (button[6].isSelected && JoyStick.IsOnKeyDown(3)) {
			}
			if (button[7].isSelected && JoyStick.IsOnKeyDown(3)) {
			}
			if (button[8].isSelected && JoyStick.IsOnKeyDown(3)) {
			}
			if (button[9].isSelected && JoyStick.IsOnKeyDown(3)) {
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			/*spriteBatch.Draw(backGround, Vector2.Zero, Color.White);
			spriteBatch.DrawString(game.Arial, "Debug Options", new Vector2(250, 30), Color.Orange);
			spriteBatch.DrawString(game.menuFont, "change on/off", new Vector2(200, 70), Color.Orange);
			spriteBatch.DrawString(game.menuFont, "inDebugMode", new Vector2(200, 100), button[0].color);
			spriteBatch.DrawString(game.menuFont, "isMuted", new Vector2(200, 120), button[1].color);
			spriteBatch.DrawString(game.menuFont, "visibleSword", new Vector2(200, 140), button[2].color);
			spriteBatch.DrawString(game.menuFont, "score", new Vector2(200, 160), button[3].color);
			spriteBatch.DrawString(game.menuFont, "Back", new Vector2(200, 180), button[4].color);
			spriteBatch.DrawString(game.menuFont, "syoryuMode (Stage1 only)", new Vector2(200, 200), button[5].color);
			spriteBatch.DrawString(game.menuFont, "empty", new Vector2(200, 220), button[6].color);
			spriteBatch.DrawString(game.menuFont, "empty", new Vector2(200, 240), button[7].color);
			spriteBatch.DrawString(game.menuFont, "empty", new Vector2(200, 260), button[8].color);
			spriteBatch.DrawString(game.menuFont, "empty", new Vector2(200, 280), button[9].color);*/
		}
	}
}
