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
	/// 弾の基本クラス
	/// </summary>
	public class Bullet : Object
	{
		/// <summary>
		/// 弾がPlayerの剣によって跳ね返された時に鳴るSE。
		/// </summary>
		private SoundEffect reflectSound;
		private ShootingEnemy shootingEnemy;
		private Boss boss;
		private SoundEffect shootSound;
		private SoundEffectInstance shootSoundInstance;
		//protected int damageTime;
		protected int textureType;
		/// <summary>
		/// 画面外に出てからBulletが消滅するまでの距離
		/// </summary>
		protected readonly float marginalDistance = 320;//120;
		
		/// <summary>
		/// userの座標を基準にしたBUlletの位置。Turret手動に移行した今となっては使用していない。
		/// </summary>
		public Vector2 shootPosition { get; protected set; }
		/// <summary>
		/// 射撃を管理するWeaponクラス。BUlletの挙動は完全にこのクラスに任せている。
		/// </summary>
		public Turret turret { get; private set; }
		/// <summary>
		/// 敵性かどうか。Playerが跳ね返した後はfalseになる。
		/// </summary>
		public bool isHostile { get; internal set; }
		/// <summary>
		/// 射撃されたかどうか。（Turret等に射撃された時にtrueになる）
		/// </summary>
		public bool isShot { get; internal set; }
		/// <summary>
		/// isAliveでいいのではないか説
		/// </summary>
		public bool hasFlied { get; internal set; }
		/// <summary>
		/// テクスチャの描画の計算に使う角度変数
		/// </summary>
		public float rot { get; internal set; }
		/// <summary>
		/// isAliveでいいのではないか説
		/// </summary>
		public bool isEnd { get; internal set; }
		/// <summary>
		/// isShotは撃たれた瞬間の処理に限定して、isEndまでの間はこちらのフラグを使う。
		/// </summary>
		public bool isFlying { get; internal set; }
		/// <summary>
		/// 弾の消滅条件(=isEndが立つ条件)
		/// </summary>
		public int disappearPattern { get; protected set; }
		public bool hasBeenShot { get; private set; }
		public double movedDistance { get; internal set; }
		// コンストラクタ
		public Bullet(Stage stage, Turret turret, int width, int height)
			: this(stage, turret, width, height, 0)
		{
		}
		public Bullet(Stage stage, Turret turret, int width, int height, int textureType)
			: this(stage, turret, width, height, textureType, 0, "")
		{
		}
		public Bullet(Stage stage, Turret turret, int width, int height, int textureType, int dissapearType, string seName)
			: base(stage, width, height)
		{
			this.turret = turret;
			this.shootPosition = turret.position;
			this.position = turret.position;
			this.textureType = textureType;
			this.disappearPattern = dissapearType;
			activeDistance = stage.inBossBattle ? Game1.Width * 2 : Game1.Width + 120; ;
			marginalDistance = stage.inBossBattle ? Game1.Width * 2 : 320;

			// Load後にテクスチャのサイズから取得した方がいいよな...?
			switch (textureType) {
				case 0:
					this.width = 32;
					this.height = 32;
					break;
				case 1:
					this.width = 32;
					this.height = 32;
					break;
				case 2:
					this.width = 32;
					this.height = 32;
					break;
				case 3:
					this.width = 32;
					this.height = 16;
					//degree = -180;
					break;
			}
			animation = new Animation(this.width, this.height);
			if (seName != "") {
				shootSound = game.Content.Load<SoundEffect>("Audio\\SE\\" + seName);
				shootSoundInstance = shootSound.CreateInstance();
				shootSoundInstance.Volume = .5f;
			}
			isHostile = true;

			Load();
		}

		protected override void Load()
		{
			switch (textureType) {
				case 0:
					texture = content.Load<Texture2D>("Object\\Turret&Bullet\\Bullet_type0");
					break;
				case 1:
					texture = content.Load<Texture2D>("Object\\Turret&Bullet\\Bullet_type1");
					break;
				case 2:
					texture = content.Load<Texture2D>("Object\\Turret&Bullet\\Bullet_type2");
					break;
				case 3:
					texture = content.Load<Texture2D>("Object\\Turret&Bullet\\Bullet_type3a");
					break;
				case 4:
					texture = content.Load<Texture2D>("Object\\Turret&Bullet\\airCutter");
					this.width = 32; this.height = 48;
					break;
			}

			reflectSound = content.Load<SoundEffect>("Audio\\SE\\magic");
		}
		public override void Update()
		{
			if (IsBeingUsed() && IsActive()) {
				UpdateAnimation();
				UpdateFlying(120);// 120 flyingTImeをどのように調整するか？
			}
			//if (turret != null && !turret.isBeingUsed) isShot = false;

			// 画面外に出たら消す
			if (isShot /*&& !isActive */&& IsOutside()) {// activeDistance ==760
				isAlive = false;
				isShot = false;
				//turret.hasShotBullets.Remove(this);// 全体的にここに到達せずにRemoveされてない
				stage.bullets.Remove(this);
			}
		}
		public override void UpdateAnimation()
		{
			switch (textureType) {
				case 0:
					animation.Update(3, 0, width, height, 6, 1);
					break;
				case 4:
					animation.Update(1, 0, 32, 48, 6, 1);
					break;
			}
		}

		/// <summary>
		/// 回収を想定して、射撃後の飛行が終わった後のパラメータの初期化を行うメソッド。Turretから呼んでもいいかもしれない
		/// </summary>
		public void EndFlying()
		{
			isShot = false;
			//isAlive = false;// これは気持ち悪い
			counter = 0;
			movedDistance = 0;
		}
		/// <summary>
		/// 「射撃後」の挙動のUpdate。（消滅条件など）
		/// </summary>
		/// <param name="flyingTime">消滅するまでの時間[frame]</param>
		public virtual void UpdateFlying(int flyingTime)
		{
			if (isShot) {
				if (counter == 0) {
					this.position = shootPosition;		// TUrretで全部やるならここのブロック要らないよな
					if (shootSoundInstance != null) shootSoundInstance.Play();
				}

				movedDistance += Math.Abs(speed.X);     // 加算して距離を計る(4dP2)
				counter++;

				// Bulletを使いまわさないTurretから撃たれた場合はここで終了処理は行わない
				switch (disappearPattern) {
					case 0:
						if (counter > flyingTime) EndFlying();
						break;
					case 1:
						//if () EndFlying();
						break;
					case 2:
						break;
				}

				UpdateNumbers();
			}
		}
		/// <summary>
		/// 弾は重力等の影響を受けない仕様なので、基本的に速度による位置の更新だけ
		/// </summary>
		protected override void UpdateNumbers()
		{
			if (System.Math.Abs(speed.Y) > maxSpeed) speed.Y = maxSpeed;

			// 位置に加算
			position += speed * new Vector2(timeCoef);

			// positionを更新
			if (turret != null) {
				shootPosition = turret.position;
			} else if (shootingEnemy != null) {// sEいらね
				shootPosition = shootingEnemy.position;
			}

			// 軌跡
			locus.Add(this.drawPos);
			if (locus.Count > 2) locus.RemoveAt(0);
		}
		public override void MotionUpdate()
		{
			double rad;
			float speed = 14;

			// 移動量を計算
			if (turret != null) {
				rad = Math.Atan2(turret.position.Y - stage.player.position.Y, turret.position.X - stage.player.position.X);
			} else {
				rad = 0;
			}
			this.speed = new Vector2((float)Math.Cos(rad) * speed, (float)Math.Sin(rad) * speed);

			if (!game.isMuted) reflectSound.Play(SoundControl.volumeAll, 0f, 0f);
		}
		/// <summary>
		/// カメラの位置を中心にすればoverrideする必要もないのだが....
		/// </summary>
		/// <returns></returns>
		protected override bool IsOutside()
		{
			return drawPos.X <= -width - marginalDistance || Game1.Width + marginalDistance <= drawPos.X
				   || drawPos.Y <= -height - marginalDistance || Game1.Height + marginalDistance <= drawPos.Y;
		}
		public override bool IsBeingUsed()
		{
			// turret.isBeingUsedの時に画面内のBulletが消えるのはきもい
			return isAlive && isActive && isShot;
		}

		// MotionUpdate(Obj)にしないと統一できん...
		public void isCollideWith(Object obj)
		{
			if (obj is Sword && obj.user is Player) {//obj is Weapon
				isHostile = false;
			} else if (obj is Sword && obj.user is Enemy) {
				if (!isHostile) {
					isEffected = true;
					damageEffected = true;
				}
				isHostile = true;
			}

			if (obj is Enemy || obj is Player) speed = Vector2.Zero;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (IsBeingUsed()) {
				if (textureType == 4) {// 角度の計算が必要なタイプのテクスチャなら
					spriteBatch.Draw(texture, drawPos, animation.rect, Color.Red, -rot - (float)Math.PI, Vector2.Zero, 1, SpriteEffects.None, 0f);
				} else {
					spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, -rot, Vector2.Zero, 1, SpriteEffects.None, 0f);
				}
			}
		}

	}
}
