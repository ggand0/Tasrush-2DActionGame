using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;


namespace _2DActionGame
{
	/// <summary>
	/// 武器全般の基本クラス。
	/// </summary>
	public class Weapon : Object
	{
		/// <summary>
		/// userの座標を基準とした位置
		/// </summary>
		protected Vector2 localPosition;
		/// <summary>
		/// 自立して動くのか否か
		/// </summary>
		public bool isAutonomous { get; protected set; }
		/// <summary>
		/// 攻撃が終了したらtrue、攻撃開始時にfalseに設定
		/// </summary>
		public bool isEnd { get; internal set; }
		/// <summary>
		/// デバッグ用（weaponは判別しにくいので）
		/// </summary>
		public bool isMarked { get; internal set; }

		public Weapon()
			: base()
		{
		}
		public Weapon(Stage stage, float x, float y, int width, int height)
			: this(stage, x, y, width, height, null)
		{
		}
		public Weapon(Stage stage, float x, float y, int width, int height, Object user)
			: this(stage, x, y, width, height, user, Vector2.Zero)
		{
		}
		public Weapon(Stage stage, float x, float y, int width, int height, Object user, Vector2 localPosition)
			: base(stage, x, y, width, height)
		{
			this.localPosition = localPosition;
			this.user = user;
			animation = new Animation(width, height);
			activeDistance = user is Boss ? 1280 : 640;
		}

		public virtual void Initialize()
		{
		}
		public virtual void Attack()
		{
		}
		public virtual void Move()
		{
		}

		public override void MotionUpdate()
		{
			/* 毎フレーム削られないための対策：
			 * ①ishitがfalse→true→falseと変わって攻撃が終わったときにダメージ
			 * ②単純な無敵時間の追加
			 * ③一度当たったら、Playerが再び攻撃キーを押さない限り無敵
			 * ④1回の攻撃につき当たるのは1回のみ、攻撃は基本的に自動終了←これを採用.　Playerは自動TASにするのでそこまで詳細は必要ない
			 */
			if (isDamaged && isAlive) {
				if (stage.player.isCuttingUp) {
					//BlownAwayMotionUp();isBlownAway = true; //counter = 0;//BlownAwayMotion2();
				} else if (stage.player.isCuttingAway) {//(stage.player.isCuttingDown || stage.player.isCuttingDownFromAirV2) {
					//BlownAwayMotionRight();
				} else if (stage.player.isCuttingDown) {//(stage.player.isCuttingDown || stage.player.isCuttingDownFromAirV2) {
					//BlownAwayMotionDown();
				} else if (stage.player.isAttacking3) {
					speed.X += 5;
					speed.Y -= 5;
				} else if (stage.player.isThrusting && time % 3 == 0) {
					speed.X += 1;
					speed.Y -= 1;
				} else if (!stage.player.isThrusting) {
					speed.X += 1.5f;// 現状では3段入れるならこのくらい　２段なら10～20でいい　
				}
				//motionDelayTime++;

				HP--;
				if (HP <= 0) {
					isAlive = false;
					//Cue cue = game.soundBank.GetCue("damage");
					//if(!game.isMuted) cue.Play(SoundControl.volumeAll, 0f, 0f);
				} else {
					//Cue cue = game.soundBank.GetCue("critical");
					//if(!game.isMuted) cue.Play(SoundControl.volumeAll, 0f, 0f);
				}

				//totalHits += 1;
				time = 0;
				//delayTime = 0;

				isEffected = true;
				damageEffected = true;

				/*if(time < deathComboTime) {
					comboCount++;
				}*/
				time++;
			}
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			if (/*user.isAttacking &&*/user != null ? (user.isAlive && isBeingUsed) : isBeingUsed)
				//-degreeで引数に持たせると普通の座標系で使う感覚で使える
				spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, MathHelper.ToRadians(-degree), Vector2.Zero, 1, SpriteEffects.None, .0f);
		}
	}
}
