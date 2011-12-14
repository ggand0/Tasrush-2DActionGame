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
	/// 敵の基本クラス。
	/// </summary>
	[Author("pentium", Affiliation = "Kawaz")]
	public class Enemy : Character
	{
		/// <summary>
		/// 攻撃を受けているあいだ(反動が小さいもの)空中に留まらせたり
		/// </summary>
		public readonly int motionDelayTime = 60;
		public readonly byte defMovePattern;
		/// <summary>
		/// 何コンボ目で強制的に死ぬか。まだ未使用
		/// </summary>
		protected readonly int maxComboTime = 6;
		
		protected SoundEffect damageSound, hitSound;
		protected Vector2 defPos;
		/// <summary>
		/// 往復移動する仕様のときに使っていたもの
		/// </summary>
		public float moveDistance = 80;//20
		protected int delayTime = 120;// = 120の意味ないよね→地味にあった
		public byte movePattern { get; protected set; }
		/// <summary>
		/// 移動するかどうか
		/// </summary>
		public bool isMovingAround { get; protected set; }
		public float scalarSpeed;
		
		/// <summary>
		/// 怯んでいる状態か(接触によるダメージ判定が無い状態)
		/// </summary>
		public bool isWinced { get; protected set; }

		public Enemy()
		{
		}
		public Enemy(Stage stage, float x, float y, int width, int height)
			: this(stage, x, y, width, height, 2)
		{
		}
		public Enemy(Stage stage, float x, float y, int width, int height, int HP)
			: this(stage, x, y, width, height, HP, null)
		{
		}
		public Enemy(Stage stage, float x, float y, int width, int height, int HP, Character user)
			: base(stage, x, y, width, height, user)
		{
			this.HP = HP;
			defPos = new Vector2(x, y);

			Load();
		}
		protected override void Load()
		{
			base.Load();

			damageSound = content.Load<SoundEffect>("Audio\\SE\\damage");
			hitSound = content.Load<SoundEffect>("Audio\\SE\\hit_big");
			LoadXML("Enemy", "Xml\\Objects_Base.xml");
		}

		public override void Update()
		{
			base.Update();

			// HP0でフルボッコが終わったら死亡
			if (HP <= 0 && time > deathComboTime) {
				if (!game.isMuted)
					if (!hasPlayedSoundEffect) {
						damageSound.Play(SoundControl.volumeAll, 0f, 0f);
						hasPlayedSoundEffect = true;
					}
				isAlive = false;
			}
			if (!isAlive && counter == 0) {
				isEffected = true;
				deathEffected = true;
				counter++;
			}
			if (time > deathComboTime) {
				comboCount = 0;
			}
			MotionDelay();

			time++;
			delayTime++;
		}
		public override void UpdateAnimation()
		{
			animation.Update(3, 0, width, height, 6, 1);
		}

		/// <summary>
		/// Combotimeを超えない限り死なない設定:isDamagedがtrueになったときにcounter=0にしてUpdate内で++;としたい
		/// counter ＜ combtimeなら続行　差分でその分ボーナスとかも可
		/// </summary>
		public override void MotionUpdate()
		{
			base.MotionUpdate();
			/* 毎フレーム削られないための対策案：
			 * ①ishitがfalse→true→falseと変わって攻撃が終わったときにダメージ
			 * ②単純な無敵時間の追加
			 * ③一度当たったら、Playerが再び攻撃キーを押さない限り無敵
			 * ④1回の攻撃につき当たるのは1回のみ、攻撃は基本的に自動終了←これを採用
			 */
			float distance = position.X - stage.player.position.X;

			if (isDamaged && isAlive) {
				if (stage.player.isCuttingUp) {
					BlownAwayMotionUp(5, 65);
					isBlownAway = true;
				} else if (stage.player.isCuttingAway) {
					BlownAwayMotionRight(2, 60);
				} else if (stage.player.isCuttingDown) {
					BlownAwayMotionDown(5, 60);
				} else if (stage.player.isAttacking3) {
					if (distance > 0) speed.X += 5 * timeCoef;
					else speed.X += -5 * timeCoef;
					speed.Y -= 5;
					HP--;
				} else if (stage.player.isAirial) {
					if (distance > 0) speed.X += 5 * timeCoef;
					else speed.X += -5 * timeCoef;
					speed.Y += 5 * timeCoef;
					//HP--;
				} else if (stage.player.isThrusting && time % 5 == 0) {
					if (distance > 0) speed.X += .1f * timeCoef;// 1 * timeCoef
					else speed.X += -.1f * timeCoef;
					speed.Y -= .1f * timeCoef;
				} else if (!stage.player.isThrusting) {
					if (distance > 0) speed.X += 1.5f * timeCoef;
					else speed.X += -1.5f * timeCoef;
				}
				
				if (!game.isMuted) hitSound.Play(SoundControl.volumeAll, 0f, 0f);
				if (stage.player.normalComboCount >= 3) deathComboTime = 60;
				else deathComboTime = 30;

				HP--;
				totalHits += 1;
				time = 0;
				delayTime = 0;
				isEffected = true;
				damageEffected = true;
				if (time < deathComboTime) comboCount++;
				time++;

				int adj = 0;
				adj = stage.gameStatus.maxComboCount;
				game.stageScores[game.stageNum-1] += stage.inComboObjects.Count + (1 + stage.gameStatus.maxComboCount * .01f);//10 * stage.inComboObjects.Count * stage.maxComboCount;
			}
		}
		protected virtual void MotionDelay()
		{
			if (delayTime < motionDelayTime) {
				gravity = defGravity;
				if (stage.player.normalComboCount < 3) {
					speed.Y = 0;
					gravity = 0;
				}
				isMovingAround = false;
				isInDamageMotion = true;
				isWinced = true;
			} else {
				isMovingAround = true;
				isInDamageMotion = false;
				gravity = defGravity;
				isWinced = false;
			}
		}
		protected override void RoundTripMotion(Vector2 defPos, float moveDistance, float speed)
		{
			// turnsRightだと他の挙動で変える時に干渉されるのでisMovingRightを使う必要がある
			if (isMovingRight && defPos.X - moveDistance <= position.X && position.X < defPos.X + moveDistance) {
				//turnsRight = true;
				//position.X += speed * timeCoef;
				this.speed.X = speed > 0 ? speed : -speed * timeCoef;
			} else if (position.X >= defPos.X + moveDistance) {
				//isMovingRight = false;
				this.speed.X = speed > 0 ? -speed : speed * timeCoef;
			} else if (!isMovingRight && defPos.X - moveDistance <= position.X && position.X <= defPos.X + moveDistance) {
				this.speed.X =  speed > 0 ? -speed :speed * timeCoef;
			} else if (position.X <= defPos.X - moveDistance) {
				//isMovingRight = true;
				this.speed.X =  speed > 0 ? speed : -speed * timeCoef;
			}
		}
		protected virtual void BlownAwayMotionUp(float speed, int degree)
		{
			double radius = MathHelper.ToRadians(-degree);

			this.speed.X += (speed + 10) * (float)Math.Cos(radius) * timeCoef;
			this.speed.Y += (speed + 13) * (float)Math.Sin(radius) * timeCoef;
		}
		/// <summary>
		/// 斬り上げが当たった時の決まった軌跡を描く吹っ飛びのモーション
		/// </summary>
		protected virtual void BlownAwayMotionUp2()
		{
			float x = 0;

			if (counter > 30) { gravity = defGravity; return; }
			x += 2;
			gravity = 0;
			position.X += x * timeCoef;
			position.Y += -(float)(2 * Math.Pow(x, 2)) * timeCoef;

			counter++;
		}
		protected virtual void BlownAwayMotionRight(float speed, int degree)
		{
			double radius = MathHelper.ToRadians(-degree);

			this.speed.X += (speed + 32) * (float)Math.Cos(radius) * timeCoef;
			this.speed.Y += (speed + 18) * (float)Math.Sin(radius) * timeCoef;
		}
		protected virtual void BlownAwayMotionDown(float speed, int degree)
		{
			double radius = MathHelper.ToRadians(-degree);

			this.speed.X += (speed) * (float)Math.Cos(radius) * timeCoef;
			this.speed.Y += -(speed + 32) * (float)Math.Sin(radius) * timeCoef;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (isAlive && isActive) {
				if (!inDmgMotion) {
					spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, 0, Vector2.Zero, Vector2.One, !turnsRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, .0f);
					DrawComboCount(spriteBatch);
				} else {// 一旦戻す
					spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, 0, Vector2.Zero, Vector2.One, !turnsRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, .0f);
					DrawComboCount(spriteBatch);
					//DrawDamageBlink(spriteBatch, Color.Red, .05f);//.60f 15f
				}
			}
		}
		protected void DrawComboCount(SpriteBatch spriteBatch)
		{
			if (isWinced) {
				spriteBatch.DrawString(game.pumpDemi, comboCount.ToString() + "Hit!!", drawPos + new Vector2(width, 0), Color.Orange, 0, Vector2.Zero, new Vector2(.3f), SpriteEffects.None, 0f);
			}
		}
	}
}
