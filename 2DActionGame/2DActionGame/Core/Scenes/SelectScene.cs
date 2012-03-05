﻿using System;
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
		protected string sceneTitle;
		protected string[] menuString;
		protected static Vector2 TITLE_POSITION;
		protected Vector2 TEXT_POSITION;
		protected Button[] button;
		protected int buttonNum, curButton;
		protected bool drawBackGround = true;

		public SelectScene(Scene privousScene)
			: base(privousScene)
		{
		}

		public override void Load()
		{
			TEXT_POSITION = new Vector2(Game1.Width / 2,
				Game1.Height / 2 - game.menuFont.MeasureString("A").Y * (buttonNum * 2 / 4));// * (buttonNum * 1 / 4)
			TITLE_POSITION = new Vector2(Game1.Width / 2, game.menuFont.MeasureString("A").Y / 2);
			backGround = content.Load<Texture2D>("General\\Menu\\MenuBG");
			mask = content.Load<Texture2D>("General\\Menu\\MaskTexture");
		}
		protected virtual void UpdateTexts()
		{
			/*TEXT_POSITION = new Vector2(Game1.Width / 2,
				Game1.Height / 2 - game.menuFont.MeasureString("A").Y * (buttonNum * 3 / 4));*/
		}
		public override void Update(double dt)
		{
			if (counter % sensitivity == 0) {
				if (JoyStick.stickDirection == Direction.DOWN) curButton++;
				else if (JoyStick.stickDirection == Direction.UP) curButton--;
			}
			if (curButton > buttonNum - 1) curButton = buttonNum - 1;
			else if (curButton < 0)	curButton = 0;

			for (int i = 0; i < buttonNum; i++) {
				if (i == curButton) {
					button[i].isSelected = true;
					button[i].color = Color.Orange;
				} else {
					button[i].isSelected = false;
					button[i].color = Color.Blue;
				}
			}

			ButtonUpdate();
			UpdateTexts();
			Debug();
			counter++;
		}
		protected virtual void ButtonUpdate()
		{
			if (JoyStick.IsOnKeyDown(3) || JoyStick.IsOnKeyDown(2)) {
				isEndScene = true;
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (drawBackGround) spriteBatch.Draw(backGround, Vector2.Zero, Color.White);

			Vector2 origin = game.titleFont.MeasureString(sceneTitle) / 2;
			spriteBatch.DrawString(game.titleFont, sceneTitle, TITLE_POSITION + new Vector2(0, origin.Y * 1), Color.DarkOrange, 0, origin, 1, SpriteEffects.None, 0);
			DrawTexts(spriteBatch, 1);
		}
		public virtual void Draw(SpriteBatch spriteBatch, SpriteFont font)
		{
			if (drawBackGround) spriteBatch.Draw(backGround, Vector2.Zero, Color.White);

			Vector2 origin = game.titleFont.MeasureString(sceneTitle) / 2;
			spriteBatch.DrawString(font, sceneTitle, TITLE_POSITION + new Vector2(0, origin.Y * 1), Color.DarkOrange, 0, origin, 1, SpriteEffects.None, 0);
			DrawTexts(spriteBatch, 1);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="spriteBatch"></param>
		/// <param name="textMargin">文字何列分空けるか</param>
		protected virtual void DrawTexts(SpriteBatch spriteBatch, float textMargin)
		{
			Vector2 v = TEXT_POSITION;
			Vector2 origin;

			for (int i = 0; i < buttonNum; i++) {
				origin = game.menuFont.MeasureString(button[i].name) / 2;
				game.spriteBatch.DrawString(game.menuFont, button[i].name,
					v, (i == curButton ? Color.White : Color.Gray),
				   0, origin, 1, SpriteEffects.None, 0);
				//1列分空けて次のメニューを表示
				v.Y += origin.Y * 3 * textMargin;//origin.Y * 4;
			}
		}
	}
}
