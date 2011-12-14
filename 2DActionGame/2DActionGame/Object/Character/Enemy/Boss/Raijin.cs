using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
	/// <summary>
	/// 今のところ、基本右端にいて、直接攻撃時は任意の地点まで移動するイメージ。
	/// 敵を出したり攻撃オブジェクトを出す場合はあまり動く必要は無いだろう
	/// </summary>
	public class Raijin : Boss
	{
		#region Member variable
		private float distanceD, distanceT, distanceA;
		private Vector2 tmpPosition;
		public bool isUsingTurret { get; set; }

		// Weapons
		private Weapon singleLightning;
		private DivisionWeapon dividingLightning;
		/// <summary>
		/// 稲妻で地面に向けて攻撃する子機 
		/// </summary>
		private Turret ballTurret;
		/// <summary>
		/// 自機狙いthunder用
		/// </summary>
		private Turret thunderTurret, thunderTurret2;
		/// <summary>
		/// 真下に否妻
		/// </summary>
		private Turret beamTurret;
		private JumpingEnemy jumpingEnemy;

		private Obstacle obstacle;
		private List<Obstacle> obstacles = new List<Obstacle>();
		/// <summary>
		/// 一度の攻撃で出現させる障害地形の数.種類とかについては後で決めよう
		/// </summary>
		private int obstacleNum;

		private bool hasPlayedSoundEffectSub;
		//private bool /*hasPut,*/ hasMoved;
		private Turret thunderTurret3;
		private Turret thunderTurret8Way;
		private List<Turret> thunderTurrets3Way = new List<Turret>();
		private List<Turret> thunderTurrets = new List<Turret>();
		private SoundEffect thunderSoundBig, thunderSoundSmall, spawnSound;

		
		#endregion
		private int[] attackPattern0 = { 0, 2, 3, 4, 5 };// XML化予定
		private int[] attackPattern1 = { 7, 8, 4, 9, 1 };

		#region OldPattern(Old_Easy)
		private void MoveLite(Vector2 startPos, Vector2 wayPos, Vector2 endPos, bool endAttack)
		{
			Vector2 returnVector, wayVector, attackVector;
			// 攻撃終了地点からデフォルト位置に引くベクトル
			returnVector = startPos - position;
			wayVector = wayPos - position;
			attackVector = endPos - position;
			// 単位ベクトル化(正規化)
			Vector2 baseVector = Vector2.Normalize(returnVector);
			Vector2 baseVectorT = Vector2.Normalize(wayVector);
			Vector2 baseVectorA = Vector2.Normalize(attackVector);
			baseVector *= new Vector2(5);
			baseVectorT *= new Vector2(5);
			baseVectorA *= new Vector2(5);
			distanceD = Vector2.Distance(position, startPos);
			distanceT = Vector2.Distance(position, wayPos);
			distanceA = Vector2.Distance(position, endPos);

			if (isStartingAttack) {
				speed = baseVectorT;
			}
			if (isStartingAttack && distanceT < 5) {
				counter = 0;
				isAttacking = true;
				isStartingAttack = false;
			}

			if (isAttacking) {
				speed = baseVectorA;
			}
			if (isAttacking && distanceA < 5) {
				isAttacking = false;
				isEndingAttack = true;
			}

			if (isEndingAttack /*&& distanceD < 5*/ || counter > 240) {
				hasPlayedSoundEffect = false;
				hasMoved = true;
				speed = Vector2.Zero;
				if (endAttack) {
					isEndingAttack = false;
					isAttacking = false;
					isWaiting = true;
					attackCounter = 0;
					counter = 0;

					isEnds[5] = true;
				}
			}
			counter++;
		}
		private void AttackWithBeam(Vector2 startPos, Vector2 wayPos, Vector2 attackPos, bool willReturn)// 8
		{
			Vector2 returnVector, wayVector, attackVector;

			// 攻撃終了地点からデフォルト位置に引くベクトル
			returnVector = startPos - position;
			wayVector = wayPos - position;
			attackVector = attackPos - position;
			// 単位ベクトル化(正規化)
			Vector2 baseVector = Vector2.Normalize(returnVector);
			Vector2 baseVectorT = Vector2.Normalize(wayVector);
			Vector2 baseVectorA = Vector2.Normalize(attackVector);
			//Vector2.Multiply(baseVector, 3); // これだと上手く乗算されてないようだ
			baseVector *= new Vector2(5);
			baseVectorT *= new Vector2(5);
			baseVectorA *= new Vector2(5);
			distanceD = Vector2.Distance(position, startPos);
			distanceT = Vector2.Distance(position, wayPos);
			distanceA = Vector2.Distance(position, attackPos);

			if (isStartingAttack) {
				beamTurret.isBeingUsed = true;
				beamTurret.Inicialize();
				speed = baseVectorT;
				hasPlayedSoundEffect = false;
				attackPos = new Vector2(startPos.X - 480, startPos.Y - 50);
			}
			if (isStartingAttack && distanceT < 5) {// 攻撃位置まで来たらフラグをたてて攻撃開始.
				counter = 0;
				isStartingAttack = false;
				isAttacking = true;
			}
			if (isAttacking) {
				speed = baseVectorA;
			}
			if (isStartingAttack || isAttacking || isEndingAttack) {
				if (!hasPlayedSoundEffect) {
					if (counter % 8 == 0) {
						if (!game.isMuted) thunderSoundBig.Play(SoundControl.volumeAll, 0f, 0f);
					}
				}
			}

			if (isAttacking && distanceA < 5) {
				isAttacking = false;
				isEndingAttack = true;
				willReturn = true;
			}
			if (willReturn) {
				if (isEndingAttack) speed = baseVector;
				//if (isEndingAttack && distanceD < 5) isEndingAttack = false;
				if (isEndingAttack && distanceD < 5) {
					speed.X = 0; speed.Y = 0;
					beamTurret.isBeingUsed = false;
					isEndingAttack = false;
					isAttacking = false;
					isWaiting = true;
					hasPlayedSoundEffect = false;
					attackCounter = 0;
					counter = 0;
				}
			} else if (isEndingAttack) {
				speed.X = 0; speed.Y = 0;
				beamTurret.isBeingUsed = false;
				isEndingAttack = false;
				isAttacking = false;
				isWaiting = true;
				hasPlayedSoundEffect = false;
				attackCounter = 0;
				counter = 0;

				isEnds[8] = true;
			}

			counter++;
		}
		/// <summary>
		/// マルクのアレみたいな攻撃
		/// </summary>
		private void AttackDivision()
		{
			// 攻撃終了地点からデフォルト位置へベクトルを引く
			Vector2 returnVector = defaultPosition - position;
			Vector2 baseVectorD = Vector2.Normalize(returnVector);
			//Vector2.Multiply(baseVectorD, 16);
			baseVectorD *= new Vector2(3);
			// 多分ベクトルで移動するなら使わない
			distanceD = Vector2.Distance(position, defaultPosition);
			// 攻撃位置へのベクトル
			attackPosition = new Vector2(defaultPosition.X - 300, defaultPosition.Y - 50);
			Vector2 attackVector = attackPosition - position;
			Vector2 baseVectorA = Vector2.Normalize(attackVector);
			//Vector2.Multiply(baseVectorA, 16);
			baseVectorA *= new Vector2(3);
			distanceA = Vector2.Distance(position, attackPosition);

			if (isStartingAttack) {// 攻撃位置へ
				speed = baseVectorA;
			}
			if (isStartingAttack && /*position == attackPosition*/distanceA < 5) {// 攻撃位置へ来たら
				speed = Vector2.Zero;
				isAttacking = true;
				isStartingAttack = false;
				dividingLightning.isBeingUsed = true;
				dividingLightning.isDivided = false;// あとで直す
				dividingLightning.isEnd = false;
			}
			if (isAttacking) {
				dividingLightning.MovePattern1();
				speed = Vector2.Zero;// 書かないと落ちる
				if (!hasPlayedSoundEffectSub) {
					if (!game.isMuted) thunderSoundSmall.Play(SoundControl.volumeAll, 0f, 0f);
					hasPlayedSoundEffectSub = true;
				}
			}
			if (isAttacking && dividingLightning.isEnd/*&& dividingLightning.counter > 60*//*position.X < startPosition.X - 300*/) {
				//isStartingAttack = false;
				isAttacking = false;
				dividingLightning.isBeingUsed = false;
				isEndingAttack = true;
			}
			if (isEndingAttack && distanceD > 5) {
				speed = baseVectorD;
			} else if (isEndingAttack && distanceD < 5) {
				speed = Vector2.Zero;
				isEndingAttack = false;
				isAttacking = false;
				hasPlayedSoundEffect = false;
				isWaiting = true;
				attackCounter = 0;
			}
			counter++;
		}
		/// <summary>
		/// No movement ver.
		/// </summary>
		private void Division()
		{
			if (isStartingAttack) {
				speed = Vector2.Zero;
				isAttacking = true;
				isStartingAttack = false;

				dividingLightning.isBeingUsed = true;
				dividingLightning.isDivided = false;// あとで直す
				dividingLightning.isEnd = false;
			}
			if (isAttacking) {
				dividingLightning.MovePattern1();
				speed = Vector2.Zero;// 書かないと落ちる
				if (!hasPlayedSoundEffectSub) {
					if (!game.isMuted) thunderSoundSmall.Play(SoundControl.volumeAll, 0f, 0f);
					hasPlayedSoundEffectSub = true;
				}
			}
			if (isAttacking && dividingLightning.isEnd) {
				isAttacking = false;
				dividingLightning.isBeingUsed = false;
				isEndingAttack = true;
			}
			counter++;

			if (isEndingAttack) {
				speed = Vector2.Zero;
				isStartingAttack = false;
				isEndingAttack = false;
				isAttacking = false;
				hasPlayedSoundEffect = false;
				isWaiting = true;
				attackCounter = 0;
				counter = 0;

				isEnds[4] = true;
			}
		}
		private void AttackPattern3()
		{
			// lightningBall的な何かturretに任せる
			UpdateTurrets(-1);
		}
		private void AttackWithThunder()
		{
			// Kirby2のダークマターが出すアレのイメージ、直線的なもの.(稲妻の演出はEffectやアニメーションでやる)
			if (isStartingAttack) {
				isStartingAttack = false;
				thunderTurret.isBeingUsed = true;
				thunderTurret.isEnd = false;
				thunderTurret2.isBeingUsed = true;
				thunderTurret2.isEnd = false;
				thunderTurret.Inicialize();
				thunderTurret2.Inicialize();

				thunderTurret2.position = thunderTurret.position + new Vector2(-300, 0);
			}
			thunderTurret.Update();
			if (counter > 20) thunderTurret2.Update();

			if (thunderTurret2.isEnd || counter > 180) {
				thunderTurret.isBeingUsed = false;
				thunderTurret2.isBeingUsed = false;
				isAttacking = false;
				isWaiting = true;
				attackCounter = 0;
				counter = 0;

				isEnds[1] = true;
			}
			counter++;
		}
		private void AttackWithThunder2()
		{
			if (isStartingAttack) {
				isStartingAttack = false;
				thunderTurret8Way.isBeingUsed = true;
				thunderTurret8Way.isEnd = false;
				thunderTurret8Way.Inicialize();
			}

			if (thunderTurret8Way.isEnd || counter > 1500) {
				thunderTurret8Way.isBeingUsed = false;
				isAttacking = false;
				isWaiting = true;
				attackCounter = 0;
				counter = 0;

				isEnds[2] = true;
			}
			counter++;
		}
		protected override void Move(float moveSpeed, Vector2[] wayPoints)//params 
		{
			base.Move(moveSpeed, wayPoints);

			//if (isStartingAttack) height = (int)bindSize.Y;

			if (hasMoved) {
				//height = texture.Height;
				isEnds[3] = true;
				isWaiting = true;
			}
		}
		protected virtual void SpawnEnemy()
		{
			this.jumpingEnemy.isBeingUsed = true;

			if (!jumpingEnemy.isAlive) {// 蘇生する
				jumpingEnemy.HP = 2;
				jumpingEnemy.isAlive = true;
				jumpingEnemy.isActive = true;
				jumpingEnemy.position = this.position;
			}
			if (!game.isMuted) spawnSound.Play(SoundControl.volumeAll, 0f, 0f);
			attackCounter = 0;
			isWaiting = true;

			isEnds[0] = true;
		}
		protected virtual void UseObstacle()
		{
			// index順に流し、最後の要素で終了フラグがたったら攻撃を終了させる
			if (obstacles[obstacles.Count - 1].isEnd || obsCounter > 800) {  // バグを想定して時間で終了するようにしておく
				foreach (Obstacle obs in obstacles) {
					obs.isEnd = false;
					obs.isBeingUsed = false;
					obs.attackCounter = 0;
				}
				attackCounter = 0;
				obsCounter = 0;
				isWaiting = true;
			}

			ControlObstacles(obstacles, 60);
		}
		private void SpawnATurret()
		{
			Vector2 returnVector = defaultPosition - position;
			Vector2 baseVectorD = Vector2.Normalize(returnVector);
			baseVectorD *= new Vector2(3, 3);
			distanceD = Vector2.Distance(position, defaultPosition);

			// Spawn位置へのベクトル
			attackPosition = new Vector2(defaultPosition.X - 300, defaultPosition.Y);
			Vector2 attackVector = attackPosition - position;
			Vector2 baseVectorA = Vector2.Normalize(attackVector);
			//Vector2.Multiply(baseVectorA, 16);
			baseVectorA *= new Vector2(3, 3);

			distanceA = Vector2.Distance(position, attackPosition);

			if (isStartingAttack) speed = baseVectorA;   // Spawn位置へ
			if (isStartingAttack && distanceA < 5) {     // Spawn位置へ北ら
				speed = Vector2.Zero;
				isAttacking = true;
				isStartingAttack = false;

				ballTurret.isBeingUsed = true;
				isUsingTurret = true;
				// 蘇生
				if (!ballTurret.isAlive) {
					ballTurret.position = position;
					ballTurret.HP += 3;
					ballTurret.isAlive = true;
				}
			}
			if (isAttacking) {
				speed = Vector2.Zero;                   // 書かないと落ちる
			}
			if (isAttacking && dividingLightning.isEnd) {
				isAttacking = false;
				isEndingAttack = true;
			}
			if (isEndingAttack && distanceD > 5) {
				speed = baseVectorD;
			} else if (isEndingAttack && distanceD < 5) {
				speed = Vector2.Zero;
				isEndingAttack = false;
				isAttacking = false;
				isWaiting = true;
				attackCounter = 0;

				isEnds[6] = true;
				//counter = 0;
			}
			counter++;

		}
		private void ControlTurret(Turret targetTurret, int type, float speedX)
		{
			targetTurret.isBeingUsed = true;
			switch (type) {
				case 0:
					//targetTurret.RoundTripMotion(defaultPosition - new Vector2(200, 0), 200, speedX);
					break;
				case 1:
					targetTurret.speed = new Vector2(-speedX, 0);
					targetTurret.position += targetTurret.speed;
					break;
				//UpdateTurrets(3);
			}

			if (!targetTurret.isAlive) {
				targetTurret.isBeingUsed = false;
				isUsingTurret = false;
			}
		}
		/// <summary>
		/// 障害物の移動処理をObstacleに指示
		/// </summary>
		/// <param name="obstacles"></param>
		protected virtual void ControlObstacles(List<Obstacle> obstacles, int spwanInterval)
		{
			obsCounter++;

			for (int i = 0; i < obstacles.Count; i++) {
				if (obsCounter % (spwanInterval * (i + 1)) == 0)
					obstacles[i].isBeingUsed = true;
			}
			foreach (Obstacle obs in obstacles) {
				if (obs.isBeingUsed) {
					obs.MovePattern1(obstacle.trapSet
						, new Vector2(defaultPosition.X - 32, stage.CheckGroundHeight(defaultPosition.X) + obs.height)
						, new Vector2(defaultPosition.X - 32, stage.CheckGroundHeight(defaultPosition.X) - obs.height)
						, new Vector2(defaultPosition.X - 800, stage.CheckGroundHeight(defaultPosition.X) - obs.height)
						, new Vector2(0, -12), new Vector2(-2.5f, 0));
				}
			}
		}
		#endregion
		#region New_Hard
		private void UseTurrets()
		{
			if (isStartingAttack) {
				foreach (Turret tur in thunderTurrets)
					tur.isVisible = true;
				isStartingAttack = false;
				attackCounter++;
			}
			//if (thunderTurrets[0].isBeingUsed && thunderTurrets[1].isBeingUsed && thunderTurrets[2].isBeingUsed) { }// 全部isBeingUsedなのにthunderが出ない
			//if (thunderTurrets[0].shootNumTotal) {}

			if (attackCounter % 40 == 0) { }
			for (int i = 0; i < thunderTurrets.Count; i++) {
				if (attackCounter % (40 * (i + 1)) == 0) {
					thunderTurrets[i].Inicialize();
					thunderTurrets[i].isBeingUsed = true;
				}
			}
			foreach (Turret tur in thunderTurrets) {
				if (tur.isBeingUsed)
					ControlTurret(tur, 1, 8);
			}

			if (attackCounter > 480) isEndingAttack = true;
			if (isEndingAttack) {
				foreach (Turret tur in thunderTurrets) {
					tur.isVisible = false;
					tur.isBeingUsed = false;
					tur.position = defaultPosition;
				}
				isStartingAttack = false;
				isEndingAttack = false;
				isAttacking = false;
				isWaiting = true;
				attackCounter = 0;

				isEnds[7] = true;
			}
		}
		private bool hasInicialized;
		private void PutTurrets(Vector2[] vecs)
		{
			if (isStartingAttack) {
				foreach (Turret tur in thunderTurrets3Way) {
					tur.Inicialize();
					tur.isEnd = false;
				}
			}
			if (!hasMoved) {
				MoveLite(
					new Vector2(defaultPosition.X - 350, defaultPosition.Y - 150)
					, new Vector2(defaultPosition.X - 200, defaultPosition.Y - 100)
					, defaultPosition
					, false
				);
			}

			attackCounter++;
			for (int i = 0; i < thunderTurrets3Way.Count; i++) {
				if (attackCounter % (40 * (i + 1)) == 0) {
					thunderTurrets3Way[i].isVisible = true;
					if (i < thunderTurrets3Way.Count) thunderTurrets3Way[i].position = vecs[i];
				}
			}

			if (!hasInicialized && hasMoved && !thunderTurret.isEnd) {
				foreach (Turret tur in thunderTurrets3Way) {
					tur.isBeingUsed = true;
					//tur.bulletCounter = 0;  // 何故かおかしい
					//tur.Create();
					tur.isVisible = true;
				}
				//hasMoved = false;
				hasInicialized = true;
			}

			if (thunderTurret.isEnd || attackCounter > 720) {
				foreach (Turret tur in thunderTurrets3Way) {
					tur.isBeingUsed = false;
					tur.isVisible = false;
					tur.isEnd = false;
				}
				hasMoved = false;
				thunderTurret.isEnd = false;
				attackCounter = 0;
				counter = 0;
				isWaiting = true;
				isStartingAttack = false;
				isEndingAttack = false;
				isAttacking = false;
				hasInicialized = false;

				isEnds[9] = true;
			}
			attackCounter++;
		}
		#endregion

		public Raijin(Stage stage, float x, float y, int width, int height, int HP)
			: base(stage, x, y, width, height, HP)
		{
			this.defaultPosition = position;

			bindPos = new Vector2(100, 0);
			tmpPosition = defaultPosition + new Vector2(-150, -50);
			attackPosition = new Vector2(defaultPosition.X - 640, defaultPosition.Y - 50);
			singleLightning = new Weapon(stage, position.X, position.Y, 16, 200, this);					// アニメーションの関係でWeaponじゃ処理しきれないのでBeamに移行
			dividingLightning = new DivisionWeapon(stage, position.X, position.Y, 32, 32, this, 1.0f);
			obstacle = new Obstacle(stage, this, x, y, 32, 64, 0);
			ballTurret = new Turret(stage, this, position, 32, 32, 0, 0, 1, false, false, 0, 1);		// 他目的の雲object生成  , , 0
			ballTurret.bulletSpeed = new Vector2(0, 8);

			thunderTurret = new Turret(stage, this, this.position, 64, 48, 2, 0, 3, false, false, 3, 3, 14, 120, 10);//, , 3...2
			thunderTurret.isVisible = true;
			thunderTurret2 = new Turret(stage, this, this.position, 64, 48, 2, 0, 3, false, false, 3, 3, 14, 120, 10);//, , 3...2
			thunderTurret3 = new Turret(stage, this, this.position, 64, 48, 2, 3, 3, false, false, 3, 3, 14, 120, 10);//, , 3...2
            thunderTurret.isVisible = true; thunderTurret2.isVisible = true; thunderTurret3.isVisible = true;

			beamTurret = new Turret(stage, this, new Vector2(width / 2 - 16, height - 32), 32, 32, 1, 0, 1, true);//, , 2 //new Vector2(width / 2 + 32, height)
			thunderTurret8Way = new Turret(stage, this, this.position, 64, 48, 2, 3, 8, false, false, 3, 3, 14, 360, 30);//...3
            thunderTurret8Way.isVisible = true;

			for (int i = 0; i < 10; i++) {// 10
				thunderTurrets.Add(new Turret(stage, this, this.position, 64, 48, 2, 1, 5, false, false, 3, 3, 14, 240, 40));//...1 // 240
                thunderTurrets[i].isVisible = true;
            }
			thunderTurrets3Way.Add(thunderTurret);
			thunderTurrets3Way.Add(thunderTurret2);
			thunderTurrets3Way.Add(thunderTurret3);
			turrets.Add(thunderTurret8Way);

			// Add them to turrets
			turrets.Add(ballTurret);
			turrets.Add(beamTurret);
			foreach (Turret tur in thunderTurrets) {
				turrets.Add(tur);
			}
			foreach (Turret tur in thunderTurrets3Way) {
				turrets.Add(tur);
			}

			obstacleNum = 10;
			for (int i = 0; i < obstacleNum; i++) {
				obstacles.Add(new Obstacle(stage, this, x, y, 32, 64, 0));//OdueOPSNOa
			}

			// Add them to this.weapons
			weapons.Add(singleLightning);
			weapons.Add(dividingLightning);
			weapons.Add(obstacle);
			foreach (Turret tur in turrets) weapons.Add(tur);
			foreach (Obstacle obs in obstacles) weapons.Add(obs);

			// Add weapons to stage.weapons
			foreach (Weapon weapon in weapons) stage.weapons.Add(weapon);
			foreach (Weapon weapon in weapons) stage.objects.Add(weapon);

			jumpingEnemy = new JumpingEnemy(stage, position.X, position.Y, 32, 32, 2, this);
			stage.characters.Add(jumpingEnemy);

			Initialize();
		}
		protected override void Load()
		{
			base.Load();

			texture = content.Load<Texture2D>("Object\\Character\\Raijin");
			thunderSoundBig = content.Load<SoundEffect>("Audio\\SE\\thunder_big");
			thunderSoundSmall = content.Load<SoundEffect>("Audio\\SE\\thunder_small");
			spawnSound = content.Load<SoundEffect>("Audio\\SE\\zako_tama");
		}
		protected override void Initialize()
		{
			base.Initialize();

			isWaiting = true;
			attackList = new List<int>();
			for (int i = 0; i < 12; i++) isEnds.Add(false);
			attackPatternNumList.Add(attackPattern0);
			attackPatternNumList.Add(attackPattern1);
			if (!game.isHighLvl) attackPatternNum = 0;
			else attackPatternNum = 1;
			//attackList.Add(attackPatternNumList[attackPatternNum][0]);

			attackMethods.Add(attackMethodType0 = SpawnEnemy);
			attackMethods.Add(attackMethodType0 = AttackWithThunder);
			attackMethods.Add(attackMethodType0 = AttackWithThunder2);
			attackMethods.Add(attackMethodType3 = Move);// 3
			attackMethods.Add(attackMethodType0 = Division);
			attackMethods.Add(attackMethodType4 = MoveLite);//5
			attackMethods.Add(attackMethodType0 = SpawnATurret);
			attackMethods.Add(attackMethodType0 = UseTurrets);//7
			attackMethods.Add(attackMethodType4 = AttackWithBeam);//8
			//attackMethods.Add(attackMethodType0 = Division);
			attackMethods.Add(attackMethodType5 = PutTurrets);
			//attackMethods.Add(attackMethodType0 = DropIciclesInTurn);

			attackMethodsArgs.Add(null);
			attackMethodsArgs.Add(null);
			attackMethodsArgs.Add(null);
			attackMethodsArgs.Add(new object[] { 5,  new Vector2[] { defaultPosition//14000
							, new Vector2(defaultPosition.X - 100, defaultPosition.Y - 100)
							, new Vector2(defaultPosition.X - 360, defaultPosition.Y - 200)
							, new Vector2(defaultPosition.X - 720, defaultPosition.Y - 200)}});
			/*new Vector2[] { defaultPosition
							, new Vector2(defaultPosition.X, defaultPosition.Y + 100)
							, new Vector2(defaultPosition.X - 240, defaultPosition.Y - 150)
							, new Vector2(defaultPosition.X - 480, defaultPosition.Y - 150)}*/
			attackMethodsArgs.Add(null);
			attackMethodsArgs.Add(new object[] { new Vector2(defaultPosition.X - 350, defaultPosition.Y - 150)
							, new Vector2(defaultPosition.X - 200, defaultPosition.Y - 100)
							, defaultPosition
							, true });
			attackMethodsArgs.Add(null);
			attackMethodsArgs.Add(null);
			attackMethodsArgs.Add(new object[] { defaultPosition
							 , new Vector2(defaultPosition.X - 100, defaultPosition.Y - 50)
							 , new Vector2(defaultPosition.X - 720, defaultPosition.Y - 200)
							 , false });
			/* defaultPosition
							 , new Vector2(defaultPosition.X, defaultPosition.Y + 100)
							 , new Vector2(defaultPosition.X - 480, defaultPosition.Y - 150)*/
			//attackMethodsArgs.Add(null);
			attackMethodsArgs.Add(new object[] { new Vector2[] {
                             new Vector2(defaultPosition.X - 350, defaultPosition.Y - 50)
                             , new Vector2(defaultPosition.X - 200, defaultPosition.Y - 0)
                             , new Vector2(defaultPosition.X - 100, defaultPosition.Y + 50)} });
			//attackMethodsArgs.Add(null);
		}
		protected override void Attack(int waitTime)
		{
			if (isWaiting) {
				if (waitCounter > waitTime) {
					isWaiting = false;
					isStartingAttack = true;
					isAttacking = isEndingAttack = false;
					waitCounter = attackCounter = counter = 0;

					attackList.Add(attackPatternNumList[attackPatternNum][attackNum]);
					attackNum++;
					if (attackNum == attackPatternNumList[attackPatternNum].Length) attackNum = 0;
				}
				waitCounter++;
			} else {
				UpdateAttacking();
				attackCounter++;

				if (isUsingTurret) ControlTurret(ballTurret, 0, 3);
			}
		}
		protected override void MotionDelay()
		{
		}
		public override void Update()
		{
			if (IsActive()) {//&& stage.inBossBattle
				Attack(30);
				base.Update();
			}
		}
		public override void UpdateAnimation()
		{
			animation.Update(2, 0, 300, 280, 12, 1);
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			if (IsBeingUsed() && IsActive()) {
				if (game.inDebugMode && game.stageNum != 3) spriteBatch.Draw(bind.texture, new Rectangle((int)bind.drawPos.X, (int)bind.drawPos.Y, (int)bindSize.X, (int)bindSize.Y)
					, new Rectangle(0, 0, (int)bindSize.X, (int)bindSize.Y), Color.White);
			}
		}
	}
}
