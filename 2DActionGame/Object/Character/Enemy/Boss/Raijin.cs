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
		#region Numbers
		/*private int attackNum; // 攻撃の種類:現在の
        private Vector2 defaultPosition;
        private Vector2 attackPosition;// 移動しながら攻撃するパターンの場合は使用しない
        private int attackCounter;
        private int obsCounter;
        private int waitCounter;

        //public bool hasEnded = true;//{ get; set; }// 終わった瞬間
        public bool isWaiting = true;//{ get; set; }// 待機中
        public bool isStartingAttack { get; set; }// 攻撃開始のモーションとかなんとか
        public bool isEndingAttack { get; set; }  // 攻撃終了してから元の位置に戻るときなど
        */
		private float distanceD, distanceT, distanceA;
		private Vector2 tmpPosition;
		public bool isUsingTurret { get; set; }

		// Weapons
		private Weapon singleLightning;
		private DivisionWeapon dividingLightning;
		private Turret ballTurret;                      // 稲妻で地面に向けて攻撃する子機 
		private Turret thunderTurret, thunderTurret2;                   // 自機狙いthunder用
		private Turret beamTurret;                      // 真下に否妻
		private JumpingEnemy jumpingEnemy;

		private Obstacle obstacle;
		private List<Obstacle> obstacles = new List<Obstacle>();
		private int obstacleNum;                        // 一度の攻撃で出現させる障害地形の数.種類とかについては後で決めよう
		#endregion
		private bool hasPlayedSE2;
		private bool /*hasPut,*/ hasMoved;
		private Turret thunderTurret3;
		private Turret thunderTurret8Way;
		private List<Turret> thunderTurrets3Way = new List<Turret>();
		private List<Turret> thunderTurrets = new List<Turret>();
		private SoundEffect thunderSoundBig, thunderSoundSmall, spawnSound;

		public Raijin(Stage stage, float x, float y, int width, int height, int HP)
			: base(stage, x, y, width, height, HP)
		{
			this.defaultPosition = position;
			tmpPosition = defaultPosition + new Vector2(-150, -50);
			attackPosition = new Vector2(defaultPosition.X - 640, defaultPosition.Y - 50);
			singleLightning = new Weapon(stage, position.X, position.Y, 16, 200, this);                             // アニメーションの関係でWeaponじゃ処理しきれないのでBeamに移行
			dividingLightning = new DivisionWeapon(stage, position.X, position.Y, 32, 32, this, 1.0f);
			obstacle = new Obstacle(stage, this, x, y, 32, 64, 0);
			ballTurret = new Turret(stage, this, position, 32, 32, 0, 0, 1, false, false, true, 3, 0, 1);      // 他目的の雲object生成  , , 0
			ballTurret.bulletSpeed = new Vector2(0, 8);

			thunderTurret = new Turret(stage, this, this.position, 64, 48, 2, 0, 3, false, false, true, 3, 3, 3, 14, 120, 10);//, , 3...2
			thunderTurret2 = new Turret(stage, this, this.position, 64, 48, 2, 0, 3, false, false, true, 3, 3, 3, 14, 120, 10);//, , 3...2
			thunderTurret2.turnsRight = true;
			thunderTurret3 = new Turret(stage, this, this.position, 64, 48, 2, 3, 3, false, false, true, 3, 3, 3, 14, 120, 10);//, , 3...2
			beamTurret = new Turret(stage, this, new Vector2(width / 2 + 32, height), 32, 32, 1, 0, 1, true);//, , 2
			thunderTurret8Way = new Turret(stage, this, this.position, 64, 48, 2, 3, 8, false, false, true, 3, 3, 3, 14, 360, 30);//...3

			for (int i = 0; i < 10; i++)
				thunderTurrets.Add(new Turret(stage, this, this.position, 64, 48, 2, 1, 5, false, false, true, 3, 3, 3, 14, 240, 40));//...1
			thunderTurrets3Way.Add(thunderTurret);
			thunderTurrets3Way.Add(thunderTurret2);
			thunderTurrets3Way.Add(thunderTurret3);
			turrets.Add(thunderTurret8Way);

			// Add them to turrets
			//turrets.Add(thunderTurret);
			//turrets.Add(thunderTurret2);
			turrets.Add(ballTurret);
			turrets.Add(beamTurret);
			foreach (Turret tur in thunderTurrets)
				turrets.Add(tur);
			foreach (Turret tur in thunderTurrets3Way)
				turrets.Add(tur);

			obstacleNum = 10;
			for (int i = 0; i < obstacleNum; i++)
				obstacles.Add(new Obstacle(stage, this, x, y, 32, 64, 0));//OdueOPSNOa

			// Add them to this.weapons
			weapons.Add(singleLightning);
			weapons.Add(dividingLightning);
			weapons.Add(obstacle);
			//weapons.Add(ballTurret);
			//weapons.Add(thunderTurret);
			//weapons.Add(thunderTurret2);
			//weapons.Add(beamTurret);
			foreach (Turret tur in turrets) weapons.Add(tur);
			foreach (Obstacle obs in obstacles) weapons.Add(obs);

			// Add weapons to stage.weapons
			foreach (Weapon weapon in weapons) stage.weapons.Add(weapon);
			foreach (Weapon weapon in weapons) stage.objects.Add(weapon);

			jumpingEnemy = new JumpingEnemy(stage, position.X, position.Y, 32, 32, 2, this);
			stage.characters.Add(jumpingEnemy);
			attackNum = 2;

			Load();
		}
		protected override void Load()
		{
			base.Load();

			texture = content.Load<Texture2D>("Object\\Character\\Raijin");
			thunderSoundBig = content.Load<SoundEffect>("Audio\\SE\\thunder_big");
			thunderSoundSmall = content.Load<SoundEffect>("Audio\\SE\\thunder_small");
			spawnSound = content.Load<SoundEffect>("Audio\\SE\\zako_tama");
		}

		public override void Update()
		{
			if (isAlive && stage.inBossBattle) {
				Attack(30);
				base.Update();
			}
		}
		public override void UpdateAnimation()
		{
			animation.Update(2, 0, 300, 280, 12, 1);
		}
		protected override void MotionDelay()
		{
		}

		protected override void Attack(int waitTime)
		{
			if (isWaiting) {
				if (waitCounter > waitTime) {
					isWaiting = false;
					waitCounter = attackCounter = 0;//
					attackNum++;
					if (game.isHighLvl)
						if (attackNum >= 6)
							attackNum = 1;
					if (!game.isHighLvl)
						if (attackNum >= 5) attackNum = 1;

					//if (!game.isHighLvl && attackNum == 3) attackNum = 4;
				}
				waitCounter++;
			} else {
				if (attackCounter == 0) {
					isStartingAttack = true;
					attackCounter++;
					counter = 0;
				}
				if (!game.isHighLvl) {
					#region old
					/*switch (attackNum) {
                        case 0: SpawnEnemy(); break;
                        case 1: AttackPattern1A(defaultPosition, tmpPosition, attackPosition, true); break;
                        case 2: AttackDivision(); break;
                        case 3: UseObstacle(); break;
                        case 4: AttackWithThunder(); break;
                        case 5: SpawnATurret(); break;
                    }*/
					#endregion
					switch (attackNum) {
						case 0:
							AttackWithThunder();
							break;
						case 1:
							AttackWithThunder2();
							break;
						case 2: /*AttackPattern1A(
								defaultPosition
								, new Vector2(defaultPosition.X, defaultPosition.Y + 100)
								, new Vector2(defaultPosition.X - 480, defaultPosition.Y - 150)
								, false
								);*/
							Move(5, defaultPosition
							, new Vector2(defaultPosition.X, defaultPosition.Y + 100)
							, new Vector2(defaultPosition.X - 480, defaultPosition.Y - 150));
							break;//AttackPattern1A(defaultPosition, tmpPosition, attackPosition, true);
						case 3:
							Division();
							break;//AttackDivision();
						case 4:
							MoveLite(
								new Vector2(defaultPosition.X - 350, defaultPosition.Y - 150)
								, new Vector2(defaultPosition.X - 200, defaultPosition.Y - 100)
								, defaultPosition
								, true
							);
							break;//isWaiting = true;
						case 5:
							SpawnATurret();
							break;
					}
				} else {
					switch (attackNum) {
						case 1:
							UseTurrets();
							break;
						case 2:
							AttackPattern1A(
								defaultPosition
								, new Vector2(defaultPosition.X, defaultPosition.Y + 100)
								, new Vector2(defaultPosition.X - 480, defaultPosition.Y - 150)
								, false
								);
							break;
						case 3:
							Division();
							break;
						case 4:
							PutTurrets(new Vector2[] {
                                new Vector2(defaultPosition.X - 350, defaultPosition.Y - 50)
                                , new Vector2(defaultPosition.X - 200, defaultPosition.Y - 0)
                                , new Vector2(defaultPosition.X - 100, defaultPosition.Y + 50)}
								);
							break;
						case 5:
							/*PutTurrets(
								new Vector2(defaultPosition.X - 350, defaultPosition.Y - 150)
								, new Vector2(defaultPosition.X - 200, defaultPosition.Y - 100)
								, new Vector2(defaultPosition.X - 100, defaultPosition.Y - 100)
								);*/
							isWaiting = true;
							break;
						case 6:
							break;
					}
				}
				if (isUsingTurret) ControlTurret(ballTurret, 0, 3);
			}
		}
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
			baseVector *= new Vector2(5, 5);
			baseVectorT *= new Vector2(5, 5);
			baseVectorA *= new Vector2(5, 5);
			distanceD = Vector2.Distance(position, startPos);
			distanceT = Vector2.Distance(position, wayPos);
			distanceA = Vector2.Distance(position, endPos);

			if (isStartingAttack)
				speed = baseVectorT;
			if (isStartingAttack && distanceT < 5) {
				counter = 0;
				isAttacking = true;
				isStartingAttack = false;
			}

			if (isAttacking)
				speed = baseVectorA;
			if (isAttacking && distanceA < 5) {
				isAttacking = false;
				isEndingAttack = true;
			}

			//if(isEndingAttack)
			//    speed = baseVector;

			if (isEndingAttack /*&& distanceD < 5*/ || counter > 240) {
				hasPlayedSE = false;
				hasMoved = true;
				speed = Vector2.Zero;
				if (endAttack) {
					isEndingAttack = false;
					isAttacking = false;
					isWaiting = true;
					attackCounter = 0;
					counter = 0;
				}
			}
			counter++;
		}
		private void AttackPattern1A(Vector2 startPos, Vector2 wayPos, Vector2 attackPos, bool willReturn)
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
				beamTurret.isBeingUsed = true; // isBeingUsedがtrueになる前に一瞬描画されてるぜfuck
				//beamTurret.Inicialize();
				speed = baseVectorT;
				hasPlayedSE = false;
				attackPos = new Vector2(startPos.X - 480, startPos.Y - 50);
			}
			if (isStartingAttack && distanceT < 5) {// 攻撃位置まで来たらフラグをたてて攻撃開始.
				counter = 0;
				isStartingAttack = false;
				isAttacking = true;
			}
			if (isAttacking) {
				//hasPlayedSE = true;
				speed = baseVectorA;
				//beamTurret.Update();
			}
			if (isStartingAttack || isAttacking || isEndingAttack) {
				if (isEndingAttack) { }
				if (!hasPlayedSE) {
					if (counter % 8 == 0) {
						if (!game.isMuted) thunderSoundBig.Play(SoundControl.volumeAll, 0f, 0f);
					}
				}
			}

			if (isAttacking && distanceA < 5) {
				isAttacking = false;
				//hasPlayedSE = true;
				isEndingAttack = true;
				willReturn = true;
			}
			if (willReturn) {
				if (isEndingAttack) speed = baseVector;

				//if (isEndingAttack && distanceD < 5) isEndingAttack = false;
				attackCounter++;
				if (isEndingAttack && distanceD < 5) {
					speed.X = 0; speed.Y = 0;
					beamTurret.isBeingUsed = false;
					isEndingAttack = false;
					isAttacking = false;
					isWaiting = true;
					hasPlayedSE = false;
					attackCounter = 0;
					counter = 0;
				}
			} else
				if (isEndingAttack) {
					speed.X = 0; speed.Y = 0;
					beamTurret.isBeingUsed = false;
					isEndingAttack = false;
					isAttacking = false;
					isWaiting = true;
					hasPlayedSE = false;
					attackCounter = 0;
					counter = 0;
				}

			counter++;
		}
		private void AttackDivision()
		{
			// マルクのアレみたいな攻撃

			// 攻撃終了地点からデフォルト位置へベクトルを引く
			Vector2 returnVector = defaultPosition - position; ;
			Vector2 baseVectorD = Vector2.Normalize(returnVector);
			//Vector2.Multiply(baseVectorD, 16);
			baseVectorD *= new Vector2(3, 3);
			// 多分ベクトルで移動するなら使わない
			distanceD = Vector2.Distance(position, defaultPosition);

			// 攻撃位置へのベクトル
			attackPosition = new Vector2(defaultPosition.X - 300, defaultPosition.Y - 50);
			Vector2 attackVector = attackPosition - position;
			Vector2 baseVectorA = Vector2.Normalize(attackVector);
			//Vector2.Multiply(baseVectorA, 16);
			baseVectorA *= new Vector2(3, 3);

			distanceA = Vector2.Distance(position, attackPosition);

			if (isStartingAttack) {// 攻撃位置へ
				speed = baseVectorA;
			}
			if (isStartingAttack && /*position == attackPosition*/distanceA < 5) {// 攻撃位置へ北ら
				speed = Vector2.Zero;
				isAttacking = true;
				isStartingAttack = false;

				dividingLightning.isBeingUsed = true;
				dividingLightning.isDivided = false;// あとで直す
				dividingLightning.isEnd = false;
				//dividingLightning.
			}
			if (isAttacking) {
				dividingLightning.MovePattern1();
				speed = Vector2.Zero;// 書かないと落ちる
				if (!hasPlayedSE2) {
					if (!game.isMuted) thunderSoundSmall.Play(SoundControl.volumeAll, 0f, 0f);
					hasPlayedSE2 = true;
				}

			}
			if (isAttacking && dividingLightning.isEnd /*&& dividingLightning.counter > 60*//*position.X < startPosition.X - 300*/) {
				//isStartingAttack = false;
				isAttacking = false;
				dividingLightning.isBeingUsed = false;
				isEndingAttack = true;
			}
			if (isEndingAttack && distanceD > 5)
				speed = baseVectorD;

			else if (isEndingAttack && distanceD < 5) {
				speed = Vector2.Zero;
				isEndingAttack = false;
				isAttacking = false;
				hasPlayedSE = false;
				isWaiting = true;
				attackCounter = 0;
				//counter = 0;
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
				//dividingLightning.
			}
			if (isAttacking) {
				dividingLightning.MovePattern1();
				speed = Vector2.Zero;// 書かないと落ちる
				if (!hasPlayedSE2) {
					thunderSoundSmall.Play(SoundControl.volumeAll, 0f, 0f);
					hasPlayedSE2 = true;
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
				hasPlayedSE = false;
				isWaiting = true;
				attackCounter = 0;
				counter = 0;
			}

		}
		private void AttackPattern3()
		{
			// lightningBall的な何かturretに任せる
			UpdateTurrets(-1);
		}
		private void AttackWithThunder()//Turret[] turs, Vector2[] shootPos)
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
				thunderTurret2.turnsRight = true;
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
			//thunderTurret8Way.Update();
			if (thunderTurret8Way.isEnd || counter > 1500) {
				thunderTurret8Way.isBeingUsed = false;
				isAttacking = false;
				isWaiting = true;
				attackCounter = 0;
				counter = 0;
			}
			counter++;
		}
		protected virtual void SpawnEnemy()
		{
			this.jumpingEnemy.isBeingUsed = true;
			if (!jumpingEnemy.isAlive) {
				// 蘇生☆
				jumpingEnemy.HP = 2;
				jumpingEnemy.isAlive = true;
				jumpingEnemy.isActive = true;
				jumpingEnemy.position = this.position;
			}
			spawnSound.Play(SoundControl.volumeAll, 0f, 0f);
			//isEndingAttack = 
			attackCounter = 0;
			isWaiting = true;
			//jumpingEnemy.speed.X = -5;
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
			if (isEndingAttack && distanceD > 5)
				speed = baseVectorD;

			else if (isEndingAttack && distanceD < 5) {
				speed = Vector2.Zero;
				isEndingAttack = false;
				isAttacking = false;
				isWaiting = true;
				attackCounter = 0;
				//counter = 0;
			}
			counter++;

		}
		private void ControlTurret(Turret targetTurret, int type, float speedX)
		{
			targetTurret.isBeingUsed = true;
			switch (type) {
				case 0:
					targetTurret.RoundTripMotion(defaultPosition - new Vector2(200, 0), 200, speedX);
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
			for (int i = 0; i < obstacles.Count; i++)
				if (obsCounter % (spwanInterval * (i + 1)) == 0)
					obstacles[i].isBeingUsed = true;
			foreach (Obstacle obs in obstacles)
				if (obs.isBeingUsed)
					obs.MovePattern1(obstacle.trapSet
						, new Vector2(defaultPosition.X - 32, stage.CheckGroundHeight(defaultPosition.X) + obs.height)
						, new Vector2(defaultPosition.X - 32, stage.CheckGroundHeight(defaultPosition.X) - obs.height)
						, new Vector2(defaultPosition.X - 800, stage.CheckGroundHeight(defaultPosition.X) - obs.height)
						, new Vector2(0, -12), new Vector2(-2.5f, 0));
		}
		#endregion
		#region New_Hard
		private void UseTurrets()
		{
			if (isStartingAttack) {
				foreach (Turret tur in thunderTurrets)
					tur.isVisible = false;
				isStartingAttack = false;
			}

			for (int i = 0; i < thunderTurrets.Count; i++)
				if (attackCounter % (40 * (i + 1)) == 0) {
					thunderTurrets[i].Inicialize();
					thunderTurrets[i].isBeingUsed = true;
				}
			foreach (Turret tur in thunderTurrets)
				if (tur.isBeingUsed)
					ControlTurret(tur, 1, 8);

			if (attackCounter > 480)
				isEndingAttack = true;

			attackCounter++;
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
			}
		}
		private bool hasInicialized;
		private void PutTurrets(Vector2[] vecs)//Vector2 t1, Vector2 t2, Vector2 t3)
		{
			if (isStartingAttack)
				foreach (Turret tur in thunderTurrets3Way) {
					tur.Inicialize();
					tur.isEnd = false;
				}

			if (!hasMoved)
				MoveLite(
					new Vector2(defaultPosition.X - 350, defaultPosition.Y - 150)
					, new Vector2(defaultPosition.X - 200, defaultPosition.Y - 100)
					, defaultPosition
					, false
				);
			//if (isEndingAttack) isPutting = true;
			/*thunderTurret.position = vecs[0];
			thunderTurret2.position = vecs[1];
			thunderTurret3.position = vecs[2];*/

			attackCounter++;
			for (int i = 0; i < thunderTurrets3Way.Count; i++) {
				if (attackCounter % (40 * (i + 1)) == 0) {
					thunderTurrets[i].isVisible = true;
					if (i < thunderTurrets3Way.Count) thunderTurrets3Way[i].position = vecs[i];
				}
			}

			//if (hasMoved) Fire = true;

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

			if (thunderTurret.isEnd /**/|| attackCounter > 720) {
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
			}
			attackCounter++;
		}
		#endregion
	}
}
