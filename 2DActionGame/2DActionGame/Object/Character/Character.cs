using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
    public class Character : Object
    {
        #region Member variable
		public int deathComboTime = 30;

		/// <summary>
		/// このオブジェクトを使用しているオブジェクト
		/// </summary>
		//public Object user { get; internal set; }                  // BossなどがEnemtを使ったりとかする際に。nullの場合は通常の処理
		/// <summary>
		/// 現在乗っている地形のリスト
		/// </summary>
        public List<Terrain> ridingTerrains = new List<Terrain>();
		/// <summary>
		/// 所有武器のリスト
		/// </summary>
        public List<Weapon> weapons = new List<Weapon>();           // 当たり判定の計算用に使うので、派生クラスで別の変数を使ってUpdateの最後に代入という形でもいいかも

		public bool turnsLeft { get; protected set; }
        public bool isDashing { get; set; }
		public bool isBlownAway { get; protected set; }
        public bool isInDamageMotion { get; set; }
        public bool onConveyor { get; set; }
		public bool deathByFalling { get; set; }
		

        public int totalHits { get; set; }                          // デバッグ用
        public int comboCount { get; protected set; }
        #endregion

		/// <summary>
		/// ダメージ時の点滅処理に使用するカウンタ
		/// </summary>
		protected int blinkCount;
		/// <summary>
		/// 透過処理時に使用。名前変えたい
		/// </summary>
		protected float e;
        public bool inDmgMotion { get; protected set; }
		protected virtual void DrawDamageBlink(SpriteBatch spriteBatch, Color color, float blinkSpeed)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
			if (blinkCount % 5 == 0) e += blinkSpeed;
			dColor = (float)Math.Sin(e * 8) / 2.0f + 0.5f;

			spriteBatch.Draw(texture, drawPos, animation.rect, color * dColor);
            //spriteBatch.Draw(texture, drawPos, animation.rect, Color.Red);
			blinkCount++;

			if (blinkCount > 20) {
				dColor = 1.0f;
				inDmgMotion = false;
			}
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		}
		protected virtual void DrawDamageBlinkOnce(SpriteBatch spriteBatch, Color color)
		{
			/*spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
			if (blinkCount <= 3) {
				dColor = 1.0f;
				spriteBatch.Draw(texture, drawPos, animation.rect, color * dColor);
			} else if (blinkCount <= 20) {
				dColor = 0f;
				spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);
			} else if (blinkCount > 20) {
				dColor = 1.0f;
				inDmgMotion = false;
			}
			blinkCount++;

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);*/
			if (blinkCount <= 10) {
				spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
				dColor = 1.0f;
				//spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);//Color.Transparent);//Color.White);
				spriteBatch.Draw(texture, drawPos, animation.rect, color);
				spriteBatch.Draw(texture, drawPos, animation.rect, color);
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			} else if (blinkCount <= 20) {
				spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);
			} else if (blinkCount > 20) {
				inDmgMotion = false;
				spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);
			}
			if (!stage.isPausing) blinkCount++;
		}

        public Character()
            : this(null, 0, 0, 32, 32)
        {
        }
        public Character(Stage stage,  float x, float y, int width, int height)
            : this(stage, x, y, width, height, null)
        {
        }
        public Character(Stage stage, float x, float y, int width, int height, Character user)
            : base(stage, x, y, width, height)
        {
            this.user = user;
			effectPos = new Vector2();
        }

        public override void Update()
        {
            if (IsActive()) {//isActive
                UpdateNumbers();
                UpdateAnimation();
            }
			if (user != null && !isAlive) {
				isBeingUsed = false;
			}
        }
		public override void MotionUpdate()
		{
			base.MotionUpdate();
			inDmgMotion = true;
			blinkCount = 0;
			e = 0;
		}

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive()) {
				if (IsBeingUsed()) {// userが指定されているときはuserのフラグで管理させる
					//spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .0f);
					if (!inDmgMotion) {
						if (!turnsRight) {
							spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);
						} else {
							spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
						}
					} else {
						//DrawDamageBlink(spriteBatch, Color.Red, .60f);

					}
				}
            }
        }
    }
}
