using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
	/// <summary>
	/// Stage上のオブジェクト全て(Terrain, Bullet, Weapon, Character)の基本クラス。
	/// </summary>
	public class Object : ICloneable
	{
		protected static Game1 game;
		protected static SpriteBatch spriteBatch;
		protected static ContentManager content;
		public static void Inicialize(Game1 game, SpriteBatch spriteBatch, ContentManager content)
		{
			Object.game = game;
			Object.spriteBatch = spriteBatch;
			Object.content = content;
		}

		#region Member variable
		// const
		public readonly float timeCoefObject = 0.3f;
		public readonly float defActiveDistance = 640;
		public readonly int defHP = 3;

		/// <summary>
		/// デバッグ用
		/// </summary>
		protected int c, d;
		protected float dColor;

		// Basis
		public Stage stage { get; protected set; }
		public Animation animation { get; protected set; }
		public Animation animation2 { get; protected set; }
		public Texture2D texture, texture2;						// refで渡してる所があるのでプロパティ化できない.要修正
		public bool loadManually { get; set; }
		/// <summary>
		/// このObjectを使用しているObject
		/// </summary>
		public Object user { get; protected set; }
		/// <summary>
		/// 右を向いているか
		/// </summary>
		public bool turnsRight { get; internal set; } // protected

		// Physic
		/// <summary>
		/// 後でVector2→Vectorに変更する
		/// </summary>
		public Vector2 position;
		public Vector2 drawPos;
		public Vector2 speed;

		protected float degreeSpeed;
		internal int width, height;
		internal float degree, radius;
		public float timeCoef { get; set; }


		private double gravity = .60;
		public double Gravity
		{
			get { return this.gravity; }
			set { this.gravity = value; }
		}
		protected float accel = .40f;
		internal float friction = .40f;
		protected float maxSpeed = 32f;
		/// <summary>
		/// めり込み許容範囲
		/// </summary>
		internal int maxLength = 32;

		// behavior
		public bool isActive { get; internal set; }
		public bool isAlive { get; internal set; }
		public bool canBeDestroyed { get; protected set; }
		public bool isBeingUsed { get; internal set; }
		public bool hasAttacked { get; internal set; }
		public bool isAttacking { get; internal set; }
		public bool isDamaged { get; set; }
		public bool damageFromAttacking { get; set; }
		public bool damageFromTouching { get; set; }

		public bool isJumping { get; internal set; }
		public bool isExposed { get; internal set; }
		public bool isMovingRight { get; internal set; }         // GAR用
		public bool hasDashed { get; internal set; }
		/// <summary>
		/// 地形をすり抜けるタイプかどうか：Obstacleなどに活用
		/// </summary>
		public bool canSlipThrough { get; internal set; }
		/// <summary>
		/// 他の何かのObjectに当たっているか
		/// </summary>
		public bool isHit { get; internal set; }
		/// <summary>
		/// 何らかのObjectに乗っているかどうか
		/// </summary>
		public bool isOnSomething { get; internal set; }
		/// <summary>
		/// 水系の地形に触れたか：多分使わない
		/// </summary>
		public bool hasTouchedWater { get; internal set; }
		/// <summary>
		/// 水中にいるか
		/// </summary>
		public bool isInWater { get; internal set; }
		public int jumpCount;
		public double movedDistance;
		public float distanceToCamara;
		public float activeDistance { get; protected set; }
		public int HP { get; internal set; }

		// Effect
		/// <summary>
		/// 移動中かどうか。アニメーション時などに使用
		/// </summary>
		protected bool isMoving;
		/// <summary>
		/// Moveメソッドで使用。
		/// </summary>
		protected int curLoc;
		/// <summary>
		/// Moveメソッドで使用。
		/// </summary>
		protected bool isStartingMove;
		internal bool hasPlayedSE;
		public bool isEffected { get; set; }
		public bool damageEffected { get; set; }
		public bool hasDeathEffected { get; set; }
		public bool deathEffected { get; set; }
		public bool dashEffected { get; set; }

		// logs
		public List<Vector2> locus = new List<Vector2>();
		public List<int> locusDegree = new List<int>();
		public List<bool> prevIsHit = new List<bool>();

		// else
		/// <summary>
		///  多目的のカウンタ
		/// </summary>
		internal int counter, time;
		internal bool isFirstTimeInAFrame, isFirstTimevsCB, isHitCB;
		#endregion
		#region Constructors
		public Object()
			: this(null, 0, 0, 32, 32)
		{
		}
		public Object(Stage stage, int width, int height)
			: this(stage, 0, 0, width, height)
		{
		}
		public Object(Stage stage, float x, float y, int width, int height)
		{
			this.stage = stage;
			this.width = width;
			this.height = height;
			this.position.X = x;
			this.position.Y = y;

			activeDistance = defActiveDistance;
			HP = defHP;
			isAlive = true;
			animation = new Animation(width, height);
			locus.Add(Vector2.Zero);
		}
		#endregion
		#region Load
		protected virtual void Load()
		{
		}
		// ↓Stageからtextureを読み込ませていた時に使っていたメソッド
		public virtual void Load(ContentManager content)
		{
		}
		public virtual void Load(ContentManager content, string texture_name)
		{
			texture = content.Load<Texture2D>(texture_name);
		}
		public virtual void Load(ContentManager content, string texture_name, string texture_name2)
		{
			texture = content.Load<Texture2D>(texture_name);
			texture2 = content.Load<Texture2D>(texture_name2);
		}
		#endregion
		public virtual void Update()
		{
		}
		public virtual void UpdateAnimation()
		{
		}
		protected virtual void UpdateNumbers()
		{
			// 加速・減速
			/*if (isBlownAway) {
				scalarSpeed += gravityInAction;//438
				friction = frinctionAir;
			}
			else{
				scalarSpeed += gravity;
				friction = .30;
			}*/
			/*if (isOnSomething) scalarSpeed *= friction;
			else scalarSpeed *= frinctionAir;*/

			speed.Y += (float)Gravity * timeCoef;
			if (speed.X > 0) {
				speed.X += -(.40f * friction) * timeCoef;
				if (speed.X < 0) speed.X = 0;
			} else if (speed.X < 0) {
				speed.X += (.40f * friction) * timeCoef;
				if (speed.X > 0) speed.X = 0;
			}
			if (Math.Abs(speed.Y) > maxSpeed) speed.Y = maxSpeed;

			// 位置に加算
			position.X += speed.X * timeCoef;
			position.Y += speed.Y * timeCoef;

			// 端
			if (position.Y < 0) position.Y = 0;

			locus.Add(this.drawPos);
			if (locus.Count > 2) locus.RemoveAt(0);
		}
		public virtual void UpdateTimeCoef()
		{
			if (stage.slowmotion.isSlow)
				this.timeCoef = timeCoefObject;
			else
				this.timeCoef = 1.0f;
		}
		/// <summary>
		/// ダメージを受けたときの挙動を記述するメソッド。
		/// </summary>
		public virtual void MotionUpdate()
		{
		}
		/// <summary>
		/// 自身のスクリーン座標を、基準となる位置に合わせて更新するメソッド。
		/// </summary>
		/// <param name="criteriaPosition">基準となる位置</param>
		public virtual void ScrollUpdate(Vector2 criteriaPosition, bool autoScroll)// cameraPosの方がいいか...
		{
			drawPos.Y = position.Y;

			if (!autoScroll) {
				drawPos.X = position.X - criteriaPosition.X + Player.screenPosition.X;/**/
				//drawPos = position - criteriaPosition;
			} else {
				drawPos.X = position.X - criteriaPosition.X;
			}
			distanceToCamara = Math.Abs(criteriaPosition.X - position.X);
		}

		public virtual void IsHit(Object targetObject)
		{
			isHit = false;
			//targetObject.isOnSomething = false;
			if (targetObject.position.X + targetObject.width < position.X) {
			} else if (position.X + width < targetObject.position.X) {
			} else if (targetObject.position.Y + targetObject.height < position.Y) {
			} else if (position.Y + height < targetObject.position.Y) {
			} else {   // 当たりあり
				isHit = true;
				//targetObject.isOnSomething = true;

				// 当たり判定処理↓
				// 下に移動中
				if (targetObject.speed.Y > 0) {
					//if (targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width)//座標を指定しないとBlockに接してジャンプしたときにｷﾞﾘｷﾞﾘで乗ってしまう
					// ブロックの上端
					if (targetObject.position.Y + targetObject.height - position.Y < maxLength) {
						targetObject.speed.Y = 0;
						targetObject.position.Y = position.Y - (targetObject.height);  // 上に補正
						targetObject.isOnSomething = true;
					}
				}// 上に移動中
				else {
					// ブロックの下端
					//if (targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width)
					if (position.Y + height - targetObject.position.Y < maxLength) {
						targetObject.speed.Y = 0;
						targetObject.position.Y = position.Y + height;   // 下に補正
					}
				}
				// 右に移動中
				if (targetObject.speed.X > 0) {
					if (targetObject.position.Y > position.Y - targetObject.height && targetObject.position.Y < position.Y + height) {
						// ブロックの左端
						if (targetObject.position.X + targetObject.width - position.X < maxLength) {
							targetObject.position.X = position.X - targetObject.width;  // 左に補正
						}
					}

				} // 左に移動中
				else {
					if (targetObject.position.Y > position.Y - targetObject.height && targetObject.position.Y < position.Y + height) {
						// ブロックの右端
						if (position.X + width - targetObject.position.X < maxLength) {
							targetObject.position.X = position.X + width;   // 右に補正
						}
					}
				}
			}
		}
		/// <summary>
		/// プロタイプシューティングのぱくりだが、実質MotionUpdateがこれに相当するので要らない事に気づいた
		/// </summary>
		/// <param name="obj"></param>
		//public abstract void isCollideWith(Object obj);

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (IsBeingUsed()) {
				spriteBatch.Draw(texture, drawPos, Color.White);
			}
		}

		// 補助メソッド
		/// <summary>
		/// 自分をウェイポイントに沿って移動させるメソッド。使い始めにisStartingMove = trueにする必要がある。
		/// </summary>
		/// <param name="moveSpeed">移動速度[pixel/frame]</param>
		/// <param name="wayPoints">ウェイポイント配列</param>
		protected virtual void Move(float moveSpeed, params Vector2[] wayPoints)
		{
			if (isStartingMove) {
				isStartingMove = false;
				isMoving = true;
				curLoc = 0;
			} else if (isMoving) {
				// 予め引くと実際の移動の時とずれる！！
				Vector2 speed = Vector2.Normalize(wayPoints[curLoc] - position) * new Vector2(moveSpeed);
				float distance = Vector2.Distance(position, wayPoints[curLoc]);
				bool isReached = distance < moveSpeed;
				this.speed = speed;

				if (isReached) {
					curLoc++;
					isReached = false;
					if (curLoc == wayPoints.Length) curLoc = 0;
				}
			}
		}
		/// <summary>
		/// 往復移動させるメソッド。
		/// </summary>
		/// <param name="defPos"></param>
		/// <param name="moveDistance"></param>
		/// <param name="speed"></param>
		public virtual void RoundTripMotion(Vector2 defPos, float moveDistance, float speed)
		{
			if (isMovingRight && defPos.X - moveDistance <= position.X && position.X < defPos.X + moveDistance) {
				position.X += speed;
			} else if (position.X >= defPos.X + moveDistance) {
				isMovingRight = false;
				position.X += -speed;
			} else if (!isMovingRight && defPos.X - moveDistance <= position.X && position.X <= defPos.X + moveDistance) {
				position.X += -speed;
			} else if (position.X <= defPos.X - moveDistance) {
				isMovingRight = true;
				position.X += speed;
			}
		}
		public object Clone()
		{
			return MemberwiseClone();
		}
		public virtual bool IsActive()
		{
			return isAlive && isActive;
		}
		public virtual bool IsBeingUsed()
		{
			return user != null && isBeingUsed || user == null;
		}
		/// <summary>
		/// （当たり判定の円が）完全にゲーム画面から完全に外れているか。
		/// すなわち、少しでも表示されていればtrueを返す。
		/// Bulletなど、完全に見えなくなってから破棄したいオブジェクトで用いる。
		/// </summary>
		/// <returns></returns>
		protected bool IsOutside()
		{
			return position.X <= -width || Game1.Width  <= position.X
				   || position.Y <= -height|| Game1.Height <= position.Y;
		}
	}

	/// <summary>
	/// Objectの対を定義するクラス。
	/// </summary>
	public class Object2
	{
		public Object object1 { get; set; }
		public Object object2 { get; set; }

		public Object2(Object object1, Object object2)
		{
			this.object1 = object1;
			this.object2 = object2;
		}
	}
}
