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
			buttonNum = 10;
			button = new Button[buttonNum];

			for (int i = 0; i < button.Length; i++)
				button[i].color = Color.Blue;

			Load();
		}

		protected override void ButtonUpdate()
		{
			if (button[0].isSelected && Controller.IsOnKeyDown(3)) { //IsOnKeyDown(3)
				game.stageNum = 0;

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
				
			}
			if (button[1].isSelected && Controller.IsOnKeyDown(3)) {
				game.stageNum = 1;
				PushScene(new LvlSelect(this));

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[2].isSelected && Controller.IsOnKeyDown(3)) {
				game.stageNum = 5;
				PushScene(new LvlSelect(this));

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[3].isSelected && Controller.IsOnKeyDown(3)) {
				game.stageNum = 6;
				PushScene(new LvlSelect(this));

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}

			if (button[4].isSelected && Controller.IsOnKeyDown(3)) {
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
				isEndScene = true;
			}
			if (button[5].isSelected && Controller.IsOnKeyDown(3)) {
				game.stageNum = 2;
				PushScene(new LvlSelect(this));

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[6].isSelected && Controller.IsOnKeyDown(3)) {
				game.stageNum = 3;
				PushScene(new LvlSelect(this));

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}

			// ↓Hard↓
			/*if (button[7].isSelected && Controller.IsOnKeyDown(3)) {
				game.stageNum = 1;
				game.isHighLvl = true;
				game.ReloadStage(game.isHighLvl);

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[8].isSelected && Controller.IsOnKeyDown(3)) {
				game.stageNum = 2;
				game.isHighLvl = true;
				game.ReloadStage(game.isHighLvl);

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[9].isSelected && Controller.IsOnKeyDown(3)) {
				game.stageNum = 3;
				game.isHighLvl = true;
				game.ReloadStage(game.isHighLvl);

				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}*/
		}

		public override void Draw(SpriteBatch sprite)
		{
			sprite.Draw(backGround, Vector2.Zero, Color.White);
			sprite.DrawString(game.Arial, "Stage Select", new Vector2(250, 50), Color.Orange);
			//spriteBatch.DrawString(game.Arial, "choose stage ", new Vector2(200, 100), Color.Orange);

			sprite.DrawString(game.Arial, "Stage0(4debug)", new Vector2(200, 100), button[0].color);
			sprite.DrawString(game.Arial, "Stage1_Easy", new Vector2(200, 140), button[1].color);
			sprite.DrawString(game.Arial, "BossTest(4debug)", new Vector2(200, 180), button[2].color);
			sprite.DrawString(game.Arial, "SizeTest(4debug)", new Vector2(200, 220), button[3].color);
			sprite.DrawString(game.Arial, "Back", new Vector2(200, 260), button[4].color);
			sprite.DrawString(game.Arial, "Stage2_Easy", new Vector2(200, 300), button[5].color);
			sprite.DrawString(game.Arial, "Stage3_Easy", new Vector2(200, 340), button[6].color);

			sprite.DrawString(game.Arial, "Stage1_Hard", new Vector2(200, 380), button[7].color);
			sprite.DrawString(game.Arial, "Stage2_Hard", new Vector2(200, 420), button[8].color);
			sprite.DrawString(game.Arial, "Stage3_Hard", new Vector2(200, 460), button[9].color);
		}
	}
}
