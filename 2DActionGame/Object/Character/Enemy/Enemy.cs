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
	public interface IEnemyAttribute
	{

	}

	/// <summary>
	/// 敵性Objectに付けるパーツ？クラス。
	/// 属性を使おうかと思ったがUpdate()もメンバに入れるためにクラスにした。
	/// </summary>
	public class EnemyAttribute
	{
		#region 保留
		private static Game1 game;
		private static SpriteBatch spriteBatch;
		private static ContentManager content;
		public static void Inicialize(Game1 game, SpriteBatch spriteBatch, ContentManager content)
		{
			EnemyAttribute.game = game;
			EnemyAttribute.spriteBatch = spriteBatch;
			EnemyAttribute.content = content;
		}

		private SoundEffect damageSound, hitSound;
		private int totalHits, delayTime, comboTime, comboCount, time, counter;
		private bool isBlownAway, isMovingAround, isInDamageMotion, isWinced;
		private Object user;
		private Stage stage;

		public int motionDelayTime = 60;
		public EnemyAttribute(Stage stage, Object user)
		{
			this.stage = stage;
			this.user = user;
		}

		public void Load()
		{
			damageSound = content.Load<SoundEffect>("Audio\\SE\\damage");
			hitSound = content.Load<SoundEffect>("Audio\\SE\\hit_big");
		}
		public void Update()
		{
			//base.Update();//含UpdateNumbers();
			if (user.HP <= 0 && time > comboTime) {
				if (!game.isMuted) damageSound.Play(SoundControl.volumeAll, 0f, 0f);
				user.isAlive = false;
			}// HP0でフルボッコが終わったら志望
			if (!user.isAlive && counter == 0) {
				user.isEffected = true;
				user.deathEffected = true; 
				counter++;
			}
			if (time > comboTime) {
				comboCount = 0;
			}
			if (stage.player.isThrusting && user.isDamaged) {
				MotionUpdate(user.speed, user.timeCoef);
			}
			MotionDelay();

			time++;
			delayTime++;
		}

		/// <summary>
		/// Combotimeを超えない限り死なない設定:isDamagedがtrueになったときにcounter=0にしてUpdate内で++;としたい
		/// counter ＜ combtimeなら続行　差分でその分ボーナスとかも可
		/// </summary>
		public void MotionUpdate(Vector2 speed, float timeCoef)
		{
			float distance = user.position.X - stage.player.position.X;

			if (user.isDamaged && user.isAlive) {
				if (stage.player.isCuttingUp) {
					BlownAwayMotionUp();
					isBlownAway = true;
				} else if (stage.player.isCuttingAway) BlownAwayMotionRight();
				else if (stage.player.isCuttingDown) BlownAwayMotionDown();
				else if (stage.player.isAttacking3) {
					if (distance > 0) user.speed.X += 5 * user.timeCoef;
					else user.speed.X += -5 * user.timeCoef;
					user.speed.Y -= 5;
					user.HP--;
				} else if (stage.player.isAirial) {
					if (distance > 0) user.speed.X += 5 * user.timeCoef;
					else user.speed.X += -5 * user.timeCoef;
					user.speed.Y += 5 * user.timeCoef;
					//HP--;
				} else if (stage.player.isThrusting && time % 3 == 0) {
					if (distance > 0) user.speed.X += 1 * user.timeCoef;
					else user.speed.X += -1 * user.timeCoef;
					speed.Y -= 1 * timeCoef;
				} else if (!stage.player.isThrusting) {
					if (distance > 0) user.speed.X += 1.5f * user.timeCoef;
					else user.speed.X += -1.5f * user.timeCoef;
				}

				user.HP--;

				if (!game.isMuted) hitSound.Play(SoundControl.volumeAll, 0f, 0f);

				if (stage.player.normalComboCount >= 3)
					comboTime = 60;
				else
					comboTime = 30;

				totalHits += 1;
				time = 0;
				delayTime = 0;
				user.isEffected = true;
				user.damageEffected = true;
				if (time < comboTime) comboCount++;
				time++;

				//int adj = 0;
				//adj = stage.maxComboCount;
				game.score += stage.inComboObjects.Count + (1 + stage.gameStatus.maxComboCount * .01f);//10 * stage.inComboObjects.Count * stage.maxComboCount;
			}
		}
		private void MotionDelay()
		{
			if (delayTime < motionDelayTime) {
				user.Gravity = .60;
				if (stage.player.normalComboCount < 3) {
					user.speed.Y = 0;
					user.Gravity = 0;
				}
				isMovingAround = false;
				isInDamageMotion = true;
				isWinced = true;
			} else {
				isMovingAround = true;
				isInDamageMotion = false;
				user.Gravity = .60;
				isWinced = false;
			}
		}
		private void BlownAwayMotionUp()
		{
			float speed = 5;
			int degree = 65;
			double radius = MathHelper.ToRadians(-degree);

			user.speed.X += (speed + 10) * (float)Math.Cos(radius) * user.timeCoef;
			user.speed.Y += (speed + 13) * (float)Math.Sin(radius) * user.timeCoef;
		}
		/// <summary>
		/// 斬り上げが当たった時の決まった軌跡を描く吹っ飛びのモーション
		/// </summary>
		private void BlownAwayMotionUp2()
		{
			float x = 0;

			if (counter > 30) { user.Gravity = .60; return; }
			x += 2;
			user.Gravity = 0;
			user.position.X += x * user.timeCoef;
			user.position.Y += -(float)(2 * Math.Pow(x, 2)) * user.timeCoef;// position.Xの2乗は凄まじいに決まってる！当たった地点からの位置にしよう

			counter++;
		}
		private void BlownAwayMotionRight()
		{
			float speed = 2;
			int degree = 60;//-65
			double radius = MathHelper.ToRadians(-degree);
			user.speed.X += (speed + 32) * (float)Math.Cos(radius) * user.timeCoef;
			user.speed.Y += (speed + 18) * (float)Math.Sin(radius) * user.timeCoef;
		}
		private void BlownAwayMotionDown()
		{
			float speed = 5;
			int degree = 60;//-65
			double radius = MathHelper.ToRadians(-degree);

			user.speed.X += (speed) * (float)Math.Cos(radius) * user.timeCoef;
			user.speed.Y += -(speed + 32) * (float)Math.Sin(radius) * user.timeCoef;
		}
		#endregion
	}

	/// <summary>
	/// 敵の基本クラス。
	/// </summary>
	[Author("pentium", Affiliation = "Kawaz")]
	public class Enemy : Character
	{
		protected int delayTime = 120;
		protected SoundEffect damageSound, hitSound;

		public float moveDistance = 20;
		protected Vector2 defPos;
		public bool isMovingAround { get; protected set; }
		public float scalarSpeed;
		/// <summary>
		/// 攻撃を受けているあいだ(反動が小さいもの)空中に留まらせたり
		/// </summary>
		public int motionDelayTime = 60;
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
		}

		public override void Update()
		{
			base.Update();

			if (HP <= 0 && time > deathComboTime) {
				if (!game.isMuted)
					if (!hasPlayedSE) {
						damageSound.Play(SoundControl.volumeAll, 0f, 0f);
						hasPlayedSE = true;
					}
				isAlive = false;
			}// HP0でフルボッコが終わったら志望
			if (!isAlive && counter == 0) {
				isEffected = true;
				deathEffected = true;
				counter++;
			}

			if (time > deathComboTime) {
				comboCount = 0;
				//hasPlayedSE = false;
			}
			if (!stage.player.isThrusting && isDamaged) MotionUpdate();// !いるよな...
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
				} else if (stage.player.isThrusting && time % 3 == 0) {
					if (distance > 0) speed.X += 1 * timeCoef;
					else speed.X += -1 * timeCoef;
					speed.Y -= 1 * timeCoef;
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
				game.score += stage.inComboObjects.Count + (1 + stage.gameStatus.maxComboCount * .01f);//10 * stage.inComboObjects.Count * stage.maxComboCount;
			}
		}
		protected virtual void MotionDelay()
		{
			if (delayTime < motionDelayTime) {
				Gravity = .60;
				if (stage.player.normalComboCount < 3) {
					speed.Y = 0;
					Gravity = 0;
				}
				isMovingAround = false;
				isInDamageMotion = true;
				isWinced = true;
			} else {
				isMovingAround = true;
				isInDamageMotion = false;
				Gravity = .60;
				isWinced = false;
			}
		}
		protected virtual void RoundTripMotion(float speed)
		{
			if (isMovingRight && defPos.X - moveDistance <= position.X && position.X < defPos.X + moveDistance) {
				turnsRight = true;
				position.X += speed * timeCoef;
			} else if (position.X >= defPos.X + moveDistance) {
				turnsRight = false;
				isMovingRight = false;
				position.X += -speed * timeCoef;
			} else if (!isMovingRight && defPos.X - moveDistance <= position.X && position.X <= defPos.X + moveDistance) {
				position.X += -speed * timeCoef;
			} else if (position.X <= defPos.X - moveDistance) {
				turnsRight = true;
				isMovingRight = true;
				position.X += speed * timeCoef;
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

			if (counter > 30) { Gravity = .60; return; }
			x += 2;
			Gravity = 0;
			position.X += x * timeCoef;
			position.Y += -(float)(2 * Math.Pow(x, 2)) * timeCoef;// position.Xの2乗は凄まじいに決まってる！当たった地点からの位置にしよう

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
				spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, .0f);
				DrawComboCount(spriteBatch);
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
