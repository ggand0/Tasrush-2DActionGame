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
		private const int displayNum = 5;
		private int[] scores = new int[displayNum];

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
			LoadRanking("Ranking.txt");
		}

		protected override void ButtonUpdate()
		{
			base.ButtonUpdate();

			if (button[0].isSelected && Controller.IsOnKeyDown(3)) {
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
				isEndScene = true;
			}
		}

		private void LoadRanking(string fileName)
		{
			StreamReader sr = new StreamReader("Ranking.txt");
			string original;
			string[] tmp;
			string[,] tmp2;

			original = sr.ReadToEnd();
			tmp = original.Replace("\r\n", "\n").Split('\n');
			tmp2 = new string[tmp.Length, 2];
			for (int i = 0; i < tmp.Length; i++) {
				string[] t = tmp[i].Split(' ');
				tmp2[i, 0] = t[0];
				tmp2[i, 1] = t[1];
			}
			for (int i = 0; i < tmp.Length; i++) {
				scores[i] = Int32.Parse(tmp2[i, 1]);
			}
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			for (int i = 0; i < scores.Length; i++) {
				spriteBatch.DrawString(game.Arial2, scores[i].ToString(), new Vector2(100, 100 + i * 50), Color.White);
			}
		}
	}
}
