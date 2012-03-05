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
	///  Stage3のボス：主人公のライバルのクラス。似たような技を持たせる。剣＋Stage1, 2のボスの技で攻撃
	///  剣での攻撃は、主人公と同じ攻撃パターンとコードを使う。（なるべくPlayerのコードを再利用した）
	///  本当は剣撃スキル的なクラスを作って、それをPlayerとRivalに持たせるべき。
	/// </summary>
	public class Rival : Boss
	{
		#region Member variable
		public readonly double first_jumpSpeed = -13.0;
		/// <summary>
		/// 2段ジャンプ
		/// </summary>
		public readonly double second_jumpSpeed = -10.0;
		/// <summary>
		/// 40;// 20frame≒1/3[scene]
		/// </summary>
		private readonly int normalFirstComboTime = 60;
		private readonly int normalSecondComboTime = 50;

		private Random rnd;
		private SoundEffect footStep, jumpSound;
		private Texture2D debugPoint;

		// Weapons
		public Sword sword { get; private set; }
		private Animation jumpAnimation { get; set; }
		public Turret syuriken { get; private set; }
		/// <summary>
		/// 自機狙いthunder用
		/// </summary>
		public Turret thunderTurret { get; private set; }
		private Turret cutterTurret;
		private Obstacle obstacleWindSmall, obstacleWindLarge;

		// Points
		/// <summary>
		/// 左側の定位置
		/// </summary>
		private Vector2 defPosOtherSide;
		/// <summary>
		/// Rivalの移動範囲の中心位置。自分やPlayerがボス戦エリアの左右どちらにいるのか
		/// 判断するのに使ったりする。
		/// </summary>
		private Vector2 CenterPos;
		/// <summary>
		/// 画面外との境界位置。これを超えた場合に復帰させるようにする。
		/// </summary>
		private Vector2 endPosRight, endPosLeft;

		// Attacking
		public bool isInCombo { get; private set; }
		public bool isInNormalCombo { get; private set; }
		public bool isAttacking1 { get; private set; }
		public bool isAttacking2 { get; private set; }
		public bool isAttacking3 { get; private set; }
		public bool isCuttingUp { get; private set; }
		public bool isCuttingUpV2 { get; private set; }
		public bool isCuttingDown { get; private set; }
		public bool isCuttingDownFromAir { get; private set; }
		public bool isCuttingDownFromAirV2 { get; private set; }
		public bool isCuttingAway { get; private set; }
		public bool isToChargingMotion { get; private set; }
		public bool isChargingPower { get; private set; }
		public bool isShootingBeam { get; private set; }
		public bool isTrackingEnemy { get; private set; }
		public bool isDashAttacking { get; private set; }
		public bool isThrusting { get; private set; }
		public bool isReversing { get; private set; }
		/// <summary>
		/// これは配列にすべきｗ
		/// </summary>
		private bool inCombo1, inCombo2, inCombo3
			, inCombo4, inCombo5, inCombo6, inCombo7;
		private bool circle, square, cross, triangle
			, Right, Down, Up, Shift, ShiftOnce;
		public bool Left { get; private set; }
		public bool hasJumped { get; private set; }
		public bool spawnEnemy { get; private set; }
		private int inputCounter;
		private int normalComboCount;
		private int jumpTime, interceptTime;
		public int jumpcomboTime { get; private set; }
		public int damagecomboTime { get; private set; }
		private int timeB;
		/// <summary>
		/// 動的に変更するための変数
		/// </summary>
		private int windType;
		private int comboTime;

		// Behaviour
		private bool inDamageMotion;
		private bool isRunnnig;
		/// <summary>
		/// Playerに近づいて攻撃→定位置までジャンプ のパターンで、Playerに近づききったかどうか
		/// </summary>
		private bool hasReachedPlayer;
		/// <summary>
		/// BackWardJump中かどうか
		/// </summary>
		private bool isBacking;
		/// <summary>
		/// 特定の位置まで辿り着いたか（攻撃メソッドで使用）
		/// </summary>
		private bool hasReached;
		/// <summary>
		/// ボス周辺でどちら側にいるか
		/// </summary>
		private bool inRightSide = true;
		/// <summary>
		/// 定位置に戻るハイジャンプでどちら側に行くか（左/右）
		/// </summary>
		private bool toRightSide;
		/// <summary>
		/// 攻撃対象(PLayer)がいる側が自分の座標の右側かどうか。毎フレーム更新される。
		/// </summary>
		private bool targetInRightSide;
		/// <summary>
		/// 風弾を撃つ方向。その攻撃の開始時にだけ更新される。
		/// </summary>
		private bool shootRightSide;
		/// <summary>
		/// メイン攻撃パターンメソッドがサブ攻撃パターンが終了したかどうか判断するためのフラグ
		/// </summary>
		private bool isShootingTornade, isShootingThunder;
		#endregion
		/// <summary>
		/// Easy用のメインパターン（これをループ）
		/// </summary>
		private int[] attackPattern0 = { 1, 2, 3 };//, 4 };
		/// <summary>
		/// Hard用のパターン
		/// </summary>
		private int[] attackPattern1 = { 7, 1, 9 };// hard
        private int attackBlinkCount;
		
		#region Update
		/// <summary>
		/// （仮想的に）キーの入力に従ってUpdateするメソッド
		/// </summary>
		private void UpdateInput()
		{
			#region Attacking
			#region △
			if (triangle) {// 1回だけじゃないとダメなのかもしれない...?umu
				if (normalComboCount == 0) {// 2回目だがnCC=0のまま
					isAttacking = true; sword.isBeingUsed = true;
					//if(isDashing)isDashAttacking = true;
					//else{
					isAttacking1 = true;
					isAttacking2 = false;
					//}//キャンセルした時用
					comboTime = 0;// 最初に押したときから計り始める
					isInCombo = true;
					normalComboCount = 1;
					inCombo1 = true;
				} else if (normalComboCount == 1) {// norimalComboCount=0になってなかった
					if (comboTime < normalFirstComboTime) {// 時間制限
						isAttacking1 = false;      // 自動終了する前に強制的に次の攻撃に移るのでfalseに調整
						isAttacking2 = true;
						isInCombo = true;
						isAttacking = true; sword.isBeingUsed = true;
						normalComboCount = 2;
						inCombo1 = false;
						inCombo2 = true;
						comboTime = 0;
					}
				} else if (normalComboCount == 2 && JoyStick.stickDirection != Direction.UP) {// すぐに1段目を出せるようにする。地上の3段目は○とか△＋○とかにしよう　上に書く
					isAttacking2 = false;// 自動終了する前に強制的に次の攻撃に移るのでfalseに調整
					isAttacking1 = true;
					isInCombo = true;
					isInNormalCombo = true;
					hasAttacked = true;
					isAttacking = true; sword.isBeingUsed = true;
					normalComboCount = 1;
					//sword.degreeCounter = 0;// 角度が初期化されないので必要 :12/8　改良したしそんなことはないよな...? ！！ここでもplayerのswordに干渉していた..
					inCombo2 = false;
					inCombo1 = true;              // launch comboとnormal comboで時間を分けてもいいかも
					comboTime = 0;
				} else if (normalComboCount == 4) {
					if (comboTime < normalSecondComboTime) {
						isAttacking = true; sword.isBeingUsed = true;
						isAttacking1 = true;
						isInCombo = true;
						normalComboCount = 5;
						inCombo4 = false;
						inCombo5 = true;
						comboTime = 0;
					}
				} else if (normalComboCount == 5) {
					if (comboTime < normalSecondComboTime) {
						isAttacking = true; sword.isBeingUsed = true;
						isAttacking1 = false;
						isAttacking2 = true;
						isInCombo = true;
						normalComboCount = 6;
						inCombo5 = false;
						inCombo6 = true;
						comboTime = 0;
					}
				}
			}
			#region ため攻撃
			// これって、このままにしたらPlayerと同時攻撃になるのかな？
			/*if(JoyStick.KEY(1) && normalComboCount==1) {
				if(time > 60) {
					hasAttacked = true;
					isAttacking = true; sword.isBeingUsed=true;
					isToChargingMotion = true;
					time = 0;

					normalComboCount = 2;
				}
			}
			if(JoyStick.IsOnKeyUp(1)) {
				if(isChargingPower && time > 40) {
					hasAttacked = true;
					isAttacking = true; sword.isBeingUsed=true;
					isChargingPower = false;
					isShootingBeam = true;
					time = 0;
					normalComboCount = 0;
				}
				else if(isChargingPower && time < 40) {
					hasAttacked = true;
					isAttacking = false;
					isChargingPower = false;
					isShootingBeam = false;
					time = 0;
					normalComboCount = 0;
				}
			}*/
			#endregion
			#endregion
			#region ○
			if (circle) {// JoyStick.IsOnKeyDown(3)) {
				if (normalComboCount == 0) { // 単発の強攻撃？
					hasAttacked = true;
					isAttacking = true;

					sword.isBeingUsed = true;
					isAttacking3 = true;
					//isAttacking1 = false;
					comboTime = 0;              // 最初に押したときから計り始める
					isInCombo = true;
					normalComboCount = 1;
					inCombo1 = true;
					sword.degreeCounter = 0;//
				} else if (normalComboCount == 2) {// 地上３段目
					if (comboTime < normalSecondComboTime) {
						isAttacking = true; sword.isBeingUsed = true;
						isAttacking2 = false;
						isAttacking3 = true;
						sword.degreeCounter = 0;//
						isInCombo = true;
						normalComboCount = 3;
						inCombo2 = false;
						inCombo3 = true;
						comboTime = 0;
					}
				} else if (normalComboCount == 4) {
					if (comboTime < normalSecondComboTime) {
						isAttacking = true; sword.isBeingUsed = true;
						isTrackingEnemy = false;
						isCuttingAway = true;
						isInCombo = true;
						normalComboCount = 5;
						inCombo4 = false;
						inCombo5 = true;
						comboTime = 0;
					}
				} else if (normalComboCount == 6) {
					if (comboTime < normalSecondComboTime) {
						isAttacking = true; sword.isBeingUsed = true;
						isAttacking2 = false;
						isCuttingAway = true;
						isInCombo = true;
						normalComboCount = 7;
						inCombo6 = false;
						inCombo7 = true;
						comboTime = 0;
					}
				}
			}
			#endregion
			comboTime++;// 死ぬときに使う方と被ってしまったorz
			#endregion
			#region Moving
			// 左右移動
			if (Right) {
				turnsRight = true;
				if (isDashing) speed.X = 8.0f;
				else speed.X = 6.0f;

				if (counter % 15 == 0 && !isJumping && !isDashing) {
					if (!game.isMuted) footStep.Play(SoundControl.volumeAll, 0f, 0f);
				} else if (counter % 10 == 0 && !isJumping && isDashing) {
					if (!game.isMuted) footStep.Play(SoundControl.volumeAll, 0f, 0f);
				}
			}
			if (Left) {
				turnsRight = false;
				if (isDashing) speed.X = -8.0f;
				else speed.X = -6.0f;

				if (counter % 15 == 0 && !isJumping && !isDashing) {
					if (!game.isMuted) footStep.Play(SoundControl.volumeAll, 0f, 0f);
				} else if (counter % 10 == 0 && !isJumping && isDashing) {
					if (!game.isMuted) footStep.Play(SoundControl.volumeAll, 0f, 0f);
				}
			}
			// 落下速度うｐ
			if (Down) speed.Y += 5;
			// ダッシュ
			if (ShiftOnce) {
				isDashing = true;
				isEffected = true;
				hasDashed = false;
				dashEffected = true;
			}
			if (JoyStick.IsOnKeyUp(4)) isDashing = false;
			if (Shift) hasDashed = true;
			#endregion
			#region Jumping
			if (cross) {
				speed.Y = -12f;
			}
			// ジャンプ : これに関してはif(cross) vy = -12fとかでいいような
			/*if (JoyStick.KEY(2) && !isInCombo) {
				jumpTime++;// 押下時間をチェック
				if (jumpTime > 12  && !isJumping) {
					Jump(12);
					counter = 0;
					hasJumped = true;
				}
			}
            
			if(cross) {// 一気にjumpCountが0～2まで
				if (jumpCount == 1 && isJumping && counter > 30) {
					Jump(12);//12は使われない
				}
				else if(game.inDebugMode && jumpCount >= 2&& isJumping && counter > 30) {
					Jump(12);
				}
			}
			if(!cross && jumpTime > 0&& !isInCombo && !isJumping) {
				if(!isJumping)
					Jump(jumpTime);
			}*/
			counter++;
			#endregion
		}
		private void EndCombo()
		{
			if ((inCombo1 && comboTime > normalFirstComboTime && !triangle) || (inCombo2 && comboTime > normalSecondComboTime && !isChargingPower)// 段別に時間制限を設定可
				|| (inCombo3 && comboTime > normalSecondComboTime) || (inCombo4 && comboTime > normalSecondComboTime) /*|| (inCombo4 && isOnSomething)*///開始時に地面に乗っていると0になってしまう
				|| (inCombo5 && comboTime > 10)// 最後の技を出した後すぐ終わらせたい場合はCombocomboTimeを短くすればおｋ
				|| (inCombo6 && comboTime > normalSecondComboTime) || (inCombo7 && comboTime > normalSecondComboTime)) {// 遅かったら勝手にコンボ終了
				isInCombo = false;
				if (inCombo1) inCombo1 = false;
				if (inCombo2) inCombo2 = false;
				if (inCombo3) inCombo3 = false;
				if (inCombo4) { inCombo4 = false; isTrackingEnemy = false; }
				if (inCombo5) inCombo5 = false;
				if (inCombo6) inCombo6 = false;
				if (inCombo7) inCombo7 = false;

				normalComboCount = 0;//　ここで初期化されてしまってた
				//isAttacking = false; // 攻撃全体を終了
				comboTime = 0;
			}
		}
		private void AttackProcess()
		{
			if (isAttacking) {
				if (isAttacking1) {// なぜかPlayerのswordだぞ...?
					sword.SlashVertically(turnsRight, 120, 8);
					if (sword.isEnd) {// isEndはsword内のメソッド内で使い始めに初期化
						isAttacking1 = false;
						hasAttacked = true;                          // ダメージ判定に関わる
						isAttacking = false;

						// attackNumでこの辺の初期化をするかどうか決めればよい...？
						if (attackNum == 1 && isEndingAttack) {//1) {
							EndProcess();
						}
					}
				} else if (isAttacking2) {
					sword.SlashHorizontally(turnsRight, 120, 8);
					if (sword.isEnd) {
						isAttacking2 = false;
						hasAttacked = true;
						isAttacking = false;
					}
				} else if (isAttacking3) {
					sword.SlashHardlly(turnsRight, 70, 8);
					if (sword.isEnd) {
						isAttacking3 = false;
						hasAttacked = true;
						isAttacking = false;

						// 3回だけ振って終了したが...
						/*if(attackNum==2 && isEndingAttack) {// まず確認してかr
							EndProcess();
						}*/
					}
				}

			}
		}
		private void EndProcess()
		{
			inputCounter = 0;
			isWaiting = true;
			comboTime = 0;
			attackCounter = 0;
			waitCounter = 0;
			normalComboCount = 0;//
			isStartingAttack = false;// attackCounter++にしてしまう
		}

		private void Jump(int jumpPower)
		{
			isJumping = true;

			if (jumpCount == 0) {
				if (jumpTime <= 5)
					speed.Y = -10;
				else if (jumpTime > 5 && jumpTime <= 12) speed.Y = -14;
				else speed.Y = -14;

				if (!game.isMuted) jumpSound.Play(SoundControl.volumeAll, 0f, 0f);
				jumpCount = 1;
			} else if (jumpCount == 1) {
				speed.Y = (float)second_jumpSpeed;
				position.Y += (float)speed.Y;
				jumpCount = 2;
				if (!game.isMuted) jumpSound.Play(SoundControl.volumeAll, 0f, 0f);
			} else if (game.inDebugMode && jumpCount >= 2) {
				speed.Y = (float)second_jumpSpeed;
				position.Y += (float)speed.Y;
				jumpCount++;
				if (!game.isMuted) jumpSound.Play(SoundControl.volumeAll, 0f, 0f);
			} else { }
			jumpTime = 0;

			hasJumped = false;
		}
		/// <summary>
		/// 前を向いたまま、後ろへ放物線を描きつつジャンプさせるメソッド。
		/// あらかじめ対象までの時間を設定してから計算するパターンで、毎フレーム呼び出す必要がある。
		/// </summary>
		/// <param name="destination">ジャンプ先の座標</param>
		/// <param name="speedX">X軸方向の移動速度</param>
		/// <see cref="http://hakuhin.jp/as/shot.html#SHOT_02_02"/>
		private void BackwardJump(Vector2 destination, float speedX)
		{
			float dx;
			float dt = .05f;							// ゆっくり動かすための補整値
			float t;									// 到達時間（フレーム）
			Vector2 distance = destination - position;	// x方向のターゲットとの距離

			if (position.X - destination.X > 0) {
				dx = -speedX;
			} else {
				dx = speedX;
			}
			t = distance.X / dx;
			speed.X = dx;
			speed.Y = (distance.Y - .5f * t * t * 7.2f * dt) / t;// 9.8
			//speed.Y = (distanceToPlayer.Y - .5f * t * t * (float)gravity * dt) / t;
		}
		/// <summary>
		/// 飛ぶはずだった方向と実際のジャンプ方向が食い違ってしまった場合に呼ばれるジャンプメソッド。
		/// </summary>
		/// <param name="destination">ジャンプ先の座標</param>
		/// <param name="speedX">X方向の速度</param>
		private void RestoreJump(Vector2 destination, float speedX)
		{
			if (isBacking) {
				if (toRightSide && speed.X < 0) BackwardJump(destination, speedX);
				else if (!toRightSide && speed.X > 0) BackwardJump(destination, speedX);
			}
		}
		/// <summary>
		/// ボス戦シーンの境界を越えてしまった場合にステージに戻るメソッド
		/// </summary>
		private void RestorePosition(Vector2 restorePos, float speedX)
		{
			if (position.X < defaultPosition.X - 640 - 200 || position.X > defaultPosition.X + 100 + 440)
				BackwardJump(restorePos, speedX);
		}

		/// <summary>
		/// 剣同士が当たった時に（それぞれ）呼ばれて反動を剣のuser両者に与える
		/// </summary>
		/// <param name="targetObject"></param>
		public void SwordReflection(Object targetObject)
		{
			if (position.X - targetObject.position.X > 0) speed.X = 5;
			else speed.X = -5;
			position.X += speed.X * timeCoef;
		}
		#endregion
		#region Attacking
		/// <summary>
		/// Attack(int)-UpdateAttacking()-InputControl()-InputControlSubMethods ic

		/// Playerの攻撃処理のコードを再利用したいので、PlayerのKeyDown(i)に相当するフラグをそれぞれ作り、管理メソッドで適当に
		/// 押させて攻撃させる感じで.
		/// </summary>
		protected override void Attack(int waitTime)
		{
			if (isWaiting) {
				if (waitCounter > waitTime) {
					speed = Vector2.Zero;
					isStartingAttack = true;
					isWaiting = isEndingAttack = isAttacking = false;
					waitCounter = inputCounter = attackCounter = counter= 0;

					attackList.Add(attackPatternNumList[attackPatternNum][attackNum]);
					attackNum++;
					if (attackNum == attackPatternNumList[attackPatternNum].Length) attackNum = 0;
					//if (attackNum > 4) attackNum = 2;
				}
				waitCounter++;
			} else {
				UpdateAttacking();
				//if (isStartingAttack) attackCounter++; // 各メソッド内で？
				//if (isEndingAttack) EndProcess();		 // 要らない気がしてきた。復活させるかもしれないがどこでisWaiting == trueになるのかなどデバッグしづらいので最後にまわす
				attackCounter++;
			}

			InterceptA(2);
			UpdateInput();
			// 剣撃処理
			AttackProcess();
			EndCombo();
			//EndSkill();
		}
		/// <summary>
		/// PlayerのUpdateInputの移植：PlayerLikeな攻撃をするときはここでUpdateさせる
		/// </summary>
		protected override void UpdateAttacking()
		{
			base.UpdateAttacking();

			GeneralMoving();
			//InputControl();
			inputCounter++;
			//if (isShooting) AttackeWithWind(0);// 消す予定
		}

		#region Normal Attack
		/// <summary>
		/// 端に来すぎたら反対側に飛ぶなど、戦闘全般にわたる動きの記述
		/// </summary>
		protected void GeneralMoving()
		{
			// 自身の位置
			if (toRightSide && hasReachedPlayer && speed.X < 0 || !toRightSide && hasReachedPlayer && speed.X > 0)
				RestoreJump(toRightSide ? defaultPosition : defPosOtherSide, 5);
			// 画面外に出ないように
			if (stage.inBossBattle && position.X < stage.bossScreenEdgeLeft - Player.screenPosition.X) {
				position.X = stage.bossScreenEdgeLeft - Player.screenPosition.X;								// 位置を前に変更
				speed.X = stage.scrollSpeed;											// 押す
			} else if (stage.inBossBattle && position.X > stage.bossScreenEdgeRight) {
				this.position.X = stage.bossScreenEdgeRight;
			}
			//RestorePosition(CenterPos, 5);

			if ((position.X > defaultPosition.X - 250 && position.X < defaultPosition.X + 100) || position.X < defPosOtherSide.X - 100) {
				inRightSide = true;
			} else {
				inRightSide = false;
			}
			// toRightSide == true && speed = (-6, 0.49)

			if (position.X > defaultPosition.X && toRightSide) { }
			// 飛ぶ方向の決定
			if (!isBacking) {
				if (position.X - (defaultPosition.X - 240) < 0) {
					toRightSide = true;
				} else if (position.X - (defaultPosition.X - 240) >= 0) {
					toRightSide = false;
				}
			}

			// 攻撃する方向：Playerのいるsideに。
			if (position.X - stage.player.position.X > 0) {
				turnsRight = false;
				targetInRightSide = false;
				cutterTurret.bulletSpeed = new Vector2(-10, 0);
				cutterTurret.bullets[0].degree = 0;
			} else {
				turnsRight = true;
				targetInRightSide = true;
				cutterTurret.bulletSpeed = new Vector2(10, 0);
				cutterTurret.bullets[0].degree = 180;
			}
		}
		/// <summary>
		/// Playerへ向かって跳ねるだけのパターン（テスト用）
		/// </summary>
		protected void InputControl0()
		{
			double rad;
			float speed = 10;

			rad = Math.Atan2(position.Y - stage.player.position.Y, position.X - stage.player.position.X);

			this.speed.X = -(float)Math.Cos(rad) * speed;
			this.speed.Y = -(float)Math.Sin(rad) * speed;

			if (this.speed.X < 0) turnsRight = false;
			else turnsRight = true;
		}
		/// <summary>
		/// 移動せずに3回、1段目を振るだけ
		/// </summary>
		protected void InputControlA()
		{
			if (inputCounter == 0 || inputCounter == 40 || inputCounter == 80) {
				triangle = true;
				if (inputCounter == 80) {
					isEndingAttack = true;

					isEnds[0] = true;
				}
			} else {
				triangle = false;
			}
		}
		/// <summary>
		/// 画面左側へ走る→攻撃(2→3段コンボ)
		/// →ジャンプして後退→default位置へ戻る、を実行するパターン。
		/// </summary>
		protected void InputControlB()// 1
		{
			int timeWaiting = 300;
			float permissibleRange = 32;	// ジャンプし始める瞬間などにPlayerの剣撃を食らうなどして位置がずれた場合に備えるための補正値

			if (inputCounter == 0 || inputCounter == 30 || inputCounter == 80) {
				//if (inputCounter == 0) InputControl0();
				triangle = true;
			} else if (inputCounter == 40) {
				circle = true;
				//isEndingAttack = true;
			} else {
				triangle = false;
				circle = false;
				//Left = true;  // ずっと左に動き続ける
			}



			if (inputCounter == 0) {
				isRunnnig = true;
				timeB = 0;
			}
			//if (!hasReachedPlayer && !isBacking) Left = true;				// これだとジャンプする瞬間trueになってしまうようだ
			if (isRunnnig) {
				if (!targetInRightSide) Left = true;
				else Right = true;
			} else {
				if (!targetInRightSide) Left = false;
				else Right = false;
			}
			//else if (hasReachedPlayer) Left = false;
			/*else if (!isBacking) {
				Left = false;
				//Right = true;
				//cross = true;
			}
			else if (timeB > 60) {
				//Right = false;9
				//cross = false;
			}*/
			
			if (!isBacking && (Math.Abs(stage.player.position.X - this.position.X) < 72 || timeB > 240)) {
				hasReachedPlayer = true;
				isRunnnig = false;
				Left = false;													//　重要
				Right = false;
				timeB = 0;
			}
			if (!isBacking && hasReachedPlayer) {//&& timeB > 30) {				// 3/2 timeB > 30を外したらゲッダン状態に
				isBacking = true;
				isJumping = true;												// 一応

				if (position.X - CenterPos.X < 0) {			// バグりまくるのでGM()ではなく直前に更新してみる
					toRightSide = true;
				} else if (position.X - CenterPos.X >= 0) {
					toRightSide = false;
				}

				if (toRightSide) {
					BackwardJump(defaultPosition, 5);
				} else {
					BackwardJump(defPosOtherSide, 5);							// 3/20 落ちるでござる
				}

				timeB = 0;
				if (!game.isMuted) jumpSound.Play(SoundControl.volumeAll, 0f, 0f);
			}

			// 定位置の両端側に居る時にPlayerの攻撃を受けるとそのまま滑り続けてしまうバグあり（ここが原因）
			// speed.Yが早すぎて(34台)すり抜ける時も...
			// 条件追加で改善後も、toRightSide == falseなのにも関わらず右に滑るバグが

			// 最終的には両端に保険に復帰ポイントを設定するにしても、toRightSideとspeedが一致しないバグは直したい
			if (isBacking && ((Vector2.Distance(position, toRightSide ? defaultPosition : defPosOtherSide) < speed.Length()
				|| toRightSide ? position.X > defaultPosition.X + permissibleRange : position.X < defPosOtherSide.X - permissibleRange) || timeB > timeWaiting)) {// < 32

				speed = Vector2.Zero;
				hasReachedPlayer = false;
				isBacking = false;
				timeB = 0;
				isEndingAttack = true;
				isWaiting = true;

				isEnds[1] = true;
			}

			timeB++;
		}
		/// <summary>
		/// 雷撃用
		/// </summary>
		protected void InputControlC()// 2
		{
			bool hasShot = hasReached && isShootingThunder;

			if (inputCounter == 0) {// このせいで、同時に複数の攻撃メソッド(inputCounterを使う)を走らせるとジャンプしないでござる？
				speed.Y = -16;
			} else if (inputCounter <= 5) {
				//if (inputCounter == 0) InputControl0();
				triangle = true;
				//cross = true;
				speed.Y = -18;
			} else {
				triangle = false;
				cross = false;
			}

			if (!hasReached && (Math.Abs(attackPosition.Y - position.Y/*Vector2.Distance(position, defaultPosition + new Vector2(0, -12)*//*attackPosition*/) < 5
				|| inputCounter > 60/**/)) {// やっぱりタイムアウトは必要
				hasReached = true;
				counter = 0;
			} 
			if (hasReached && !isShootingThunder && counter > 60/**/) {
				gravity = 0;
				speed = Vector2.Zero;
				//shootRightSide = targetInRightSide;

				isShootingThunder = true;
				isStartingAttack = true;
				attackList.Add(6);
			}
			
			// 終了処理:ATWTの終了を待って終わるようにしたい
			if (thunderTurret.isEnd || attackCounter > 240) {
				thunderTurret.isEnd = false;
				hasReached = false;
				gravity = defGravity;
				isEndingAttack = true;
				isWaiting = true;
				isShootingThunder = false;
				//thunderTurret.isBeingUsed = false;
				counter = 0;

				isEnds[2] = true;
			}

			counter++;
		}
		/// <summary>
		/// カッターと竜巻攻撃
		/// </summary>
		protected void InputControlD(int type)// 3
		{
			//if (inputCounter == 0) speed.Y = -18;
			if (inputCounter == 5) {
				//if (inputCounter == 0) InputControl0();
				triangle = true;
				//cross = true;
				//speed.Y = -18;
			} else if (inputCounter == 10) {
				//windType = 1;


				triangle = false;
				cross = false;
			}/* else {
				triangle = false;
				cross = false;
			}*/



			if (isStartingAttack) {
				obstacleWindSmall.isBeingUsed = true;
				shootRightSide = targetInRightSide;
				attackList.Add(5);
				isShootingThunder = true;

				isStartingAttack = false;
			}

			// 撃ってすぐ動かしたい
			/*isShooting = true;
			isEndingAttack = true;
			isWaiting = true;
			waitCounter = 0;*/
			//attackList.Add(5);
			//isEnds[3] = true;

			//if(Math.Abs(attackPosition.Y - position.Y) < 12) hasReached = true;
			// 終了処理
			if (obstacleWindSmall.isEnd) {//isWaiting || attackCounter > 240) {
				obstacleWindSmall.isEnd = false;
				isWaiting = true;
				isShootingThunder = false;

				isEnds[3] = true;
			}
		}

		/// <summary>
		/// 雷攻撃などの前のジャンプを行ったかどうか
		/// </summary>
        bool hasAttackJumped;
		/// <summary>
		/// 攻撃予告状態。大竜巻の前などに使用
		/// </summary>
		bool inAttackNotice;
		// Hard
		/// <summary>
		/// InputControlCのsyuriken版
		/// </summary>
		protected void InputControlE()// 7
		{
			bool hasShot = hasReached && isShootingThunder;

            if (!hasAttackJumped && isOnSomething) {
                speed.Y = -16;
                isJumping = true;
                hasAttackJumped = true;
            }
            // 着地を待たせよう...
			if (inputCounter == 0) {
                
			} else if (inputCounter == 2 || inputCounter == 7) {
				//if (inputCounter == 0) InputControl0();
				triangle = true;
				speed.Y = -18;
			} else {
				triangle = false;
				cross = false;
			}

			if (!hasReached && (Math.Abs(attackPosition.Y - position.Y/*Vector2.Distance(position, defaultPosition + new Vector2(0, -12)*//*attackPosition*/) < 5
				/*|| inputCounter > 60*/)) {
				hasReached = true;
				counter = 0;
			} else if (hasReached && !isShootingThunder && counter > 60) {
				// カッターの時は止めない方がｶｯｺｲｲかつ効率的
				//gravity = 0;
				//speed = Vector2.Zero;

				isShootingThunder = true;
				attackList.Add(8);
			}
			counter++;

			// 終了処理:syurikenの終了を待って終わる
			if (syuriken.isEnd || attackCounter > 240) {//isWaiting ) {
				hasReached = false;
                isShootingThunder = false;
                hasAttackJumped = false;
				//gravity = defGravity;
				isEndingAttack = true;
                syuriken.isEnd = false;
				isWaiting = true;
				//thunderTurret.isBeingUsed = false;
				counter = 0;
				isEnds[7] = true;
			}
		}
		/// <summary>
		/// 大型竜巻攻撃。若干予告を入れることに。
		/// </summary>
		protected void InputControlF()// 9
		{
			if (inputCounter == 0 ) {
				triangle = true;
				inAttackNotice = true;
			} else {
				triangle = false;
			}
			if (!isShootingThunder && attackCounter > 60) inAttackNotice = false;
			if (!isShootingThunder && attackCounter > 90) {
				
				attackMethodsArgs[5] = new object[] {1};// なんというHCでしょう！
				attackList.Add(5);
				shootRightSide = targetInRightSide;
				obstacleWindLarge.isBeingUsed = true;// これもAWWに含めるべき
				isShootingThunder = true;
			}

			if (obstacleWindLarge.isEnd) {
				isWaiting = true;
				isShootingThunder = false;
				isEnds[9] = true;
				obstacleWindLarge.isEnd = false;
			}

		}
		/// <summary>
		/// 近くまで来たら指定した確率で攻撃してくる
		/// </summary>
		protected void InterceptA(float chance)
		{
            if (isWaiting && Vector2.Distance(position, stage.player.position) < 80) {
                if (interceptTime % 100 == 0) triangle = true;//60
                else triangle = false;
            }

			interceptTime++;
		}
		#endregion
		#region Special Attack
		/// <summary>
		/// 地面隆起攻撃
		/// </summary>
		private void GroundUp()
		{
		}
		private void AttackWithThunder()
		{
			if (isStartingAttack) {
				thunderTurret.position = this.position; // isActiveがtrueになっていなかった
				thunderTurret.isBeingUsed = true;
				thunderTurret.isEnd = false;
				isStartingAttack = false;
				thunderTurret.Inicialize();
				thunderTurret.isActive = true;
			}

			if (thunderTurret.isEnd /*|| attackCounter > 480*/) {
				thunderTurret.isBeingUsed = false;
				//thunderTurret.isEnd = false;
				//isShootingThunder = false;
				//isAttacking = false;
				//isWaiting = true;
				//attackCounter = 0;

				isEnds[6] = true;
			}
		}
		private void AttackeWithWind(int type)// 5
		{
			switch (type) {
				case 0:
					if (obstacleWindSmall.isBeingUsed) {
						if (shootRightSide) {
							obstacleWindSmall.MovePattern2(obstacleWindSmall.trapSet, position, Vector2.Zero, position + new Vector2(240, 0), 1, 10);
						} else {
							obstacleWindSmall.MovePattern2(obstacleWindSmall.trapSet, position, Vector2.Zero, position + new Vector2(-240, 0), 1, 10);
						}
						/*if (!shootRightSide)//-100
							obstacleWindSmall.MovePattern2(obstacleWindSmall.trapSet, defaultPosition + new Vector2(-100, 0),
								Vector2.Zero, defaultPosition + new Vector2(-240, 0), 1, 5);// -500, 3
						else
							obstacleWindSmall.MovePattern2(obstacleWindSmall.trapSet, defPosOtherSide + new Vector2(100, 0),
								Vector2.Zero, defaultPosition + new Vector2(240, 0), 1, 5);// -500, 3*/
					}
					break;
				case 1:
					if (obstacleWindLarge.isBeingUsed) {
						/*if (!shootRightSide)
							obstacleWindSmall.MovePattern2(obstacleWindSmall.trapSet, defaultPosition + new Vector2(-100, -100),//new Vector2(defaultPosition.X - 32, stage.surfaceHeightAtBoss -256),
								defaultPosition + new Vector2(-500, 100), defaultPosition + new Vector2(-600, -100), 4, 5);
						else
							obstacleWindSmall.MovePattern2(obstacleWindSmall.trapSet, defPosOtherSide + new Vector2(100, -100),//new Vector2(defaultPosition.X - 32, stage.surfaceHeightAtBoss -256),
							   Vector2.Zero, defaultPosition + new Vector2(500, -100), 4, 5);*/
						if (shootRightSide) {
							obstacleWindLarge.MovePattern2(obstacleWindLarge.trapSet, new Vector2(position.X, 64), Vector2.Zero, new Vector2(position.X + 300, 64), 1, 10);
						} else {
							obstacleWindLarge.MovePattern2(obstacleWindLarge.trapSet, new Vector2(position.X, 64), Vector2.Zero, new Vector2(position.X - 300, 64), 1, 10);// position, position + (-300, 0)
						}
					}
					break;
			}

			if (obstacleWindSmall.isEnd) {
				//obstacleWindSmall.isEnd = false;
				obstacleWindSmall.isBeingUsed = false;
				obstacleWindSmall.attackCounter = 0;
				obsCounter = 0;
				isEndingAttack = true;
				gravity = defGravity;
				isShootingTornade = false;
				//isWaiting = true;

				isEnds[5] = true;
			}
			if (obstacleWindLarge.isEnd) {
				//obstacleWindLarge.isEnd = false;
				obstacleWindLarge.isBeingUsed = false;
				obstacleWindLarge.attackCounter = 0;
				obsCounter = 0;
				isEndingAttack = true;
				gravity = defGravity;
				isShootingTornade = false;
				isWaiting = true;

				isEnds[5] = true;
			}
			//EndSkill();
			/*isWaiting = true;
			waitCounter = 0;
			attackCounter++;*/
		}
		private void EndSkill()
		{
			if (obstacleWindSmall.isEnd) {
				obstacleWindSmall.isEnd = false;
				obstacleWindSmall.isBeingUsed = false;
				obstacleWindSmall.attackCounter = 0;
				obsCounter = 0;

				/*attackCounter = 0;
				isWaiting = true;
				waitCounter = 0;*/
				//hasReached = false;

				isEndingAttack = true;
				gravity = defGravity;
				isShootingTornade = false;

				isEnds[5] = true;
			}
		}
		private void AttackWithCutter()
		{
		}
		private void AttackWithShuriken(int shootTime, bool goWait)// 8
		{
			if (isStartingAttack) {
				syuriken.isBeingUsed = true;
				syuriken.isEnd = false;
				syuriken.position = this.position + syuriken.defaultPosition;
				syuriken.Inicialize();
				syuriken.shootNumTotal = 0;
				syuriken.isActive = true;

				isStartingAttack = false;
			}

			if (syuriken.shootNumTotal == shootTime/*|| attackCounter > resWaitTime*/) {/*syuriken.isEnd*/
				if (goWait) isWaiting = true;
				attackCounter = 0;
				syuriken.isBeingUsed = false;
				syuriken.isEnd = true;
				isShootingThunder = false;

				isEnds[8] = true;
			}
		}
		#endregion
		#endregion
		private void DrawDebugStatus(SpriteBatch spritebatch)
		{
			Vector2 origin = new Vector2(2.5f);

			spriteBatch.Draw(debugPoint, defaultPosition - stage.camera.position, null, Color.White, 0, origin, 1, SpriteEffects.None, 0);
			spriteBatch.Draw(debugPoint, defPosOtherSide - stage.camera.position, null, Color.White, 0, origin, 1, SpriteEffects.None, 0);
			/*game.GraphicsDevice.RenderState.PointSize = 5;
			game.GraphicsDevice.DrawPrimitives(PrimitiveType.PointList, 0, 1);*/
		}

		// コンストラクタ
		public Rival(Stage stage, float x, float y, int width, int height, int HP, float vx, int bulletType, int shootType)
			: base(stage, x, y, width, height, HP, bulletType, shootType)
		{
			bind = new Object(stage, x, y, 16, 16);
			sword = new Sword(stage, 200, 100, 64, 8, this);
			thunderTurret = new Turret(stage, this, new Vector2(5), 64, 48, 2, 0, 3, false, true, 3, 3);
			cutterTurret = new Turret(stage, this, this.position, 32, 32, 0, 0, 1, false, true, 0, 4);
			cutterTurret.bulletSpeed = new Vector2(-20, 0);
			syuriken = new Turret(stage, this, new Vector2(5), 32, 32, 0, 0, 1, false, true, 0, 2, 10, 10, 5, new Vector2(-10, 0), false, false, "katana");
			//syuriken = new Turret(stage, this, this.position, 32, 32, 0, 1, 1, false, true, false, 3, 0, 2, 10, 120, 30, new Vector2(-10, 0), false, false);
			//syuriken = new Turret(stage, this, this.position, 32, 32, 0, 0, 1, false, true, false, 3, 0, 4);
			//syuriken.bulletSpeed = new Vector2(-10, 0);
			turrets.Add(thunderTurret);
			turrets.Add(cutterTurret);
			turrets.Add(syuriken);
			weapons.Add(thunderTurret);

			obstacleWindSmall = new Obstacle(stage, this, x, y, 32, 32, 2, 3);	// =MapObjectsを生成 // 2
			obstacleWindLarge = new Obstacle(stage, this, x, y, 32, 32, 2, 4);
			weapons.Add(obstacleWindSmall);
			weapons.Add(obstacleWindLarge);
			weapons.Add(sword);
			foreach (Turret tur in turrets) weapons.Add(tur);
			foreach (Weapon weapon in weapons) stage.weapons.Add(weapon);		// stageのweaponsに追加

			defaultPosition = new Vector2(x, y);// 15000, 360
			defPosOtherSide = new Vector2(x - 480, y);
			attackPosition = new Vector2(x, y - 200);							// 雷撃とか用
			CenterPos = new Vector2(defaultPosition.X - 240, 240);
			turnsRight = true;
			animation = new Animation(48, 48);
			jumpAnimation = new Animation(48, 48);
			rnd = new Random();
			maxSpeed = 28;
			//Load();
		}

		// overrideメソッド
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\Rival");
			footStep = content.Load<SoundEffect>("Audio\\SE\\foot");
			jumpSound = content.Load<SoundEffect>("Audio\\SE\\jump");
			debugPoint = content.Load<Texture2D>("Debug\\debugPoint");
			
			for (int i = 0; i < 12; i++) isEnds.Add(false);
			attackPatternNumList.Add(attackPattern1);//1
			attackPatternNumList.Add(attackPattern0);//0
			attackList = new List<int>();
			if (game.isHighLvl) {
				attackPatternNum = 1;
			} else {
				attackPatternNum = 0;
				//attackList.Add(attackPatternNumList[attackPatternNum][0]);
			}
			//attackList.Add(0);

			// メソッドを登録
			//attackMethods.Add(attackMethodType0 = InputControl0);
			attackMethods.Add(attackMethodType0 = InputControlA);		// 0
			attackMethods.Add(attackMethodType0 = InputControlB);
			attackMethods.Add(attackMethodType0 = InputControlC);
			attackMethods.Add(attackMethodType2 = InputControlD);
			attackMethods.Add(attackMethodType0  = InputControlE);		// 4
			attackMethods.Add(attackMethodType2 = AttackeWithWind);		// 5
			attackMethods.Add(attackMethodType0 = AttackWithThunder);	// 6
			attackMethods.Add(attackMethodType0 = InputControlE);		// 7
			attackMethods.Add(attackMethodType6 = AttackWithShuriken);	// 8
			attackMethods.Add(attackMethodType0 = InputControlF);

			// 引数を登録:リストに追加したデリゲート分必要
			//attackMethodsArgs.Add(null);
			attackMethodsArgs.Add(null);
			attackMethodsArgs.Add(null);
			attackMethodsArgs.Add(null);					// 2
			attackMethodsArgs.Add(new object[] {0});
			attackMethodsArgs.Add(null);
			attackMethodsArgs.Add(new object[] {windType});	// 5
			attackMethodsArgs.Add(null);					// 6
			attackMethodsArgs.Add(null);					// 7
			attackMethodsArgs.Add(new object[] {3, false});	// 8
			attackMethodsArgs.Add(null);
		}
		public override void Update()
		{
			if (isAlive && stage.inBossBattle) {
				base.Update();
                Attack(30);// 動いてからtoRightSideを判断する？
			}
		}
		public override void UpdateAnimation()
		{
			bool isMoving = false;

			if (isOnSomething && (Right || Left)) {
				animation.Update(3, 0, 60, 60, 4, 1);
				isMoving = true;
			}
			if (isJumping || isBacking) {
				animation.Update(2, 1, 60, 60, 6, 0);
				isMoving = true;
			}
			if (!isMoving) animation.Update(0, 1, 60, 60, 6, 0);
		}
		/// <summary>
		/// BackWardJump中は摩擦等の補正を行わないようにする
		/// </summary>
		protected override void UpdateNumbers()
		{
			if (!isBacking) {//&& !isOnSomething && !isAttacking) {
				speed.Y += (float)gravity;
				if (speed.X > 0) {
					speed.X += (float)-(.40 * friction);
					if (speed.X < 0) speed.X = 0;
				} else if (speed.X < 0) {
					speed.X += (float)(.40 * friction);
					if (speed.X > 0) speed.X = 0;
				}
			} else {
				speed.Y += 9.8f * .05f;
				/*if (speed.X > 0) {
					speed.X += -(.20f * friction);
					if(speed.X < 0) speed.X = 0;
				}
				else if (speed.X < 0) {
					speed.X += (.20f * friction);
					if (speed.X > 0) speed.X = 0;
				}*/
			}

			if (/*!isBacking && */System.Math.Abs(speed.Y) > maxSpeed) {
				if (speed.Y > 0) speed.Y = maxSpeed;
				else speed.Y = -maxSpeed;
			}
			/*else {
				speed.Y += 9.8f * .05f;
				speed.X = 5;
			}*/

			// 位置に加算
			position += speed * timeCoef;

			// 端
			//if (!isBacking && position.Y < 0) position.Y = 0;

			// 軌跡
			locus.Add(position);
			if (locus.Count > 2) locus.RemoveAt(0);
		}
		/// <summary>
		/// ちょっと仰け反る仕様
		/// </summary>
		public override void MotionUpdate()
		{
			float distance = position.X - stage.player.position.X;

			if (isDamaged && isAlive) {
				if (stage.player.isCuttingUp) {
					BlownAwayMotionUp(3, 65);
					//isBlownAway = true;
				} else if (stage.player.isCuttingAway) BlownAwayMotionRight(1, 60);
				else if (stage.player.isCuttingDown) BlownAwayMotionDown(1.5f, 60);// 3,60だとブロックをすり抜ける...
				else if (stage.player.isAttacking3) {
					if (distance > 0) speed.X += 5 * timeCoef;
					else speed.X += -5 * timeCoef;

					speed.Y -= 5;
					HP--;
				} else if (stage.player.isAirial) {
					if (distance > 0) speed.X += 5 * timeCoef;
					else speed.X += -5 * timeCoef;

					speed.Y += 5 * timeCoef;
				} else if (stage.player.isThrusting && time % 3 == 0) {
					if (distance > 0) speed.X += 1 * timeCoef;
					else speed.X += -1 * timeCoef;
					speed.Y -= 1 * timeCoef;
				} else if (!stage.player.isThrusting) {
					if (distance > 0) speed.X += 1.5f * timeCoef;
					else speed.X += -1.5f * timeCoef;
				}

				HP--;
				if (!game.isMuted) hitSound.Play(SoundControl.volumeAll, 0f, 0f);
				if (stage.player.normalComboCount >= 3) {
					comboTime = 60;
				} else {
					comboTime = 30;
				}
				totalHits += 1;
				time = 0;
				delayTime = 0;
                blinkCount = 0;
                e = 0;
				isEffected = true;
				damageEffected = true;
				if (time < comboTime) comboCount++;
				time++;

				//int adj = 0;
				//adj = stage.maxComboCount;
				game.stageScores[game.stageNum-1] += stage.inComboObjects.Count + (1 + stage.gameStatus.maxComboCount * .01f);//10 * stage.inComboObjects.Count * stage.maxComboCount;
			}
		}
		protected override void MotionDelay()
		{
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			//base.Draw(spriteBatch);
			if (IsActive()) {
				if (game.inDebugMode) DrawDebugStatus(spriteBatch);
				if (inAttackNotice) {
					if (attackBlinkCount % 5 == 0) e += .05f;
					dColor = (float)Math.Sin(e * 8) / 2.0f + 0.5f;

					spriteBatch.Draw(texture, drawPos, animation.rect, Color.Blue * dColor);
					attackBlinkCount++;
				} else {
					//base.Draw(spriteBatch);
                    DrawComboCount(spriteBatch);
                    DrawDamageBlinkOnce(spriteBatch, Color.Red);
				}
			}
		}
	}
}
