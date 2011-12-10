using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
    /// <summary>
    /// 横から飛んでくる敵
    /// </summary>
    public class SkatingEnemy2 : SkatingEnemy
    {
		protected new readonly float defSpeed = -2.5f;
		protected new readonly float spFriction = .25f;
		protected new readonly float defStopDistance = 125;

        public SkatingEnemy2(Stage stage, float x, float y, int width, int height, int HP)
            : base(stage, x, y, width, height, HP)
        {
			LoadXML("SkatingEnemy2", "Xml\\Objects_Enemy_Stage3.xml");
			friction = spFriction;
			stopDistance = defStopDistance;


			speed.X = defSpeed;
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\SkatingEnemy2");
		}

        public override void Update()
        {
			gravity = speed.Y = 0;
            speed.X = defSpeed;

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
			if (IsActive()) {
				// 回転を考慮して描画座標を幅分ずらす
				spriteBatch.Draw(texture, drawPos + new Vector2(width, 0), animation.rect, Color.White, MathHelper.ToRadians(90), Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
				DrawComboCount(spriteBatch);
			}
        }

    }
}
