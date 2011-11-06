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
    /// Stage3 飛んでくる敵
    /// </summary>
    public class SkatingEnemy2 : SkatingEnemy
    {
        public SkatingEnemy2(Stage stage, float x, float y, int width, int height, int HP)
            : base(stage, x, y, width, height, HP)
        {
            stopDistance = 125;
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\SkatingEnemy2");
		}

        public override void Update()
        {
            gravity = 0;
            speed.Y = 0;
            speed.X = -2.0f;
            base.Update();
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if(isAlive) {
                spriteBatch.Draw(texture, drawPos + new Vector2(width, 0), animation.rect, Color.White, MathHelper.ToRadians(90), Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);// 回転を考慮して描画座標を幅分ずらす
                DrawComboCount(spriteBatch);
            }
        }
    }
}
