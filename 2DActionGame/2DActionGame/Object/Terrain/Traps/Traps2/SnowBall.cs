using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
	/// <summary>
	/// ゆきだまクラス。
	/// </summary>
	public class SnowBall : Enemy
	{
		private float defSpeed;
		/// <summary>
		/// staticTerrainとの当たり判定用
		/// </summary>
		public DamageBlock block { get; private set; }

		public SnowBall(Stage stage, float x, float y, int width, int height)
			: this(stage, x, y, width, height, 0, null, new Vector2())
		{
		}
		public SnowBall(Stage stage, float x, float y, int width, int height, int type, Object user, Vector2 localPosition)
			: base(stage, x, y, width, height, type)
		{
			this.width = 96;
			this.height = 96;
			this.user = user;
			defSpeed = -8;
			HP = 2;

			//block = new DamageBlock (stage, x + 8, y + 32, 48, 48);     // 下端は合わせる    // DamageBlock
			block = new DamageBlock(stage, x, y, 96, 96, this, Vector2.Zero);
			stage.dynamicTerrains.Add(block);

			Load();
		}
		protected override void Load()
		{
			base.Load();
			texture = game.Content.Load<Texture2D>("Object\\Terrain\\SnowBall");
		}

		public override void Update()
		{
			if (IsActive() && IsBeingUsed()) {
				//position = block.position - new Vector2(0, -height / 2);
				defSpeed = -1;
				block.speed.X = defSpeed;
				d++;

				base.Update();
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (IsActive() && IsBeingUsed()) {
				spriteBatch.Draw(texture, block.drawPos, null, Color.White, -d, new Vector2(width / 2, height / 2), 1, SpriteEffects.None, 0);
				if (game.inDebugMode) block.Draw(spriteBatch);
			}
		}
	}
}
