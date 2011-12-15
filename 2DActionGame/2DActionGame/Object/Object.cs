using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
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
		// Const
		protected readonly double defGravity = .60;
		protected readonly float accel = .40f;
		protected float maxSpeed = 32f;
		protected readonly float defFriction = .40f;
		/// <summary>
		/// めり込み許容範囲
		/// </summary>
		public readonly float maxLength = 32;
		public readonly float defActiveDistance = 640;//640;
		public readonly int defHitPoint = 3;
		public float timeCoefObject { get; protected set; }//readonly 0.3f

		// Basis
		public Stage stage { get; protected set; }
		public Animation animation { get; protected set; }
		public Animation animation2 { get; protected set; }
		public Texture2D texture { get; set; }
		public Texture2D texture2 { get; set; }
		/// <summary>
		/// このObjectを使用しているObject
		/// </summary>
		public Object user { get; protected set; }
		/// <summary>
		/// 右を向いているか
		/// </summary>
		public bool turnsRight { get; protected set; }
		public bool loadManually { get; protected set; }

		// Physic
		/// <summary>
		/// 後でVector（自作ラッパークラス）に変更する予定
		/// </summary>
		public Vector2 position, drawPos, speed;
		protected float degreeSpeed;
		public int width { get; protected set; }
		public int height { get; protected set; }
		public float degree;										//プロパティ化：class Turret, Bulletの修正
		public float radius;										//プロパティ化：class CDの修正
		public float timeCoef { get; internal set; }				//protected化：class MapObjectの修正(ﾑﾘ)	
		public double gravity { get; internal set; }
		public float friction { get; internal set; }
		public int HP { get; internal set; }
		/// <summary>
		/// 既に当たり判定がtrueになっているObjectを重複して判定させないためのフラグ
		/// </summary>
		internal bool firstTimeInAFrame, isFirstTimevsCB, isHitCB;	// 名前改良したい

		// Behavior
		/// <summary>
		/// アクティブかどうか（画面内＋一定の範囲にいる場合trueにされる）
		/// Updateメソッド又はUpdateメソッド内の処理を実行するかどうかの基準の１つになる。
		/// </summary>
		public bool isActive { get; internal set; }
		public bool isAlive { get; internal set; }
		public bool canBeDestroyed { get; protected set; }
		public bool isBeingUsed { get; internal set; }
		public bool hasAttacked { get; internal set; }
		public bool isAttacking { get; internal set; }
		public bool isJumping { get; internal set; }
		public bool isMovingRight { get; internal set; }
		public bool hasDashed { get; internal set; }
		public bool isDamaged { get; set; }
		public bool damageFromAttacking { get; set; }
		/// <summary>
		/// 完全にハードコーディング的なフラグなので可能ならDamageを抽象化して必要ない形に修正したい
		/// </summary>
		public bool damageFromThrusting { get; set; }
		public bool damageFromTouching { get; set; }
		/// <summary>
		/// 他の何かのObjectに当たっているか
		/// </summary>
		public bool isHit { get; internal set; }
		/// <summary>
		/// 何らかのObjectに乗っているかどうか
		/// </summary>
		public bool isOnSomething { get; internal set; }
		/// <summary>
		/// 地形をすり抜けるタイプかどうか：Obstacleなどに活用
		/// </summary>
		public bool canSlipThrough { get; internal set; }
		/// <summary>
		/// 水系の地形に触れたか：多分使わない
		/// </summary>
		public bool hasTouchedWater { get; internal set; }
		/// <summary>
		/// 水中にいるか
		/// </summary>
		public bool isInWater { get; internal set; }
		public float distanceToCamara { get; protected set; }
		public float activeDistance { get; protected set; }
		/// <summary>
		/// 多段ジャンプの段数を表す。
		/// </summary>
		public byte jumpCount { get; internal set; }

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
		/// <summary>
		/// WayPointsの最後の要素の位置まで到達したらtrue
		/// </summary>
		protected bool hasMoved;
		/// <summary>
		/// 交点にエフェクトを描画する処理を実装するのが間に合わないので、
		/// これの値をそれぞれのObjectがそれに近い位置に更新して使う
		/// </summary>
		public Vector2 effectPos { get; protected set; }
		internal bool hasPlayedSoundEffect;
		public bool isEffected { get; set; }
		public bool damageEffected { get; set; }
		public bool hasDeathEffected { get; set; }
		public bool deathEffected { get; set; }
		public bool dashEffected { get; set; }


		// Etc
		protected byte moveCount;
		/// <summary>
		/// デバッグ用
		/// </summary>
		protected int d;
		/// <summary>
		/// 描画時にα値を変える際に主に使う。
		/// </summary>
		protected float dColor;
		/// <summary>
		///  多目的のカウンタ
		/// </summary>
		internal int counter, time;
		/// <summary>
		/// 位置のログ
		/// </summary>
		public List<Vector2> locus = new List<Vector2>();
		/// <summary>
		/// 矩形Objectの傾きのログ
		/// </summary>
		public List<int> locusDegree = new List<int>();
		
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
			LoadXML("Object", "Xml\\Objects_Base.xml");
			this.stage = stage;
			this.width = width;
			this.height = height;
			this.position.X = x;
			this.position.Y = y;

			activeDistance = defActiveDistance;
			HP = defHitPoint;
			gravity = defGravity;
			friction = defFriction;
			isAlive = true;
			animation = new Animation(width, height);
			locus.Add(Vector2.Zero);
		}
		#endregion
		#region Load
		protected virtual void Load()
		{
		}
		/// <summary>
		/// Stageからtextureを読み込ませていた時に使っていたメソッド
		/// </summary>
		public virtual void Load(ContentManager content, string texture_name)
		{
			texture = content.Load<Texture2D>(texture_name);
		}
		public virtual void Load(ContentManager content, string texture_name, string texture_name2)
		{
			texture = content.Load<Texture2D>(texture_name);
			texture2 = content.Load<Texture2D>(texture_name2);
		}
		protected virtual void Initialize()
		{
		}

		/// <summary>
		/// XMLファイルからパラメータ値を読み込む。
		/// </summary>
		/// <see cref="http://www.kisoplus.com/file/xml2.html"/>
		/// <seealso cref="http://note.phyllo.net/?eid=540726"/>
		protected void LoadXML(string objectName, string fileName)
		{
			XmlReader xmlReader = XmlReader.Create(fileName);

			while (xmlReader.Read()) {// XMLファイルを１ノードずつ読み込む
				xmlReader.MoveToContent();

				if (xmlReader.NodeType == XmlNodeType.Element) {
					if (xmlReader.Name == "obj") {
						xmlReader.MoveToAttribute(0);
						//xmlReader.GetAttribute(0);
						if (xmlReader.Name == "Name" && xmlReader.Value == objectName) {
							// 以下、各パラメータを読み込む処理
							while (!(xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "obj")) {//xmlReader.NodeType == XmlNodeType.Element &&
								xmlReader.Read();

								Type type = this.GetType();
								xmlReader.MoveToFirstAttribute();
								if (xmlReader.Name == "type") {
									switch (xmlReader.Value) {
										case "property":
											xmlReader.MoveToContent();
											PropertyInfo pInfo = type.GetProperty(xmlReader.Name);
											if (pInfo != null) {// whiteSpaceなどの場合はここで回避
												SetProperty(xmlReader, pInfo);
											}
											break;
										case "field":
											xmlReader.MoveToContent();
											// http://dobon.net/vb/dotnet/programing/typeinvokemember.html ↓
											FieldInfo fInfo = type.GetField(xmlReader.Name, BindingFlags.Public | BindingFlags.NonPublic |
												BindingFlags.Instance | BindingFlags.Static);// defaultでpublicフィールドしか検索されない...だと...!?
											if (fInfo != null) {
												SetField(xmlReader, fInfo);
											}
											break;
									}
								}
							}
						}
					}
				}

			}
		}
		protected void SetProperty(XmlReader xmlReader, PropertyInfo pInfo)
		{
			string str = "";

			switch (pInfo.PropertyType.Name) {
				case "Byte":
					pInfo.SetValue(this, Byte.Parse(xmlReader.ReadString()), null);
					break;
				case "Int32":
					pInfo.SetValue(this, Int32.Parse(xmlReader.ReadString()), null);
					break;
				case "Single":
					pInfo.SetValue(this, float.Parse(xmlReader.ReadString()), null);//Single
					break;
				case "Double":
					pInfo.SetValue(this, Double.Parse(xmlReader.ReadString()), null);
					break;
				case "Vector2":
					string[] tmp = new string[2];
					str = xmlReader.ReadString();
					tmp = str.Split(',');
					pInfo.SetValue(this, new Vector2(float.Parse(tmp[0]), float.Parse(tmp[1])), null);
					break;
			}
			//pInfo.SetValue(this, Int32.Parse(xmlReader.ReadString()), null);// ｷﾀｰ
		}
		protected void SetField(XmlReader xmlReader, FieldInfo fInfo)
		{
			string str = "";

			switch (fInfo.FieldType.Name) {
				case "Byte":
					fInfo.SetValue(this, Byte.Parse(xmlReader.ReadString()));
					break;
				case "Int32":
					fInfo.SetValue(this, Int32.Parse(xmlReader.ReadString()));
					break;
				case "Single":
					fInfo.SetValue(this, float.Parse(xmlReader.ReadString()));
					break;
				case "Double":
					fInfo.SetValue(this, Double.Parse(xmlReader.ReadString()));
					break;
				case "Vector2":
					string[] tmp = new string[2];
					str = xmlReader.ReadString();
					tmp = str.Split(',');
					fInfo.SetValue(this, new Vector2(float.Parse(tmp[0]), float.Parse(tmp[1])));
					break;
			}
		}
		protected virtual bool IsMyClass(XmlReader xmlReader)
		{
			return xmlReader.Name == "Object";// 再帰的に判定して値を設定しないと継承ツリー末端のクラスにしか設定できない...
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
			isMovingRight = speed.X > 0;

			speed.Y += (float)gravity * timeCoef;
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

			// 軌跡
			locus.Add(position);
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
		public virtual void ScrollUpdate(Vector2 criteriaPosition, bool autoScroll)
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
		public object Clone()
		{
			return MemberwiseClone();
		}
		/// <summary>
		/// 自分をウェイポイントに沿って移動させるメソッド。使い始めにisStartingMove = trueにする必要がある。
		/// </summary>
		/// <param name="moveSpeed">移動速度[pixel/frame]</param>
		/// <param name="wayPoints">ウェイポイント配列</param>
		protected virtual void Move(float moveSpeed, Vector2[] wayPoints)
		{
			if (isStartingMove) {
				isStartingMove = hasMoved = false;
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
					if (curLoc == wayPoints.Length) {
						curLoc = 0;
						//hasMoved = true;
					}
				}
			}
		}
		protected virtual void RoundTripMotion(float speed, float leftPos, float rightPos)
		{
			/*if (moveCount == 0) {
				this.speed.X = speed;
				moveCount++;
			}*/
			this.speed.X = turnsRight ? -speed : speed;// frictionがあるから常に代入し続ける必要がある

			if (position.X > rightPos) {
				this.speed.X *= -1;
				isMovingRight = false;
			} else if (position.X < leftPos) {
				this.speed.X *= -1;
				isMovingRight = true;
			}
		}
		/// <summary>
		/// 往復移動させるメソッド。
		/// </summary>
		/// <param name="defPos"></param>
		/// <param name="moveDistance"></param>
		/// <param name="speed"></param>
		protected virtual void RoundTripMotion(Vector2 defPos, float moveDistance, float speed)
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
		protected virtual bool IsOutside()
		{
			return drawPos.X <= -width || Game1.Width <= drawPos.X
				   || drawPos.Y <= -height || Game1.Height <= drawPos.Y;
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
