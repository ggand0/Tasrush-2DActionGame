using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
	public class Ranking : SelectScene
	{
		//private const int displayNum = 5;
		//private int[] scores = new int[displayNum];

		public Ranking(Scene upperScene)
			:base(upperScene)
		{
			buttonNum = 1;
			button = new Button[buttonNum];

			for (int i = 0; i < button.Length; i++)
				button[i].color = Color.Blue;

			Load();
		}
		public override void Load()
		{
			base.Load();
			game.LoadRanking("Ranking.txt", false, "Ranking.txt");//"Ranking_original.txt");
		}

		protected override void ButtonUpdate()
		{
			base.ButtonUpdate();

			if (button[0].isSelected && JoyStick.IsOnKeyDown(1)) {
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
				isEndScene = true;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (drawBackGround) spriteBatch.Draw(backGround, Vector2.Zero, Color.White);
			for (int i = 0; i < game.scores.Count; i++) {
				//spriteBatch.DrawString(game.Arial2, game.scores[i].ToString(), new Vector2(100, 100 + i * 50), Color.White);
				spriteBatch.DrawString(game.menuFont
					, (i+1).ToString()/*game.scores[i].rank*/ + " " + game.scores[i].name + " " + game.scores[i].score.ToString(), new Vector2(100, 30 + i * 50), Color.White);
			}
		}
	}
}
