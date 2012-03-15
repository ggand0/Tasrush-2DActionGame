using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
	/// <summary>
	/// 静的地形の背景バッファ
	/// </summary>
	public class BackGround : Object
	{
		public RenderTarget2D renderTarget { get; set; }

		public BackGround(Stage stage, int width, int height)
			: base(stage, width, height)
		{
			renderTarget = new RenderTarget2D(game.graphics.GraphicsDevice, width, height);
			this.activeDistance = width;
		}


		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, drawPos, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
		}
	}
}
