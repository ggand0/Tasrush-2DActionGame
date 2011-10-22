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
			this.stage = stage;

			buttonNum = 10;
			button = new Button[buttonNum];
			for (int i = 0; i < buttonNum; i++)
				button[i].color = Color.Blue;
			Load();
		}

		protected override void ButtonUpdate()
		{
			if (button[0].isSelected && Controller.IsOnKeyDown(3)) {
				if (!game.inDebugMode) game.inDebugMode = true;
				else game.inDebugMode = false;

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[1].isSelected && Controller.IsOnKeyDown(3)) {
				if (!game.isMuted) game.isMuted = true;
				else game.isMuted = false;

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[2].isSelected && Controller.IsOnKeyDown(3)) {
				if (!game.visibleSword) game.visibleSword = true;
				else game.visibleSword = false;

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[3].isSelected && Controller.IsOnKeyDown(3)) {
				if (!game.visibleScore) game.visibleScore = true;
				else game.visibleScore = false;

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}

			if (button[4].isSelected && Controller.IsOnKeyDown(3)) {
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
				isEndScene = true;
			}

			if (button[5].isSelected && Controller.IsOnKeyDown(3)) {
				if (!stage.player.syouryuuMode) stage.player.syouryuuMode = true;
				else stage.player.syouryuuMode = false;


				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[6].isSelected && Controller.IsOnKeyDown(3)) {
			}

			if (button[7].isSelected && Controller.IsOnKeyDown(3)) {
			}
			if (button[8].isSelected && Controller.IsOnKeyDown(3)) {
			}
			if (button[9].isSelected && Controller.IsOnKeyDown(3)) {
			}
		}

		public override void Draw(SpriteBatch sprite)
		{
			sprite.Draw(backGround, Vector2.Zero, Color.White);
			sprite.DrawString(game.pumpDemi, "Debug Options", new Vector2(250, 30), Color.Orange);
			sprite.DrawString(game.pumpDemi, "change on/off", new Vector2(200, 70), Color.Orange);
			sprite.DrawString(game.pumpDemi, "inDebugMode", new Vector2(200, 100), button[0].color);
			sprite.DrawString(game.pumpDemi, "isMuted", new Vector2(200, 120), button[1].color);
			sprite.DrawString(game.pumpDemi, "visibleSword", new Vector2(200, 140), button[2].color);
			sprite.DrawString(game.pumpDemi, "score", new Vector2(200, 160), button[3].color);
			sprite.DrawString(game.pumpDemi, "Back", new Vector2(200, 180), button[4].color);
			sprite.DrawString(game.pumpDemi, "syoryuMode (Stage1 only)", new Vector2(200, 200), button[5].color);
			sprite.DrawString(game.pumpDemi, "empty", new Vector2(200, 220), button[6].color);
			sprite.DrawString(game.pumpDemi, "empty", new Vector2(200, 240), button[7].color);
			sprite.DrawString(game.pumpDemi, "empty", new Vector2(200, 260), button[8].color);
			sprite.DrawString(game.pumpDemi, "empty", new Vector2(200, 280), button[9].color);
		}
	}
}
