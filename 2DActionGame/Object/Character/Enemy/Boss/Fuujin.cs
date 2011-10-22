using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
	/// <summary>
	/// Stage2のボス。風神
	/// </summary>
	public class Fuujin : Boss
	{
		#region Member variable
		/// <summary>
		/// windで攻撃中かどうか（animationで使用）
		/// </summary>
		private bool usingWindB;
		/// <summary>
		/// ice / windで攻撃中かどうか（射撃してすぐに次の攻撃パターンに行きたい時に使用）。リストにして汎用化予定
		/// </summary>
		private bool ice, wind;//, cutter;
		//private List<Attack> attackList; 

		/*/// <summary>
		/// 細かい挙動はAttackでは対応しきれなさそうであることを考慮に入れるとこれでいいか
		/// </summary>
		private List<int> attackList = new List<int>();
		private List<bool> isEnds = new List<bool>();
		private List<int> attackNumList = new List<int>();
		/// <summary>
		/// 攻撃パターンのリスト。管理しやすいように二次元構造にした。
		/// </summary>
		private List<int[]> attackPatternNumList = new List<int[]>();//List<int>*/
		private int[] attackPattern0 = { 1, 2, 4, 5, 6, 8 };
		private int[] attackPattern1 = { 10, 3, 11, 6, 5, 1 };

		private int attackCounterI;
		/// <summary>
		/// タックルで距離を計算するときに使用する変数
		/// </summary>
		private float distanceD, distanceT, distanceA;
		private Vector2 tmpPosition;
		/// <summary>
		/// obstacle（Icicleのような障害物攻撃）のリスト。
		/// </summary>
		public List<Obstacle> obstacles { get; private set; }
		/// <summary>
		/// Meteor / Icicle / Tornade
		/// </summary>
		private Obstacle obstacleMeteor, obstacleIcicleRandom/*, obstacleIcicleInTurn*/, obstacleTornadeSmall, obstacleTornadeLarge;
		private Weapon largeWind;
		/// <summary>
		/// spawn用
		/// </summary>
		private JumpingEnemy jumpingEnemy;
		/// <summary>
		/// カッター弾を出すTurret(5way)
		/// </summary>
		public Turret cutterTurret5Way { get; private set; }
		/// <summary>
		/// カッター弾Turret(4Hard)
		/// </summary>
		public Turret cutterTurret1WayBurst { get; private set; }
		/// <summary>
		/// 1Wayと2Wayを交互に繰り出す
		/// </summary>
		public Turret cutterTurretAlternate { get; private set; }

		/*Action attackMethodType0;
		Action<float>attackMethodType1;
		Action<int> attackMethodType2;
		/// <summary>
		/// 攻撃時に使うメソッドへの参照をまとめているリスト。デリゲートというより関数ポインタ的に使っている。
		/// </summary>
		List<Delegate> attackMethods = new List<Delegate>();
		/// <summary>
		/// デリゲートに渡す引数のリスト。indexが同じattaackMethodsの要素（メソッド）に渡される。
		/// attackMethodsの、引数が無い関数のindexと同じindexの要素はnullにする。
		/// 引数を動的に変えたい場合は該当変数への参照を追加するようにすれば良い。
		/// </summary>
		List<object> attackMethodsArgs = new List<object>();// これは面倒くさいｗｗｗ*/
		#endregion
		/// <summary>
		/// 前の技を出しながら次の技に移行しているときに、フラグが立ったら前の技を終了させる
		/// </summary>
		/// <param name="goWait">（今実行中の全て攻撃が終了しているかどうかに関わらず）待機状態にして次の攻撃に移るかどうか。</param>
		private void EndSkill(bool goWait, int patternNum, int? type)
		{
			switch (patternNum) {
				// icicle
				case 0:
					if (obstacleIcicleRandom.isEnd || attackCounterI > 600) {
						obstacleIcicleRandom.isBeingUsed = false;
						obstacleIcicleRandom.isEnd = false;    // 次フレームで再び到達してしまうのでfalseにする必要がある
						//obstacleIcicleRandom.attackCounter = 0; //obs.MP2()では不使用
						attackCounterI = 0;
						attackCounter = 0;
						if (goWait) {
							isWaiting = true;
							ice = false;

							switch (type) {
								case 0:
									isEnds[4] = true;
									break;
								case 1:
									isEnds[11] = true;
									break;
							}
						}
						ice = false;
					}
					break;
				// windL
				case 1:
					if (obstacleTornadeSmall.isEnd || attackCounter > resWaitTime) {
						isAttacking = false;
						usingWindB = false;
						obstacleTornadeSmall.isEnd = false;
						obstacleTornadeSmall.isBeingUsed = false;
						obstacleTornadeSmall.attackCounter = 0;
						attackCounter = 0;
						if (goWait) {
							isWaiting = true;
							wind = false;

							switch (type) {
								case 0:
									isEnds[5] = true;
									break;
								case 1:
									isEnds[6] = true;
									break;
							}
						} else
							wind = true;
					}
					break;
			}
			//}
			/*else {
				if (obstacleIcicleRandom.isBeingUsed) ice = true;
				if (obstacleTornadeSmall.isBeingUsed) wind = true;
			}*/
		}

		// 攻撃パターンを記述しているメソッド
		#region Common
		/// <summary>
		/// タックル攻撃。これもvirtualで用意するかなぁ
		/// </summary>
		/// <param name="Movespeed">移動速度（全体で一定）</param>
		private void Tackle(float Movespeed)
		{
			// 攻撃終了地点からデフォルト位置に引くベクトル
			Vector2 returnVector, wayVector, attackVector;
			returnVector = defaultPosition - position;
			wayVector = tmpPosition - position;
			attackVector = attackPosition - position;

			// 単位ベクトル化(正規化)
			Vector2 baseVector = Vector2.Normalize(returnVector);
			Vector2 baseVectorT = Vector2.Normalize(wayVector);
			Vector2 baseVectorA = Vector2.Normalize(attackVector);

			//Vector2.Multiply(baseVector, 3); // これだと上手く乗算されないようだ
			baseVector *= new Vector2(Movespeed, Movespeed);
			baseVectorT *= new Vector2(Movespeed, Movespeed);
			baseVectorA *= new Vector2(Movespeed + 5, Movespeed + 5);

			distanceD = Vector2.Distance(position, defaultPosition);
			distanceT = Vector2.Distance(position, tmpPosition);
			distanceA = Vector2.Distance(position, attackPosition);

			if (isStartingAttack) {
				speed = baseVectorT;
			}

			// 攻撃位置まで来たらフラグを立てて攻撃する
			if (isStartingAttack && distanceT < Movespeed) {// < 5
				isStartingAttack = false;
				isAttacking = true;
			}

			if (isAttacking) speed = baseVectorA;
			if (isAttacking && (distanceA < 5 || attackCounter > 180)) {
				isAttacking = false;
				isEndingAttack = true;
			}

			if (isEndingAttack) speed = baseVector;
			if (isEndingAttack && distanceD < 5) {
				speed = Vector2.Zero;
				isEndingAttack = false;
				isAttacking = false;
				isWaiting = true;
				attackCounter = 0;
				waitCounter = 0;

				isEnds[1] = true;
			}
		}
		/// <summary>
		/// 特定の位置まで移動してから攻撃したいときに使う。
		/// </summary>
		/// <param name="Movespeed">移動速度（スカラー）</param>
		/// <param name="tmpPos">ウェイポイント（今のところ１つだけしか設定できない）</param>
		/// <param name="atPos">攻撃する場所（目的地）</param>
		private void PreMove(float Movespeed, Vector2 tmpPos, Vector2 atPos)
		{
			// 攻撃終了地点からデフォルト位置に引くベクトル
			Vector2 returnVector, wayVector, attackVector;
			returnVector = defaultPosition - position;
			wayVector = tmpPos - position;
			attackVector = atPos - position;

			// 単位ベクトル化(正規化)
			Vector2 baseVector = Vector2.Normalize(returnVector);
			Vector2 baseVectorT = Vector2.Normalize(wayVector);
			Vector2 baseVectorA = Vector2.Normalize(attackVector);

			baseVector *= new Vector2(Movespeed, Movespeed);
			baseVectorT *= new Vector2(Movespeed, Movespeed);
			baseVectorA *= new Vector2(Movespeed + 5, Movespeed + 5);

			distanceD = Vector2.Distance(position, defaultPosition);
			distanceT = Vector2.Distance(position, tmpPos);
			distanceA = Vector2.Distance(position, atPos);

			if (isStartingAttack) speed = baseVectorT;

			// 攻撃位置まで来たらフラグをたてて攻撃。
			if (isStartingAttack && distanceT < Movespeed) {
				isStartingAttack = false;
				isAttacking = true;
			}

			if (isAttacking) speed = baseVectorA;
			if (isAttacking && (distanceA < Movespeed || attackCounter > resWaitTime)) {
				isAttacking = false;
				isEndingAttack = true;
			}
			if (isEndingAttack) speed = baseVector;

			if (isEndingAttack && distanceD < Movespeed) {
				speed = Vector2.Zero;
				isEndingAttack = false;
				isAttacking = false;
				isWaiting = true;
				attackCounter = 0;
				waitCounter = 0;
			}
		}
		/// <summary>
		/// 風(DamageObject)を射撃
		/// </summary>
		/// <param name="movementType">0:tornade,1:tornadeL</param>
		private void AttackWithWind(int type)
		{
			if (isStartingAttack) {
				obstacleTornadeSmall.isBeingUsed = true;
				isStartingAttack = false;
				isAttacking = true;
				usingWindB = true;
			}

			switch (type) {
				case 0:
					if (obstacleTornadeSmall.isBeingUsed)
						obstacleTornadeSmall.MovePattern2(obstacleTornadeSmall.trapSet
							, defaultPosition + new Vector2(100, 100)
							, Vector2.Zero
							, defaultPosition + new Vector2(-750, 100), 1, 5);//3
					break;
				case 1:
					if (obstacleTornadeSmall.isBeingUsed)
						obstacleTornadeSmall.MovePattern2(obstacleTornadeSmall.trapSet
							, defaultPosition + new Vector2(100, -32)
							, defaultPosition + new Vector2(-600, -32)
							, defaultPosition + new Vector2(-400, -32), 1, 5);//4
					break;
			}

			EndSkill(true, 1, type);
			/*if (endBeforeNext) {
				EndSkill(true, 1, type);

			} else if (attackCounter == timeToMove) {
				wind = true;
				isWaiting = true;
				attackCounter = 0;
			}*/
		}
		#endregion
		#region Easy
		/// <summary>
		/// 1Wayと2Wayで交互にカッター攻撃するパターン。
		/// shootPatternの違う射撃を交互に実行するのはどう実装するべきなのか....?
		/// </summary>
		/// <param name="turretNumber"></param>
		/// <param name="shootTime"></param>
		private void AttackWithCutterAlternate(int shootTime)
		{
			if (isStartingAttack) {
				isStartingAttack = false;
				cutterTurretAlternate.isBeingUsed = true;
				cutterTurretAlternate.isEnd = false;
				cutterTurretAlternate.position = this.position + cutterTurretAlternate.defaultPosition;
				cutterTurretAlternate.Inicialize();
				cutterTurretAlternate.shootNumTotal = 0;
			}

			if (cutterTurretAlternate.shootNumTotal == shootTime/*|| attackCounter > resWaitTime*/) {/*cutterTurretAlternate.isEnd*/
				isWaiting = true;
				attackCounter = 0;
				cutterTurretAlternate.isBeingUsed = false;

				isEnds[10] = true;
			}
		}
		/// <summary>
		/// 上からメテオを順に降らせて攻撃するパターン。
		/// </summary>
		private void AttackWithMeteo()
		{
			if (isStartingAttack) {
				obstacleMeteor.isBeingUsed = true;
				isStartingAttack = false;
			}

			if (obstacleMeteor.isBeingUsed) {
				obstacleMeteor.MovePattern2(obstacleMeteor.trapSet
					, defaultPosition + new Vector2(50, -50)
					, defaultPosition + new Vector2(50, -50)
					, defaultPosition + new Vector2(-400, 500), 1, 10);
			}

			if (obstacleMeteor.isEnd) {// || attackCounter > resWaitTime) {
				obstacleMeteor.isEnd = false;
				obstacleMeteor.isBeingUsed = false;
				obstacleMeteor.attackCounter = 0;
				attackCounter = 0;
				isWaiting = true;
				waitCounter = 0;

				isEnds[3] = true;
			}
		}
		private void DropIciclesInTurn()//bool endBeforeNext, int timeToMove)
		{
			if (isStartingAttack) {
				obstacleIcicleRandom.isBeingUsed = true;
				obstacleIcicleRandom.isEnd = false;
				isStartingAttack = false;
				obstacleIcicleRandom.trapSet[0].counter = 0;
			}

			if (obstacleIcicleRandom.isBeingUsed)
				obstacleIcicleRandom.MovePattern2(obstacleIcicleRandom.trapSet
					, new Vector2(defaultPosition.X - 200, 32)
					, defaultPosition + new Vector2(50, -50)
					, defaultPosition + new Vector2(-400, 400), 2, 5);

			EndSkill(true, 0, 1);
			/*if (endBeforeNext) {
				EndSkill(true, 0, 1);
				//isEnds[4] = true;
			} else if (attackCounterI == timeToMove) {// obsI.isEndの時点でobsI.trapset[0].counter == 601だが、attackCounter == 121. 1/5？ Attack()内if(wind || ice) ES()追加で到達. 今度はどちらも360だった
				//EndSkill(false);
				ice = true;        // 次のパターンに移るためにフラグを立てつつ現在使用中のWeaponに対しては何もせずUpdateを続ける
				isWaiting = true;
				attackCounter = 0;

				isEnds[11] = true;
			}*/
			attackCounterI++;
		}
		#endregion
		#region Hard
		/// <summary>
		/// つらら落とし
		/// </summary>
		/// <param name="endBeforeNext">攻撃を完全に終える前に次の攻撃に移るか</param>
		/// <param name="timeToMove">次の攻撃に移るまでの時間[frame]、当然上のパラメータがtrueじゃないと無効</param>
		private void DropIciclesRandom()//bool endBeforeNext, int timeToMove)
		{
			if (isStartingAttack) {
				obstacleIcicleRandom.isBeingUsed = true;
				obstacleIcicleRandom.isEnd = false;
				isStartingAttack = false;
			}

			if (obstacleIcicleRandom.isBeingUsed)
				obstacleIcicleRandom.MovePattern2(obstacleIcicleRandom.trapSet
					, new Vector2(defaultPosition.X - 200, 32)
					, defaultPosition + new Vector2(50, -50)
					, defaultPosition + new Vector2(-400, 400), 1, 5);

			EndSkill(true, 0, 0);
			/*if (endBeforeNext) {
				EndSkill(true, 0, 0);
				//isEnds[4] = true;
			} else if (attackCounterI == timeToMove) {// obsI.isEndの時点でobsI.trapset[0].counter == 601だが、attackCounter == 121. 1/5？ Attack()内if(wind || ice) ES()追加で到達. 今度はどちらも360だった
				//EndSkill(false);
				ice = true;        // 次のパターンに移るためにフラグを立てつつ現在使用中のWeaponに対しては何もせずUpdateを続ける
				isWaiting = true;
				attackCounter = 0;
				isEnds[4] = true;
			}*/
			attackCounterI++;
		}
		/// <summary>
		/// カッター弾を射撃して攻撃する。
		/// </summary>
		/// <param name="turretNumber">カッターを射撃するTurretのindex</param>
		/// <param name="shootTime">射撃してから攻撃終えるまでの時間？[frame]</param>
		private void AttackWithCutter(int turretNumber, int shootTime)
		{
			if (isStartingAttack) {
				turrets[turretNumber].position = this.position + new Vector2(150, 180);    // 口の辺りから射撃させたい
				foreach (Turret turret in turrets) {
					turret.isBeingUsed = true;
					turret.Inicialize();
				}
				isStartingAttack = false;
				turrets[turretNumber].shootNum = 0;
			}
			UpdateTurrets(turretNumber);
			//turrets[turretNumber].UpdateShootingOnce(20);

			if (turrets[turretNumber].shootNum >= shootTime || attackCounter > 480) {
				isWaiting = true;
				waitCounter = 0;
				attackCounter = 0;
				foreach (Turret turret in turrets) turret.isBeingUsed = false;
			}
		}
		private void AttackWithCutter5Way(int shootTime)
		{
			if (isStartingAttack) {
				isStartingAttack = false;
				cutterTurret5Way.isBeingUsed = true;
				cutterTurret5Way.isEnd = false;
				//cutterTurret5Way.position += new Vector2(150, 100);
				cutterTurret5Way.position = this.position + cutterTurret5Way.defaultPosition;
				cutterTurret5Way.Inicialize();
			}

			//if ()
			if (cutterTurret5Way.isEnd || attackCounter > resWaitTime) {
				isWaiting = true;
				waitCounter = 0;
				attackCounter = 0;
				cutterTurret5Way.isBeingUsed = false;

				isEnds[2] = true;
			}
		}
		private void AttackWithCutterBurst()
		{
			if (isStartingAttack) {
				isStartingAttack = false;
				cutterTurret1WayBurst.isBeingUsed = true;
				cutterTurret1WayBurst.isEnd = false;
				//cutterTurret1WayBurst.position += new Vector2(150, 100);
				cutterTurret1WayBurst.position = this.position + cutterTurret1WayBurst.defaultPosition;
				cutterTurret1WayBurst.Inicialize();
			}

			if (cutterTurret1WayBurst.isEnd || attackCounter > resWaitTime) {
				isWaiting = true;
				waitCounter = 0;
				attackCounter = 0;
				cutterTurret1WayBurst.isBeingUsed = false;

				isEnds[8] = true;
			}
		}

		#endregion
		#region Else
		private void AttackWithSnowBall()
		{
			// 蘇生
			if (isStartingAttack) {
				obstacleTornadeSmall.isBeingUsed = true;
				isStartingAttack = false;
				foreach (MapObjects mO in obstacleTornadeSmall.trapSet) {
					mO.HP = 2;
					mO.isAlive = true;
					mO.position = position;
				}
			}

			if (obstacleTornadeSmall.isBeingUsed) {
				obstacleTornadeSmall.MovePattern2(obstacleTornadeSmall.trapSet
					, defaultPosition + new Vector2(100, 100)
					, defaultPosition + new Vector2(-600, 100)
					, defaultPosition + new Vector2(-600, 100), 5, 8);
			}

			if (obstacleTornadeSmall.isEnd) {
				isWaiting = true;
				obstacleTornadeSmall.isEnd = obstacleTornadeSmall.isBeingUsed = false;
				obstacleTornadeSmall.attackCounter = attackCounter = obsCounter = waitCounter = 0;

				isEnds[7] = true;
			}
		}
		protected virtual void SpawnEnemy()
		{
			this.jumpingEnemy.isBeingUsed = true;
			if (!jumpingEnemy.isAlive) {
				// 蘇生
				jumpingEnemy.HP += 2;
				jumpingEnemy.isAlive = jumpingEnemy.isActive = true;
				jumpingEnemy.position = this.position;
			}

			isWaiting = true;
			isEnds[0] = true;
		}
		/// <summary>
		/// 没：羽ばたいてPlayerを押し戻す.出来れば岩などを飛ばせて妨害させる（未完）
		/// </summary>
		private void Flap()
		{
			// Stageのactiveなobjects(character?)に直接干渉させる？
			//stage.player.speed.X += -2.5f;
			if (isStartingAttack) {
				isAttacking = true;
			}

			foreach (Character chr in stage.activeCharacters) {
				if (!(chr is Fuujin)) chr.speed.X += -2.5f * timeCoef;
			}

			if (attackCounter > resWaitTime) {
				attackCounter = 0;
				isWaiting = true;
				isEndingAttack = true;
			}
			// 挙動としてはこれでよい.が、speedに反映されないのでBLockをすり抜ける.
			// 解決法としては恐らく、最終的なspeedの前段階として、Vector2[] speedsなどとしてそこに反映するのがいいだろう？だとしてもこれは後回しだな...
		}
		#endregion

		// コンストラクタ
		public Fuujin(Stage stage, float x, float y, int width, int height, int HP, float vx, int bulletType, int shootType)
			: base(stage, x, y, width, height, HP, bulletType, shootType) 
		{
			this.defaultPosition = position;
			tmpPosition = defaultPosition + new Vector2(-100, -200);
			attackPosition = new Vector2(defaultPosition.X - 500, defaultPosition.Y);
			animation = new Animation(380, 310);
			resWaitTime = 240;
			//animation2 = new Animation(210, 210);

			obstacles = new List<Obstacle>();
			obstacleMeteor = new Obstacle(stage, this, x, y, 32, 64, 2, 0);
			obstacleIcicleRandom = new Obstacle(stage, this, x, y, 32, 64, 2, 1);
			//obstacleIcicleAlternate = new Obstacle(stage, this, x, y, 32, 64, 2, 1);
			obstacleTornadeSmall = new Obstacle(stage, this, x, y, 32, 32, 2, 3);
			obstacleTornadeLarge = new Obstacle(stage, this, x, y, 32, 32, 2, 4);

			cutterTurret5Way = new Turret(stage, this, new Vector2(150, 100), 32, 32, 0, 5, 5, false, true, false, 3, 0, 4);
			cutterTurret1WayBurst = new Turret(stage, this, new Vector2(150, 100), 32, 32, 0, 1, 10, false, true, false, 3, 0, 4, 10, 320, 10);
			cutterTurretAlternate = new Turret(stage, this, new Vector2(150, 100), 32, 32, 0, 5, 2, false, true, false, 3, 0, 4, 5, 60, 10, Vector2.Zero, false, true, new int[] { 5, 1 });
			// shootInterval == 120だと、連発した時にPlayerを最初しか上手く追尾しない?

			obstacles.Add(obstacleMeteor);
			obstacles.Add(obstacleIcicleRandom);
			obstacles.Add(obstacleTornadeSmall);
			foreach (Obstacle obs in obstacles) weapons.Add(obs);
			turrets.Add(cutterTurret5Way);
			weapons.Add(cutterTurret5Way);
			turrets.Add(cutterTurret1WayBurst);
			weapons.Add(cutterTurret1WayBurst);
			turrets.Add(cutterTurretAlternate);
			weapons.Add(cutterTurretAlternate);

			largeWind = new Weapon(stage, x, y, 64, 400, this);
			weapons.Add(largeWind);

			jumpingEnemy = new JumpingEnemy(stage, position.X, position.Y, 32, 32, 2, this);
			stage.characters.Add(jumpingEnemy);

			foreach (Weapon weapon in weapons) stage.weapons.Add(weapon);
			
		}

		// overrideメソッド
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\Fuujin");

			isWaiting = true;
			attackList = new List<int>();
			for (int i = 0; i < 12; i++) isEnds.Add(false);
			attackPatternNumList.Add(attackPattern0);
			attackPatternNumList.Add(attackPattern1);
			if (game.isHighLvl) attackPatternNum = 0;
			else attackPatternNum = 1;
			//attackList.Add(0);
			attackList.Add(attackPatternNumList[attackPatternNum][0]);

			// delegateを使って書き換えられるかテスト
			attackMethods.Add(attackMethodType0 = SpawnEnemy);
			attackMethods.Add(attackMethodType1 = Tackle);
			attackMethods.Add(attackMethodType2 = AttackWithCutter5Way);
			attackMethods.Add(attackMethodType0 = AttackWithMeteo);
			attackMethods.Add(attackMethodType0 = DropIciclesRandom);
			attackMethods.Add(attackMethodType2 = AttackWithWind);
			attackMethods.Add(attackMethodType2 = AttackWithWind);
			attackMethods.Add(attackMethodType0 = AttackWithSnowBall);
			attackMethods.Add(attackMethodType0 = AttackWithCutterBurst);
			attackMethods.Add(attackMethodType0 = Flap);
			attackMethods.Add(attackMethodType2 = AttackWithCutterAlternate);
			attackMethods.Add(attackMethodType0 = DropIciclesInTurn);

			//Object[] args = new objec;
			//object o = null;
			//object[] x = null;
			attackMethodsArgs.Add(null);//(Object)null
			attackMethodsArgs.Add(5);
			attackMethodsArgs.Add(2);
			attackMethodsArgs.Add(null);//typeof(void) new object[] { null }
			attackMethodsArgs.Add(null);//new object[] { }
			attackMethodsArgs.Add(0);
			attackMethodsArgs.Add(1);
			attackMethodsArgs.Add(null);
			attackMethodsArgs.Add(null);
			attackMethodsArgs.Add(null);
			attackMethodsArgs.Add(5);
			attackMethodsArgs.Add(null);
		}
		public override void Update()
		{
			if (IsBeingUsed()) {
				Attack(30);
				base.Update();
			}
		}
		/// <summary>
		/// 攻撃中は口を開けるアニメーション
		/// </summary>
		public override void UpdateAnimation()
		{
			if (!isAttacking) {
				animation.Update(3, 0, 380, 310, 12, 1);
			} else {
				if (!usingWindB) animation.Update(3, 1, 380, 310, 10, 1);
				else animation.Update(3, 1, 380, 310, 8, 1);
			}
		}
		protected override void UpdateNumbers()
		{
			// 加減速
			speed.Y += (float)Gravity * timeCoef;
			if (speed.X > 0) {
				speed.X += -(.40f * friction) * timeCoef;
				if (speed.X < 0) speed.X = 0;
			} else if (speed.X < 0) {
				speed.X += (.40f * friction) * timeCoef;
				if (speed.X > 0) speed.X = 0;
			}
			if (System.Math.Abs(speed.Y) > maxSpeed) speed.Y = maxSpeed;

			// 位置に加算
			position += speed * timeCoef;
			// 端
			//if (position.Y < 0) position.Y = 0;
			// 軌跡をListに追加
			locus.Add(this.drawPos);
			if (locus.Count > 2) locus.RemoveAt(0);

			// shootPositionのUpdate
			for (int i = 0; i < shootPosition.Count; i++) {
				shootPosition[i] += new Vector2((float)this.scalarSpeed, 0);
			}
		}
		/// <summary>
		/// 攻撃のメインメソッド。待機状態→攻撃状態を繰り返す。 // これはoverrideさせた方が良いな
		/// </summary>
		/// <param name="waitTime">待機状態にいる時の時間[frame]</param>
		protected override void Attack(int waitTime)
		{
			// isWaitingも状態（パターン）の１つにしてもっと抽象化すべきか？
			if (isWaiting) {
				if (waitCounter > waitTime) {
					isWaiting = false;
					isStartingAttack = true;
					isAttacking = false;
					isEndingAttack = false;
					waitCounter = 0;
					attackCounter = 0;
					/*switch (attackPatternNum) {
						case 0:
							attackList.Add(attackPattern0[attackNum]);// 既にリムってる？
							break;
						case 1:
							attackList.Add(attackPattern1[attackNum]);
							break;
					}*/
					attackList.Add(attackPatternNumList[attackPatternNum][attackNum]);
					attackNum++;
					if (attackNum == attackPatternNumList[attackPatternNum].Length) attackNum = 0;
					//if (attackNum == attackPattern0.Length - 1) attackNum = 1;
					//if (attackNum == 3) attackNum = 4;
					//if (attackNum >= 9) attackNum = 1;

					//AttackControl0();// 機能してない　isEnds[i] = falseにすぐしてしまうから...
				}
				waitCounter++;
			} else {// 攻撃開始
				if (attackCounter == 0) {// 攻撃状態に切り替わった瞬間は初期化する
					isStartingAttack = true;
					counter = 0;
					//attackList.Add(2);
					//AttackControl0();
				}

				UpdateAttacking();
				attackCounter++;

				// ↓この辺を一般化したいでござる
				/*if (wind) {
					AttackWithWind(1, false, 0);
					EndSkill(true, 1);
				}
				if (ice) {
					DropIciclesRandom(false, 0);
					EndSkill(false, 0);
				}*/
			}
		}
		/// <summary>
		/// 攻撃パターンintをListに追加→isEnd[i] = trueになるまでそのメソッドをUpdateして攻撃
		/// これで複数同時攻撃・同時移動をやや一般的に実行可能
		/// </summary>
		protected override void UpdateAttacking()
		{
			//foreach (int i in attackList) {
			for (int i = attackList.Count - 1; i >= 0; i--) {// ++だとremoveした時に前の要素がスルーされちゃう

				// 綺麗(?)に記述したかっただけで、実際やっていることは下のswitch文と同じ
				// nullも1つの引数扱いになるので一行で書くのは無理
				//attackMethods[attackList[i]].DynamicInvoke(attackMethodsArgs[attackList[i]]);
				if (attackMethodsArgs[attackList[i]] == null) {
					attackMethods[attackList[i]].DynamicInvoke(); 
				} else {
					attackMethods[attackList[i]].DynamicInvoke(attackMethodsArgs[attackList[i]]);
				}/**/
				/*switch (attackList[i]) {
					case 0: SpawnEnemy(); break;
					case 1: Tackle(5); break;
					case 2: AttackWithCutter5Way(2); break;
					case 3: AttackWithMeteo(); break;
					case 4: DropIciclesRandom(); break;//DropIciclesRandom(false, 600 - 240);
					case 5: AttackWithWind(0); break;
					case 6: AttackWithWind(1); break;
					case 7: AttackWithSnowBall(); break;
					case 8: AttackWithCutterBurst(); break;
					case 9: Flap(); break;
					case 10: AttackWithCutterAlternate(5); break;
					case 11: DropIciclesInTurn(); break;
				}*/
				
				if (isEnds[attackList[i]]) {
					//if (attackList[i] == attackNum - 1) attackList.Add(attackNum);// attackNumが進みすぎてる 2:4 ∴ここに来る前にisWaitingに入ってしまう
					isEnds[attackList[i]] = false;
					attackList.Remove(attackList[i]);
				}
			}
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			if (base.IsBeingUsed()) {//isAlive && isActive
				//spriteBatch.Draw(texture2, drawPos, animation2.rect, Color.White);     // 判定用の矩形の描画
				spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);
				DrawComboCount(spriteBatch);
			}
		}
	}
}
