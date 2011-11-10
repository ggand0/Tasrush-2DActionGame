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
	/// 地面などから飛び出す敵。40×40なら多分当たり判定関係をいじらずにBlockに重ねることで埋められるはず
	/// </summary>
	public class FlyingOutEnemy : Enemy
	{
		private readonly float defMoveDistance = 80;
		protected readonly float defSpeed = 2;
		protected readonly float defFlyingOutDistance = 300;
		protected readonly float defFlyingOutSpeed = -12;

		/// <summary>
		/// Playerとの距離
		/// </summary>
		protected float distance;
		/// <summary>
		/// 反応する距離
		/// </summary>
		protected float flyingOutDistance;
		protected float flyingOutSpeed;
		public bool hasFlownOut { get; protected set; }

		public FlyingOutEnemy(Stage stage, float x, float y, int width, int height, int HP)
			: base(stage, x, y, width, height, HP)
		{
			LoadXML("FlyingOutEnemy", "Xml\\Objects_Enemy_Stage1.xml");
			defPos = new Vector2(x, y);
			flyingOutDistance = defFlyingOutDistance;
			moveDistance = defMoveDistance;
			flyingOutSpeed = defFlyingOutSpeed;

			//delayTime = 120;// motionDelayTime + 1;
			//isMovingRight = true;
			isMovingAround = true;

			Load();
		}
		protected override void Load()
		{
			base.Load();

			texture = content.Load<Texture2D>("Object\\Character\\FlyingOutEnemy1");
		}

		public override void Update()
		{
			if (IsActive()) {
				if (isMovingAround) MovementUpdate();
			}

			base.Update();
		}
		public override void UpdateAnimation()
		{
			if (!hasFlownOut || (hasFlownOut && !isOnSomething)) {
				animation.Update(0, 0, 40, 40, 0, 0);
			} else if (hasFlownOut && isOnSomething) {
				animation.Update(2, 0, 40, 40, 10, 1, 1, 2);
			}
		}
		public virtual void MovementUpdate()
		{
			distance = Math.Abs(this.position.X - stage.player.position.X);

			if (!hasFlownOut) {
				isOnSomething = false;										// 近づく前に飛び出してもらっては困る
				gravity = 0;												// 落ちても困る

				if (distance < flyingOutDistance) {
					if (counter == 0) {
						speed.Y = flyingOutSpeed;
					}
					counter++;
				}
				if (position.Y <= defPos.Y - 32) {
					hasFlownOut = true;
					gravity = defGravity;
				}
			} else if (hasFlownOut && isOnSomething) {						// 飛び出し着地後の歩く処理
				RoundTripMotion(defPos, moveDistance, defSpeed);
				counter = 0;
			}
		}

		public override void MotionUpdate()
		{
			float distance = position.X - stage.player.position.X;

			if (isDamaged && isAlive) {
				if (hasFlownOut) {
					if (stage.player.isCuttingUp) {
						BlownAwayMotionUp(5, 65);
						isBlownAway = true;
					} else if (stage.player.isCuttingAway) {
						BlownAwayMotionRight(2, 60);
					} else if (stage.player.isCuttingDown) {
						BlownAwayMotionDown(5, 60);
					} else if (stage.player.isAttacking3) {
						if (distance > 0) speed.X += 5;
						else speed.X += -5;
						speed.Y -= 5;
						HP--;
					} else if (stage.player.isAirial) {
						if (distance > 0) speed.X += 5;
						else speed.X += -5;
						speed.Y += 5;
						//HP--;
					} else if (stage.player.isThrusting && time % 3 == 0) {
						if (distance > 0) speed.X += 1;
						else speed.X += -1;
						speed.Y -= 1;
					} else if (!stage.player.isThrusting) {
						if (distance > 0) speed.X += 1.5f;
						else speed.X += -1.5f;
					}
				}

				HP--;
				totalHits++;
				time = 0;
				delayTime = 0;
				isEffected = true;
				damageEffected = true;

				if (!game.isMuted) hitSound.Play(SoundControl.volumeAll, 0f, 0f);
				if (time < deathComboTime) {
					comboCount++;
				}
				time++;
			}
		}
		protected override void MotionDelay()
		{
			// EnemyでG=.60とされてUN()で加算されていたのが非攻撃時にずり落ちてた原因だ！
			if (delayTime < motionDelayTime) {
				if (stage.player.normalComboCount < 3) {
					speed.Y = 0;
					if (isAlive) gravity = 0;
					else gravity = defGravity;
				}
				isMovingAround = false;
				isInDamageMotion = true;
				isWinced = true;
				//gravity = defGravity;
			} else {
				isMovingAround = true;
				isInDamageMotion = false;
				if (hasFlownOut) gravity = defGravity;
				//isWinced = true;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (IsActive()) {
				if (!hasFlownOut) {

					spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, .0f);//spriteBatch.Draw(textures, drawPos, animation.rect, Color.White);
				} else {
					spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, .0f);
				}
				DrawComboCount(spriteBatch);
			}
		}

	}
}
