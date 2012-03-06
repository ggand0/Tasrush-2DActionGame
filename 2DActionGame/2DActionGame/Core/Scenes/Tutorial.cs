using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
	public class Tutorial : Scene
	{
		private const int pageNum = 6;

		private Texture2D[] tutorial = new Texture2D[pageNum];
		private int page;

		public Tutorial(Scene privousScene)
			: base(privousScene)
		{
			Load();
		}

		public override void Load()
		{
			for (int i = 0; i < pageNum; i++) {
				tutorial[i] = content.Load<Texture2D>("General\\Menu\\HowTo\\Nsousa" + (i + 1).ToString());
			}
		}
		public override void Update(double dt)
		{
            if (JoyStick.IsOnKeyDown(2) || JoyStick.IsOnKeyDown(3)) {
				isEndScene = true;
			}
			if (JoyStick.IsOnKeyDown(1)) {
				page++;
				if (page >= pageNum) {
					page = pageNum - 1;
					isEndScene = true;
				}
			}
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			upperScene.Draw(spriteBatch);
			spriteBatch.Draw(tutorial[page], Vector2.Zero, Color.White);
		}
	}
}
