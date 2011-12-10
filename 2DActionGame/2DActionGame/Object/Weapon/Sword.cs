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
	/// 剣(+その動き)のクラス.今の予定だと内容的には剣+腕って感じ.
	/// </summary>
	public class Sword : Weapon
	{
		#region Member variable

		private Vector2 criterionVector;
		private Vector2 originVector;                        // 回転軸を変化させる.基本的に不使用
		public Rectangle rect = new Rectangle(0, 0, 80, 8);  // 大きめの画像から切り出すので剣の大きさは可変
		public Character prevDamagedEnemy;
		public Character curDamagedEnemy;
		public List<Object> damagedObjects { get; set; }
		private Player player { get; set; }
		private Turret beamTurret { get; set; }

		// Motion
		public int[] damageTime = new int[100];              // 攻撃が当たってから何フレーム目でダメージを与えるか.要素数は同時判定可能な数
		public int[] damagedObjectNum = new int[100];        // Listに直さねば...
		//public int[] damageTime2 = new int[100];
		//public int[] damagedWeaponNum = new int[100];

		private int delay;                                   // 2段目攻撃delay用のカウンタ(ローカル変数にしようとしたら何故か上手くいかなかった)
		/// <summary>
		/// Thrusting時のアニメーション用カウンタ
		/// </summary>
		private byte thrustCount;
		public int degreeCounter { get; set; }              // 初期化用
		private int randomDegree { get; set; }
		private Vector2 deltaSize;
		private bool isShootingBeam;
		#endregion
		private SoundEffect[] slashSounds = new SoundEffect[5];
		public Texture2D[] effects = new Texture2D[5];

		public Sword(Stage stage, float x, float y, int width, int height)
			: this(stage, x, y, width, height, null)
		{
		}
		public Sword(Stage stage, float x, float y, int width, int height, Character user)
			: base(stage, x, y, width, height)
		{
			this.user = user;
			//if (stage.player != null) this.player = stage.player;
			if (user is Player) this.player = user as Player;

			position = new Vector2();
			//criterionVector = new Vector2(this.player.position.X + player.width / 2, player.position.Y + player.height / 2);
			criterionVector = new Vector2(this.user.position.X + user.width / 2, user.position.Y + user.height / 2);

			damagedObjects = new List<Object>();
			degree = 100;
			//swordBeam = new Bullet(stage,this,null,this.position,40,8);
			beamTurret = new Turret(stage, this, new Vector2(5, 5), 40, 8, 0, 1, 1);
			animation = new Animation(64, 96);
			animation2 = new Animation(128, 80);

			Load();
		}
		protected override void Load()
		{
			base.Load();

			texture = content.Load<Texture2D>("Object\\Weapon\\Melee");
			if (user is Rival) {
				effects[0] = content.Load<Texture2D>("Effect\\SlashEffectVR");
				effects[1] = content.Load<Texture2D>("Effect\\SlashEffectHR");
			} else {
				effects[0] = content.Load<Texture2D>("Effect\\SlashEffectV");
				effects[1] = content.Load<Texture2D>("Effect\\SlashEffectH");
				effects[2] = content.Load<Texture2D>("Effect\\SlashEffectUp");
				effects[3] = content.Load<Texture2D>("Effect\\SwordBeam");
				effects[4] = content.Load<Texture2D>("Effect\\slash");
			}

			slashSounds[0] = content.Load<SoundEffect>("Audio\\SE\\syakiiin");
			slashSounds[1] = content.Load<SoundEffect>("Audio\\SE\\zangeki");
			slashSounds[2] = content.Load<SoundEffect>("Audio\\SE\\zangeki2");
			slashSounds[3] = content.Load<SoundEffect>("Audio\\SE\\zangeki3");
		}


		/// <summary>
		/// 12/27:Swordに関しては locusDegreeの関係でuserがいても毎フレームupdateしてもらわなくては困る！
		/// </summary>
		public override void Update()
		{
			// CharacterにisAttacking2などのメンバを全て書くわけには行かないのでuser.～ではなくplayerを使う...しかしRivalは？
			// 今の構造を維持するならPlayerとRivalを統合するクラスを作ることだが、継承関係の問題が発生してしまう。
			// userが絡まない動きの部分だけメソッド化するなどで対処？
			if (user is Player) {
				// メソッドを消して移行した
				if (!player.isAttacking1 && !player.isAttacking2 && !player.isAttacking3) {
					Inicialize();
				}
			} else if (user is Boss) {
				// これも
				(user as Rival).hasAttacked = false;
			}
			UpdateAnimation();

			// ログの更新
			locusDegree.Add((int)degree);//drawVectorにすべきか？
			if (locusDegree.Count > 2) locusDegree.RemoveAt(0);
			
			time++;
			counter++;
		}
		public override void UpdateAnimation()
		{
			if (user is Player) {
				if ((user as Player).isAttacking1 || (user as Player).isCuttingDown || (user as Player).isCuttingAway || (user as Player).isAirial)
					animation.Update(3, 0, 64, 96, 2, 5);
				else if ((user as Player).isAttacking2)
					animation2.Update(3, 0, 128, 80, 2, 5);
				else if ((user as Player).isAttacking3)
					animation.Update(3, 0, 64, 96, 4, 5);
				else if ((user as Player).isCuttingUp)
					animation.Update(3, 0, 64, 96, 4, 5);
				else if ((user as Player).isThrusting)
					animation.Update(5, 0, 64, 64, 3, 1);
					//animation.Update(1, 0, 64, 16, 3, 0);
					//animation.Update(3, 0, 64, 96, 1, 1);
			}

			if (user is Rival) {
				if ((user as Rival).isAttacking1)
					animation.Update(3, 0, 64, 96, 2, 5);
				else if ((user as Rival).isAttacking2)
					animation2.Update(3, 0, 128, 80, 2, 5);
				else if ((user as Rival).isAttacking3)
					animation.Update(3, 0, 64, 96, 4, 5);
			}
		}

		private void Inicialize()
		{
			if ((user as Character).turnsRight) {
				position = user.position + new Vector2(user.width - 8, user.height / 2);
				drawPos = user.drawPos + new Vector2(user.width - 8, user.height / 2);
			} else {
				position = user.position + new Vector2(8, user.height / 2);
				drawPos = user.drawPos + new Vector2(8, user.height / 2);
			}
		}
		#region Actions
		/// <summary>
		/// 剣の挙動。 PlayerAttackingのブロック中に書いていたのをメソッドに分割することにした。
		/// </summary>
		internal void SlashVertically(bool turnsRight, int width, int height)
		{
			int adj;                                                // means "adjustment"
			int defaultDegree;

			if (turnsRight) adj = 1;
			else adj = -1;
			defaultDegree = 90 + adj * 10;
			//hasAttacked = false;

			if (degreeCounter == 0) {// rival 再び0になってしまっているので無限ループ
				Inicialize();
				isEnd = false;
				degree = defaultDegree;
				this.width = rect.Width = width;                                 // rect初期化
				this.height = rect.Height = height;
				animation.poseCount = 0;
				animation.count = 0;
				if (!game.isMuted) slashSounds[0].Play(SoundControl.volumeAll, 0f, 0f);
				degreeCounter++;
			}
			if ((turnsRight) ?
				degree > defaultDegree + (-1) * adj * 130 : //degree < 130
				degree < defaultDegree + (-1) * adj * 130)//degree > defaultDegree + adj * 130) 

				degree += (-1) * adj * 20;                          // 振るスピード(°)
			else {                                                  // 攻撃終了
				isEnd = true;
				isBeingUsed = false;
				degreeCounter = 0;
				//DamageMethodInicialize();   // これいらなくね?
			}

		}
		internal void SlashHorizontally(bool turnsRight, int width, int height)
		{
			int adj;                                                // means "adjustment"
			int defaultDegree;

			if (turnsRight) adj = 1;
			else adj = -1;
			defaultDegree = 90 + (-1) * adj * 115;
			//hasAttacked = false;

			if (degreeCounter == 0) {
				Inicialize();
				isEnd = false;
				degree = defaultDegree;                             // 角度初期化
				this.width = rect.Width = width;                    // rect初期化
				this.height = rect.Height = height;
				//DamageMethodInicialize();
				animation2.poseCount = 0;
				animation2.count = 0;

				if (!game.isMuted) slashSounds[0].Play(SoundControl.volumeAll, 0f, 0f);

				degreeCounter++;
			}
			if ((turnsRight) ?
				degree < defaultDegree + adj * 45 :
				degree > defaultDegree + adj * 45)

				degree += adj * 5;
			else {
				isEnd = true;
				isBeingUsed = false;
				degreeCounter = 0;
				//DamageMethodInicialize();
			}

		}
		internal void SlashHardlly(bool turnsRight, int width, int height)
		{
			int adj;                                                // means "adjustment"
			int defaultDegree;

			if (turnsRight) adj = 1;
			else adj = -1;
			defaultDegree = 90 + adj * 10;

			if (degreeCounter == 0) {
				isEnd = false;
				degree = defaultDegree;
				rect.Width = width;                                 // rect初期化
				rect.Height = height;
				deltaSize = Vector2.Zero;
				//if(turnsRight)
				position = user.position + new Vector2(user.width / 2, user.height - 15);// この辺が明らかに原因！
				//else position = user.position + new Vector2(user.width / 2, user.height - 15);
				//drawPos = user.drawPos + new Vector2(user.width / 2, user.height - 15);

				//DamageMethodInicialize();
				animation.poseCount = 0;
				animation.count = 0;
				if (!game.isMuted) slashSounds[0].Play(SoundControl.volumeAll, 0f, 0f);
				/*int n = rnd.Next(1);
				switch(n){
					case 0:
						cue2 = game.soundBank.GetCue("syakiiin");
						break;
					case 1:
						cue2 = game.soundBank.GetCue("zangeki2");
						break;
				}
				if(!game.isMuted) cue2.Play(SoundControl.volumeAll, 0f, 0f);*/
				degreeCounter++;
			}
			if ((turnsRight) ?
				degree > defaultDegree + (-1) * adj * 130 :
				degree < defaultDegree + (-1) * adj * 130) {
				deltaSize += Vector2.One;//new Vector2(2,2);//
				//deltaSize.X++;
				//deltaSize.Y++;

				degree += (-1) * adj * 10;                      // 振るスピード(°)
				if (turnsRight) position = user.position + new Vector2(user.width / 2 + deltaSize.X, user.height - 15 + deltaSize.Y);
				else position = user.position + new Vector2(user.width / 2 - deltaSize.X, user.height - 15 + deltaSize.Y);
				//drawPos = user.drawPos + new Vector2(user.width / 2 + deltaSize.X, user.height - 15 + deltaSize.Y);
				rect.Width += 4;
			} else {                                                 　// 攻撃終了
				isEnd = true;
				isBeingUsed = false;
				deltaSize = Vector2.Zero;
				rect.Width = 80;
				degreeCounter = 0;
				//DamageMethodInicialize();
			}

		}
		internal void SlashUp(bool turnsRight, int width, int height)
		{
			int adj;
			int defaultDegree;
			Vector2 d;

			if (turnsRight) adj = 1;
			else adj = -1;
			defaultDegree = -90 + adj * 70;

			if (degreeCounter == 0) {
				Inicialize();
				isEnd = false;
				degree = defaultDegree;
				rect.Width = width;                                 // rect初期化
				rect.Height = height;
				animation.poseCount = 0;
				animation.count = 0;
				d = Vector2.Zero;
				drawPos = user.drawPos + new Vector2(user.width / 2, user.height - 10);
				position = user.position + new Vector2(user.width / 2, user.height - 10);

				if (!game.isMuted) slashSounds[1].Play(SoundControl.volumeAll, 0f, 0f);
				degreeCounter++;
			}
			if ((turnsRight) ?
				degree < defaultDegree + adj * 150 : //130
				degree > defaultDegree + adj * 150) {//-240

				d = new Vector2(turnsRight ? 1 : -1, -2);
				position = user.position + new Vector2(user.width / 2 + d.X, user.height - 10 + d.Y);
				drawPos = user.drawPos + new Vector2(user.width / 2 + d.X, user.height - 10 + d.Y);
				degree += adj * 15;
			} else {
				isEnd = true;
				isBeingUsed = false;
				degreeCounter = 0;
			}
		}
		internal void SlashDown(bool turnsRight, int width, int height)
		{
			int adj;
			int defaultDegree;
			Vector2 d;

			if (turnsRight) adj = 1;
			else adj = -1;
			defaultDegree = 100 + adj * 10;

			if (degreeCounter == 0) {
				Inicialize();
				isEnd = false;
				degree = defaultDegree;
				rect.Width = width;
				rect.Height = height;
				animation.poseCount = 0;
				animation.count = 0;
				d = Vector2.Zero;
				drawPos = user.drawPos + new Vector2(user.width / 2, user.height - 15);
				position = user.position + new Vector2(user.width / 2, user.height - 15);

				if (!game.isMuted) slashSounds[2].Play(SoundControl.volumeAll, 0f, 0f);
				degreeCounter++;
			}
			if ((turnsRight) ?
				degree > defaultDegree + (-1) * adj * 110 : //100→-10 //いきなりEndになってた
				degree < defaultDegree + (-1) * adj * 110) {//-240

				d = new Vector2(turnsRight ? 1 : -1, 1);
				position = user.position + new Vector2(user.width / 2 + d.X, user.height - 15 + d.Y);
				drawPos = user.drawPos + new Vector2(user.width / 2 + d.X, user.height - 15 + d.Y);
				degree += adj * -20;
			} else {
				isEnd = true;
				isBeingUsed = false;
				degreeCounter = 0;
			}
		}
		internal void SlashDownFromAir(bool turnsRight, int width, int height)
		{
			int adj;                                                // means "adjustment"
			int defaultDegree;

			if (turnsRight) adj = -1;
			else adj = 1;
			defaultDegree = 90 + adj * 10;

			if (degreeCounter == 0) {// rival 再び0になってしまっているので無限ループ
				Inicialize();
				isEnd = false;
				degree = defaultDegree;
				rect.Width = width;                                 // rect初期化
				rect.Height = height;
				animation.poseCount = 0;
				animation.count = 0;
				drawPos = user.drawPos + new Vector2(user.width / 2, user.height / 2);
				position = user.drawPos + new Vector2(user.width / 2, user.height / 2);

				if (!game.isMuted) slashSounds[3].Play(SoundControl.volumeAll, 0f, 0f);
				degreeCounter++;
			}

			if ((turnsRight) ?
				degree > defaultDegree + adj * 250 :
				degree < defaultDegree + adj * 250) {

				degree += (-1) * adj * 20;                          // 振るスピード(°)
			} else {                                                // 攻撃終了
				isEnd = true;
				isBeingUsed = false;
				degreeCounter = 0;
			}
		}
		internal void AirialSlash(bool turnsRight, int width, int height)
		{
			int adj;
			int defaultDegree;
			Vector2 d;

			if (turnsRight) adj = 1;
			else adj = -1;
			defaultDegree = 100 + adj * 10;

			if (degreeCounter == 0) {
				Inicialize();
				isEnd = false;
				degree = defaultDegree;
				rect.Width = width;
				rect.Height = height;
				animation.poseCount = 0;
				animation.count = 0;
				d = Vector2.Zero;
				drawPos = user.drawPos + new Vector2(user.width / 2, user.height - 15);
				position = user.position + new Vector2(user.width / 2, user.height - 15);

				if (!game.isMuted) slashSounds[1].Play(SoundControl.volumeAll, 0f, 0f);
				degreeCounter++;
			}
			if ((turnsRight) ?
				degree > defaultDegree + (-1) * adj * 110 : //100→-10 //いきなりEndになってた
				degree < defaultDegree + (-1) * adj * 110) {//-240

				d = new Vector2(turnsRight ? 1 : -1, 1);
				position = user.position + new Vector2(user.width / 2 + d.X, user.height - 15 + d.Y);
				drawPos = user.drawPos + new Vector2(user.width / 2 + d.X, user.height - 15 + d.Y);
				degree += adj * -20;
			} else {
				isEnd = true;
				isBeingUsed = false;
				degreeCounter = 0;
			}
		}
		internal void BlowAway(bool turnsRight, int width, int height)
		{
			int adj;                                                // means "adjustment"
			int defaultDegree;

			if (turnsRight) adj = 1;
			else adj = -1;
			defaultDegree = 90 + adj * 10;

			if (degreeCounter == 0) {// rival 再び0になってしまっているので無限ループ
				Inicialize();
				isEnd = false;
				degree = defaultDegree;
				rect.Width = width;                                 // rect初期化
				rect.Height = height;
				animation.poseCount = 0;
				animation.count = 0;

				if (!game.isMuted) slashSounds[3].Play(SoundControl.volumeAll, 0f, 0f);
				degreeCounter++;
			}
			if ((turnsRight) ?
				degree > defaultDegree + (-1) * adj * 130 : //degree < 130
				degree < defaultDegree + (-1) * adj * 130)//degree > defaultDegree + adj * 130) 

				degree += (-1) * adj * 20;                          // 振るスピード(°)
			else {                                                  // 攻撃終了
				isEnd = true;
				isBeingUsed = false;
				degreeCounter = 0;
				//DamageMethodInicialize();   // これいらなくね?
			}
		}
		internal void SwordBeam2(bool turnsRight, int width, int height)
		{
		}
		internal void MegaThrust(bool turnsRight, int width, int height)
		{
			int defaultDegree = 100;

			if (turnsRight) {
				if (degreeCounter == 0) {
					degree = defaultDegree;
					rect.Width = width;
					rect.Height = height;
					time = 0;
					animation.poseCount = 0;
					animation.count = 0;

					degreeCounter++;// 初期化は1回
					isEnd = false;
					degree = 30;
				}
				//Random rnd1 = new Random();
				//randomDegree = rnd1.Next(10,60);
				if (time % 5 == 0) {// % 3
					if (!game.isMuted) slashSounds[0].Play(SoundControl.volumeAll, 0f, 0f);
				}
				// randomDegree;
				/*Random rnd2 = new Random();
				rnd1.Next(10);
				position = player.position + new Vector2(8,rnd1.Next(0,32));
				drawPos = player.drawPos + new Vector2(8, rnd1.Next(0, 32));*/

				if (time > 30) {// 攻撃終了
					isEnd = true;
					isBeingUsed = false;
					degreeCounter = 0;
				}
			}
		}
		internal void ThrustInDashing(bool turnsRight, int width, int height)
		{
			int adj;                                                // means "adjustment"
			int defaultDegree;

			if (turnsRight) adj = -1;
			else adj = 1;
			defaultDegree = 90 + adj * 90;

			if (degreeCounter == 0) {
				Inicialize();
				isEnd = false;
				degree = defaultDegree;								// 判定矩形の角度を変えないのでここで初期化するだけ
				rect.Width = width;                                 // rect初期化
				rect.Height = height;
				animation.poseCount = 0;
				animation.count = 0;

				if (!game.isMuted) slashSounds[3].Play(SoundControl.volumeAll, 0f, 0f);
				degreeCounter++;
			}

			if (time > 30) {										// 攻撃終了
				isEnd = true;
				isBeingUsed = false;
				degreeCounter = 0;
			}

			/*if (player.isDashAttacking) {// ダッシュ攻撃1:剣
				if (turnsRight) {
					if (degreeCounter == 0) {// 角度初期化
						degree = 0;
						degreeCounter++;// 初期化は1回
						//DamageMethodInicialize();
						rect.Width = 110;
						rect.Height = 16;
					}
					delay++;
					if (delay >= 40) {//指定frame攻撃を続かせて 終 了
						player.isDashAttacking = false;
						isHit = false;
						player.hasAttacked = true;
						player.isAttacking = false; isBeingUsed = false;
						degreeCounter = 0;
						delay = 0;
						//DamageMethodInicialize();
					}
				}
			}*/
		}
		internal void ShootBeam()
		{
			// 以下、前のをコピペしただけ
			if (player.isToChargingMotion) {
				if (turnsRight) {// 右向きのとき
					if (degreeCounter == 0) {// 角度初期化
						degree = 0;
						rect.Width = 120;// rect初期化
						rect.Height = 8;
						degreeCounter++;// 初期化は1回
						//DamageMethodInicialize();
						if (!game.isMuted) slashSounds[0].Play(SoundControl.volumeAll, 0f, 0f);

						//counter = 0;
					}
					if (degree < 135) degree += 20;// 振るスピード(°)
					else {// 攻撃終了
						player.isToChargingMotion = false;
						player.isChargingPower = true;
						//player.hasAttacked = true;
						//player.isAttacking = false; isBeingUsed=false;
						degreeCounter = 0;
						//DamageMethodInicialize();
					}
				}
			}
			if (player.isShootingBeam) {
				if (turnsRight) {// 右向きのとき
					if (degreeCounter == 0) {// 角度初期化
						degree = 135;
						rect.Width = 120;// rect初期化
						rect.Height = 8;
						degreeCounter++;// 初期化は1回
						//DamageMethodInicialize();
						if (!game.isMuted) slashSounds[0].Play(SoundControl.volumeAll, 0f, 0f);
					}
					if (degree > -35) degree += -20;// 振るスピード(°)
					else {// 攻撃終了
						player.isShootingBeam = false;
						isShootingBeam = true;
						counter = 0;
						player.hasAttacked = true;
						player.isAttacking = false; isBeingUsed = false;
						degreeCounter = 0;
						//DamageMethodInicialize();
					}
				}
				beamTurret.Update();
			}

			#region ShootBeam
			if (isShootingBeam /*&& counter < 100*/) {
				if (counter == 0) beamTurret.Inicialize();
			}
			if (isShootingBeam && counter < 100) beamTurret.Update();
			else if (isShootingBeam && counter > 100) {
				isShootingBeam = false;
				player.isShootingBeam = false;
				beamTurret.Inicialize();
				counter = 0;
			}
			#endregion
		}
		/// <summary>
		/// ジャンプ時のキャンセル用に呼ぶ
		/// </summary>
		internal void EndQuickly()
		{
			isEnd = true;
			isBeingUsed = false;
			degreeCounter = 0;
		}
		#endregion


		private void DrawThrustAnimation(SpriteBatch spriteBatch, bool turnsRight)
		{
			float deg = 0;
			if (thrustCount > 0 && thrustCount <= 10) {
				deg = 11;
			} else if (thrustCount > 10 && thrustCount <= 20) {
				deg = 3;
			} else if (thrustCount > 20 && thrustCount <= 30) {
				deg = 80;
			} else if (thrustCount > 30 && thrustCount <= 40) {
				deg = -60;
			} else if (thrustCount > 40 && thrustCount <= 50) {
				deg = -25;
			} else if (thrustCount > 5 && thrustCount <= 60) {
				deg = 40;
			}
			spriteBatch.Draw(effects[3], drawPos + new Vector2(width/2, height/2), animation.rect, Color.White, MathHelper.ToRadians(deg), Vector2.Zero, 1, turnsRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			
			if (thrustCount++ > 60) thrustCount = 0;
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			if ((user as Character).isAttacking) {
				//-degreeで引数に持たせると普通の座標系で使う感覚で使える
				if (game.visibleSword)
					spriteBatch.Draw(texture, drawPos, rect, Color.White, MathHelper.ToRadians(-degree), Vector2.Zero/*originVector*/, 1, SpriteEffects.None, 0);

				if (user is Player) {
					if ((user as Player).isAttacking1 || (user as Player).isCuttingAway) {
						if ((user as Player).isCuttingDown) { }
						if ((user as Character).turnsRight) spriteBatch.Draw(effects[0], drawPos + new Vector2(24, -56), animation.rect, Color.White);
						else spriteBatch.Draw(effects[0], drawPos + new Vector2(-68, -56), animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
					}
					if ((user as Player).isAttacking2) {
						if ((user as Character).turnsRight) spriteBatch.Draw(effects[1], drawPos + new Vector2(-10, -56), animation2.rect, Color.White);
						else spriteBatch.Draw(effects[1], drawPos + new Vector2(-96, -56), animation2.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
					}
					if ((user as Player).isAttacking3) {
						if ((user as Character).turnsRight) spriteBatch.Draw(effects[0], drawPos + new Vector2(32, -56), animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
						else spriteBatch.Draw(effects[0], drawPos + new Vector2(-68, -56), animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
					}
					if ((user as Player).isCuttingUp) {
						if ((user as Character).turnsRight) spriteBatch.Draw(effects[2], drawPos + new Vector2(32, -56), animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
						else spriteBatch.Draw(effects[2], drawPos + new Vector2(-68, -56), animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
					}
					if ((user as Player).isCuttingDown || (user as Player).isAirial) {
						if ((user as Character).turnsRight) spriteBatch.Draw(effects[0], drawPos + new Vector2(24, -56), animation.rect, Color.White);
						else spriteBatch.Draw(effects[0], drawPos + new Vector2(-68, -56), animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
					}

					if ((user as Player).isThrusting) {
						if ((user as Character).turnsRight) {
							//if (counter % 5 == 0) spriteBatch.Draw(effects[0], drawPos + new Vector2(24, -56), animation.rect, Color.White);
							//else if (counter % 3 == 0) spriteBatch.Draw(effects[1], drawPos + new Vector2(24, -56), animation.rect, Color.White);
							//else if (counter % 2 == 0) spriteBatch.Draw(effects[2], drawPos + new Vector2(24, -56), animation.rect, Color.White);
							//else spriteBatch.Draw(effects[0], drawPos + new Vector2(24, -56), animation.rect, Color.White);
							spriteBatch.Draw(effects[4], drawPos + new Vector2(0, -user.height * 3/4), animation.rect, Color.White, MathHelper.ToRadians(-20), Vector2.Zero, new Vector2(1.5f), SpriteEffects.None, 0);
						} else {
							spriteBatch.Draw(effects[4], drawPos + new Vector2(-68, -56), animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
						}/**/
						//DrawThrustAnimation(spriteBatch, user.turnsRight);
					}
				}

				if (user is Rival) {
					if ((user as Rival).isAttacking1) {
						if ((user as Character).turnsRight) spriteBatch.Draw(effects[0], drawPos + new Vector2(24, -56), animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
						else spriteBatch.Draw(effects[0], drawPos + new Vector2(-68, -56), animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
					} else if ((user as Rival).isAttacking2) {
						if ((user as Character).turnsRight) spriteBatch.Draw(effects[1], drawPos + new Vector2(-10, -56), animation2.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
						else spriteBatch.Draw(effects[1], drawPos + new Vector2(-96, -56), animation2.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
					} else if ((user as Rival).isAttacking3) {
						if ((user as Character).turnsRight) spriteBatch.Draw(effects[0], drawPos + new Vector2(32, -56), animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
						else spriteBatch.Draw(effects[0], drawPos + new Vector2(-68, -56), animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
					}
				}
			}
		}
	}
}
