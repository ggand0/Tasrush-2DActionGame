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
		#region member variable

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
				if (time % 3 == 0) {
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
						DamageMethodInicialize();
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
						DamageMethodInicialize();
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
						DamageMethodInicialize();
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
						DamageMethodInicialize();
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

		/// <summary> DamageUpdate
		/// 敵のダメージ判定の管理:9/16 判定の流れ
		/// 剣と敵が重なる→交差判定→isDamaged=true→!hasAttackedなら→stage.charactersのi番目がダメージ判定されたcharacterの番号を管理する
		/// damagedEnemyNumの番号と被っていなければ(前フレームまででダメージを与えたEnemyじゃ無ければ)→damagedObjectsに敵を追加(新しく判定する敵)
		/// →damagedEnemyNumに追加した敵の番号を記録→その敵の判定開始→damagedObjectsのそれぞれのdamageTime++→damagetime==1ならその敵のダメージ判定(1回の攻撃で1つの敵にに対して1回だけ判定)
		/// →その敵のMotionUpdateでHP減らしたり吹き飛ばしたりなんたら
		/// </summary>
		/// 11/10:ListをactiveCharaactersからactiveObjectsに拡張
		/// 1月現在、Damage管理クラスを作ったのでもはや使っていない
		public void DamageUpdate()
		{
			// 敵のisDamaged:
			// 最初の1フレームだけisDamagedをtrueにして、あとは攻撃終了時までfalseにする
			if (!player.hasAttacked) {
				// 攻撃終了してない→攻撃中なら(isAttackingで統一してもいい)
				//damageTime++;// 複数を対象(暫定的)　但し剣の先端が判定されなくなる
				for (int i = 0; i < stage.activeObjects.Count; i++) {
					if (stage.activeObjects[i].isDamaged) {// 最初にtrueになったときのみに限定
						if (stage.activeObjects[i] is Player)
							player.MotionUpdate();
						//curDamagedEnemy = stage.characters[i];
						//if (curDamagedEnemy != prevDamagedEnemy) damageTime = 0;

						/*for(int k=0;k<damagedObjectNum.Length;k++) {// これでは当然ダメ
							if(i != damagedObjectNum[k])damagedObjects.Add(stage.characters[i]);
						}*/
						if (damagedObjectNum.Any((x) => x == i)) {// 11/20:listの番号で同じ敵かどうか判断するのでactiveObjectsでまとめないと変なことに。

							// 変更：iと同じ値の要素を持つならば何もし無い...はずだがそうなっていない
						} else {
							damagedObjects.Add(stage.activeObjects[i]);// このクラスのListに追加
							damagedObjectNum[0 + (damagedObjects.Count - 1)/*counter*/] = i;// 判定済みの敵を記憶
						}

						for (int l = 0; l < damagedObjects.Count; l++) {// その敵の判定開始 2回(damageC.Count分)damageTimeが判定されてしまう　面倒だ
							damageTime[l]++;
						}
						//damageTime[0 + counter]++;/

						// ↓Lengthじゃ常に10　 D_characters.Countだと毎回　
						for (int m = 0; m < stage.damagedObjects.Count; m++) {
							if (damageTime[0 + m] == 1/*(damagedObjects.Count - 1)]==1*/ /*|| damageTime==5暫定！*/) {//このdamageTimeを調整して途中でダメージが1回だけ入る仕様にも変更可。damageTimeは攻撃のたびに初期化する必要あり
								//stage.characters[i].MotionUpdate();// ここが大事！
								//for(int i=0;i<stage.damagedObjects.Count;i++) {// ソートしたのに全部判定して重複してた
								stage.activeObjects[damagedObjectNum[m]].MotionUpdate();// これじゃあ2番目くらいまでしか呼ばれないだろう！当たり前
								//}
							}
						}
						counter++;// 判定した敵の数
						stage.activeObjects[i].isDamaged = false;

					}// (解決済)技を連続して出して当たるとisDamagedがfalseのままになるようだ　３段目だけ当てたらMotionUpdateが実行されて上に跳んだ
				}
				//prevDamagedEnemy = curDamagedEnemy;

				// Weapons 根本的にlistを統一するかdamagedCharactersとdamagedWeaponsで分けるか、もしくはフレーム毎にdamagedObjectsのCharacterである要素の個数を計算して引くか。

				// (解決済)技を連続して出して当たるとisDamagedがfalseのままになるようだ　３段目だけ当てたらMotionUpdateが実行されて上に跳んだ

			}
		}
		public void DamageUpdate2()
		{
			if (!stage.player.hasAttacked) {
				for (int i = 1; i < stage.activeCharacters.Count; i++) {
					if (stage.activeCharacters[i].isDamaged) {                       // 最初にtrueになったときのみに限定
						if (damagedObjectNum.Any((x) => x == i)) { } else {
							damagedObjects.Add(stage.activeCharacters[i]);      // Listに追加
							damagedObjectNum[0 + (damagedObjects.Count - 1)] = i;// 判定済みの敵を記憶
						}
						for (int l = 0; l < damagedObjects.Count; l++) damageTime[l]++;
						for (int m = 0; m < stage.damagedObjects.Count; m++) {
							if (damageTime[0 + m] == 1) {//このdamageTimeを調整して途中でダメージが1回だけ入る仕様にも変更可。damageTimeは攻撃のたびに初期化する必要あり
								stage.activeCharacters[damagedObjectNum[m]].MotionUpdate();
							}
						}
						counter++;                   // 判定した敵の数
						stage.activeCharacters[i].isDamaged = false;
					}
				}
			}
		}
		private void DamageMethodInicialize()
		{
			// 要改良.ダメージ管理を移行した後は削除予定
			for (int i = 0; i < damageTime.Length; i++) {
				damageTime[i] = 0;
				stage.damageControl.damageTime[i] = 0;
			}
			for (int j = 0; j < damagedObjectNum.Length; j++) {
				damagedObjectNum[j] = 0;
				stage.damageControl.damagedObjectNum[j] = 0;
			}
			counter = 0;

			damagedObjects.Clear();
			stage.damageControl.damagedObjects.Clear();
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
						if ((user as Character).turnsRight) spriteBatch.Draw(effects[0], drawPos + new Vector2(24, -56), animation.rect, Color.White);
						else spriteBatch.Draw(effects[0], drawPos + new Vector2(-68, -56), animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
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
