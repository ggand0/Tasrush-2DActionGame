using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;  
using Microsoft.Xna.Framework.Audio;


namespace _2DActionGame
{
	/// <summary>
	/// プレイヤークラス。入力とアニメーションの更新、コンボ判定などがメイン。
	/// </summary>
	public class Player : Character
	{
		#region Member variable
		// Const
		public new readonly float defActiveDistance = 99999999;// ∞もしくはCamera.ScrollUpdate()内で例外的に処理させるもしくはボス戦中端に壁を設けるなりしていけないようにする
		/// <summary>
		/// スクリーン座標：画面上にもってきたい位置：画面スクロール中Playerはずっとこの位置
		/// </summary>
		public readonly static Vector2 screenPosition;// = new Vector2(200, 0);
		/// <summary>
		/// 20[frame] ~ 0.3[second]
		/// </summary>
		public static readonly int normaFirstComboTime = 40;
		public static readonly int normaSecondComboTime = 40;
		public static readonly int thrustingTime = 40;
		public static readonly Vector2 defReflectSpeed = new Vector2(4, -6);
        private static readonly int thrustPowerConsumption = 800;
		private static readonly int miniJumpWaitTime = 12;
		private static readonly int airReflectLimit = 15;//30

		private readonly float timeCoefPlayer = 0.5f;
		public readonly float firstJumpSpeed = -13.0f;
		public readonly float secondJumpSpeed = -10.0f;
		public readonly int MAXTAS = 1800;
		public readonly int initialTAS = 900;
		
		// Basis
		private SoundEffect footstep, jumpSound, landingSound, tasSound, damageSound;
		public Sword sword { get; private set; }
		private bool isInJumpAnim;//, inDmgMotion;
		private bool inCombo1, inCombo2, inCombo3, inCombo4, inCombo5, inCombo6, inCombo7;// 配列にしたい　いや、リストか...
		private bool[] inCombos = new bool[8];
		private int animCounter, animCounter2;
		public int TASpower { get; set; }					// protected化：reverseの修正

		// Input
		private int workKeyNum, workButtonNum, workStickNum;
		/// <summary>
		/// ゲームパッドのスティックを倒す方向（もしくは）
		/// </summary>
		private Direction[] stickNum = new Direction[5] { Direction.RIGHT, Direction.LEFT, Direction.UP, Direction.DOWN, Direction.NEWTRAL };
		/// <summary>
		/// キーコードの配列
		/// </summary>
		private Keys[] keyNum = new Keys[6] { Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.LeftShift, Keys.C };
		/// <summary>
		/// ゲームパッドのボタン（もしくは対応するキー）配列
		/// </summary>
		private int[] buttonNum = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };

		// Attacking
		public bool isInCombo { get; private set; }
		public bool isInNormalCombo { get; private set; }
		/// <summary>
		/// １段目弱
		/// </summary>
		public bool isAttacking1 { get; private set; }
		/// <summary>
		/// 2段目弱
		/// </summary>
		public bool isAttacking2 { get; private set; }
		/// <summary>
		/// 強
		/// </summary>
		public bool isAttacking3 { get; private set; }
		/// <summary>
		/// 斬り上げ
		/// </summary>
		public bool isCuttingUp { get; private set; }
		public bool isCuttingUpV2 { get; private set; }
		/// <summary>
		/// 斬り下げ
		/// </summary>
		public bool isCuttingDown { get; set; }				// private化：Swordの修正
		/// <summary>
		/// 空中斬り下げ
		/// </summary>
		public bool isCuttingDownFromAir { get; set; }
		/// <summary>
		/// 吹き飛ばし
		/// </summary>
		public bool isCuttingAway { get; private set; }
		/// <summary>
		/// 溜め攻撃前のモーション
		/// </summary>
		public bool isToChargingMotion { get; set; }
		/// <summary>
		/// まさに今溜めている
		/// </summary>
		public bool isChargingPower { get; set; }
		/// <summary>
		/// 溜め攻撃
		/// </summary>
		public bool isShootingBeam { get; set; }
		/// <summary>
		/// 追跡ジャンプ
		/// </summary>
		public bool isTrackingEnemy { get; set; }
		public bool isDashAttacking { get; set; }
		/// <summary>
		/// 百列斬り
		/// </summary>
		public bool isThrusting { get; set; }
		public bool isReversing { get; private set; }
		/// <summary>
		/// 3段目で切り上げる時の挙動のモード
		/// </summary>
		public bool syouryuuMode { get; set; }
		/// <summary>
		/// 空中下
		/// </summary>
		public bool isAirial { get; private set; }
		/// <summary>
		/// コンボカウンタ：現在のコンボ数
		/// </summary>
		public int normalComboCount { get; private set; }

		// Behavior
		/// <summary>
		/// 何らかの静的な地形の左側にいるかどうか（スクロールとの挟まり判定に使う）
		/// </summary>
		public bool isHitLeftSide { get; set; }
		/// <summary>
		/// デバッグ用のフラグ
		/// </summary>
		public bool spawnEnemy { get; private set; }
		/// <summary>
		/// ジャンプの挙動を定義しているメソッドで使うカウンタ
		/// </summary>
		public int jumpTime { get; private set; }
		public int damageTime { get; private set; }
		private int thrustCount = thrustingTime;

		// 空中斬りの反動関係
		public bool inAirReflection { get; private set; }
		private Vector2 reflectSpeed;
		private bool disableMovingInput;
		private int airReflectCount, airReflectTime;

		public bool scrollPush { get; set; }
		#endregion

		// コンストラクタ
		public Player()
			: this(null, 0, 0, 32, 32)
		{
		}
		public Player(Stage stage, float x, float y, int width, int height)
			: this(stage, x, y, width, height, 10)
		{
		}
		public Player(Stage stage, float x, float y, int width, int height, int HP)
			: base(stage, x, y, width, height)
		{
			LoadXML("Player", "Xml\\Objects_Base.xml");
			this.TASpower = initialTAS;
			this.HP = HP;
			this.activeDistance = defActiveDistance;
			sword = new Sword(stage, 200, 100, 64, 8, this);
			turnsRight = true;
			animation = new Animation(width, height);
			syouryuuMode = true;

			Load();
		}
		protected override void Load()
		{
			footstep = content.Load<SoundEffect>("Audio\\SE\\foot");
			jumpSound = content.Load<SoundEffect>("Audio\\SE\\jump");
			landingSound = content.Load<SoundEffect>("Audio\\SE\\zimen");
			tasSound = content.Load<SoundEffect>("Audio\\SE\\TAS");
			damageSound = content.Load<SoundEffect>("Audio\\SE\\damage2");

			texture = content.Load<Texture2D>("Object\\Character\\Player1");
		}

		#region Update
		/// <summary>
		/// 入力状態のチェック：アニメーション更新に利用
		/// </summary>
		/// <returns></returns>
		private int[] KeyCheck()
		{
			workKeyNum = keyNum.Length;						//使用するキーコード分だけチェック
			int[] keyCheck = new int[workKeyNum];			//キーが押されたかのチェック

			for (int i = 0; i < keyNum.Length; i++) {
				if (KeyInput.IsOnKeyDown(keyNum[i])) keyCheck[i] = 1;
				else keyCheck[i] = 0;
			}
			return keyCheck;								// 押されているキーを返す
		}
		private int[] ButtonCheck()
		{
			workButtonNum = buttonNum.Length;
			int[] buttonCheck = new int[workButtonNum];

			for (int i = 0; i < buttonNum.Length; i++) {
				if (JoyStick.KEY(buttonNum[i])) buttonCheck[i] = 1;
				else buttonCheck[i] = 0;
			}
			return buttonCheck;
		}
		private int[] StickCheck()
		{
			workStickNum = stickNum.Length;
			int[] stickCheck = new int[workStickNum];

			for (int i = 0; i < stickNum.Length; i++) {
				if (JoyStick.stickDirection == (stickNum[i])) stickCheck[i] = 1;
				else stickCheck[i] = 0;
			}
			return stickCheck;
		}

		/// <summary>
		/// Sword.Update()でやっていたのを(主に攻撃技フラグの上げ下げ)をここに移動させる
		/// </summary>
		private void UpdateAttackProcess()
		{
			if (isAttacking) {
				if (isAttacking1) {
					sword.SlashVertically(turnsRight, 120, 8);
					if (sword.isEnd) {									// isEndはsword内のメソッド内で使い始めに初期化
						isAttacking1 = false;
						hasAttacked = true;								// ダメージ判定に関わる
						isAttacking = false;
					}
				}
				if (isAttacking2) {
					sword.SlashHorizontally(turnsRight, 120, 8);
					if (sword.isEnd) {
						isAttacking2 = false;
						hasAttacked = true;
						isAttacking = false;
					}
				}
				if (isAttacking3) {
					sword.SlashHardlly(turnsRight, 70, 8);
					if (sword.isEnd) {
						isAttacking3 = false;
						hasAttacked = true;
						isAttacking = false;
					}
				}
				if (isCuttingUp) {
					sword.SlashUp(turnsRight, 110, 8);
					if (sword.isEnd) {
						isCuttingUp = false;
						hasAttacked = true;
						isAttacking = false;
					}
				}
				if (isCuttingDown) {
					sword.SlashDown(turnsRight, 110, 8);
					if (sword.isEnd) {
						isCuttingDown = false;
						hasAttacked = true;
						isAttacking = false;
					}
				}
				if (isCuttingDownFromAir) {
					sword.SlashDownFromAir(turnsRight,110,8);//適当
					if (sword.isEnd) {
						isCuttingDownFromAir = false;
						hasAttacked = true;
						isAttacking = false;
					}
				}
				if (isCuttingAway) {
					sword.BlowAway(turnsRight, 110, 8);
					if (sword.isEnd) {
						isCuttingAway = false;
						hasAttacked = true;
						isAttacking = false;
					}
				}
				if (isAirial) {
					sword.AirialSlash(turnsRight, 110, 8);
					if (sword.isEnd) {
						isAirial = false;
						hasAttacked = true;
						isAttacking = false;
					}
				}

				if (isThrusting) {
					sword.MegaThrust(turnsRight, 80, 8);
					if (sword.isEnd) {
						isThrusting = false;
						hasAttacked = true;
						isAttacking = false;
					}
				}
				if (isDashAttacking) {// これ実際もう１段階抽象化できるよな...Swordは動きだけ管理すればいいんだから
					sword.ThrustInDashing(turnsLeft, 110, 16);
					if (sword.isEnd) {
						isDashAttacking = false;
						hasAttacked = true;
						isAttacking = false;
					}
				}

			}
		}
		private static readonly int walkingFootStepRatio = 10;
		private static readonly int dashingFootStepRatio = 7;
		protected void UpdateInput()
		{
			#region General
			// 主にDebug用
			if (KeyInput.IsOnKeyDown(Keys.Space)) {
				if (!stage.isScrolled) stage.isScrolled = true;
				else if (stage.isScrolled) stage.camera.isScrollingToPlayer = true;
			}
			if (KeyInput.IsOnKeyDown(Keys.U)) {
				if (!game.isMuted) game.isMuted = true;
				else if (game.isMuted) game.isMuted = false;
			}
			if (KeyInput.IsOnKeyDown(Keys.T)) {
				if (!game.inDebugMode) game.inDebugMode = true;
				else if (game.inDebugMode) game.inDebugMode = false;
			}
			if (KeyInput.IsOnKeyDown(Keys.J)) {
				this.position.X = 14000f;
				drawPos.X = 200;
				stage.camera.position.X = 13800;
			}
			if (KeyInput.IsOnKeyDown(Keys.Q)) {
				stage.boss.isAlive = false;
				stage.boss.HP = -1;
			}
			if (KeyInput.IsOnKeyDown(Keys.H)) {
				HP += 10;
			}
			// Shortcut
			if (KeyInput.IsOnKeyDown(Keys.K)) { isAttacking = true; sword.isBeingUsed = true; isCuttingUp = true; }
			if (KeyInput.IsOnKeyDown(Keys.L)) { TrackEnemy2(); }
			if (KeyInput.IsOnKeyDown(Keys.O)) { isAttacking = true; sword.isBeingUsed = true; isCuttingDownFromAir = true; }
			// 向き変更
			if (KeyInput.IsOnKeyDown(Keys.CapsLock)) {
				if (turnsRight) { turnsRight = false; turnsLeft = true; } else if (!turnsRight) { turnsRight = true; turnsLeft = false; }
			}
			// StageChange
			/*if (KeyInput.IsOnKeyDown(Keys.F1)) { game.stageNum = 1; game.ReloadStage(false); }
			if (KeyInput.IsOnKeyDown(Keys.F2)) { game.stageNum = 2; game.ReloadStage(false); }
			if (KeyInput.IsOnKeyDown(Keys.F3)) { game.stageNum = 3; game.ReloadStage(false); }
			if (KeyInput.IsOnKeyDown(Keys.F4)) { game.stageNum = 4; game.ReloadStage(false); }
			if (KeyInput.IsOnKeyDown(Keys.F5)) { game.stageNum = 0; game.ReloadStage(false); }
			if (KeyInput.IsOnKeyDown(Keys.F6)) { game.stageNum = 5; game.ReloadStage(false); }
			if (KeyInput.IsOnKeyDown(Keys.F7)) { game.stageNum = 6; game.ReloadStage(false); }*/

			// TAS
			if (TASpower > 0) {
				if (game.inDebugMode) {
					switch (game.avilityNum) {
						case 0://Reverse
							if (JoyStick.KEY(5))
								stage.reverse.StartReverse();
							if (JoyStick.IsOnKeyDown(5)) {
								if (!game.isMuted) tasSound.Play(SoundControl.volumeAll, 0f, 0f);
							}

							if (!JoyStick.KEY(5) || JoyStick.IsOnKeyUp(5))
								stage.reverse.isReversed = false;
							break;
						case 1://SlowMotion
							if (JoyStick.KEY(5))
								stage.slowmotion.StartSlowMotion();
							if (JoyStick.IsOnKeyDown(5)) {
								if (!game.isMuted) tasSound.Play(SoundControl.volumeAll, 0f, 0f);
							}

							if (!JoyStick.KEY(5) || JoyStick.IsOnKeyUp(5))
								stage.slowmotion.FinishSlowMotion();
							break;
						case 2://Accel
							if (JoyStick.KEY(5))
								stage.isAccelerated = true;
							if (JoyStick.IsOnKeyDown(5)) {
								if (!game.isMuted) tasSound.Play(SoundControl.volumeAll, 0f, 0f);
							}
							if (!JoyStick.KEY(5) || JoyStick.IsOnKeyUp(5))
								stage.isAccelerated = false;
							break;
					}
				} else {
					if (JoyStick.KEY(5))
						switch (game.avilityNum) {
							case 0:
								stage.reverse.StartReverse();
								break;
							case 1:
								stage.slowmotion.StartSlowMotion();
								break;
							case 2:
								stage.isAccelerated = true;
								break;
						}
					if (!JoyStick.KEY(5) || JoyStick.IsOnKeyUp(5))
						switch (game.avilityNum) {
							case 0:
								stage.reverse.isReversed = false;
								break;
							case 1:
								stage.slowmotion.FinishSlowMotion();
								break;
							case 2:
								stage.isAccelerated = false;
								break;
						}
					if (JoyStick.IsOnKeyDown(5)) {
						if (!game.isMuted) tasSound.Play(SoundControl.volumeAll, 0f, 0f);
					}
				}
			} else if (stage.slowmotion.isSlow) {
				stage.slowmotion.FinishSlowMotion();
			}

			// Pause
			if (JoyStick.IsOnKeyDown(8)) {
				stage.isPausing = true;
			}
			#endregion
			#region Attacking
			hasAttacked = false;
			// NormalCombo: △ △ ○
			// LaunchCombo: △ △ △＋↑ ○/× △...

			#region cross(rect)
			if (/*KeyInput.IsOnKeyDown(Keys.Down) || */JoyStick.IsOnKeyDown(0)) {
				if (normalComboCount == 4) {
					if (time < normaSecondComboTime) {
						hasAttacked = isAttacking = isInCombo = isCuttingDown = sword.isBeingUsed = true;
						isTrackingEnemy = isCuttingUp = false;
						time = 0;
						inCombos[normalComboCount] = false;
						inCombos[++normalComboCount] = true;
						/*inCombo4 = false;
						inCombo5 = true;
						normalComboCount = 5;*/
					}
				}
			}
			#endregion
			#region rect
			if (TASpower >= thrustPowerConsumption && /*KeyInput.IsOnKeyDown(Keys.Right) || */JoyStick.IsOnKeyDown(0) && thrustCount >= thrustingTime) {
				//BackStep();
				if (normalComboCount == 0 || normalComboCount == 2 || normalComboCount == 3) {
					hasAttacked = isAttacking = isInCombo = isThrusting = sword.isBeingUsed = true;
					isAttacking2 = false;

					inCombos[1] = true;
					//inCombo1 = true;
					normalComboCount = 3;
					time = thrustCount = sword.degreeCounter = 0;
					//TASpower -= thrustPowerConsumption;	// デバッグのため
				}
			}
			#endregion
			#region circle
			if (JoyStick.IsOnKeyDown(3)) {
				if (normalComboCount == 0) {// 単発の強攻撃
					hasAttacked = isAttacking = isAttacking3 = isInCombo = sword.isBeingUsed = true;
					time = 0;              // 最初に押したときから計り始める
					sword.degreeCounter = 0;
					
					//normalComboCount = 1;
					//inCombo1 = true;
					inCombos[++normalComboCount] = true;
				} else if (normalComboCount == 2) {// 地上３段目
					if (time < normaSecondComboTime) {// hasAttacked = trueはデフォで無
						hasAttacked = isAttacking = isAttacking3 = isInCombo = sword.isBeingUsed = true;
						isAttacking2 = false;
						time = sword.degreeCounter = 0;
						/*normalComboCount = 3;
						inCombo2 = false;
						inCombo3 = true;*/
						inCombos[normalComboCount] = false;
						inCombos[++normalComboCount] = true;
					}
				} else if (normalComboCount == 4) {
					if (time < normaSecondComboTime) {// hasAttacked = trueはデフォで無
						hasAttacked = isAttacking = isCuttingAway = isInCombo = sword.isBeingUsed = true;
						isTrackingEnemy = false;
						
						/*normalComboCount = 5;
						inCombo4 = false;
						inCombo5 = true;*/
						inCombos[normalComboCount] = false;
						inCombos[++normalComboCount] = true;
						time = 0;
					}
				} else if (normalComboCount == 6) {
					if (time < normaSecondComboTime) {// hasAttacked = trueはデフォで無
						hasAttacked = isAttacking = isCuttingAway = isInCombo = sword.isBeingUsed = true;
						isAttacking2 = false;
						
						normalComboCount = 0;//7
						inCombos[6] = false;
						inCombos[7] = true;
						/*inCombo6 = false;
						inCombo7 = true;*/
						time = 0;
					}
				}
			}
			#endregion
			#region triangle
			if (/*KeyInput.IsOnKeyDown(Keys.Up) ||*/ JoyStick.IsOnKeyDown(1)) {
				if (normalComboCount == 0) {
					hasAttacked = isAttacking = isAttacking1 = isInCombo = sword.isBeingUsed = true;
					isAttacking2 = false;
					time = 0;

					/*normalComboCount = 1;
					inCombo1 = true;*/
					inCombos[++normalComboCount] = true;
				} else if (normalComboCount == 1) {
					if (time < normaFirstComboTime) {
						hasAttacked = isAttacking = isAttacking2 = isInCombo = sword.isBeingUsed = true;
						isAttacking1 = false;      // 自動終了する前に強制的に次の攻撃に移るのでfalseに調整

						/*normalComboCount = 2;
						inCombo1 = false;
						inCombo2 = true;*/
						inCombos[normalComboCount] = false;
						inCombos[++normalComboCount] = true;
						time = 0;
					} // すぐに1段目を出せるようにする。地上の3段目は○とか△＋○とかにしよう　上に書く
				} else if (normalComboCount == 2 && JoyStick.stickDirection != Direction.UP) {
					hasAttacked = isAttacking = isAttacking1 = isInCombo = sword.isBeingUsed = true;
					isAttacking2 = false;
					isInNormalCombo = true;

					time = 0;
					sword.degreeCounter = 0;// 角度が初期化されないので必要
					/*inCombo2 = false;
					inCombo1 = true;              // launch comboとnormal comboで時間を分けてもいいかも*/
					inCombos[normalComboCount] = false;
					normalComboCount = 1;
					inCombos[normalComboCount] = true;
				} else if (normalComboCount == 4) {
					if (time < normaSecondComboTime) {
						hasAttacked = isAttacking = isAttacking1 = isInCombo = sword.isBeingUsed = true;

						/*normalComboCount = 5;
						inCombo4 = false;
						inCombo5 = true;*/
						inCombos[normalComboCount] = false;
						inCombos[++normalComboCount] = true;
						time = 0;
					}
				} else if (normalComboCount == 5) {
					if (time < normaSecondComboTime) {
						hasAttacked = isAttacking = isAttacking2 = isInCombo = sword.isBeingUsed = true;
						isAttacking1 = false;

						/*normalComboCount = 6;
						inCombo5 = false;
						inCombo6 = true;*/
						inCombos[normalComboCount] = false;
						inCombos[++normalComboCount] = true;
						time = 0;
					}
				}
			}
			#region ShootBeam
			/*if (JoyStick.KEY(1) && normalComboCount==1) {
                if(time > 60) {
                    hasAttacked = true;
                    isAttacking = true; sword.isBeingUsed=true;
                    isToChargingMotion = true;
                    time = 0;

                    normalComboCount = 2;
                }
            }
            if(JoyStick.IsOnKeyUp(1) ) {//JoyStick.IsOnKeyUp(1) && normalComboCount == 2
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
			#region triangle+↑
			// 押しっぱなし+↑ではなく、↑押しっぱで△3回押して斬り上げる感じのほうがいいらしい　要修正
			// 2撃目を出すために△を押したフレームのときは飛ばしたい
			#region !syoryu
			if (!syouryuuMode) {
				if (JoyStick.IsOnKeyDown(1) /*&& JoyStick.onStickDirectionChanged*/ && JoyStick.stickDirection == Direction.UP) {// この設定なら、押しっぱなしで自動追撃するようにしたほうがよい(DMC3みたいに)
					if (normalComboCount == 2 && time != 0) {// stickが倒れた瞬間
						if (time < normaSecondComboTime) {
							hasAttacked = isAttacking = isCuttingUp = isTrackingEnemy = isInCombo = sword.isBeingUsed = true;
							isAttacking2 = false;

							/*normalComboCount = 3;
							//stage.sword.degreeCounter = 0; // 重要//
							
							//inCombo1 = false;//一応
							inCombo2 = false;
							inCombo3 = true;*/
							inCombos[normalComboCount] = false;
							inCombos[++normalComboCount] = true;
							time = 0;
						}
					}
					/*else if(normalComboCount == 3) {// ctrl+K)*2:bookmark　(ctrl+K),(ctrl+N/P):bookmarkに移動
						if(time < normaSecondComboTime) {
							normalComboCount = 4;
							isAttacking = false;
							isInCombo = true;
							isCuttingUp = false;
							hasAttacked = true;
							isTrackingEnemy = true;
							time = 0;
							inCombo3 = false;
							inCombo4 = true;
							TrackEnemy2(); // 物理エンジンを利用するならここで1回のみ呼んでもよい
						}
					}*/
				}
				//trackCounter++;
				if (normalComboCount == 3 && isTrackingEnemy && JoyStick.IsOnKeyUp(1) && time < 20) {// 斬り上げのコンボ中かつ指定時間以内に離さないなら自動追尾
					normalComboCount = 0;
					isAttacking = isCuttingUp = isInCombo = false;
					//inCombo3 = false;
					inCombos[normalComboCount] = false;
				}
				if (normalComboCount == 3 && isTrackingEnemy && time > 20) {//isCuttingUp && JoyStick.KEY(1) && JoyStick.stickDirection == Direction.UP) {
					hasAttacked = isAttacking = isCuttingUp = false;
					isInCombo = isTrackingEnemy = true;

					/*normalComboCount = 4;
					inCombo3 = false;
					inCombo4 = true;*/
					inCombos[normalComboCount] = false;
					inCombos[++normalComboCount] = true;
					time = 0;
					TrackEnemy2();
				}
			}
			#endregion
			#region syoryu
			 else {
				if (JoyStick.IsOnKeyDown(1) && JoyStick.stickDirection == Direction.UP /*|| (JoyStick.KEY(1) && JoyStick.onStickDirectionChanged && JoyStick.Vector.Y < .5)*/) {
					if (normalComboCount == 2 && time != 0) {// stickが倒れた瞬間
						if (time < normaSecondComboTime) {
							hasAttacked = isAttacking = isCuttingUp = isTrackingEnemy = isInCombo = sword.isBeingUsed = true;
							isAttacking2 = false;

							/*normalComboCount = 3;
							inCombo1 = false;
							inCombo2 = false;
							inCombo3 = true;*/
                            inCombos[1] = false;
							inCombos[normalComboCount] = false;
							inCombos[++normalComboCount] = true;
							time = 0;
							TrackEnemy2();
						}
					}
				}
				if (normalComboCount == 3 && isTrackingEnemy && time > 20) {
					hasAttacked = isTrackingEnemy = isInCombo = true;
					isAttacking = isCuttingUp = false;

					/*normalComboCount = 4;
					inCombo3 = false;
					inCombo4 = true;*/
					inCombos[normalComboCount] = false;
					inCombos[++normalComboCount] = true;
					time = 0;
					//TrackEnemy2();
				}
			}
			#endregion
			#endregion
			#region triangle+↓
			if ((JoyStick.IsOnKeyDown(1) && JoyStick.stickDirection == Direction.DOWN) || (JoyStick.KEY(1) && JoyStick.onStickDirectionChanged && JoyStick.Vector.Y > .5)) {// コンボを強制終了させる　とりあえずコンボから独立させよう
				if (/*normalComboCount == 2 &&*/ time != 0 || time == 0) {
					//if(time < normaSecondComboTime) {
					isAttacking1 = isAttacking2 = isAttacking3 = isCuttingAway = isCuttingDown = isCuttingUp = isTrackingEnemy = false;
					hasAttacked = isAttacking = sword.isBeingUsed = true;

					time = normalComboCount = 0;
					//inCombo1 = false;
					//inCombo2 = false;
					//inCombo3 = false;
					AirialDown();
					isAirial = true;
					//}
				}
			}
			if (isAirial && time > 20) { //atProcessで.?
				isAttacking = false;
				isAirial = false;
				hasAttacked = true;
				time = 0;
			}

			#endregion

			time++;// 攻撃開始時に初期化、次の入力までの時間を計る
			thrustCount++;
			UpdateAttackProcess();
			#region EndProcess
			// Comboの終了処理
			/*if ((inCombo1 && time > normaFirstComboTime && !JoyStick.KEY(1)) || (inCombo2 && time > normaSecondComboTime && !isChargingPower)// 段別に時間制限を設定可
				|| (inCombo3 && time > normaSecondComboTime) || (inCombo4 && time > normaSecondComboTime)
				|| (inCombo5 && time > normaSecondComboTime)// 最後の技を出した後すぐ終わらせたい場合はComboTimeを短くすればおｋ
				|| (inCombo6 && time > normaSecondComboTime) || (inCombo7 && time > 10)) {// 遅かったら勝手にコンボ終了
				isInCombo = false;
				if (inCombo1) inCombo1 = false;
				if (inCombo2) inCombo2 = false;
				if (inCombo3) inCombo3 = false;
				if (inCombo4) inCombo4 = isTrackingEnemy = false;
				if (inCombo5) inCombo5 = false;
				if (inCombo6) inCombo6 = false;
				if (inCombo7) inCombo7 = false;

				normalComboCount = 0;
				//isAttacking = false; // 攻撃全体を終了
				time = 0;
			}*/
            if (time > normaSecondComboTime) { }
			for (int i = 1; i < inCombos.Length; i++) {
                if (inCombos[1] && time > normaFirstComboTime && !JoyStick.KEY(1) || inCombos[2] && time > normaSecondComboTime && !isChargingPower
                    || i >= 3 && i < 7 && inCombos[i] && time > normaSecondComboTime || inCombos[7] && time > 10) {
					isInCombo = false;
					if (inCombos[i]) inCombos[i] = false;
					if (i == 4) isTrackingEnemy = false;
					normalComboCount = time = 0;
				}
			}
			#endregion

			#endregion
			#region Moving
			// 左右移動
			if (!disableMovingInput) {
				if (JoyStick.stickDirection == Direction.RIGHT) {
					turnsRight = true;
					if (isDashing) {
						if (stage.isAccelerated)
							if (!onConveyor) speed.X = 16.5f;
							else speed.X += 16.5f * timeCoef;
						else
							if (!onConveyor) speed.X = 7.5f;
							else speed.X += 7.5f * timeCoef;
					} else {
						if (stage.isAccelerated)
							if (!onConveyor) speed.X = 10.5f;
							else speed.X += 10.5f * timeCoef;
						else
							if (!onConveyor) speed.X = 5.0f;
							else speed.X += 5.0f * timeCoef;
					}

					if (counter % walkingFootStepRatio == 0 && !isJumping && !isDashing) {
						if (!game.isMuted) footstep.Play(SoundControl.volumeAll, 0f, 0f);
					} else if (counter % dashingFootStepRatio == 0 && !isJumping && isDashing) {
						if (!game.isMuted) footstep.Play(SoundControl.volumeAll, 0f, 0f);
					}
				}
				if (JoyStick.stickDirection == Direction.LEFT) {
					turnsRight = false;
					if (isDashing) {
						if (stage.isAccelerated)
							if (!onConveyor) speed.X = -16.5f;
							else speed.X += -16.5f * timeCoef;
						else
							if (!onConveyor) speed.X = -6.5f;
							else speed.X += -6.5f * timeCoef;
					} else {
						if (stage.isAccelerated)
							if (!onConveyor) speed.X = -10.5f;
							else speed.X += -10.5f * timeCoef;
						else
							if (!onConveyor) speed.X = -4.0f;
							else speed.X += -4.0f * timeCoef;
					}

					if (counter % walkingFootStepRatio == 0 && !isJumping && !isDashing) {
						if (!game.isMuted) footstep.Play(SoundControl.volumeAll, 0f, 0f);
					} else if (counter % dashingFootStepRatio == 0 && !isJumping && isDashing) {
						if (!game.isMuted) footstep.Play(SoundControl.volumeAll, 0f, 0f);
					}
				}
			}

			// 落下速度うｐ→4/1：二度押しにする.空中下技が欲しい
			//if (JoyStick.stickDirection == Direction.DOWN) speed.Y += 5;

			// ダッシュ
			/*if (JoyStick.KEY(4)) {//KeyInput.IsOnKeyDown(Keys.LeftShift) ||
				if(hasDashed) {
					isEffected = true;
					dashEffected = true;
					hasDashed = false;
				}
				isDashing = true; 
			}
			if (JoyStick.IsOnKeyUp(4)) {
				isDashing = false; 
				hasDashed = true; //buttonを押し込む度にエフェクトが出てしまう
			}*/
			/*if ((JoyStick.IsOnKeyDown(4) && (JoyStick.stickDirection == Direction.LEFT || JoyStick.stickDirection == Direction.RIGHT) ) ) {
					//|| (JoyStick.KEY(4) && (JoyStick.stickDirection == Direction.LEFT || JoyStick.stickDirection == Direction.RIGHT))) {
				isDashing = true;
				isEffected = true;
				hasDashed = false;
				dashEffected = true;
			}
			if (JoyStick.IsOnKeyUp(4)) isDashing = false;
			if (JoyStick.KEY(4)) hasDashed = true;*/
			if (JoyStick.KEY(4) && (JoyStick.stickDirection == Direction.LEFT || JoyStick.stickDirection == Direction.RIGHT)) {
				if (!hasDashed) {
					isEffected = true;
					dashEffected = true;
					hasDashed = true;
				}
				isDashing = true;
			}
			if (JoyStick.IsOnKeyUp(4)) {
				isDashing = false;
				hasDashed = false;
			}
			if (!hasDashed && isDashing) {
			}

			if (KeyInput.IsOnKeyDown(Keys.B) || JoyStick.KEY(7)) {// debug用超ダッシュ
				isDashing = true;
				if (speed.X > 0) {
					if (!stage.isScrolled) speed.X = 24;
					else speed.X = 24;
				}
				if (speed.X < 0) {
					if (!stage.isScrolled) speed.X = -24;
					else speed.X = -24;
				}
			}
			#endregion
			#region Jumping
			// ジャンプ
			if (JoyStick.KEY(2) && /*isOnSomething*/!isInCombo) {
				jumpTime++;                                         // 押下時間をチェック
				if (jumpTime > miniJumpWaitTime && !isJumping) {
					Jump(12);
					counter = 0;
					//hasJumped = true;

					if (isAttacking) {
						sword.EndQuickly();
						hasAttacked = true;
						/*isInCombo = inCombo1 = inCombo2 = inCombo3 = inCombo4
							= inCombo5 = inCombo6 = inCombo7 = isTrackingEnemy = false;*/
                        for (int i = 0; i < inCombos.Length; i++) inCombos[i] = false;
                        isInCombo = isTrackingEnemy = false;
						isAttacking = isAttacking1 = isAttacking2 = isAttacking3 = false;
						normalComboCount = time = 0;
					}
				}
			} else if (JoyStick.KEY(2) && isInCombo) {           // キャンセル用
				//if (isAttacking) {// falseだった　邪魔だなこれは
				sword.EndQuickly();
				hasAttacked = true;
				/*isInCombo = inCombo1 = inCombo2 = inCombo3 = inCombo4
							= inCombo5 = inCombo6 = inCombo7 = isTrackingEnemy = false;*/
                for (int i = 0; i < inCombos.Length; i++) inCombos[i] = false;
                isInCombo = isTrackingEnemy = false;
				isAttacking = isAttacking1 = isAttacking2 = isAttacking3 = false;
				normalComboCount = time = 0;
				//}
			}
			//counter++;// これってUpdate直下でしてるから要らなくないか...?
			// 2段目以降は、空中なのでIsOnKeyDownでおｋ
			if (JoyStick.IsOnKeyDown(2)) {// 一気にjumpCountが0～2までいくのを修正.3/10 ここだけだとちょっとキャンセル具合がよろしくないのでKEYでも.
				if (isAttacking) {
					sword.EndQuickly();
					hasAttacked = true;
					/*isInCombo = inCombo1 = inCombo2 = inCombo3 = inCombo4
							= inCombo5 = inCombo6 = inCombo7 = isTrackingEnemy = false;*/
                    for (int i = 0; i < inCombos.Length; i++) inCombos[i] = false;
                    isInCombo = isTrackingEnemy = false;
					isAttacking = isAttacking1 = isAttacking2 = isAttacking3 = false;
					normalComboCount = time = 0;
				}

				if (jumpCount == 1 && isJumping && counter > miniJumpWaitTime) {//30
					Jump(12);//12は使われない
				} else if (game.inDebugMode && jumpCount >= 2 && isJumping && counter > miniJumpWaitTime) {//30
					Jump(12);
				}
			}
            // mini jump
			if (JoyStick.IsOnKeyUp(2) && jumpTime >= 0 && !isInCombo && !isJumping) {// jumpTime > 0
				// 調整が難しい
				//if(jumpTime < 5) scalarSpeed = 0;
				//if(!hasJumped) 
				if (!isJumping)
					Jump(jumpTime);
			}

			// 可変長ジャンプ：押下時間で高さを変えるパターン:不使用だが残す
			if (KeyInput.IsOnKeyDown(Keys.Tab)) {
				isJumping = true;
				speed.Y = -.7f;
				position.Y += speed.Y;
			}
			#endregion
		}
		protected override void UpdateNumbers()
		{
			/*if (scrollPush) {
				isMoving = true;
			}*/

			if (isOnSomething) {
				damageTime = 0;
				if (normalComboCount == 4 || normalComboCount == 0) isTrackingEnemy = false;
				//isJumping = false;
			} else hasPlayedSoundEffect = false;
			if (isOnSomething && !hasPlayedSoundEffect) {
				if (!game.isMuted) landingSound.Play(SoundControl.volumeAll, 0f, 0f);
				hasPlayedSoundEffect = true;
			}

			if (inAirReflection && airReflectCount <= 5) AdjustReflect(reflectSpeed);
			// 加速
            if (isAttacking /*&& (!hasJumped || !isJumping)|| isOnSomething*/&& (!syouryuuMode || (syouryuuMode && !isTrackingEnemy)) && !isAirial && !inAirReflection) {
                gravity = defGravity;
                //speed.Y = 0;
                speed.Y *= .4f;
            }// isOnSomethingもいれるとばぐる
            else {
                gravity = defGravity;
            }
			speed.Y += (float)gravity * timeCoef;//if(!isAttacking)で絞ってもあまり変わらない

			// 減速 scalarSpeed *= friction;
			if (speed.X > 0) {
				if (!isInCombo) {
					speed.X += -(.60f * friction) * timeCoef;//accel
					if (/*!isInCombo */ speed.X < 0) speed.X = 0;
				} else {
					speed.X *= .90f;
				}
			}
			if (speed.X < 0) {
				//speed.X += (.60f * friction);
				//if(speed.X > 0) speed.X = 0;
				if (!isInCombo) {
					speed.X += (.60f * friction) * timeCoef;//accel
					if (/*!isInCombo */ speed.X > 0) speed.X = 0;
				} else speed.X *= .90f;
			}

			// すり抜け防止：ベクトル判定を実装して使うようにすれば不要だが、落下速度が大きくなってゲーム性を損なう恐れもある
			if (!isInCombo)
				if ((speed.Y) > maxSpeed) speed.Y = maxSpeed;
				else if (speed.Y < -maxSpeed) speed.Y = -maxSpeed;

			// 落下中は速く落ちすぎると操作性に影響してしまうので
			if (speed.Y > 20) speed.Y = 20;

			// 位置に加算
			if (stage.slowmotion.isSlow) {
				position.X += speed.X * timeCoef;
				position.Y += speed.Y * timeCoef;
			} else {
				position += speed * timeCoef;
			}

			// 端
			if (position.Y < 0) position.Y = 0;
			// 位置の情報
			locus.Add(position);//drawVectorにすべきか？
			if (locus.Count > 2) locus.RemoveAt(0);// 要素が3つ以上になったら古いのを削除

			// 未来予測型を試してみる
			/*Vector2 nextVector;
			nextVector = position;
			nextVector.X += (float)scalarSpeed;
			nextVector.Y += (float)scalarSpeed;
			locus.Clear();
			locus.Add(position);
			locus.Add(nextVector);*/

			// デバッグ
			if (game.inDebugMode) {
				this.TASpower = MAXTAS;
			}
		}
		public override void Update()
		{
			if (isAlive) {
				UpdateInput();
				UpdateAnimation(KeyCheck(), ButtonCheck(), StickCheck());
				UpdateNumbers();

				counter++;
				AirReflectUpdate();
			}
		}

		/// <summary>
		/// スローモーションのとき時間係数を掛ける
		/// </summary>
		public override void UpdateTimeCoef()
		{
            if (stage.slowmotion.isSlow) {
                timeCoef = timeCoefPlayer;
            } else {
                timeCoef = 1.0f;
            }
		}
		/// <summary>
		/// ダメージを受けたときに敵によって反動速度を変えたいならここを弄る
		/// </summary>
		public override void MotionUpdate()
		{
			base.MotionUpdate();
			/* 毎フレーム削られないための対策：
			 * ①ishitがfalse→true→falseと変わって攻撃が終わったときにダメージ
			 * ②単純な無敵時間の追加
			 * ③一度当たったら、Playerが再び攻撃キーを押さない限り無敵
			 * ④1回の攻撃につき当たるのは1回のみ、攻撃は基本的に自動終了←これを採用
			 */
			if (isDamaged && isAlive) {
				/*if(stage.player.isCuttingUp) {
					BlownAwayMotionUp();isBlownAway = true; //counter = 0;//BlownAwayMotion2();
				}
				else if(stage.player.isCuttingAway)//(stage.player.isCuttingDown || stage.player.isCuttingDownFromAirV2) {
					BlownAwayMotionRight();
                
				else if(stage.player.isCuttingDown)//(stage.player.isCuttingDown || stage.player.isCuttingDownFromAirV2) {
					BlownAwayMotionDown();
                
				if(stage.player.isAttacking3) {
					speed.X += 5;
					speed.Y -= 5;
				}
				else if(stage.player.isThrusting && time % 3 ==0) {
					speed.X += 1;
					speed.Y -= 1;
				}
				else if(!stage.player.isThrusting) {
					speed.X += 1.5f;// 現状では3段入れるならこのくらい　２段なら10～20でいい　
				}*/
				//motionDelayTime++;

				HP--;
				if (turnsRight) speed = new Vector2(-8, -8);
				else speed = new Vector2(8, -8);
				inDmgMotion = true;
				animCounter2 = 0;

				if (HP >= 0) {
					if (!game.isMuted) damageSound.Play(SoundControl.volumeAll, 0f, 0f);
				} else {
					//Cue cue = game.soundBank.GetCue("critical");
					//if(!game.isMuted) cue.Play(SoundControl.volumeAll, 0f, 0f);
				}

				totalHits += 1;
				damageTime = 0;
				//delayTime = 0;

				isEffected = true;
				damageEffected = true;

				if (damageTime < deathComboTime)
					comboCount++;
				damageTime++;
			}
		}
		#region UpdateAnimation
		private void UpdateAnimation(int[] keyCheck, int[] buttonCheck, int[] stickCheck)
		{
			// Key
			/*for (int i = 0; i < keyCheck.Length; i++)
				if (keyCheck[i] == 1) {   // キーが押されているとき
					if (i == 0 || i == 1)  // // 判定するキーをソート(左右移動
						animation.Update(4, 0, 32, 48, 6,1); // アニメ―ションパターン1
					else if (i == 6) { // ジャンプ
						animation.Update(1, 1, 32, 48, 6,2); // アニメーションパターン2
						if(!isJumping) animation.poseCount = 0;// 一応おｋ
					}//if (motionCheck) animation.Update();// いずれかのキーが押されていればアニメーション更新
				}*/

			isMoving = false;
			// 基本静止時のアニメーションで、何かアクションがあればソノアニメーションで上書きする方針
			if (inDmgMotion) {
				animation.Update(1, 1, width, height, 0, 0);
				if (animCounter2 > 20) inDmgMotion = false;
			} else {
				if (isTrackingEnemy || isAirial || !isOnSomething) {
					isMoving = true;
					isInJumpAnim = true; //isJumping = true;//isInJumpAnim = true;
					animCounter = 0;
				}
				// Pad button
				for (int i = 0; i < buttonCheck.Length; i++)
					if (buttonCheck[i] == 1) {                  // 押されていないボタンをカット            
						if (i == 2) {                          // 判定するボタンによって分ける. 今回の素材はmotionが少ないのでifで十分                   
							isInJumpAnim = true;
							animCounter = 0;
							isMoving = true;
						}
					}

				if (isInJumpAnim) {
					if (animCounter == 0)
						animation.Update(2, 2, 48, 48, 6, 2);
					animCounter++;
				}
				// Pad stick
				for (int i = 0; i < stickCheck.Length; i++) {
					if (stickCheck[i] == 1 && !isJumping)          // スティックが倒されている/従事キーが押されている && ジャンプしていない ×buttonCheck[1] == 0
						if ((i == 0 || i == 1) && !isInJumpAnim) {//buttonCheck[1] == 1)
							if (buttonCheck[4] == 0) animation.Update(3, 0, 48, 48, 5, 1);
							else if (buttonCheck[4] == 1) animation.Update(3, 0, 48, 48, 4, 1);
							isMoving = true;
						}
				}
				

				if (isOnSomething) isInJumpAnim = false;
				/*if (isOnSomething && (jumpCount > 0 || ((buttonCheck[1] == 1 && buttonCheck[4] == 1) || (buttonCheck[3] == 1 && buttonCheck[4] == 1)) || JoyStick.onStickDirectionChanged))
					isInJumpAnim = false;
				if ((buttonCheck[4] == 1 || buttonCheck[1] == 1 || buttonCheck[3] == 1 || buttonCheck[0] == 1) && JoyStick.stickDirection == Direction.NEWTRAL) 
					isMoving = false;*/

				// 何もしていなければ静止時の絵
				if (!isMoving && /*!isInJumpAnim &&*/ !isJumping)
					animation.Update(2, 1, 48, 48);
				// スクロールに押されてたら
				/*if (scrollPush) {
					animation.Update(3, 0, 48, 48, 5, 1);
				}*/
			}

			animCounter2++;
		}

		/*protected override void Dispose()
		{
			if (foot != null && foot.IsStopped) foot.Dispose();
			if (jump != null && jump.IsStopped) jump.Dispose();
		}*/
		#endregion
		#endregion
		#region Movement
		// ジャンプ・特殊移動など
		private void Jump(int jumpPower)
		{
			isJumping = true;
			if (jumpCount == 0) {
				/*if(jumpTime < 30) scalarSpeed = firstJumpSpeed-20;
				else if (jumpTime > 30 && jumpTime < 45) scalarSpeed = firstJumpSpeed - 10;
				else if (jumpTime > 45 && jumpTime < 80) scalarSpeed = firstJumpSpeed;*/
				if (jumpTime <= 5) speed.Y = -10;
				else if (jumpTime > 5 && jumpTime <= 6) speed.Y = firstJumpSpeed;//-14
				else speed.Y = firstJumpSpeed;//-14
				//scalarSpeed = firstJumpSpeed;
				//position.Y += (float)scalarSpeed;
				if (!game.isMuted) jumpSound.Play(SoundControl.volumeAll, 0f, 0f);
				jumpCount = 1;
			} else if (jumpCount == 1) {
				speed.Y = secondJumpSpeed;
				position.Y += speed.Y;
				jumpCount = 2;
				if (!game.isMuted) jumpSound.Play(SoundControl.volumeAll, 0f, 0f);
			} else if (game.inDebugMode && jumpCount >= 2) {
				speed.Y = (float)secondJumpSpeed;
				position.Y += (float)speed.Y;
				jumpCount++;
				if (!game.isMuted) jumpSound.Play(SoundControl.volumeAll, 0f, 0f);
			}
			jumpTime = 0;

			//hasJumped = false;
		}// jumpPower使われてない...
		private void TrackEnemy()
		{
			// ゲーム内の物理エンジンを利用するパターン：これはこれでよいが不安定
			// ①敵のvectorからちょっと左めに飛ぶようにする
			// ②敵に与えたのと同じ初速（もしくは+α）を与える。
			double angle;
			double speed;
			int counter = 0;
			isTrackingEnemy = true;
			//stage.damagedCharacters[0]
			speed = 18;
			//counter++;

			//if(counter < 10) {
			if (stage.damagedCharacters.Count > 0 && stage.damagedCharacters[0].speed.X < 10 /*&& sword.damagedCharacters[0].scalarSpeed < 10*/) {//stage.characters[i].isDamagedはダメ  
				angle = Math.Atan2(stage.damagedCharacters[0].position.Y - position.Y,
					(stage.damagedCharacters[0].position.X - stage.damagedCharacters[0].width - 10) - position.X);

				this.speed.X = (float)-Math.Cos(angle) * (float)speed;	// x方向の移動量を計算 多分座標軸の関係で-に
				this.speed.Y = (float)Math.Sin(angle) * (float)speed;	// y方向の移動量を計算
				//if (stage.damagedCharacters[0].scalarSpeed > 0) scalarSpeed = -scalarSpeed;
			}
			//}
			//if (counter > 10) normalComboCount = 0;
		}
		private void TrackEnemy2()
		{
			// 物理エンジンを利用するタイプ
			//float speed = 0;
			//if (isOnSomething) speed = 2;
			//else speed = 8;

			int degree = 74;//-65
			double radius = MathHelper.ToRadians(-degree);

			if (isOnSomething)
				this.speed.Y += (16) * (float)Math.Sin(radius) * timeCoef;
			else
				this.speed.Y += (22) * (float)Math.Sin(radius) * timeCoef;//24

			if (turnsRight) this.speed.X += (34) * (float)Math.Cos(radius) * timeCoef;// →17       speed + 32
			else this.speed.X += -(34) * (float)Math.Cos(radius) * timeCoef;//→-17

			//this.speed.Y += (25) * (float)Math.Sin(radius);// speed + 14 (isOnSomething ? 2 : 24)
		}
		private void AirialDown()
		{
			int degree = -74;//-65
			double radius = MathHelper.ToRadians(-degree);
			if (isOnSomething)
				this.speed.Y += (0) * (float)Math.Sin(radius) * timeCoef;
			else
				this.speed.Y += (24) * (float)Math.Sin(radius) * timeCoef;

			if (!isOnSomething) {
				if (turnsRight) this.speed.X += (34) * (float)Math.Cos(radius) * timeCoef;// →17       speed + 32
				else this.speed.X += -(34) * (float)Math.Cos(radius) * timeCoef;//→-17
			}
		}
		private void BackStep()
		{
			if (isOnSomething) {
				speed.Y = -10;
				if (turnsRight) speed.X = -6;
				else speed.X = 10;
			}
		}
		private void DamageMotion1()
		{
			isBlownAway = true;
			speed.Y = -10;
			if (turnsRight) speed.X = -6;
			else speed.X = 10;
			position.X += speed.X * timeCoef;
			position.Y += speed.Y * timeCoef;
		}
		private void DamageMotion2()
		{
			isBlownAway = true;
			if (turnsRight) {
				speed.X = -4;       // ちょっと後ろへのけぞる感じ
				speed.Y = -4;
			}
			position.X += speed.X * timeCoef;
		}
		private void AdjustReflect(Vector2 reflectSpeed)
		{
			if (airReflectTime != 0)
				this.reflectSpeed = reflectSpeed * 1 / (float)airReflectTime;// 回数補正
			else
				this.reflectSpeed = reflectSpeed;

			if (turnsRight) speed.X = -reflectSpeed.X;//speed.X > 0
			else speed.X = reflectSpeed.X;

			speed.Y = reflectSpeed.Y;
		}
		private void AirReflectUpdate()
		{
			airReflectCount++;
			if (inAirReflection && airReflectCount == 1) AdjustReflect(reflectSpeed);
			if (inAirReflection && airReflectCount > airReflectLimit) {
				inAirReflection = disableMovingInput = false;
				//airReflectTime++;
			}
			if (isOnSomething) airReflectTime = 0;
		}
		public void SwordReflection(Object targetObject)
		{
			if (position.X - targetObject.position.X > 0) speed.X = 5;
			else speed.X = -5;
			position.X += speed.X * timeCoef;
		}
		public void AirSlashReflection(Vector2 reflectSpeeed)
		{
			if (isJumping && (isAttacking1 || isAttacking2)) {//!isOnSomething
				inAirReflection = true;
				//disableMovingInput = true;
				airReflectCount = 0;

				AdjustReflect(reflectSpeeed);
			}
		}
		#endregion

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (isAlive) {
				if (!inDmgMotion) {
					if (turnsRight) {
						spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);
					} else {
						spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
					}
				} else {// 点滅させる
					if (!stage.isPausing) {
						if (blinkCount % 5 == 0) e += 60f;//30f;//.02f;
						dColor = (float)Math.Sin(e * 8) / 2.0f + 0.5f;
					}

					spriteBatch.Draw(texture, drawPos, animation.rect, Color.Red * dColor);
					blinkCount++;
				}
				spriteBatch.DrawString(game.pumpDemi, HP.ToString(), drawPos + new Vector2(width, -10), Color.Orange, 0, Vector2.Zero, new Vector2(.3f), SpriteEffects.None, 0f);
			}
		}
	}
}