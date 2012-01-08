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
    /// <summary>
    /// 水から飛び出して攻撃する魚
    /// </summary>
    public class RushingOutEnemy : FlyingOutEnemy
    {
		protected new readonly float defFlyingOutSpeed = -18;
		protected new readonly float defFlyingOutDistance = 200;

		private SoundEffect splashSound;
		/// <summary>
		/// (ジャンプして)頂点に辿り着いたか：一旦水面より上に飛んで欲しい
		/// </summary>
        private bool hasReached;
        private bool isEndingAttack;

        public RushingOutEnemy(Stage stage, float x, float y, int width, int height, float vx, int HP)
            : base(stage, x, y, width, height, HP)
        {
			LoadXML("RushingOutEnemy", "Xml\\Objects_Enemy_Stage2.xml");
			flyingOutSpeed = defFlyingOutSpeed;
            flyingOutDistance = defFlyingOutDistance;

			Load();
        }
		protected override void Load()
		{
			base.Load();

			texture = content.Load<Texture2D>("Object\\Character\\RushingOutEnemy");
			splashSound = content.Load<SoundEffect>("Audio\\SE\\water");
			animation = new Animation(texture.Width, texture.Height);
		}

        public override void UpdateAnimation()
        {
            animation.Update(2, 0, 48, 48, 9, 1);
        }
        public override void MovementUpdate()
        {
            distance = Math.Abs(this.position.X - stage.player.position.X);

			if (!hasFlownOut && !isEndingAttack) {
				isOnSomething = false;
				gravity = 0;
				if (distance < flyingOutDistance) {
					if (flowCount == 0) {
						hasFlownOut = true;
						speed.Y = flyingOutSpeed;
						gravity = defGravity;
						if (!game.isMuted) splashSound.Play(SoundControl.volumeAll, 0f, 0f);
					}
					flowCount++;
				}
			} else if (hasFlownOut) {
				if (Math.Abs(speed.Y) < 5) {// 頂点付近 : 回転させる
					if (this.degree < 180) degree += 15;
					else degree = 180;
					hasReached = true;
				}

				// Waterの当たり判定に頼らず、元の位置を記憶しておいてそれを基準にフラグを立ててもいいかも.
				if (hasReached && (isInWater || Vector2.Distance(position, defPos/*+ new Vector2(0, -30)*/) < 5)) {// できればisInWAterで管理したいが...
					hasFlownOut = false;
					//degree = 0;   // 1frameで回転して不自然な動きになるので終了処理としてもう１段階必要だ.
					hasReached = false;
					flowCount = 0;
					speed = Vector2.Zero;
					gravity = 0;// 一応
					isEndingAttack = true;
				}
			}

			if (isEndingAttack) {
				if (this.degree < 360) degree += 15;
				else {
					degree = 0;
					isEndingAttack = false;
					speed = Vector2.Zero;
					position = defPos;
					gravity = 0;
				}
			}
        }
		protected override void DrawDamageBlinkOnce(SpriteBatch spriteBatch, Color color)
		{
			if (blinkCount <= 10) {
				spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, MathHelper.ToRadians(-degree), new Vector2(width / 2, height / 2), 1, SpriteEffects.None, 0);
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

				dColor = 1.0f;
				spriteBatch.Draw(texture, drawPos, animation.rect, color, MathHelper.ToRadians(-degree), new Vector2(width / 2, height / 2), 1, SpriteEffects.None, 0);
				spriteBatch.Draw(texture, drawPos, animation.rect, color, MathHelper.ToRadians(-degree), new Vector2(width / 2, height / 2), 1, SpriteEffects.None, 0);
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			} else if (blinkCount <= 20) {
				spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, MathHelper.ToRadians(-degree), new Vector2(width / 2, height / 2), 1, SpriteEffects.None, 0);
			} else if (blinkCount > 20) {
				inDmgMotion = false;
				spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, MathHelper.ToRadians(-degree), new Vector2(width / 2, height / 2), 1, SpriteEffects.None, 0);
			}
			if (!stage.isPausing) blinkCount++;
		}
        public override void Draw(SpriteBatch spriteBatch)
        {
			if (game.inDebugMode && IsActive()) spriteBatch.DrawString(game.titleFont, this.drawPos.ToString(), new Vector2(320, 48), Color.Orange);
			if (drawPos.X > 600) { }//639
			if (speed.X > 0) { }//15.6
			/*if (IsActive() && IsBeingUsed()) {
				//base.DrawDamageBlinkOnce(spriteBatch, Color.Red);
				spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, MathHelper.ToRadians(-degree), new Vector2(width / 2, height / 2), 1, SpriteEffects.None, 0);
			}*/
			if (IsActive()) {
				if (!inDmgMotion) {
					spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, MathHelper.ToRadians(-degree), new Vector2(width / 2, height / 2), 1, SpriteEffects.None, 0);
					DrawComboCount(spriteBatch);
				} else {
					//spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, 0, Vector2.Zero, Vector2.One, !turnsRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, .0f);
					DrawComboCount(spriteBatch);
					//DrawDamageBlink(spriteBatch, /*new Color(100, 50, 50)*/Color.Red, .1f);//.05f
					DrawDamageBlinkOnce(spriteBatch, Color.Red);
				}
			}
        }
    }
}
