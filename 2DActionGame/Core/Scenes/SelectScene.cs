using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace _2DActionGame
{
	/// <summary>
	/// 選択画面系シーンの基本クラス。
	/// </summary>
	public class SelectScene : Scene
	{
		private static readonly int sensitivity = 5;

		protected Button[] button;
		protected int buttonNum, curButton;

		public SelectScene(Scene privousScene/*, params Button[] button*/)
			: base(privousScene)
		{
			/*this.button = button;
			for (int i = 0; i < button.Length; i++) button[i].color = Color.Blue;*/
		}

		public override void Load()
		{
			backGround = content.Load<Texture2D>("General\\Menu\\MenuBG");
			mask = content.Load<Texture2D>("General\\Menu\\MaskTexture");
		}
		public override void Update(double dt)
		{
			if (counter % sensitivity == 0) {// Controllerを使って書ければcounterを消せる
				if (JoyStick.stickDirection == Direction.DOWN) curButton++;
				else if (JoyStick.stickDirection == Direction.UP) curButton--;
			}
			Debug();

			if (curButton > buttonNum - 1) curButton = buttonNum - 1;
			else if (curButton < 0) curButton = 0;

			for (int i = 0; i < buttonNum; i++)
				if (i == curButton) {
					button[i].isSelected = true;
					button[i].color = Color.Orange;
				}
				else {
					button[i].isSelected = false;
					button[i].color = Color.Blue;
				}

			ButtonUpdate();
			counter++;
		}
		protected virtual void ButtonUpdate()
		{
			if (JoyStick.IsOnKeyDown(2)) {
				isEndScene = true;
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(backGround, Vector2.Zero, Color.White);
		}
	}
}
