using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
	/// <summary>
	/// 地形の基本クラス
	/// </summary>
	public class Terrain : Object
	{
		#region Member Variable
		/// <summary>
		/// 動的地形かどうか。
		/// </summary>
		public bool isDynamic { get; private set; }
		/// <summary>
		/// 自分の上下左右の地形との関係
		/// </summary>
		public bool isOn { get; set; }
		public bool isUnder { get; set; }
		public bool isRight { get; set; }
		public bool isLeft { get; set; }

		public bool isUnderSomeone { get; set; }
		public bool fadeOut { get; set; }

		// Slope
		public bool isRightSlope { get; set; }
		public bool isLeftSlope { get; set; }
		public int sideNum { get; set; }// CDで使用
		public Vector2 localPosition { get; set; }

		/// <summary>
		/// 属性を持たせるのに使用：Blockだと通常/凍ってる/燃えてるとか。これによってfrictionとかを変えればおｋ。汎用的
		/// </summary>
		public int type { get; set; }

		// General
		/// <summary>
		/// 斜面を構成するVector（デバッグ用）
		/// </summary>
		public Vector2 slopeVectorDown { get; set; }
		public Vector2 slopeVectorUp { get; set; }
		public Vector2 slopeVector { get; set; }

		/// <summary>
		/// targetObjectの回転してないときの位置ベクトル
		/// </summary>
		private Vector2[] defPositionVector = new Vector2[5];
		/// <summary>
		/// 矩形の辺に沿って引くベクトル
		/// </summary>
		private Vector2[] vectors = new Vector2[6];
		/// <summary>
		/// 外積
		/// </summary>
		private float[,] crossProduct = new float[5, 5];
		private float[] crossProduct2 = new float[3];
		#endregion
		#region Constructors
		public Terrain()
		{
		}
		public Terrain(Stage stage, float x, float y, int width, int height)
			: base(stage, x, y, width, height)
		{
		}
		public Terrain(Stage stage, float x, float y, int width, int height, int type)
			: base(stage, x, y, width, height)
		{
			this.type = type;
		}

		public Terrain(Stage stage, float x, float y, int width, int height, Vector2 localPosition)
			: base(stage, x, y, width, height)
		{
			this.localPosition = localPosition;
		}
		public Terrain(Stage stage, float x, float y, int width, int height, Object user, Vector2 localPosition)
			: base(stage, x, y, width, height)
		{
			this.user = user;
			this.localPosition = localPosition;
		}
		#endregion

		/// <summary>
		/// 諸々の変数を初期化する。外部から動的に使う場合に呼ぶ。
		/// </summary>
		public virtual void Initialize()
		{
		}

		/// <summary>
		/// 当たり判定と処理 デフォルトでブロックの処理
		/// </summary>
		/// <param name="targetObject"></param>
		public override void IsHit(Object targetObject)
		{
			ChangeFlags(targetObject);

			if (targetObject.position.X + targetObject.width < position.X) {
			} else if (position.X + width < targetObject.position.X) {
			} else if (targetObject.position.Y + targetObject.height < position.Y) {
			} else if (position.Y + height < targetObject.position.Y) {
			} else {
				// 当たりあり
				ChangeHitFlags(targetObject);

				if (targetObject.speed.Y > 0 && !isUnder) {// 下に移動中
					if (targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width) {
						//座標を指定しないとBlockに接してジャンプしたときにｷﾞﾘｷﾞﾘで乗ってしまう
						// ブロックの上端
						if (targetObject.position.Y + targetObject.height - position.Y < maxLength) {
							if (!stage.reverse.isReversed) {
								targetObject.speed.Y = 0;
								targetObject.position.Y = position.Y - targetObject.height;  // 上に補正
							}

							targetObject.isOnSomething = true;
							targetObject.jumpCount = 0;      // Playerに限定したかったが諦めた
							targetObject.isJumping = false;　// 着地したらJumpできるように
						}
					}
				} else if (targetObject.speed.Y < 0 && !isOn) {// 上に移動中
					if (!stage.reverse.isReversed) {
						if (targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width) {
							// ブロックの下端
							if (position.Y + height - targetObject.position.Y < maxLength) {
								targetObject.speed.Y = 0;
								targetObject.position.Y = position.Y + height;   // 下に補正
							}
						}
					}
				}
				if (targetObject.speed.X > 0 && !isRight) {// 右に移動中
					if (!stage.reverse.isReversed) {
						if (targetObject.position.Y > position.Y - targetObject.height && targetObject.position.Y < position.Y + height) {
							// ブロックの左端
							if ((targetObject.position.X + targetObject.width) - position.X < maxLength) {
								targetObject.position.X = position.X - targetObject.width;  // 左に補正
							}
						}
					}
				} else if (targetObject.speed.X < 0 && !isLeft) {// 左に移動中
					if (!stage.reverse.isReversed) {
						if (targetObject.position.Y + targetObject.height > position.Y && targetObject.position.Y < position.Y + height) {
							// ブロックの右端
							if ((position.X + width) - targetObject.position.X < maxLength) {
								targetObject.position.X = position.X + width;   // 右に補正
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// 範囲にあったらとにかく押し上げるパターン:Obstacle用
		/// </summary>
		/// <param name="targetObject"></param>
		public void IsHit2(Object targetObject)
		{
			if (isFirstTimeInAFrame) {
				isHit = false;// 複数の敵と判定するので結局falseになってしまう
				//isOnSomething = false;
			}
			targetObject.isHit = false;

			if (targetObject.position.X + targetObject.width < position.X) {
			} else if (position.X + width < targetObject.position.X) {
			} else if (targetObject.position.Y + targetObject.height < position.Y) {
			} else if (position.Y + height < targetObject.position.Y) {
			} else {
				Vector2 criterionVector = targetObject.position + new Vector2(targetObject.width / 2, targetObject.height);
				// 当たりあり
				isHit = true;// collapsingBlock:objectsのはtrueになってもdynamicTerrrainsのほうはtrueにならないでござる(解決)
				targetObject.isHit = true;
				//targetObject.isOnSomething ~ 
				isFirstTimeInAFrame = false;
				//ontop = false; onleft = false; onright = false;// 初期化(デバッグ用)

				// targetObjectが下に移動中
				//if (targetObject.speed.Y > 0 && !isUnder) {
				//座標を指定しないとBlockに接してジャンプしたときにｷﾞﾘｷﾞﾘで乗ってしまう
				// ブロックの上端

				if (targetObject.position.Y + targetObject.height - position.Y < maxLength) {
					targetObject.speed.Y = 0;
					targetObject.position.Y = position.Y - targetObject.height;  // 上に補正
					targetObject.isOnSomething = true;
					targetObject.jumpCount = 0;      // Playerに限定したかったが諦めた
					targetObject.isJumping = false;　// 着地したらJumpできるように

					if (type == 0) targetObject.friction = .40f;
					else if (type == 1) targetObject.friction = .05f;

					//ontop = true;
					isUnderSomeone = true;
				}

				// }
			}

		}
		/// <summary>
		/// 重なったら斜面上に押し出すだけの簡易版メソッド。地形としてはこのほうが使える
		/// </summary>
		/// <param name="targetObjecy"></param>
		public virtual void IsHitLite(Object targetObject)
		{
			slopeVectorDown = position + new Vector2(0, height);      // 斜面下側
			slopeVectorUp = position + new Vector2(width, 0);         // 斜面上側
			slopeVector = (slopeVectorUp - slopeVectorDown);        // 斜面のベクトル

			defPositionVector[0] = targetObject.position;
			//defPositionVector[1] = defPositionVector[0] + new Vector2(targetObject.width, 0);
			defPositionVector[3] = defPositionVector[0] + new Vector2(targetObject.width / 2, targetObject.height);

			vectors[0] = defPositionVector[3] - slopeVectorDown;

			crossProduct[0, 0] = (slopeVector.X * vectors[0].Y) - (vectors[0].X * slopeVector.Y);

			if (position.X + width < targetObject.position.X) {
			} else if (targetObject.position.X + targetObject.width < position.X) {
			} else if (targetObject.position.Y + targetObject.height < position.Y) {
			} else if (position.Y + height < targetObject.position.Y) {
			} else {
				// targetObjectは矩形内にいる
				if (crossProduct[0, 0] > 0) {
					targetObject.isHit = true;
					isHit = true;
					// 着地処理
					targetObject.isOnSomething = true;
					targetObject.jumpCount = 0;
					targetObject.isJumping = false;
					isUnderSomeone = true;

					float adjustDistance;
					targetObject.speed.Y = 0;

					if (defPositionVector[3].X > position.X && defPositionVector[3].X < position.X + width) {// 原因は範囲を絞ってなかったせい
						switch (type) {
							case 0:
								adjustDistance = (targetObject.position.X + targetObject.width / 2 - position.X);
								targetObject.position.Y = (position.Y + height - adjustDistance) - targetObject.height;
								break;
							case 1:
								adjustDistance = (targetObject.position.X + targetObject.width / 2 - position.X) * 1 / 2;// typeの倍数表現でまとめてもよい
								targetObject.position.Y = (position.Y + height - adjustDistance) - targetObject.height;
								break;
							case 2:
								adjustDistance = (targetObject.position.X + targetObject.width / 2 - position.X) * 1 / 4;
								targetObject.position.Y = (position.Y + height - adjustDistance) - targetObject.height;
								break;
						}
					}
					if (defPositionVector[3].X > position.X + width && targetObject.position.X < position.X + width)// スルー
						targetObject.position.Y = position.Y - targetObject.height;


				}
			}

		}

		// 補助メソッド
		protected virtual void ChangeFlags(Object targetObject)
		{
			if (targetObject.isFirstTimeInAFrame) { //3/14 もう何かに乗ってると判断されてるなら飛ばしてもいいかもしれない
				targetObject.isHit = false;// 複数の敵と判定するので結局falseになってしまう
				//(targetObject as Character).onConveyor = false;
				targetObject.isFirstTimeInAFrame = false;
				isHitCB = false;
				targetObject.isOnSomething = false;
				if (targetObject is Player) (targetObject as Player).isHitLeftSide = false;
			}
		}
		protected virtual void ChangeHitFlags(Object targetObject)
		{
			isHit = true;
			targetObject.isHit = true;
			isFirstTimeInAFrame = false;

		}

	}
}

