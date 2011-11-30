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
	///  Bulletの射撃の管理クラス。射撃する敵は全てTurretを生成して射撃処理を委譲させるのが楽。
	/// </summary>
	public class Turret : Weapon
	{
		#region Member variable

		private static readonly int defBulletNum = 3;
		private static readonly int defShootInterval = 120;
		private static readonly int defBulletInterval = 20;

		/// <summary>
		/// Turretが持つbulletの個数。射撃が終わるたびに回収して使いまわす
		/// </summary>
		private int bulletNumber = defBulletNum;
		/// <summary>
		/// 攻撃の間隔[frame]
		/// </summary>
		private int shootInterval = defShootInterval;
		/// <summary>
		/// １回の攻撃における射撃の間隔[frame]
		/// </summary>
		private int bulletInterval = defBulletInterval;

		/// <summary>
		/// Turretが大きいなら複数射撃点があってもいい、もしくはturretにturretを作る
		/// </summary>
		private List<Vector2> shootPositions;
		private SoundEffect damageSound, hitSound;
		private Vector2 shootPosition;
		private int timeInterval;
		private int bulletCounter;
		/// <summary>
		/// (撃ってからの)時間経過[frame]
		/// </summary>
		private int shootCounter;
		
		private int motionDelayTime = 120;
		/// <summary>
		/// shootPatternの中での細かいパターン。後で消す
		/// </summary>
		private int shootOption;
		/// <summary>
		/// trueにすると自動で射撃しなくなる。射撃させたい時にuserがUpdateメソッドを呼ぶ。
		/// </summary>
		private bool shootManually;

		public List<Bullet> bullets { get; private set; }
		public Vector2 defaultPosition { get; private set; }
		public bool isStandard { get; private set; }
		public int textureType { get; private set; }
		public int bulletTextureType { get; private set; }
		public int bulletType { get; private set; }
		public int shootPattern { get; private set; }
		/// <summary>
		///  (位置的に)userに従属するか否か
		/// </summary>
		public bool isSubsidiary { get; private set; }
		/// <summary>
		/// 表示(描画)するか否か
		/// </summary>
		public bool isVisible { get; internal set; }
		/// <summary>
		/// 弾を射撃した瞬間
		/// </summary>
		public bool hasShot { get; private set; }
		/*/// <summary>
		/// 1回の射撃が終了したか
		/// </summary>
		public bool isEnd { get; internal set; }*/
		/// <summary>
		/// 1回のまとまった攻撃単位の中での射撃した回数
		/// </summary>
		public int shootNum { get; internal set; }
		/// <summary>
		/// 累計での射撃した回数
		/// </summary>
		public int shootNumTotal { get; internal set; }
		/// <summary>
		/// 射撃した弾を回収して次の射撃に使うか。まだ使ってない
		/// </summary>
		public bool reuseBullets { get; private set; }
		public List<Bullet> hasShotBullets = new List<Bullet>();

		/// <summary>
		/// userが任意の方向に飛ばさせたい時に指定
		/// </summary>
		public Vector2 bulletSpeed { get; set; }
		public float bulletSpeed1D { get; private set; }

		/// <summary>
		/// １つの射撃パターンのみを使って射撃する通常モードなのか、それらの組み合わせて使うモードなのかどうか
		/// </summary>
		private bool customShootPattern;
		/// <summary>
		/// カスタム用のリスト内でどのパターンを実行中かのindex。
		/// </summary>
		private int customShootPatternNum;
		/// <summary>
		/// カスタム用のリスト：ループさせたい射撃パターンを追加する。
		/// </summary>
		private List<int> shootPatternLoop = new List<int>();

		#endregion
		#region Constructors																																				// ↓参照してるクラス
		public Turret(Stage stage, float x, float y, int width, int height)
			: this(stage, null, new Vector2(0, 0), width, height, 0, 0, 1)
		{
		}
		public Turret(Stage stage, Object user, Vector2 shootPosition, int width, int height, int bulletType, int shootPattern, int bulletNum)													// FE, FE3, SE, SE3, Sword
			: this(stage, user, shootPosition, width, height, bulletType, shootPattern, bulletNum, false)
		{
		}
		public Turret(Stage stage, Object user, Vector2 shootPosition, int width, int height, int bulletType, int shootPattern, int bulletNum, bool shootManually)								// Raijin.beamTurret
			: this(stage, user, shootPosition, width, height, bulletType, shootPattern, bulletNum, shootManually, true)
		{
		}
		public Turret(Stage stage, Object user, Vector2 shootPosition, int width, int height, int bulletType, int shootPattern, int bulletNum,
			bool shootManually, bool isSubsidary)// nothing
			: this(stage, user, shootPosition, width, height, bulletType, shootPattern, bulletNum, shootManually, isSubsidary, 0, 0)
		{
		}

		public Turret(Stage stage, Object user, Vector2 shootPosition, int width, int height, int bulletType, int shootPattern, int bulletNum,
			bool shootManually, bool isSubsidiary, int textureType, int bulletTextureType)																			// FE2, Raijin.ballTurret, Fuujin.cutterTurre, Rival.(thunderT, cutterT, syuriken)
			: this(stage, user, shootPosition, width, height, bulletType, shootPattern, bulletNum, shootManually, isSubsidiary, textureType, bulletTextureType, 14)
		{
		}

		public Turret(Stage stage, Object user, Vector2 shootPosition, int width, int height, int bulletType, int shootPattern, int bulletNum,
			bool shootManually, bool isSubsidiary, int textureType, int bulletTextureType, float bulletSpeed)
			: this(stage, user, shootPosition, width, height, bulletType, shootPattern, bulletNum, shootManually, isSubsidiary, textureType, bulletTextureType, bulletSpeed, 180, 20)
		{
		}

		/// <summary>
		/// 一番細かくパラメータを設定できるコンストラクタ。
		/// </summary>
		public Turret(Stage stage, Object user, Vector2 defPosition, int width, int height, int bulletType, int shootPattern, int bulletNum,
			bool shootManually, bool isSubsidiary, int textureType, int bulletTextureType, float bulletSpeed, int shootInterval, int bulletInterval)				// Raijin.thnderTurret, Raijin.thnderTurrets, Raijin.thnderTurret8Way
			: this(stage, user, defPosition, width, height, bulletType, shootPattern, bulletNum, shootManually, isSubsidiary, textureType, bulletTextureType
			,bulletSpeed, shootInterval, bulletInterval, new Vector2(-bulletSpeed, 0), false, false)
		{
		}

		public Turret(Stage stage, Object user, Vector2 defPosition, int width, int height, int bulletType, int shootPattern, int bulletNum,
			bool shootManually, bool isSubsidiary, int textureType, int bulletTextureType
			, float bulletSpeed, int shootInterval, int bulletInterval, Vector2 bulletSpeed1D, bool reuseBullet, bool customPattern, params int[] patternIndexes)																						// Raijin.thnderTurret, Raijin.thnderTurrets, Raijin.thnderTurret8Way
			: base(stage, defPosition.X, defPosition.Y, width, height, user)
		{
			// type0:bullet、type1:beam、type3:else
			shootPositions = new List<Vector2>();
			shootPositions.Add(shootPosition);
			//this.shootPosition = shootPosition;
			isVisible = true;//false
			//reuseBullets = true;
			this.shootInterval = shootInterval;
			this.bulletInterval = bulletInterval;
			this.defaultPosition = defPosition;
			this.bulletType = bulletType;
			this.shootPattern = shootPattern;
			this.bulletNumber = bulletNum;
			if (isSubsidiary) {
				this.position = user.position + defPosition/**/;					// (isSubsidiaryで呼び出すとこの時点でdefPosition = shootPositionである)
			} else {
				this.position = defPosition;
			}
			this.shootManually = shootManually;
			this.isSubsidiary = isSubsidiary;
			this.canBeDestroyed = canBeDestroyed;
			this.HP = HP;
			this.textureType = textureType;
			this.bulletTextureType = bulletTextureType;
			this.bulletSpeed1D = bulletSpeed;
			this.bulletSpeed = bulletSpeed1D;
			this.reuseBullets = reuseBullet;
			this.customShootPattern = customPattern;
			for (int i = 0; i < patternIndexes.Length; i++) this.shootPatternLoop.Add(patternIndexes[i]);

			bullets = new List<Bullet>();
			animation = new Animation(width, height);
			AddBullets();
			Load();

			if (bullets.Count <= 0) throw new Exception("No bullets to shoot in this turret.");
		}
		#endregion

		protected override void Load()
		{
			base.Load();

			texture = content.Load<Texture2D>("Object\\Weapon\\Turret");
			damageSound = content.Load<SoundEffect>("Audio\\SE\\damage");
			hitSound = content.Load<SoundEffect>("Audio\\SE\\hit_big");
		}
		/// <summary>
		/// 途中で止めてまた再開するときなどに(撃つObjectが意図的に調整するときに使用)
		/// </summary>
		public void Inicialize()
		{
			foreach (Bullet bullet in bullets) {
				//bullet.position = this.position;
				bullet.isShot = false;//true
				bullet.counter = 0;
				bullet.movedDistance = 0;
				if (bullet is Thunder) (bullet as Thunder).Inicialize();
			}
			bulletCounter = 0;
			timeInterval = 0;
			counter = 0;
			hasShot = false;
			turnsRight = true;
			if (isSubsidiary) position = user.position + shootPosition;
		}
		/// <summary>
		/// 指定された弾の個数だけListに追加
		/// </summary>
		private void AddBullets()
		{
			switch (bulletType) {
				case 0:
					for (int i = 0; i < bulletNumber; i++)
						if (reuseBullets) bullets.Add(new Bullet(stage, this, 16, 16, bulletTextureType));
						else bullets.Add(new Bullet(stage, this, 16, 16, bulletTextureType, 3));
					break;
				case 1:
					for (int i = 0; i < bulletNumber; i++)
						if (reuseBullets) bullets.Add(new Beam(stage, this, 64, 400, 1));
						else bullets.Add(new Beam(stage, this, 64, 400, 1, 3));
					break;
				case 2:
					for (int i = 0; i < bulletNumber; i++)
						if (reuseBullets) bullets.Add(new Thunder(stage, this, 100, 8, 0));
						else bullets.Add(new Thunder(stage, this, 100, 8, 0, 3));
					break;
			}
			foreach (Bullet bullet in bullets) stage.unitToAdd.Add(bullet);	// StageのListに追加
		}

		public override void Update()
		{
			if (user is Rival && shootPattern == 0 && bulletType == 0 && bulletTextureType == 2) { }
			if (IsBeingUsed()) {
				// Method USN0でまとめようとしたが、Manualで射撃させようとするとBeamが表示されないバグが
				if (!shootManually) {
					UpdateShootingN0(this.shootInterval, this.bulletInterval, shootManually);
					//UpdateShootingPattern();
				} else if (shootManually && isBeingUsed) {
					UpdateShootingOnce(this.bulletInterval);
				}

				UpdateNumbers();
				UpdateAnimation();
				base.Update();
			}
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			if ((isBeingUsed /*&& canBeDestroyed*/ && isVisible) /*|| (canBeDestroyed && isVisible)*/) {// 壊れない&&描画したいときもあるだろうが今はこの仕様で。
				spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, MathHelper.ToRadians(-degree), Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
			}
		}

		// 補助メソッド
		#region ShootPatterns
		// old:
		/// <summary>
		/// 弾に直線的な速度を与えるパターン
		/// </summary>
		private void ShootPattern0(Bullet bullet, Vector2 bulletSpeed)
		{
			bullet.speed = bulletSpeed;
			bullet.isShot = true;
		}
		/// <summary>
		/// Playerへ直線的にホーミングさせる速度を与えるパターン。
		/// </summary>
		private void ShootPattern1(Object target_object, Bullet bullet, float speed)
		{
			double rad = Math.Atan2(target_object.position.Y - position.Y, target_object.position.X - position.X);
			bullet.rot = -(float)rad + (float)Math.PI;
			bullet.degree = -(float)MathHelper.ToDegrees((float)rad + (float)Math.PI);

			// 移動量を計算
			bullet.speed =  new Vector2((float)Math.Cos(rad) * speed, (float)Math.Sin(rad) * speed);	
			bullet.isShot = true;
		}
		/// <summary>
		/// Thunder用のメソッド。 4/14 角度の計算までここに入れてしまおうか?
		/// </summary>
		private void ShootPattern2(/*Bullet bullet, */params int[] degrees)
		{
			//bullet.position = position;// 発射時に同じ処理してるし意味無いよな... つまりdegree調整処理を追加してthunderの時もここを参照するようにすべき
			//bullet.degree = degree;

			if (degrees.Length == 1) {
				foreach (Bullet bul in bullets)
					bul.degree = degrees[0];
			} else {
				for (int i = 0; i < bullets.Count; i++) {
					bullets[i].degree = degrees[i];
					if (i == degrees.Length - 1) break;
				}
			}
		}
		/// <summary>
		/// 参考：http://hakuhin.jp/as/shot.html#SHOT_00_00 の「拡散させる動き」
		/// </summary>
		/// <param name="bullet"></param>
		private void ShootPattern5(Object targetObject)
		{
			float speed = 5;	                                                                            // 個々のbulletのスピード
			float width = 60;                                                                               // 射撃するbulletsの幅(degree)
			double rad = Math.Atan2(targetObject.position.Y - position.Y, targetObject.position.X - position.X);    // 全体としての基準となる向き.
			double rotation = 0;                                                                            // 最終的な個々のbulletに与える角度(radian)

			// 角度の割り振り
			for (int i = 0; i < bullets.Count; i++) {// 最初以外は全部同じangleになってるっぽい.全然割り振られていない.
				float tmp = (((float)bullets.Count - 1) - i) / ((float)bullets.Count - 1);//(bullets.Count==1 ? 2 :bullets.Count - 1);	// 0.0～1.0に丸める (i.e. *1/5, *2/5,...  <結論：整数が切り捨てられてた>
				tmp -= .5f;	                // -0.5～0.5にする
				double rot = tmp * width;	// 角度を求める
				rotation = rad + MathHelper.ToRadians((float)rot); // rotはもしかしてdegreeで来てるな？これが原因か

				//bullets[i].degree = MathHelper.ToDegrees((float)rotation) + 180;//MathHelper.ToDegrees((float)rad) + (float)rot;//MathHelper.ToDegrees((float)rad);     // 元絵の関係で調整←- 180は要ら内っぽい  何故characterによって角度が違ってくるのか？
				bullets[i].rot = -(float)rotation;
				bullets[i].degree = MathHelper.ToDegrees((float)rotation);

				bullets[i].speed.X = (float)Math.Cos(rotation) * speed;	        // x方向の移動量を計算
				bullets[i].speed.Y = (float)Math.Sin(rotation) * speed;         // y方向の移動量を計算
			}
		}

		// new: 8/16　弾種→射撃パターンでまとめる
		private void ShootPatternNormal0(Bullet bullet, Vector2 bulletSpeed)
		{
			bullet.speed = bulletSpeed;
			bullet.isShot = true;
		}
		private void ShootPatternNormal1(Object targetObject, Bullet bullet, float speed)
		{
			// 180°分回転度数を変更したい
			double rad = Math.Atan2(targetObject.position.Y - position.Y, targetObject.position.X - position.X);

			bullet.rot = -(float)rad + (float)Math.PI;
			bullet.degree = -(float)MathHelper.ToDegrees((float)rad + (float)Math.PI) + 180;// + 180で向きは修正された？

			// 移動量を計算
			bullet.speed = new Vector2((float)Math.Cos(rad) * speed, (float)Math.Sin(rad) * speed);
			bullet.isShot = true;
		}
		private void ShootPatternBeam0(float degree)
		{
			foreach (Bullet bul in bullets) bul.degree = degree;
		}
		private void ShootPatternBeam1(params float[] degrees)
		{
			//foreach (Bullet bul in bullets) bul.degree = degree;
			if (degrees.Length == 1) {
				foreach (Bullet bul in bullets)
					bul.degree = degrees[0];
			} else {
				for (int i = 0; i < bullets.Count; i++) {
					bullets[i].degree = degrees[i];
					if (i == degrees.Length - 1) break;
				}
			}
		}
		private void ShootPatternThunder0(float defDegree, float unitDegree)
		{
			for (int i = 0; i < bullets.Count; i++) bullets[i].degree = defDegree + i * unitDegree;
		}
		private void ShootPatternAll0(Object targetObject)
		{
			float speed = 5;	                                                                            // 個々のbulletのスピード
			float width = 60;                                                                               // 射撃するbulletsの幅(degree)
			double rad = Math.Atan2(targetObject.position.Y - position.Y, targetObject.position.X - position.X);    // 全体としての基準となる向き.
			double rotation = 0;                                                                            // 最終的な個々のbulletに与える角度(radian)

			// 角度の割り振り
			for (int i = 0; i < bullets.Count; i++) {
				float tmp = (((float)bullets.Count - 1) - i) / ((float)bullets.Count - 1);
				tmp -= .5f;					// -0.5～0.5にする
				double rot = tmp * width;	// 角度を求める
				rotation = rad + MathHelper.ToRadians((float)rot);

				bullets[i].rot = -(float)rotation;
				bullets[i].degree = MathHelper.ToDegrees((float)rotation);

				bullets[i].speed.X = (float)Math.Cos(rotation) * speed;
				bullets[i].speed.Y = (float)Math.Sin(rotation) * speed;
			}
		}
		#endregion
		/// <summary>
		/// 与えられたBulletに初期化が必要かどうか判定するメソッド
		/// </summary>
		/// <param name="bullet">判定したいBullet</param>
		/// <returns>Bulletに初期化が必要ならtrue、必要ないならfalseを返す</returns>
		private bool NeedIni(Bullet bullet)
		{
			return bullet.isShot && bullet.counter == 0;
		}

		// 射撃管理メイン
		/// <summary>
		/// 複数の射撃パターンを管理するメソッド。isEnd == trueになり次第、リスト内の次の射撃パターンに見合うように変数を更新する。
		/// </summary>
		private void UpdateShootingPattern()
		{
			/*if (customShootPattern) {
				if (isEnd) {
					customShootPatternNum++;
					if (customShootPatternNum == shootPatternLoop.Count - 1) customShootPatternNum = 0;
					shootPattern = shootPatternLoop[customShootPatternNum];
					isEnd = false;
				}
				UpdateShootingN0(this.shootInterval, this.bulletInterval, shootManually);
			} else {
				UpdateShootingN0(this.shootInterval, this.bulletInterval, shootManually);
			}*/
			if (customShootPattern) {
				customShootPatternNum++;
				if (customShootPatternNum == shootPatternLoop.Count) customShootPatternNum = 0;
				shootPattern = shootPatternLoop[customShootPatternNum];
				isEnd = false;
			}
		}
		/// <summary>
		/// リファクタリング中の代替メソッド。
		/// </summary>
		/// <param name="shootInterval">射撃間隔</param>
		/// <param name="bulletInterval">>1回の射撃における弾の間隔</param>
		public void UpdateShootingN0(int shootInterval, int bulletsInterval, bool isManual)
		{
			// 初期化
			switch (shootPattern) {
				default:
					InicializeShootingN0(true, isManual);
					break;
				case 5:
					InicializeShootingN0(false, isManual);
					break;
			}

			// bulletsの挙動を管理する。最初の１フレームだけだったり毎フレームだったり
			if (hasShot) {
				switch (bulletType) {
					case 0:							// normal
						switch (shootPattern) {
							case 0:
								foreach (Bullet bullet in bullets) if (NeedIni(bullets[bulletCounter - 1])) ShootPatternNormal0(bullet, bulletSpeed);//bulletSpeed
								break;
							case 1:// 現在射撃中の弾のみホーミング座標更新 // bulletCounter - 1か...?
								if (user is Fuujin) { }// FUujinはbC==1 ShootingEnemyはbC==2...
								/*foreach (Bullet bullet in bullets)*/ if (NeedIni(bullets[bulletCounter - 1])) ShootPatternNormal1(stage.player, bullets[bulletCounter - 1], bulletSpeed1D);// 跳ね返した次の射撃で呼ばれてないでござる
								break;
							case 2:// 毎フレーム座標更新：鬼ホーミング
								// 全弾に対して更新＋弾同士の間隔が狭いと無理ゲー
								foreach (Bullet bullet in bullets) ShootPatternNormal1(stage.player, bullet, bulletSpeed1D);
								break;
							case 5:
								if (NeedIni(bullets[0])) ShootPatternAll0(stage.player);
								break;
						}
						break;
					case 1:							// beam
						switch (shootPattern) {
							case 0:
								if (NeedIni(bullets[0])) ShootPatternBeam1(0);
								break;
							case 1:
								if (NeedIni(bullets[0])) ShootPatternBeam1(0);
								break;
						}
						break;
					case 2:							// thunder
						switch (shootPattern) {
							case 0:
								if (!(user is Raijin)) {
									if (!user.turnsRight) {
										if (NeedIni(bullets[0])) ShootPatternBeam1(135, 215, 250);
									} else if (user.turnsRight)
										if (NeedIni(bullets[0])) ShootPatternBeam1(40, -10, -65);
								} else {
									if (!turnsRight) ShootPatternBeam1(135, 215, -250);
									else if (turnsRight) ShootPatternBeam1(40, -10, -65);
								}
								break;
							case 1:
								ShootPatternBeam0(-90);
								break;
							case 2:
								ShootPatternBeam1(-100, -135, -60);
								break;
							case 3:
								ShootPatternThunder0(90, 30);
								break;
						}
						break;
				}
				shootCounter++;// 2ずつ増えてますねふぁっ区
			}
			timeInterval++;

			// bulletが自然にexpireするようにしなきゃ... hasShotは単に全弾撃ち終わった意味にしたい
			// 912:sI次第で全弾打ち終わる前に終わる
			if (hasShot && /*!bullets[bullets.Count - 1].isShot*/ ++shootCounter == shootInterval/**/) {
				hasShot = false;
				shootCounter = bulletCounter = shootNum = 0;
				//if (isManual) isEnd = true;
				if (reuseBullets) {
					foreach (Bullet bul in bullets) {
						bul.isHostile = true;// 終了する時にも一応
						bul.position = position + defaultPosition;
						bul.EndFlying();
					}
				} else {
					//bullets.Clear(); // stageのbulletsはココのbulletsを”参照”してるので消したら意味内　Updateするだけ用のリストに移す必要あり
					foreach (Bullet bullet in bullets) hasShotBullets.Add(bullet);// これでも多分ダメだなー
					bullets.Clear();
					AddBullets();
				}
				UpdateShootingPattern();

				isEnd = true;
				//if (bullets[bullets.Count - 1].isEnd) isEnd = true;// thunder用
			}
		}
		/// <summary>
		/// 射撃開始時に呼ばれる初期化メソッド。
		/// </summary>
		/// <param name="shootInTurn">順に射撃するか一斉射撃か</param>
		/// <param name="immediate">ShootOnceをメソッド内に組み込むために使う予定のパラメータ。まだ使えない</param>
		private void InicializeShootingN0(bool shootInTurn, bool immediate)
		{
			// １回の射撃の開始
			if (!immediate && timeInterval % shootInterval == 0 || immediate) {
				// 弾を使いまわさない場合はnewし直す。既に射撃した弾はStageが後処理する
				/*if (!reuseBullets) {
					AddBullets();
				}*/

				hasShot = true;
				isEnd = false;
				bulletCounter = shootCounter = 0;
				foreach (Bullet bul in bullets) {
					bul.position = position + defaultPosition;
					bul.hasFlied = bul.isShot = bul.isEnd = false;
					bul.counter = 0;
					bul.isHostile = true;
					if (!shootInTurn) bul.isShot = true;
				}
				shootNumTotal++;
			}

			// 1射撃の中での１弾目の射撃開始
			if (shootPattern == 1) { }// sNが既に1..
			if (hasShot && timeInterval % bulletInterval == 0 && shootNum <= bulletNumber/* + 1*/) {// bulletが1個だとsIが長いせいで1回の射撃で2回よばれてるな...? shootNum < bulletNumberを追加
				if (shootInTurn) {
					if (bulletCounter < bulletNumber) {
						bullets[bulletCounter].isShot = true;
						bullets[bulletCounter].counter = 0;
					}
					if (bulletCounter /*+ 1*/ < bulletNumber || bulletNumber == 1 && bulletCounter == 0/**/) bulletCounter++;// 2弾目が打たれないのはここでbC++されないでNeedIni(bullet)を通れないから
				}
				shootNum++;
			}
		}
		/// <summary>
		/// 1回射撃して終了させたいときに使用するメソッド。通常の射撃パターンはUSN0にまとめたがまだバグがあるのでこちらを残している。
		/// さっさと消したいところ
		/// </summary>
		/// <param name="shootInterval">射撃間隔</param>
		/// <param name="bulletInterval">1回の射撃における弾の間隔</param>
		public void UpdateShootingOnce(int bulletsInterval)
		{
			if (timeInterval == 0) {
				hasShot = true;
				isEnd = false;
				bulletCounter = 0;
				foreach (Bullet bul in bullets) {
					bul.position = position;
					bul.isShot = false;
				}
			}
			if (hasShot && timeInterval % bulletsInterval == 0) {
				if (shootPattern != 5) {
					bullets[bulletCounter].isShot = true;
					if (bulletCounter + 1 < bullets.Count) bulletCounter++;
					if (bulletCounter > bulletNumber) bulletCounter = 0;
				} else
					foreach (Bullet bul in bullets) bul.isShot = true;
			}
			timeInterval++;

			if (hasShot) {
				if (shootPattern != 5)
					foreach (Bullet bullet in bullets)													// Shoot()は2個目以降も呼ばれている
						if (bullet.isShot)
							if (bullet.counter == 0) {													// 0以上なら既に撃たれた後のはず
								switch (shootPattern) {
									case 0: ShootPattern0(bullet, bulletSpeed); break;
									case 1: ShootPattern1(stage.player, bullet, bulletSpeed1D); break;
									case 2: ShootPattern2(0); break;// 4Raijin
									case 3: ShootPattern2(0); break;// 4Raijin
									default: break;
								}
							} else if (bullets[0].isShot && bullets[0].counter == 0) {
								ShootPattern5(stage.player);
							}
			}
			if (hasShot) counter++;

			if (!bullets[bullets.Count - 1].isShot && hasShot && shootPattern != 3/* && counter > shootInterval*/) {
				hasShot = false;
				isEnd = true;
				counter = bulletCounter = timeInterval = 0;
				
				//if ((bullets[bullets.Count - 1] is Thunder) && (bullets[bullets.Count - 1] as Thunder).isEnd) isEnd = true;
			}
			//if ((bullets[bullets.Count - 1] is Thunder) && (bullets[bullets.Count - 1] as Thunder).isEnd) isEnd = true;
		}
		/// <summary>
		/// 1回射撃して終了。工事中:UpdateShootingN0に統合するのが一番良い
		/// </summary>
		/// <param name="shootInterval">射撃間隔</param>
		/// <param name="bulletInterval">1回の射撃における弾の間隔</param>
		public void UpdateShootingOnceN(int bulletsInterval, bool ini)
		{
			if (ini) {
				hasShot = true;
				isEnd = false;
				bulletCounter = 0;
				foreach (Bullet bul in bullets) {
					bul.position = position;
					bul.isShot = false;
				}
			}

			if (hasShot && timeInterval % bulletsInterval == 0) {
				if (shootPattern != 5) {
					bullets[bulletCounter].isShot = true;
					if (bulletCounter + 1 < bullets.Count) bulletCounter++;
					if (bulletCounter > bulletNumber) bulletCounter = 0;
				} else
					foreach (Bullet bul in bullets) bul.isShot = true;
			}
			timeInterval++;

			if (hasShot) {
				if (shootPattern != 5)
					foreach (Bullet bullet in bullets)// Shoot()は2個目以降も呼ばれている
						if (bullet.isShot)
							if (bullet.counter == 0)// 0以上なら既に撃たれた後のはず
								switch (shootPattern) {
									case 0: ShootPattern0(bullet, bulletSpeed); break;
									case 1: ShootPattern1(stage.player, bullet, bulletSpeed1D); break;
									case 2: ShootPattern2(0); break;// 4Raijin
									case 3: ShootPattern2(0); break;// 4Raijin
									default: break;
								} else if (bullets[0].isShot && bullets[0].counter == 0)
								ShootPattern5(stage.player);
			}
			if (hasShot) counter++;

			if (!bullets[bullets.Count - 1].isShot && hasShot && shootPattern != 3/* && counter > shootInterval*/) {
				hasShot = false;
				counter = 0;
				bulletCounter = 0;
				timeInterval = 0;
				isEnd = true;
				//if ((bullets[bullets.Count - 1] is Thunder) && (bullets[bullets.Count - 1] as Thunder).isEnd) isEnd = true;
			}
			if ((bullets[bullets.Count - 1] is Thunder) && (bullets[bullets.Count - 1] as Thunder).isEnd) isEnd = true;
		}

		protected override void UpdateNumbers()
		{
			if (user != null && isSubsidiary) {
				if (shootPosition != Vector2.Zero) this.position = user.position + shootPosition;
				else this.position = user.position + this.defaultPosition;
			}
		}
		public override void UpdateAnimation()
		{
			animation.Update(2, 0, 64, 48, 6, 1);
		}
	}
}
