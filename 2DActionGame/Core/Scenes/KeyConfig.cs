using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
	/// <summary>
	/// （予定）
	/// </summary>
	public class KeyConfig : SelectScene
	{
		public KeyConfig(Scene previousScene)
			: base(previousScene)
		{
			buttonNum = 3;
			button = new Button[buttonNum];
			for (int i = 0; i < buttonNum; i++) {
				button[i].color = Color.Blue;
			}

			Load();
		}
		public override void Load()
		{
			base.Load();

			for (int i = 0; i < buttonNum; i++)	// 最後がデバッグ用なので-1
				button[i].texture = content.Load<Texture2D>("General\\Menu\\MenuButton" + i);
			button[0].name = "cross";
			button[1].name = "leftButton";
		}
		protected override void ButtonUpdate()
		{
			base.ButtonUpdate();

			if (button[0].isSelected && Controller.IsOnKeyDown(3)) {
				// crossのconfig処理
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[1].isSelected && Controller.IsOnKeyDown(3)) {
				// leftButtonのconfig処理
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[2].isSelected && Controller.IsOnKeyDown(3)) {
				isEndScene = true;
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(backGround, Vector2.Zero, Color.White);

			for (int i = 0; i < buttonNum; i++) {
				if (button[i].isSelected) spriteBatch.Draw(button[i].texture, Vector2.Zero, Color.White);
			}
		}
	}
}
