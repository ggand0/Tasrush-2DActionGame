﻿using System;
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
	public class Boss : Enemy
    {
        #region Member variable
        private SoundEffect hitSoundSmall;
		public Object bind { get; protected set; }
		protected Vector2 bindSize = new Vector2(210, 210);
		protected  Vector2 bindPos = new Vector2(100, 100);
        //protected Vector2 bindPosOther = new Vector2(100, 100);

        //protected readonly Vector2 defBindPos= new Vector2(100, 100);
        //protected readonly Vector2 defBindPosOther = new Vector2(100, 100);
		//private int turretNum;// turretの数

		/// <summary>
		/// 現在行っている攻撃パターンのindex
		/// </summary>
		protected int attackNum;
		protected int resWaitTime;
		public int attackPatternNum { get; protected set; }
		public Vector2 defaultPosition { get; protected set; }
		public List<Vector2> shootPosition { get; protected set; }
		/// <summary>
		/// 所持しているTurretのリスト。
		/// </summary>
		public List<Turret> turrets { get; protected set; }
		/// <summary>
		/// 移動しながら攻撃するパターンの場合は使用しない
		/// </summary>
		protected Vector2 attackPosition;
		/// <summary>
		/// 1つの攻撃の中でのカウンタ
		/// </summary>
		protected int attackCounter;
		/// <summary>
		/// obstacleを操作するときに使うカウンタ
		/// </summary>
		protected int obsCounter;
		/// <summary>
		/// wait状態の時に使うカウンタ
		/// </summary>
		protected int waitCounter;

		/// <summary>
		/// 待機中かどうか
		/// </summary>
		public bool isWaiting { get; protected set; }
		/// <summary>
		/// 攻撃開始を意味するフラグ（初期化等に使う）
		/// </summary>
		public bool isStartingAttack { get; protected set; }
		/// <summary>
		/// 攻撃終了時に使用するフラグ（終了処理などに使う）
		/// </summary>
		public bool isEndingAttack { get; protected set; }

		


		private List<Vector2> speedVectors = new List<Vector2>();
		private List<float> distances = new List<float>();
		private List<bool> isReachedPoints = new List<bool>();
		//private int curLoc;
		//private bool isStartingMove = true;//, isMoving;

		// 攻撃関係
		/// <summary>
		/// 細かい挙動はAttackでは対応しきれなさそうであることを考慮に入れるとこれでいいか
		/// </summary>
		public List<int> attackList { get; protected set; }
		protected List<bool> isEnds = new List<bool>();
		protected List<int> attackNumList = new List<int>();
		/// <summary>
		/// 攻撃パターンのリスト。管理しやすいように二次元構造にした。
		/// </summary>
		protected List<int[]> attackPatternNumList = new List<int[]>();
		// この辺もっとスマートにしたかった
		protected Action attackMethodType0;
		protected Action<float> attackMethodType1;
		protected Action<int> attackMethodType2;
		protected Action<float, Vector2[]> attackMethodType3;
		protected Action<Vector2, Vector2, Vector2, bool> attackMethodType4;
		protected Action<Vector2[]> attackMethodType5;
        protected Action<int, bool> attackMethodType6;
		/// <summary>
		/// 攻撃時に使うメソッドへの参照をまとめているリスト。デリゲートというより関数ポインタ的に使っている。
		/// </summary>
		protected List<Delegate> attackMethods = new List<Delegate>();
		/// <summary>
		/// デリゲートに渡す引数のリスト。indexが同じattaackMethodsの要素（メソッド）に渡される。
		/// attackMethodsの、引数が無い関数のindexと同じindexの要素はnullにする。
		/// 引数を動的に変えたい場合は該当変数への参照を追加するようにすれば良い。
		/// 11/9 引数の数が２つ以上のメソッドに対応させるために二次元に拡張。いよいよクラス化したくなってきた
		/// </summary>
		protected List<object[]> attackMethodsArgs = new List<object[]>();
        #endregion
        protected Vector2 defBindPos = new Vector2(100, 100);
        protected Vector2 defBindPosOther = new Vector2(100, 100);

		/// <summary>
		/// 射撃をturretに任せるパターン
		/// </summary>
		/// <param name="turretNumber">射撃したいTurretのindex</param>
		protected void UpdateTurrets(int turretNumber)
		{
			if (turretNumber == -1) {// 一斉射撃
				for (int i = 0; i < turrets.Count; i++) {
					turrets[i].Update();
				}
			} else if (turretNumber < turrets.Count) {// 個別
				turrets[turretNumber].Update();
			} else {// 範囲外の値を指定された場合はListの最後のturretに射撃させる
				turrets[turrets.Count - 1].Update();
			}
		}
        /// <summary>
		/// 自Objectをウェイポイントに沿って移動させるメソッド。
		/// </summary>
		/// <param name="moveSpeed">移動速度[pixel/frame]</param>
		/// <param name="wayPoints">ウェイポイントのリスト</param>
		protected override void Move(float moveSpeed, Vector2[] wayPoints)
		{
			if (isStartingAttack) {
				speedVectors.Clear();

				speedVectors.Add(Vector2.Normalize(wayPoints[0] - position) * new Vector2(moveSpeed));
				for (int i = 0; i < wayPoints.Length - 1; i++) {
					speedVectors.Add(Vector2.Normalize(wayPoints[i + 1] - wayPoints[i]) * new Vector2(moveSpeed));
					//distances.Add(Vector2.Distance(position, wayPoints[i]));	// 要素の数だけ合わせておく。
					//isReachedPoints.Add(false);
				}
				speedVectors.Add(Vector2.Normalize(wayPoints[0] - wayPoints[wayPoints.Length - 1]) * new Vector2(moveSpeed));
				isStartingAttack = hasMoved = false;
				isAttacking = true;
				curLoc = 0;
			} else if (isAttacking) {
				//int cur = 0;
				/*for (int i = 0; i < wayPoints.Length; i++) {
					distances[i] = Vector2.Distance(position, wayPoints[i]);
					isReachedPoints[i] = distances[i] < 50;//moveSpeed;// つまり個々の点があまり近いと誤判定される
					if (isReachedPoints[i]) {
						if (i == wayPoints.Length - 1) {
							isEndingAttack = true;
							break;
						} else {
							cur = i;  
						}
					}
				}*/
				// 予め引くと実際の移動の時にずれる！！
				Vector2 speed = Vector2.Normalize(wayPoints[curLoc] - position) * new Vector2(moveSpeed);//speedVectors[curLoc];
				float distance = Vector2.Distance(position, wayPoints[curLoc]);//curLoc != wayPoints.Length - 1 ? Vector2.Distance(position, wayPoints[curLoc]) : Vector2.Distance(position, wayPoints[0]);// speedVectorの引き方を間違ったか？
				bool isReached = distance < moveSpeed;
				this.speed = speed;//speedVectors[curLoc];

				if (isReached) {
					curLoc++;
					isReached = false;
					if (curLoc == wayPoints.Length) {
						//isWaiting = true;// + 1
						hasMoved = true;
					}
				}
			}
		}
        protected override void UpdateDamage()
        {
            if (HP <= 0 && time > deathComboTime || isBlownAway && HP <= 0 && drawPos.X > Game1.Width - width) {// 吹っ飛び時の条件も加えてみた
                if (!game.isMuted)
                    if (!hasPlayedSoundEffect) {
                        damageSound.Play(SoundControl.volumeAll, 0f, 0f);
                        hasPlayedSoundEffect = true;
                    }
                //if (isAlive) game.stageScores[game.stageNum - 1] += 10000; // Clear時に移動す
                isAlive = false;
                isMovingAround = false;
                speed = Vector2.Zero;
                gravity = 0;
            }
            if (!isAlive && counter == 0) {
                isEffected = true;
                deathEffected = true;
                counter++;
            }
            if (time > deathComboTime) {
                comboCount = 0;
            }
            if (IsActive()) MotionDelay();

            time++;
            delayTime++;
        }

		protected virtual void MoveIni()
		{
			hasPlayedSoundEffect = false;
		}
		protected virtual void PlaySEWhileMoving()
		{
		}
		protected virtual void Attack(int waitTime)
		{
			if (isWaiting) {
				if (waitCounter > waitTime) {// 待機状態を終える前に初期化
					isStartingAttack = true;
					isAttacking = isEndingAttack = false;
					waitCounter = attackCounter = counter = 0;
					
					attackList.Add(attackPatternNumList[attackPatternNum][attackNum]);
					attackNum++;
					if (attackNum == attackPatternNumList[attackPatternNum].Length) attackNum = 0;

					isWaiting = false;
				}
				waitCounter++;
			} else {// 攻撃開始
				UpdateAttacking();
				attackCounter++;
			}
		}
		/// <summary>
		/// attackListに登録されているメソッドのindexに従って、attackMethodsに格納されている同indexの要素のデリゲートに
		/// 登録されている攻撃パターンメソッドを実行する。攻撃処理の核となる部分を担当するメソッド。
		/// </summary>
		protected virtual void UpdateAttacking()
		{
			for (int i = attackList.Count - 1; i >= 0; i--) {// ++だとremoveした時に前の要素がスルーされちゃう
				// 綺麗(?)に記述したかっただけで、実際やっていることはswitch文と同じ
				// nullも1つの引数扱いになるので一行で書くのは無理
				//attackMethods[attackList[i]].DynamicInvoke(attackMethodsArgs[attackList[i]]);
				if (attackMethodsArgs[attackList[i]] == null) {
					attackMethods[attackList[i]].DynamicInvoke();
				} else {
					// params object[]！すごい勘違いしやすい仕様だ　なんとか２個以上渡すようにしたいのだが...
					
					/*if (attackList[i] == 5) 
						attackMethods[5].DynamicInvoke(new object[] { new Vector2(defaultPosition.X - 350, defaultPosition.Y - 150)
								, new Vector2(defaultPosition.X - 200, defaultPosition.Y - 100)
								, defaultPosition
								, true });*/	// 通る
					/*object[] debug = new object[] { new Vector2(defaultPosition.X - 350, defaultPosition.Y - 150)
								, new Vector2(defaultPosition.X - 200, defaultPosition.Y - 100)
								, defaultPosition
								, true };*/
					//attackMethods[attackList[i]].DynamicInvoke(debug);//attackMethodsArgs[attackList[i]]);	// 通る
					attackMethods[attackList[i]].DynamicInvoke(attackMethodsArgs[attackList[i]]);			// 通らない
				}
				if (isEnds[attackList[i]]) {
					isEnds[attackList[i]] = false;
					attackList.Remove(attackList[i]);
				}
			}
		}
		protected override void DrawDamageBlinkOnce(SpriteBatch spriteBatch, Color color)
		{
			if (blinkCount <= 5) {
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

		// コンストラクタ
		public Boss(Stage stage, float x, float y, int width, int height, int HP)
			: this(stage, x, y, width, height, HP, 0, 0)
		{
		}
		public Boss(Stage stage, float x, float y, int width, int height, int HP, int bulletType, int shootType)// 4Fuujin?
			: base(stage, x, y, width, height, HP)
		{
			if (attackList == null) attackList = new List<int>();

			//bindSize = new Vector2(width, height);
			bindPos = Vector2.Zero;
			bind = new Object(stage, (int)bindSize.X, (int)bindSize.Y);
			bind.Load(game.Content, "Debug\\debugBind");
			//stage.objects.Add(bind);

			isWaiting = true;
			// Turretの、boss矩形内の配置位置（適当においただけ）
			shootPosition = new List<Vector2>();
			shootPosition.Add(new Vector2(30, 30));
			shootPosition.Add(new Vector2(30, 130));
			shootPosition.Add(new Vector2(30, 260));
			shootPosition.Add(new Vector2(width / 2, height / 2));

			// Turrets
			turrets = new List<Turret>();
			for (int i = 0; i < shootPosition.Count; i++) {
				turrets.Add(new Turret(stage, this, shootPosition[i], 32, 32, bulletType, shootType, 1));
			}
			//Load();
		}

		// overrideメソッド
		protected override void Load()
		{
			base.Load();
			hitSoundSmall = content.Load<SoundEffect>("Audio\\SE\\hit_Small");
		}
		public override void Update()
		{
			if (!isAlive) {
				foreach (Weapon weapon in weapons) {
					weapon.isBeingUsed = false;
				}
			}
			if ((position.X + width/2) - stage.player.position.X > 0) {
				turnsRight = false;
			} else if ((position.X + width / 2) - stage.player.position.X < 0) {
				turnsRight = true;
			}
            bindPos = !turnsRight ? defBindPos : defBindPosOther;
			bind.position = position + bindPos;
			bind.drawPos = drawPos + bindPos;
			if (stage.inBossBattle) activeDistance = 9999;// ボス戦時に端に行ったときにstage.activeObjectsに追加されなくなるため

			base.Update();
		}

		/// <summary>
		/// 今のところBossは仰け反らない仕様
		/// </summary>
		public override void MotionUpdate()
		{
			if (isDamaged) {// 固定画面なら攻撃されても動かない方がいいだろう
				/*if(stage.player.isCuttingUp) {
					BlownAwayMotion();isBlownAway = true; //counter = 0;//BlownAwayMotion2();
				}
				else if(stage.player.isCuttingAway) {//(stage.player.isCuttingDown || stage.player.isCuttingDownFromAirV2) {
					BlownAwayMotion3();
				}
				else{
					speed.X += 1.5;// 現状では3段入れるならこのくらい　２段なら10～20でいい　
				}*/
				//motionDelayTime++;

				if (!game.isMuted) {
					if (time % 5 == 0) {
						hitSound.Play(SoundControl.volumeAll, 0f, 0f);
					} else {
						hitSoundSmall.Play(SoundControl.volumeAll, 0f, 0f);
					}
				}
				HP--;

				totalHits += 1;
				isEffected = true;
				damageEffected = true;
				time = 0;   //raivalでcomboで使ってるtimeと同じだから干渉しないかどうか...
				delayTime = 0;
				inDmgMotion = true;
				blinkCount = 0;
				e = 0;
				// ４方向だけ差を出して妥協
				effectPos = new Vector2(position.X + width / 2 > stage.player.position.X + stage.player.width / 2 ? width/4 : width * 3/4
					, position.Y + height/2 > stage.player.position.Y + stage.player.height/2 ? height/4 : height * 3/4);

				if (time < deathComboTime) {
					comboCount++;
				}
				game.stageScores[game.stageNum-1] += stage.inComboObjects.Count * 200 + (1 + stage.gameStatus.maxComboCount * .01f);
                if (HP == 0) game.stageScores[game.stageNum - 1] += 1000;
			}
		}
		/// <summary>
		/// ひるみ無の仕様により何も書かない。
		/// </summary>
		protected override void MotionDelay()
		{
		}
		public override void UpdateAnimation()
		{
			animation.Update(3, 0, width, height, 6, 1);
		}
		protected override void UpdateNumbers()
		{
			speed.Y += (float)gravity;

			if (speed.X > 0) {
				speed.X += -(.40f * friction);
				if (speed.X < 0) speed.X = 0;
			} else if (speed.X < 0) {
				speed.X += (.40f * friction);
				if (speed.X > 0) speed.X = 0;
			}
			if (System.Math.Abs(speed.Y) > maxSpeed) speed.Y = maxSpeed;
			position += speed * timeCoef;

			locus.Add(this.drawPos);
			if (locus.Count > 2) locus.RemoveAt(0);

			// shootPositionのUpdate
			for (int i = 0; i < shootPosition.Count; i++) {
				shootPosition[i] += new Vector2((float)this.scalarSpeed, 0);
			}

			// 軌跡
			locus.Add(position);
			if (locus.Count > 2) locus.RemoveAt(0);
		}
        
	}
}
