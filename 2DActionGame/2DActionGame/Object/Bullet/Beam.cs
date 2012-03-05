using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
	/// <summary>
	/// ビーム弾クラス：(サイズが変化し特殊なAnimationを必要とするBullet)
	/// </summary>
	public class Beam : Bullet
	{
		private Animation[] animations;
		/*private Rectangle rect;
		private int maxWidth;
		private int defaultWidth;*/

		public Beam(Stage stage, Turret turret, int width, int height)
			: base(stage, turret, width, height)
		{
			// Turretに管理させるパターン
			// 射撃位置に合わせる
			this.shootPosition = turret.position;
			this.position.X = shootPosition.X;
			this.position.Y = shootPosition.Y;

			isShot = false;
			animation = new Animation(width, height);
			animation2 = new Animation(48, 24);             // 完全にエフェクト専用にしてハードコーディング
			animations = new Animation[10];					// 汎用的にやるのは面倒なので。これくらいあれば十分だろう.
			for (int i = 0; i < animations.Length; i++)
				animations[i] = new Animation(64, 64);
		}
		public Beam(Stage stage, Turret turret, int width, int height, int type)
			: this(stage, turret, width, height, type, 0)
		{
		}
		public Beam(Stage stage, Turret turret, int width, int height, int type, int disappearType)
			: base(stage, turret, width, height)
		{
			// turretに追従させるパターン
			this.shootPosition = turret.position;
			this.position = shootPosition;
			this.disappearPattern = disappearType;

			animation2 = new Animation(48, 24);
			animations = new Animation[10];
			for (int i = 0; i < animations.Length; i++)
				animations[i] = new Animation(64, 64);
		}
		protected override void Load()
		{
			base.Load();

			texture = content.Load<Texture2D>("Object\\Turret&Bullet" + "\\" + "thunder");
			texture2 = content.Load<Texture2D>("Object\\Turret&Bullet\\sparkEffect2");
		}

		public override void Load(ContentManager content, string texture_name, string texture_name2)
		{
			base.Load(content, texture_name, texture_name2);

			animation = new Animation(width, height);                                   // 判定矩形
			animation2 = new Animation(texture2.Width / 3, texture.Height);             // sparkEffect 48,24
			animations = new Animation[10];
			for (int i = 0; i < animations.Length; i++)
				animations[i] = new Animation(texture.Width / 3, texture.Height);       // beam 64,64

            animation = new Animation(texture.Width, texture.Height);
		}

		public override void UpdateFlying(int flyingTime)
		{
			if (isShot) {
				position = turret.position;
				counter++;

				if (counter > flyingTime) {
					isShot = false;
					counter = 0;
					movedDistance = 0;
				}

				UpdateNumbers();
			}
		}
		public override void UpdateAnimation()
		{
			animation.Update(1, 0, width, height, 6, 1);
			animation2.Update(3, 0, 48, 24, 6, 1);

			foreach (Animation ani in animations)
				ani.Update(3, 0, 64, 64, 6, 1);
		}
		/// <summary>
		/// 地面までbeamを伸ばしたいのでその座標を調べる。
		/// </summary>
		protected override void UpdateNumbers()
		{
			base.UpdateNumbers();
			int t = (int)stage.CheckGroundHeight(position.X);
			height = Math.Abs(416 - (int)position.Y);
			if (t != 416) { }//100!?
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (isActive && isShot && (turret == null || (turret != null && turret.isBeingUsed))) { // RaijinのturretのBeamでなぜか最初!isShotなのに描画されてる
				//spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);//if(isShot)      // 判定用の矩形
				DrawBeams(spriteBatch);                                                             // 実際のテクスチャ
				DrawSparkEffects(spriteBatch);                                                      // 着弾位置と射撃位置付近のエフェクト
			}
		}
		/// <summary>
		/// 地面に到達するギリギリまで描画させる。改善の余地有
		/// </summary>
		private void DrawBeams(SpriteBatch spriteBatch)
		{
			int debug = 0;
			for (int i = 0; i < animations.Length; i++) {// toolong:height:205 heightのせいだった明らかに目視で100くらいなのに
				animations[i].hasStarted = false;
				//animations[i].vector = position;
				animations[i].vector = drawPos + new Vector2(0, 64 * i);
			}
			for (int i = 0; i < animations.Length; i++) {
				if (animations[i].vector.Y > position.Y + height) {// これ以降は明らかに描画する必要がないのでbreak
					break;
				} /*else if (animations[i].vector.Y < position.Y + height && animations[i].vector.Y + animations[i].rect.Height > position.Y + height) {// ここの条件が違う気がする
					animations[i].rect.Height = (int)((position.Y + height) - animations[i].vector.Y);
					animations[i].hasStarted = true;
					break;
				}*/
				else if (animations[i].vector.Y < position.Y + height && animations[i].vector.Y + animations[i].rect.Height < position.Y + height) {// 収まってる
					animations[i].hasStarted = true;
					debug += 64;
				} else if (animations[i].vector.Y < position.Y + height && animations[i].vector.Y + animations[i].rect.Height >= position.Y + height) {// はみ出てる
					animations[i].rect.Height = (int)((position.Y + height) - animations[i].vector.Y);
					debug += animations[i].rect.Height;
					animations[i].hasStarted = true;
				}

				//animations[i].position = position;
				//animations[i].position = drawPos + new Vector2(0, 64 * i);
				//animations[i].hasStarted = true;
			}
			if (debug > height) { }

			foreach (Animation ani in animations) {
				if (ani.hasStarted)
					spriteBatch.Draw(texture, ani.vector, ani.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .0f);
			}
		}
		private void DrawSparkEffects(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture2, drawPos, animation2.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .2f);
			spriteBatch.Draw(texture2, drawPos + new Vector2(0, height - animation2.rect.Height), animation2.rect, Color.White, 0, new Vector2(), new Vector2(1, 1), SpriteEffects.FlipVertically, .2f);
			// debug
			spriteBatch.DrawString(game.menuFont, height.ToString(), drawPos, Color.Red);
			if (height > 242) { }
		}

	}
}
