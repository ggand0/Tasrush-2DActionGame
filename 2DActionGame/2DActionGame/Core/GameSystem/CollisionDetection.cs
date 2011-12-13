using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _2DActionGame
{
	/// <summary>
	/// 当たり判定処理を行うクラス。
	/// </summary>
	public static class CollisionDetection
	{
		#region Member valuable
		public static Game1 game { get; set; }
		private static readonly int maxLength = 32;

		// CDinAction1 & RectangleCross:
		
		/// <summary>
		/// 矩形の辺に沿って引くベクトル
		/// </summary>
		public static Vector2[] vectors = new Vector2[6];
		/// <summary>
		/// 自矩形の頂点から、比較対象矩形の頂点に引くベクトル
		/// </summary>
		public static Vector2[,] targetVector = new Vector2[4, 4];
		/// <summary>
		/// 自矩形の回転してないときの位置ベクトル testで一旦5
		/// </summary>
		public static Vector2[] defPositionVector = new Vector2[5];
		/// <summary>
		/// 回転後の自矩形の位置ベクトル
		/// </summary>
		public static Vector2[] myPositionVector = new Vector2[4];
		/// <summary>
		/// 対象矩形の位置ベクトル
		/// </summary>
		public static Vector2[] targetPositionVector = new Vector2[4];
		/// <summary>
		/// 外積
		/// </summary>
		public static float[,] crossProduct = new float[5, 5];

		// RectangleCross:

		/// <summary>
		/// 等式左辺の値
		/// </summary>
		public static float[] Result = new float[5];
		// 交差判定有のときの4点
		//private static Vector2 myEndPoint1, myEndPoint2;
		//private static Vector2 targetEndPoint1, targetEndPoint2;

		/// CDatTerrain2:
		public static Vector2 prevVector, curVector;															// 相手矩形の現在位置と前の位置とを結ぶベクトル
		//public static Vector2[] vectors2 = new Vector2[7];
		/// <summary>
		/// 自矩形頂点の位置ベクトル
		/// </summary>
		public static Vector2[] apexVector = new Vector2[5];
		/// <summary>
		/// 外積計算用：1辺を判定するのに6つ必要
		/// </summary>
		public static Vector2[,] vectorFromMyself = new Vector2[4, 7], vectorFromTarget = new Vector2[4, 7];
		/// <summary>
		/// 外積
		/// </summary>
		public static float[,] crossProduct2 = new float[5, 5];
		public static bool[] isHits = new bool[4] { isHitTop, isHitBottom, isHitLeft, isHitRight };
		public static bool isHitTop, isHitBottom, isHitLeft, isHitRight;

		/// <summary>
		/// 交差点
		/// </summary>
		public static Vector2 crossPoint;
		public static Vector2 adjustment;
		public static float ratio1, ratio2;

		// RectangleCrossDetailed
		private static Vector2[] locusVectors = new Vector2[5];
		private static int locusDegree;
		private static int[] locusDegrees = new int[33];

		// RightTriangle
		public static int sideNum;
		private static Vector2 speedVector;
		private static float[] crossProducts2 = new float[3];
		// CDatTerrain4
		public static Vector2[] crossPoints = new Vector2[4];
		private static bool[] isCrossSide = new bool[3];
		#endregion
		static CollisionDetection()
		{
		}

		/// <summary>
		/// 地形との衝突など簡易な判定
		/// </summary>
		public static void CDatTerrain(Object myObject, Block targetObject)
		{
			targetObject.isHit = false;
			if (targetObject.position.X + targetObject.width < myObject.position.X) {
			} else if (myObject.position.X + myObject.width < targetObject.position.X) {
			} else if (targetObject.position.Y + targetObject.height < myObject.position.Y) {
			} else if (myObject.position.Y + myObject.height < targetObject.position.Y) {
			} else {   // 当たりあり
				targetObject.isHit = true;

				// 当たり判定処理
				// 下に移動中
				if (myObject.speed.Y > 0 && !targetObject.isUnder) {
					if (targetObject.position.X + targetObject.width > myObject.position.X && targetObject.position.X < myObject.position.X + myObject.width) {
						//座標を指定しないとBlockに接してジャンプしたときにｷﾞﾘｷﾞﾘで乗ってしまう
						// ブロックの上端
						if (targetObject.position.Y + targetObject.height - myObject.position.Y < targetObject.maxLength) {
							myObject.speed.Y = 0;
							targetObject.position.Y = myObject.position.Y - targetObject.height;  // 上に補正
							myObject.jumpCount = 0;
							//myObject.isJumping = false;　// 着地したらJumpできるように
							targetObject.ontop = true;
						}
					}
				}// 上に移動中
				else if (myObject.speed.Y < 0 && !targetObject.isOn) {
					if (targetObject.position.X + targetObject.width > myObject.position.X && targetObject.position.X < myObject.position.X + myObject.width) {
						// ブロックの下端
						if (myObject.position.Y + myObject.height - targetObject.position.Y < targetObject.maxLength) {
							myObject.speed.Y = 0;
							targetObject.position.Y = myObject.position.Y + myObject.height;   // 下に補正
						}
					}
				}

				// 右に移動中
				//if (myObject.scalarSpeed/* + System.Math.Abs(speed.X)*/ > 0) 
				if (myObject.speed.X /*- (game.isScrolled ? targetObject.scalarSpeed : 0)*/ > 0) {//(stage.game.isScrolled ? 0 : this.scalarSpeed))
					if (targetObject.position.Y > myObject.position.Y - targetObject.height && targetObject.position.Y < myObject.position.Y + myObject.height) {
						// ブロックの左端
						if ((targetObject.position.X + targetObject.width) - myObject.position.X < targetObject.maxLength) {
							targetObject.position.X = myObject.position.X - targetObject.width;  // 左に補正
							targetObject.onleft = true;
						}
					}
				} // 左に移動中
				else { //if (myObject.scalarSpeed < 0)
					if (targetObject.position.Y + targetObject.height > myObject.position.Y && targetObject.position.Y < myObject.position.Y + myObject.height) {
						// ブロックの右端
						if ((myObject.position.X + myObject.width) - targetObject.position.X < targetObject.maxLength) {
							targetObject.position.X = myObject.position.X + myObject.width;   // 右に補正
							targetObject.onright = true;
						}
					}
				}
			}
		}

		#region CDatTerrain2
		/// <summary>
		/// ベクトルを用いた普通のブロック用の衝突判定メソッド。
		/// 銃弾/ビームなど動きの速いObjectの判定を想定
		/// 自矩形は地形で(動かない),相手が動く対象　地形の辺と対象の位置で交差判定する
		/// </summary>
		public static void CDatTerrain2(Object myObject, Object targetObject)
		{
			Calculate_Apexes(myObject);
			Calculate_Vectors(myObject, targetObject);
			Calculate_CrossProducts();
			Is_Cross2(targetObject);
			Calculate_CrossPoint();
			AdjustObject(myObject, targetObject);
		}
		private static void Calculate_Apexes(Object myObject)
		{
			apexVector[0] = myObject.position;
			apexVector[1] = myObject.position + new Vector2(myObject.width, 0);
			apexVector[2] = myObject.position + new Vector2(myObject.width, myObject.height);
			apexVector[3] = myObject.position + new Vector2(0, myObject.height);
		}
		private static void Calculate_Vectors(Object myObject, Object targetObject)
		{
			// 1回の判定につき自矩形から3本、相手矩形から3本引く必要あり
			/*vectorFromTarget[0, 0] = targetObject.position - targetObject.locus;// ABﾍﾞｸﾄﾙ
			vectorFromTarget[0, 1] = apexVector[0] - targetObject.locus;// AC
			vectorFromTarget[0, 2] = apexVector[1] - targetObject.locus;// AD

			vectorFromMyself[0, 0] = targetObject.locus - apexVector[0];// CA
			vectorFromMyself[0, 1] = targetObject.position - apexVector[0];// CB
			vectorFromMyself[0, 2] = apexVector[1] - apexVector[0];// CD*/

			// targetの速度で軌跡を判定する点を補正する(暫定) 10/21:drawVectorでやるべき？
			prevVector = targetObject.locus[0];
			curVector = targetObject.position;

			if (targetObject.speed.Y > 0) {
				prevVector += new Vector2(0, targetObject.height);
				curVector += new Vector2(0, targetObject.height);
			}
			if (targetObject.speed.X > 0) {
				prevVector += new Vector2(targetObject.width, 0);
				curVector += new Vector2(targetObject.width, 0);
			}

			for (int i = 0; i < 3; i++) {
				// for(int i=0;i<3;i++) {
				vectorFromTarget[i, 0] = curVector - prevVector;
				vectorFromTarget[i, 1] = apexVector[i] - prevVector;
				vectorFromTarget[i, 2] = apexVector[i + 1] - prevVector;

				vectorFromMyself[i, 0] = apexVector[i + 1] - apexVector[i];
				vectorFromMyself[i, 1] = targetObject.position - apexVector[i];
				vectorFromMyself[i, 2] = prevVector - apexVector[i];
			}
			vectorFromTarget[3, 0] = curVector - prevVector;
			vectorFromTarget[3, 1] = apexVector[3] - prevVector;
			vectorFromTarget[3, 2] = apexVector[0] - prevVector;

			vectorFromMyself[3, 0] = apexVector[0] - apexVector[3];
			vectorFromMyself[3, 1] = curVector - apexVector[3];
			vectorFromMyself[3, 2] = prevVector - apexVector[3];
		}
		private static void Calculate_CrossProducts()
		{
			//crossProduct[0, 0] = (vectorFromTarget[0, 0].X * vectorFromTarget[0, 1].Y) - (vectorFromTarget[0, 1].X * vectorFromTarget[0, 0].Y);
			for (int i = 0; i < 4; i++) {
				crossProduct2[i, 0] = (vectorFromTarget[i, 0].X * vectorFromTarget[i, 1].Y) - (vectorFromTarget[i, 1].X * vectorFromTarget[i, 0].Y);//0 1
				crossProduct2[i, 1] = (vectorFromTarget[i, 0].X * vectorFromTarget[i, 2].Y) - (vectorFromTarget[i, 2].X * vectorFromTarget[i, 0].Y);//0 2 *
				crossProduct2[i, 2] = (vectorFromMyself[i, 0].X * vectorFromMyself[i, 1].Y) - (vectorFromMyself[i, 1].X * vectorFromMyself[i, 0].Y);//0 1
				crossProduct2[i, 3] = (vectorFromMyself[i, 0].X * vectorFromMyself[i, 2].Y) - (vectorFromMyself[i, 2].X * vectorFromMyself[i, 0].Y);//0 2
			}
		}
		private static void Is_Cross2(Object targetObject)
		{
			/*for(int i=0;i<4;i++) {
				//isHits[i] = false;
				isHitTop = false; isHitBottom = false; isHitLeft = false; isHitRight = false;
				if((crossProduct2[i,0] * crossProduct2[i,1]) <= 0 && (crossProduct2[i,2] * crossProduct2[i,3]) <= 0) {
					isHits[i] = true; 
				}
			}*/
			// 初期化
			isHitTop = false; isHitBottom = false; isHitLeft = false; isHitRight = false;
			for (int i = 0; i < 4; i++) isHits[i] = false;

			// 境界は含めるべきか？ ずっとtrueになり続けるのはPlayerのprevVector[0]がずっと(0,0)で更新されていないことが原因←解決
			// dObj.vectorの軌跡しかみていないので、幅と高さを考慮に加えた仕様にする必要あり
			// →はじめにtargetの速度によって判定する点を変えればよい
			// isHIts[i]で管理した方が楽だろう
			if ((crossProduct2[0, 0] * crossProduct2[0, 1]) <= 0 && (crossProduct2[0, 2] * crossProduct2[0, 3]) <= 0
				&& targetObject.speed.Y > 0) { isHitTop = true; isHits[0] = true; } else if ((crossProduct2[1, 0] * crossProduct2[1, 1]) <= 0 && (crossProduct2[1, 2] * crossProduct2[1, 3]) <= 0
				&& targetObject.speed.Y < 0) { isHitBottom = true; isHits[1] = true; }
			if ((crossProduct2[2, 0] * crossProduct2[2, 1]) <= 0 && (crossProduct2[2, 2] * crossProduct2[2, 3]) <= 0
				&& targetObject.speed.X > 0) { isHitLeft = true; isHits[2] = true; } else if ((crossProduct2[3, 0] * crossProduct2[3, 1]) <= 0 && (crossProduct2[3, 2] * crossProduct2[3, 3]) <= 0
				&& targetObject.speed.X < 0) { isHitRight = true; isHits[3] = true; }
		}
		private static void Calculate_CrossPoint()
		{   // 交差点をまたぐ比を計算する←触れた瞬間に画面上端にワープするので多分(0,0)以下
			//for(int i=0;i<4;i++) {
			//if(isHitTop) {
			// ex) CAとCDの外積:CBとCD外積
			ratio1 = (crossProduct2[0, 2]);//(vectorFromMyself[i, 1].X * vectorFromMyself[i, 0].Y) - (vectorFromMyself[i, 0].X * vectorFromMyself[i, 1].Y));
			ratio2 = (crossProduct2[0, 3]);//(vectorFromMyself[i, 2].X * vectorFromMyself[i, 0].Y) - (vectorFromMyself[i, 0].X * vectorFromMyself[i, 2].Y));
			//}
			// }
			crossPoint = prevVector + (curVector - prevVector) * (ratio1 / (ratio1 + ratio2));
			//adj = curVector + (locus - curVector) * (ratio2 / (ratio1 + ratio2));
		}
		private static void AdjustObject(Object my_object, Object target_object)
		{   // my_objectを基準にすると辺のどこに触れても左端に補正されてしまうので、交差点を出す必要がある
			// 座標は補正されたが埋まる

			if (isHitTop && target_object.speed.Y > 0) {
				target_object.speed.Y = 0;
				target_object.position = crossPoint - new Vector2(0, target_object.height);
			} else if (isHitBottom && target_object.speed.Y < 0) target_object.position = crossPoint + new Vector2(0, my_object.height);
			if (isHitLeft && target_object.speed.X > 0) target_object.position = crossPoint - new Vector2(target_object.width, 0);
			else if (isHitRight && target_object.speed.X < 0) target_object.position = crossPoint + new Vector2(my_object.width, 0);
		}
		#endregion

		#region SquareDetection
		public static void Square(Block myObject, Object targetObject)
		{
		}
		public static void CalculateVectors4(Block myObject, Object targetObject)
		{
			// targetObjectの矩形の判定に使う点を求める
			defPositionVector[0] = targetObject.position;
			defPositionVector[1] = defPositionVector[0] + new Vector2(targetObject.width, 0);
			defPositionVector[2] = defPositionVector[0] + new Vector2(targetObject.width, targetObject.height);
			defPositionVector[3] = defPositionVector[0] + new Vector2(targetObject.width / 2, targetObject.height);
			defPositionVector[4] = defPositionVector[0] + new Vector2(0, targetObject.height);

			// 直角三角形の辺ベクトルを計算(時計回り)
			/*                    __________ [0]End
			 *  [0]Start([3]End) |          |
			 *                   |          |
			 *                   |          |
			 *                   |          |
			 *                   |          |
			 *           [2]End  |__________|[1]End
			 */
			myObject.sideVectorStart[0] = myObject.position + new Vector2(0, myObject.height);            // 斜面下側
			myObject.sideVectorEnd[0] = myObject.position + new Vector2(myObject.width, 0);               // 斜面上側

			myObject.sideVectorStart[1] = myObject.sideVectorEnd[0];                                    // 右端上側
			myObject.sideVectorEnd[1] = myObject.position + new Vector2(myObject.width, myObject.height); // 右端下側

			myObject.sideVectorStart[2] = myObject.sideVectorEnd[1];                                    // 下端右側
			myObject.sideVectorEnd[2] = myObject.sideVectorStart[0];                                    // 下端左側

			// foreachは割り当てには使えない
			for (int i = 0; i < myObject.sideVector.Length; i++)
				myObject.sideVector[i] = myObject.sideVectorEnd[i] - myObject.sideVectorStart[i];
		}
		#endregion
		#region RightTriangleDetection
		/// <summary>
		/// ベクトルを用いた斜面用の衝突判定メソッドを直角三角形の全ての辺にぶつかるように拡張したもの
		/// </summary>
		/// <param name="myObject"></param>
		/// <param name="targetObject"></param>
		public static void RightTriangle(Slope myObject, Object targetObject)
		{
			// ３辺のベクトルの右左ではいってくる方向わかるじゃないか　何故気づかなかった...
			CalculateVectors3(myObject, targetObject);// 辺ではなく頂点
			CalculateCrossProduct3(myObject, targetObject);
		}
		private static void CalculateVectors3(Slope myObject, Object targetObject)
		{
			// targetObjectの矩形の判定に使う点を求める
			defPositionVector[0] = targetObject.position;
			defPositionVector[1] = defPositionVector[0] + new Vector2(targetObject.width, 0);
			defPositionVector[2] = defPositionVector[0] + new Vector2(targetObject.width, targetObject.height);
			defPositionVector[3] = defPositionVector[0] + new Vector2(targetObject.width / 2, targetObject.height);
			defPositionVector[4] = defPositionVector[0] + new Vector2(0, targetObject.height);

			// 直角三角形の辺ベクトルを計算(時計回り)
			/*                         [0]End
			 *                        /|
			 *                       / |
			 *                      /  |
			 *                     /   |
			 *                    /    |
			 *   [0]Start([2]End)/_____|[1]End
			 */
			myObject.sideVectorStart[0] = myObject.position + new Vector2(0, myObject.height);				// 斜面下側
			myObject.sideVectorEnd[0] = myObject.position + new Vector2(myObject.width, 0);					// 斜面上側

			myObject.sideVectorStart[1] = myObject.sideVectorEnd[0];										// 右端上側
			myObject.sideVectorEnd[1] = myObject.position + new Vector2(myObject.width, myObject.height);	// 右端下側

			myObject.sideVectorStart[2] = myObject.sideVectorEnd[1];										// 下端右側
			myObject.sideVectorEnd[2] = myObject.sideVectorStart[0];										// 下端左側

			// foreachは割り当てには使えない
			for (int i = 0; i < myObject.sideVector.Length; i++)
				myObject.sideVector[i] = myObject.sideVectorEnd[i] - myObject.sideVectorStart[i];
		}
		private static void CalculateCrossProduct3(Slope myObject, Object targetObject)
		{
			//myObject.sideNum = 0;
			vectors[0] = defPositionVector[2] - myObject.sideVectorStart[0];// 3
			vectors[1] = defPositionVector[0] - myObject.sideVectorStart[1];// 4 1
			vectors[2] = defPositionVector[0] - myObject.sideVectorStart[2];//

			CheckDirection(myObject, targetObject);

			for (int i = 0; i <= 2; i++)
				crossProduct[0, i] = (myObject.sideVector[i].X * vectors[i].Y) - (vectors[i].X * myObject.sideVector[i].Y);

			// 初期化
			targetObject.isHit = false;
			myObject.isHit = false;
			myObject.isUnderSomeone = false;
			targetObject.isOnSomething = false;

			// マップチップの矩形領域内に入っているか否かで大まかに絞る
			if (myObject.position.X + myObject.width < targetObject.position.X) {
			} else if (targetObject.position.X + targetObject.width < myObject.position.X) {
			} else if (targetObject.position.Y + targetObject.height < myObject.position.Y) {
			} else if (myObject.position.Y + myObject.height < targetObject.position.Y) {
			} else {
				// targetObjectは矩形内にいる
				if (targetObject.speed.Y > 0 && myObject.sideNum == 0 && crossProduct[0, 0] > 0) {
					targetObject.isHit = true;
					myObject.isHit = true;

					// 着地処理
					targetObject.isOnSomething = true;
					targetObject.jumpCount = 0;
					targetObject.isJumping = false;
					myObject.isUnderSomeone = true;

					adjustObject3(myObject, targetObject);
				}
				if (targetObject.speed.X < 0 && myObject.sideNum == 1 && crossProduct[0, 1] > 0) {// 右側が壁の斜面チップを対象
					targetObject.isHit = true;
					myObject.isHit = true;
					if (myObject.position.X + myObject.width - targetObject.position.X < myObject.maxLength) {
						if (targetObject.position.Y + targetObject.height > myObject.position.Y && targetObject.position.Y < myObject.position.Y + myObject.height) {
							targetObject.position.X = myObject.position.X + myObject.width;// 右に補正
						}
					}
				} else if (targetObject.speed.Y < 0 && myObject.sideNum == 2 && crossProduct[0, 2] > 0) {
					targetObject.isHit = true;
					myObject.isHit = true;
					if (targetObject.position.X + targetObject.width > myObject.position.X && targetObject.position.X < myObject.position.X + myObject.width) {
						if (myObject.position.Y + myObject.height - targetObject.position.Y < maxLength) {
							targetObject.speed.Y = 0;
							targetObject.position.Y = myObject.position.Y + myObject.height;// 下に補正
						}
					}
				}
			}

		}
		private static void CheckDirection(Slope myObject, Object targetObject)
		{
			for (int i = 0; i <= 2; i++) {
				crossProducts2[i] = (myObject.sideVector[i].X * vectors[i].Y) - (vectors[i].X * myObject.sideVector[i].Y);
				if (crossProducts2[i] < 0) myObject.sideNum = i;
			}
		}
		private static void CalculateCrossPoint3(Slope myObject, Object targetObject)
		{
			/*　alice syndromeが...
			 * 交点＝Ａ＋ベクトルＡＢ＊①/ (①＋②)
			 * ①：(defPositionVector[sideNum]   - myObject.slopeVectorDown) × slopeVector
			 * ②：(defPositionVector[sideNum+1] - myObject.slopeVectorDown) × slopeVector
			 */
			float crossProduct1 = 0;
			float crossProduct2 = 0;

			// Characterの速度ベクトルを引く point3の軌跡にしてみる
			prevVector = targetObject.locus[0] + new Vector2(targetObject.width, targetObject.height);
			speedVector = curVector - prevVector;// ここまではおｋそう

			// 下記はCharacterの辺との交点計算用
			/*crossProduct1 = (defPositionVector[sideNum].X - myObject.slopeVectorDown.X) * myObject.slopeVector.Y 
				- myObject.slopeVector.X * (defPositionVector[sideNum].Y - myObject.slopeVectorDown.Y);
			crossProduct2 = (defPositionVector[sideNum+1].X - myObject.slopeVectorDown.X) * myObject.slopeVector.Y
				- myObject.slopeVector.X * (defPositionVector[sideNum+1].Y - myObject.slopeVectorDown.Y);

			crossPoint = defPositionVector[sideNum]
				+ new Vector2 (vectors[sideNum].X * crossProduct1 / crossProduct1 + crossProduct2, 
					vectors[sideNum].Y * crossProduct1 / crossProduct1 + crossProduct2);*/

			crossProduct1 = (prevVector.X - myObject.slopeVectorDown.X) * myObject.slopeVector.Y// もしかして正負逆転の必要？それはないか...
				- myObject.slopeVector.X * (prevVector.Y - myObject.slopeVectorDown.Y);
			crossProduct2 = (curVector.X - myObject.slopeVectorDown.X) * myObject.slopeVector.Y
				- myObject.slopeVector.X * (curVector.Y - myObject.slopeVectorDown.Y);//Math.Abs()やったらもっとひどくなった

			// X成分はまともだがYが異常である
			crossPoint = prevVector
				+ new Vector2(speedVector.X * (crossProduct1 / (crossProduct1 + crossProduct2)),// crosuProduct2がおかしい　そもそも比が－ってなんだ
					speedVector.Y * (crossProduct1 / (crossProduct1 + crossProduct2)));// ratioを()でくくるのを忘れてた＊２
		}
		private static void adjustObject3(Slope myObject, Object targetObject)
		{
			Vector2 criterionVector = defPositionVector[3];//new Vector2(targetObject.position.X + targetObject.width / 2, targetObject.position.X + targetObject.height);// 底辺の中点
			float adjustDistance = 0;

			/*if (myObject.degree == 45)
				adjustDistance = targetObject.position.X + targetObject.width / 2 - myObject.position.X;
			else if (myObject.degree == 22.5f)
				adjustDistance = 5;
			else if (myObject.degree == 11.25f)
				adjustDistance = 5;*/

			// CDクラスでは一般的に書いて場合分けはSlope classでやろう
			targetObject.speed.Y = 0;

			if (criterionVector.X > myObject.position.X && criterionVector.X < myObject.position.X + myObject.width) {
				adjustDistance = targetObject.position.X + targetObject.width / 2 - myObject.position.X;
				targetObject.position.Y = myObject.position.Y + myObject.height - adjustDistance;
				targetObject.position.Y -= targetObject.height;
			}
				// 上端付近での当たり判定
			else if (criterionVector.X > myObject.position.X + myObject.width && targetObject.position.X < myObject.position.X + myObject.width) {
				//if(targetObject.position.Y -targetObject.height == myObject.position.Y)
				//if (targetObject.position.Y + targetObject.height - myObject.position.Y < myObject.maxLength) {
				targetObject.position.Y = myObject.position.Y - targetObject.height;  // 上に補正
				//targetObject.position.Y = myObject.position.Y - targetObject.height;// 上端付近

			}
				// 下端付近での当たり判定
			else if (criterionVector.X < myObject.position.X && targetObject.position.X + targetObject.width > myObject.position.X) {
				targetObject.position.Y = myObject.position.Y + myObject.height - targetObject.height; // これだと上るときに引っかかる。斜面の高さに合わせる
				//targetObject.gravity = 0;
				//adjustDistance = targetObject.position.X + targetObject.width - myObject.position.X;
				//targetObject.position.Y = (myObject.position.Y + myObject.height /*- adjustDistance*/) - targetObject.height;
			}
		}
		#endregion
		#region CDatTerrain4
		/// <summary>
		/// 斜面ベクトルとの交差判定での当たり判定のテスト：矩形内にある→速度ベクトルと斜面ベクトルとで交差判定→
		/// 交差してたら交点計算→prevVectorと比較して一番近い交点を持つ辺を判定。
		/// </summary>
		public static void CDatTerrain4(Slope myObject, Object targetObject)
		{
			// 速度ベクトルを使って押し戻すのは何回もやってきたが、結局それは無理だということが明らかになりますた
			CalculateVectors4(myObject, targetObject);// 辺ではなく頂点
			IsCross4(myObject, targetObject);
		}
		private static void CalculateVectors4(Slope myObject, Object targetObject)
		{
			// Character矩形の頂点を求める
			// Point1:
			defPositionVector[1] = targetObject.position;
			// Point2:
			defPositionVector[2] = defPositionVector[1] + new Vector2(targetObject.width, 0);
			// Point3:
			defPositionVector[3] = defPositionVector[1] + new Vector2(targetObject.width, targetObject.height);
			// Point4:
			defPositionVector[4] = defPositionVector[1] + new Vector2(0, targetObject.height);

			defPositionVector[0] = defPositionVector[1] + new Vector2(targetObject.width / 2, targetObject.height);

			// Characterの速度ベクトルを引く point0(底辺中点)になるように補正
			// 速度だと全部等しくなるので一旦辺にしてみるか...
			prevVector = targetObject.locus[0] + new Vector2(targetObject.width / 2, targetObject.height);//defPositionVector[3];
			curVector = targetObject.position + new Vector2(targetObject.width / 2, targetObject.height);  //drawPos;//defPositionVector[4];
			speedVector = curVector - prevVector;

			// 斜面のslopeVector
			myObject.slopeVectorDown = myObject.position + new Vector2(0, myObject.height); // 斜面下側
			myObject.slopeVectorUp = myObject.position + new Vector2(myObject.width, 0);    // 斜面上側
			myObject.slopeVector = (myObject.slopeVectorUp - myObject.slopeVectorDown);   // 斜面のベクトル

			// 上を三角形の辺全てに拡張
			myObject.sideVectorStart[0] = myObject.position + new Vector2(0, myObject.height); // 斜面下側
			myObject.sideVectorEnd[0] = myObject.position + new Vector2(myObject.width, 0);    // 斜面上側

			myObject.sideVectorStart[1] = myObject.sideVectorEnd[0]; // 右端上側
			myObject.sideVectorEnd[1] = myObject.position + new Vector2(myObject.width, myObject.height);// 右端下側

			myObject.sideVectorStart[2] = myObject.sideVectorEnd[1];// 下端右側
			myObject.sideVectorEnd[2] = myObject.sideVectorStart[0];// 下端左側

			for (int i = 0; i < 3; i++) myObject.sideVector[i] = myObject.sideVectorEnd[i] - myObject.sideVectorStart[i];

			// vectors
			vectors[0] = defPositionVector[0] - myObject.slopeVectorDown;
		}
		private static void IsCross4(Slope myObject, Object targetObject)
		{
			// slopeVectorとVectorsとで外積を計算する
			// 判定すべきVectorsはslopeVectorの始点からtarget_objectの各頂点に引いた4つだけなので、slopeVectorと4回だけ判定すればよい
			for (int i = 1; i < 5; i++)
				crossProduct[0, i] = (myObject.slopeVector.X * vectors[i].Y) - (vectors[i].X * myObject.slopeVector.Y);

			myObject.isHit = false;
			myObject.isUnderSomeone = false;
			targetObject.isHit = false;
			targetObject.isOnSomething = false;
			// 重力を切ると上がるときに判定できない。
			/*for(int i=1;i<5;i++) {// Vectorsのどれか一つでもslopeVectorの右に位置していれば重なっている。
				//if(targetObject.drawPos + targetObject.width > myObject.drawPos && targetObject.drawPos > myObject.drawPos + myObject.width)
				if (myObject.position.X + myObject.width < targetObject.position.X) {
				}else if (targetObject.position.X + targetObject.width < myObject.position.X) {
				}else{// x方向で範囲内に入っている
					if(crossProduct[0,i] > 0) {// slopeVectorの右にあればめりこんでいるので負になる しかしy軸の関係で正？
						targetObject.isHit = true;
						myObject.isHit = true;
						targetObject.isOnSomething = true;
						myObject.isUnderSomeone = true;
						sideNum = i;

						CalculateCrossPoint3(targetObject, myObject);
						adjustObjectT1(targetObject, myObject);
					}
				}
			}*/

			crossProduct[0, 0] = (myObject.slopeVector.X * vectors[0].Y) - (vectors[0].X * myObject.slopeVector.Y);

			// 先にマップチップの矩形の判定でおおまかに切る
			/*if (myObject.position.X + myObject.width < targetObject.position.X) {
			}else if (targetObject.position.X + targetObject.width < myObject.position.X) {
			}else if (targetObject.position.Y + targetObject.height < myObject.position.Y) {
			}else if (myObject.position.Y +　myObject.height < targetObject.position.Y) {
			}else{
				// 次に速度ベクトルと辺ベクトルとの交点でどの判定をするか決定

				if(crossProduct[0,0] >= 0) {// slopeVectorの右にあればめりこんでいるので負になる しかしy軸の関係で正？
					targetObject.isHit = true;
					myObject.isHit = true;
					//sideNum = 0;

					targetObject.isOnSomething = true;
					targetObject.jumpCount = 0;
					targetObject.isJumping = false;
					myObject.isUnderSomeone = true;
					CalculateCrossPoints4(myObject, targetObject);
					//if(sideNum==0)
						adjustObject4(myObject, targetObject);
                    
			   }
			}*/

			for (int i = 0; i < isCrossSide.Length; i++) isCrossSide[i] = false;
			// 10/25:当たり判定もどの辺に押し戻すかも両方交差判定に統一する
			//速度ベクトルと3本の辺ベクトルに対して3回交差判定を行う　外積を使った交差判定との計算量については厳密には比較していないが、
			//大体同じくらいだろうと判断して既存の直線と方程式での交差判定を使う jをspeedVectorに。
			//for(int i=0;i<=0;i++) {
			/*Result[0] = (myObject.slopeVectorUp.X - myObject.slopeVectorDown.X) * (prevVector.Y - myObject.slopeVectorDown.Y)
				- (myObject.slopeVectorUp.Y - myObject.slopeVectorDown.Y) * (prevVector.X - myObject.slopeVectorDown.X);

			Result[1] = (myObject.slopeVectorUp.X - myObject.slopeVectorDown.X) * (curVector.Y - myObject.slopeVectorDown.Y)
				- (myObject.slopeVectorUp.Y - myObject.slopeVectorDown.Y) * (curVector.X - myObject.slopeVectorDown.X);


			Result[2] = (curVector.X - prevVector.X) * (myObject.slopeVectorDown.Y - prevVector.Y)
				- (curVector.Y - prevVector.Y) * (myObject.slopeVectorDown.X - prevVector.X);

			Result[3] = (curVector.X - prevVector.X) * (myObject.slopeVectorUp.Y - prevVector.Y)
				- (curVector.Y - prevVector.Y) * (myObject.slopeVectorUp.X - prevVector.X);*/
			Result[0] = (myObject.slopeVectorUp.X - myObject.slopeVectorDown.X) * (prevVector.Y - myObject.slopeVectorDown.Y)
				- (myObject.slopeVectorUp.Y - myObject.slopeVectorDown.Y) * (prevVector.X - myObject.slopeVectorDown.X);

			Result[1] = (myObject.slopeVectorUp.X - myObject.slopeVectorDown.X) * (curVector.Y - myObject.slopeVectorDown.Y)
				- (myObject.slopeVectorUp.Y - myObject.slopeVectorDown.Y) * (curVector.X - myObject.slopeVectorDown.X);


			Result[2] = (curVector.X - prevVector.X) * (myObject.slopeVectorDown.Y - prevVector.Y)
				- (curVector.Y - prevVector.Y) * (myObject.slopeVectorDown.X - prevVector.X);

			Result[3] = (curVector.X - prevVector.X) * (myObject.slopeVectorUp.Y - prevVector.Y)
				- (curVector.Y - prevVector.Y) * (myObject.slopeVectorUp.X - prevVector.X);


			// 1つの線分の組に対して値を計算し終わったら判定
			CompareCross4(myObject, targetObject);
			if (targetObject.isHit) {
				//isCrossSide[i] = true;
				adjustObject4(myObject, targetObject);
			}
			//}
		}
		private static void CompareCross4(Object myObject, Object targetObject)
		{
			targetObject.isHit = false;
			if (Result[0] * Result[1] < 0 && Result[2] * Result[3] < 0) {
				targetObject.isHit = true;
				targetObject.isOnSomething = true;
			}
		}
		private static void CalculateCrossPoints4(Slope myObject, Object targetObject)
		{
			/*　alice syndromeが...
			 * 交点＝Ａ＋ベクトルＡＢ＊①/ (①＋②)
			 * ①：(defPositionVector[sideNum]   - myObject.slopeVectorDown) × slopeVector
			 * ②：(defPositionVector[sideNum+1] - myObject.slopeVectorDown) × slopeVector
			 */
			float crossProduct1 = 0;
			float crossProduct2 = 0;



			crossProduct1 = (prevVector.X - myObject.slopeVectorDown.X) * myObject.slopeVector.Y// もしかして正負逆転の必要？それはないか...
					- myObject.slopeVector.X * (prevVector.Y - myObject.slopeVectorDown.Y);
			crossProduct2 = (curVector.X - myObject.slopeVectorDown.X) * myObject.slopeVector.Y
					- myObject.slopeVector.X * (curVector.Y - myObject.slopeVectorDown.Y);//Math.Abs()やったらもっとひどくなった*/
			crossPoint = prevVector
					+ new Vector2(speedVector.X * (crossProduct1 / (crossProduct1 + crossProduct2)),// crosuProduct2がおかしい　そもそも比が－ってなんだ
						speedVector.Y * (crossProduct1 / (crossProduct1 + crossProduct2)));// ratioを()でくくるのを忘れてた＊２

			// 交点計算(交差する場所が遠い場合変な値にもなるだろうが最近の交点にはたどり着けるだろう)
			for (int i = 0; i < 3; i++) {
				crossProduct1 = (prevVector.X - myObject.sideVectorStart[i].X) * myObject.sideVector[i].Y// もしかして正負逆転の必要？それはないか...
					- myObject.sideVector[i].X * (prevVector.Y - myObject.sideVectorStart[i].Y);
				crossProduct2 = (curVector.X - myObject.sideVectorStart[i].X) * myObject.sideVector[i].Y
					- myObject.sideVector[i].X * (curVector.Y - myObject.sideVectorStart[i].Y);
				// X成分はまともだがYが異常である たまにInfinityとかになる
				crossPoints[i] = prevVector
					+ new Vector2(speedVector.X * (crossProduct1 / (crossProduct1 + crossProduct2)),
						speedVector.Y * (crossProduct1 / (crossProduct1 + crossProduct2)));
			}// 動かないと当然speedVectorは0

			Vector2 max = new Vector2(0, 0);
			float maxDistance = 0;
			//float prevMaxDistance = 0;
			//foreach(Vector2 crossPoint in crossPoints) {
			for (int i = 0; i < crossPoints.Length; i++) {// crossPointsがみんな同値になってしまう。辺でやるとやはり変わったが
				float distance = Vector2.Distance(crossPoints[i], defPositionVector[0]);//crossPoint - prevVector;//targetObject.position; 
				//if (max < distanceD) {
				//prevMaxDistance = distanceD;
				if (i == 0) maxDistance = distance;
				if (maxDistance >= distance) {
					maxDistance = distance;
					sideNum = i;
				}
			}
			if (sideNum == 1) { }

		}
		private static void adjustObject4(Slope myObject, Object targetObject)
		{
			//Vector2 adjustVector;
			Vector2 criterionVector = new Vector2(targetObject.position.X + targetObject.width / 2, targetObject.position.Y + targetObject.height);// 底辺の中点
			float xFromDown;
			/*targetObject.scalarSpeed = 0;
			adjustVector = (criterionVector - crossPoint) * (-1);//curVector
			targetObject.position += adjustVector;// drawVectorじゃだめっぽい？*/

			//targetObject.position = crossPoint -　criterionVector;//new Vector2(targetObject.width, targetObject.height);

			if (targetObject.isHit) {
				if (criterionVector.X < myObject.position.X + myObject.width) {
					xFromDown = targetObject.position.X + targetObject.width / 2 - myObject.position.X;//kokoda
					targetObject.position.Y = myObject.position.Y + myObject.height - xFromDown;
					targetObject.position.Y -= targetObject.height - 1;
					targetObject.speed.Y = (float)-targetObject.gravity - 1;// 振動☆
				}
			}
			/*if(isCrossSide[0]) {
				if (criterionVector.X < myObject.position.X + myObject.width) {
					adjustDistance = targetObject.position.X + targetObject.width / 2 - myObject.position.X;//kokoda
					targetObject.position.Y = myObject.position.Y + myObject.height - adjustDistance;
					targetObject.position.Y -= targetObject.height;
				}
			}
			else if(isCrossSide[1]) {
				if (targetObject.scalarSpeed < 0  )
				{// 右側が壁の斜面チップを対象
					// このようにy方向を絞ると斜面を下ってる途中でめりこんでると判定されてしまう。
					if (myObject.position.X + myObject.width - targetObject.position.X < myObject.maxLength) {// ブロックの右端
						if (targetObject.position.Y + targetObject.height > myObject.position.Y  && targetObject.position.Y < myObject.position.Y + myObject.height) {
							targetObject.position.X = myObject.position.X + myObject.width;// 右に補正
						}
					}
				}
			}*/
		}
		#endregion

		#region RectangleDetection
		/// <summary>
		/// 軸に平行な矩形同士の当たり判定
		/// </summary>
		public static void RectangleDetection(Object myObject, Object targetObject, int type)
		{
			myObject.isHit = false;
			targetObject.isDamaged = false;
			targetObject.isHit = false;
			if (targetObject.position.X + targetObject.width < myObject.position.X) {
			} else if (myObject.position.X + myObject.width < targetObject.position.X) {
			} else if (targetObject.position.Y + targetObject.height < myObject.position.Y) {
			} else if (myObject.position.Y + myObject.height < targetObject.position.Y) {
			} else {
				// 当たりあり
				myObject.isHit = true;
				if (type == 0) {
					targetObject.isDamaged = true;
					targetObject.isHit = true;
				}

				// 下に移動中
				if (targetObject.speed.Y - myObject.speed.Y > 0) {
					if (targetObject.position.X + targetObject.width > myObject.position.X && targetObject.position.X < myObject.position.X + myObject.width) {
						//座標を指定しないとBlockに接してジャンプしたときにｷﾞﾘｷﾞﾘで乗ってしまう
						// ブロックの上端
						if (targetObject.position.Y + targetObject.height - myObject.position.Y < maxLength) {
							targetObject.speed.Y = 0;
							targetObject.position.Y = myObject.position.Y - targetObject.height;  // 上に補正

							targetObject.isOnSomething = true;
							targetObject.jumpCount = 0;      // Playerに限定したかったが諦めた
							targetObject.isJumping = false;　// 着地したらJumpできるように
							targetObject.position.X += myObject.speed.X;  // これで慣性を再現できるか！？
						}
					}
				}
					// 上に移動中
				else if (targetObject.speed.Y - myObject.speed.Y < 0) {// 相対速度で。
					if (targetObject.position.X + targetObject.width > myObject.position.X && targetObject.position.X < myObject.position.X + myObject.width) {
						// ブロックの下端
						if (myObject.position.Y + myObject.height - targetObject.position.Y < maxLength) {
							targetObject.speed.Y = 0;
							targetObject.position.Y = myObject.position.Y + myObject.height;   // 下に補正
						}
					}
				}
				// 右に移動中
				if (targetObject.speed.X - myObject.speed.X > 0) {
					if (targetObject.position.Y > myObject.position.Y - targetObject.height && targetObject.position.Y < myObject.position.Y + myObject.height) {
						// ブロックの左端
						if ((targetObject.position.X + targetObject.width) - myObject.position.X < maxLength) {
							targetObject.position.X = myObject.position.X - targetObject.width;  // 左に補正
						}
					}
				}
					// 左に移動中
				else if (targetObject.speed.X - myObject.speed.X < 0) {
					if (targetObject.position.Y + targetObject.height > myObject.position.Y && targetObject.position.Y < myObject.position.Y + myObject.height) {
						// ブロックの右端
						if ((myObject.position.X + myObject.width) - targetObject.position.X < maxLength) {
							targetObject.position.X = myObject.position.X + myObject.width;   // 右に補正
						}
					}
				}
			}

		}
		#endregion
		#region RectangleDetection_Lite
		public static void RectangleDetectionLite(Object myObject, Object targetObject, int type)
		{
			targetObject.isDamaged = false;
			targetObject.isHit = false;
			myObject.isHit = false;
			if (targetObject.position.X + targetObject.width < myObject.position.X) {
			} else if (myObject.position.X + myObject.width < targetObject.position.X) {
			} else if (targetObject.position.Y + targetObject.height < myObject.position.Y) {
			} else if (myObject.position.Y + myObject.height < targetObject.position.Y) {
			} else {
				// 当たりあり
				myObject.isHit = true;
				if (type == 0) {
					targetObject.isDamaged = true;
					targetObject.isHit = true;
				}
			}
		}
		#endregion
		#region RectangleCrossDetection
		public static void RectangleCross(Object myObject, Object targetObject, float myDegree, float targetDegree)
		{
			// 交差判定　参考http://www5d.biglobe.ne.jp/~tomoya03/shtml/algorithm/Intersection.htm
			CalculateVectors1(myObject, targetObject, -myDegree, -targetDegree);// 相手は回転しない簡易ver
			CalculateEquation(myObject, targetObject);

			// 交点計算　参考http://www.deqnotes.net/acmicpc/2d_geometry/lines
		}
		private static void CalculateVectors1(Object myObject, Object targetObject, float myDegree, float targetDegree)
		{
			Vector2 tmpPositionVector;// 計算用

			myObject.radius = MathHelper.ToRadians(myDegree);
			/* 頂点の配置
			 * P0------------P1 
			 * |              |
			 * |              |
			 * P3------------P2
			 */

			if (myDegree == 0) {
				myPositionVector[0] = myObject.position;
				myPositionVector[1] = myPositionVector[0] + new Vector2(myObject.width, 0);
				myPositionVector[2] = myPositionVector[0] + new Vector2(myObject.width, myObject.height);
				myPositionVector[3] = myPositionVector[0] + new Vector2(0, myObject.height);
			} else {// まず、回転してない状態の座標を求める
				// Point1:回転してないときの左上の頂点
				defPositionVector[0] = myObject.position;
				// Point2:
				defPositionVector[1] = defPositionVector[0] + new Vector2(myObject.width, 0);
				// Point3:
				defPositionVector[2] = defPositionVector[0] + new Vector2(myObject.width, myObject.height);
				// Point4:
				defPositionVector[3] = defPositionVector[0] + new Vector2(0, myObject.height);

				/* 
				 * 回転行列をかけて、回転後の座標を求める(Point1中心の回転)
				 * (X) =（ cosθ -sinθ）(x)
				 * (Y)  （ sinθ cosθ ）(y)
				 * 
				 * そのままかけると原点中心の回転になってしまうので、Point1(矩形左上の点)が原点に位置するように4頂点を平行移動
				 * →回転行列をかけて回転→元の位置に戻す、という手順をとる。
				 */

				//Point1:
				myPositionVector[0] = defPositionVector[0];
				// 他の3つ
				for (int i = 1; i <= 3; i++) {
					tmpPositionVector = defPositionVector[i] - defPositionVector[0];
					myPositionVector[i].X = (tmpPositionVector.X * (float)Math.Cos(myObject.radius)) - (tmpPositionVector.Y * (float)Math.Sin(myObject.radius));
					myPositionVector[i].Y = (tmpPositionVector.X * (float)Math.Sin(myObject.radius)) + (tmpPositionVector.Y * (float)Math.Cos(myObject.radius));
					myPositionVector[i] += defPositionVector[0];
				}
			}

			// 比較対象矩形の頂点
			if (targetDegree == 0) {
				targetPositionVector[0] = targetObject.position;
				targetPositionVector[1] = targetPositionVector[0] + new Vector2(targetObject.width, 0);
				targetPositionVector[2] = targetPositionVector[0] + new Vector2(targetObject.width, targetObject.height);
				targetPositionVector[3] = targetPositionVector[0] + new Vector2(0, targetObject.height);
			} else {
				// myObjectの時と同じ
				defPositionVector[0] = targetObject.position;
				defPositionVector[1] = defPositionVector[0] + new Vector2(targetObject.width, 0);
				defPositionVector[2] = defPositionVector[0] + new Vector2(targetObject.width, targetObject.height);
				defPositionVector[3] = defPositionVector[0] + new Vector2(0, targetObject.height);

				targetPositionVector[0] = defPositionVector[0];
				for (int i = 1; i <= 3; i++) {
					tmpPositionVector = defPositionVector[i] - defPositionVector[0];
					targetPositionVector[i].X = (tmpPositionVector.X * (float)Math.Cos(targetObject.radius)) - (tmpPositionVector.Y * (float)Math.Sin(targetObject.radius));
					targetPositionVector[i].Y = (tmpPositionVector.X * (float)Math.Sin(targetObject.radius)) + (tmpPositionVector.Y * (float)Math.Cos(targetObject.radius));
					targetPositionVector[i] += defPositionVector[0];
				}
			}

		}
		/// <summary>
		/// 苦労した箇所。正直もういじりたくない^q^
		/// </summary>
		private static void CalculateEquation(Object myObject, Object targetObject)
		{
			//16回判定する必要あり 交差判定.
			for (int i = 0; i <= 3; i++) {
				if (targetObject.isDamaged) break;
				for (int j = 0; j <= 3; j++) {
					Result[0] = (myPositionVector[(i + 1) % 4].X - myPositionVector[i].X) * (targetPositionVector[j].Y - myPositionVector[i].Y)
						- (myPositionVector[(i + 1) % 4].Y - myPositionVector[i].Y) * (targetPositionVector[j].X - myPositionVector[i].X);

					Result[1] = (myPositionVector[(i + 1) % 4].X - myPositionVector[i].X) * (targetPositionVector[(j + 1) % 4].Y - myPositionVector[i].Y)
						- (myPositionVector[(i + 1) % 4].Y - myPositionVector[i].Y) * (targetPositionVector[(j + 1) % 4].X - myPositionVector[i].X);

					Result[2] = (targetPositionVector[(j + 1) % 4].X - targetPositionVector[j].X) * (myPositionVector[i].Y - targetPositionVector[j].Y)
						- (targetPositionVector[(j + 1) % 4].Y - targetPositionVector[j].Y) * (myPositionVector[i].X - targetPositionVector[j].X);

					Result[3] = (targetPositionVector[(j + 1) % 4].X - targetPositionVector[j].X) * (myPositionVector[(i + 1) % 4].Y - targetPositionVector[j].Y)
						- (targetPositionVector[(j + 1) % 4].Y - targetPositionVector[j].Y) * (myPositionVector[(i + 1) % 4].X - targetPositionVector[j].X);

					// 計算し終わったら判定
					CompareCross(myObject, targetObject);
					if (targetObject.isDamaged) break;
				}
			}

		}
		private static void CompareCross(Object myObject, Object targetObject)
		{
			targetObject.isDamaged = false;

			if (myObject.firstTimeInAFrame) {
				myObject.isHit = false;
				targetObject.firstTimeInAFrame = false;
			}
			if (targetObject.firstTimeInAFrame) {
				targetObject.isHit = false;
				targetObject.firstTimeInAFrame = false;
			}
			//targetObject.isHit = false;
			if (Result[0] * Result[1] < 0 && Result[2] * Result[3] < 0) {
				targetObject.isDamaged = true;  // Rivalのときもここまで到達している=当たってる
				targetObject.isHit = true;
				myObject.isHit = true;
				if (targetObject is Bullet) { }
				if (myObject is Sword) { }
			}
		}
		private static void CalculateCrossPoint1(Object my_object, Object target_object)
		{   /*
             * Effect発生箇所用に交差点を算出する
             * 交差判定に倣っての直線の連立はしない
             * ベクトルを2本引き片方を直線と扱ってもう一方を伸び縮みさせる
             * 関数が呼ばれる時点で,平行であることはあり得ないのでその点は気にしない
             */

			// ※斬撃Effectは交点求めなくてもいけたのでこのメソッドは無かったことになりました
			// 交点を求める必要性はない

		}
		#endregion
		#region RectangleCrossDetectionDetailed
		/// <summary>
		/// myObejctの１フレーム間の動きを何分割化して細かく交差判定する
		/// </summary>
		/// <param name="myObject"></param>
		/// <param name="targetObject"></param>
		/// <param name="myDegree"></param>
		/// <param name="targetDegree"></param>
		/// <param name="isHostile">(重要) myObjがtargetObjに対してhostileかどうか。もしそうならtargetにダメージを与える</param>
		public static void RectangleCrossDetailed(Object myObject, Object targetObject, float myDegree, float targetDegree, int detail)//, bool isHostile)
		{
			DivideLocus(myObject, detail);//16
			// 交差判定　参考http://www5d.biglobe.ne.jp/~tomoya03/shtml/algorithm/Intersection.htm
			/*foreach(Vector2 locus in locusVectors) {
				CalculatePointDetailed(myObject, targetObject, -myDegree,locus);// 相手は回転しない簡易ver
				CalculateEquation_D(myObject, targetObject,locus);
			}*/
			foreach (int locusDegree in locusDegrees) {// 縦斬りのときで12刻み10等分はされているようだ？
				//CalculatePointDetailed(myObject, targetObject, -locusDegree);// 相手は回転しない簡易ver
				CalculateVectors1(myObject, targetObject, -locusDegree, -targetDegree);// 再利用　-myDegre←これだろう
				//CalculateEquation_D(myObject, targetObject);
				CalculateEquation(myObject, targetObject);
				if (targetObject.isDamaged) break;
			}

			// 交点計算　参考http://www.deqnotes.net/acmicpc/2d_geometry/lines
			//if (targetObject.isDamaged) CalculateCrossPoint1(myObject, targetObject);

			/*targetObject.prevIsHit.Add(targetObject.isHit);
			if (targetObject.prevIsHit.Count > 2) targetObject.prevIsHit.RemoveAt(0);*/
		}
		private static void DivideLocus(Object myObject, int division)
		{
			/*if(myObject.locus.Count >= 2) {
				locus = myObject.locus[1] - myObject.locus[0];// drawVectorで
				for(int i=0;i<4;i++) locusVectors[i] = locus * (i / 5);   //剣の場合変化するのは角度なのでvectorは意味が無い
			}*/
			if (myObject.locusDegree.Count >= 2) {
				locusDegree = myObject.locusDegree[1] - myObject.locusDegree[0];//80 - -40  //preb- cur(1frameで移動した分がでる)   //3/19 この辺が怪しい　振り始めはlocusDegreeがおかしくなる
				for (int i = 0; i <= division; i++) locusDegrees[i] = /*locusDegree*/myObject.locusDegree[0] + i * (locusDegree / division);// 3/19 i=1→i=0
			}//0 -16 8 32 56
		}
		private static void CalculatePointDetailed(Object myObject, Object targetObject, float myDegree)
		{
			Vector2 tmpPositionVector;

			myObject.radius = MathHelper.ToRadians(myDegree);
			/* 頂点の配置
			 * P1------------P2 
			 * |              |
			 * |              |
			 * P4------------P3
			 */

			// Point1:回転してないときの左上の頂点
			defPositionVector[0] = myObject.drawPos;//my_locusVector;//myObject.drawPos;//強制スクロール時：position

			// まず、回転してない状態の座標を求める
			// Point2:
			defPositionVector[1] = defPositionVector[1] + new Vector2(myObject.width, 0);
			// Point3:
			defPositionVector[2] = defPositionVector[1] + new Vector2(myObject.width, myObject.height);
			// Point4:
			defPositionVector[3] = defPositionVector[1] + new Vector2(0, myObject.height);

			/* 
			 * 回転行列をかけて、回転後の座標を求める(Point1中心の回転)
			 * (X) =（ cosθ -sinθ）(x)
			 * (Y)  （ sinθ cosθ ）(y)
			 * 
			 * そのままかけると原点中心の回転になってしまうので、Point1(矩形左上の点)が原点に位置するように4頂点を平行移動
			 * →回転行列をかけて回転→元の位置に戻す、という手順をとる。
			 */

			//Point1:
			myPositionVector[0] = defPositionVector[0];

			// 他の3つ
			for (int i = 1; i <= 3; i++) {
				tmpPositionVector = defPositionVector[i] - defPositionVector[0];
				myPositionVector[i].X = (tmpPositionVector.X * (float)System.Math.Cos(myObject.radius)) - (tmpPositionVector.Y * (float)System.Math.Sin(myObject.radius));
				myPositionVector[i].Y = (tmpPositionVector.X * (float)System.Math.Sin(myObject.radius)) + (tmpPositionVector.Y * (float)System.Math.Cos(myObject.radius));
				myPositionVector[i] += defPositionVector[0];
			}

			// 比較対象矩形の頂点：とりあえずは、軸に平行な矩形だけを対象として計算
			targetPositionVector[0] = targetObject.drawPos;
			targetPositionVector[1] = targetPositionVector[0] + new Vector2(targetObject.width, 0);
			targetPositionVector[2] = targetPositionVector[0] + new Vector2(targetObject.width, targetObject.height);
			targetPositionVector[3] = targetPositionVector[0] + new Vector2(0, targetObject.height);
		}

		#endregion
		#region Rectangle, Point
		/// <summary>
		/// 回転した矩形同士の判定を行う
		/// </summary>
		public static void CDinAction1(Object my_object, Object target_object, float my_degree, float target_degree)
		{
			/*
			 * 回転した矩形の４頂点の座標を求める→P1からベクトルを４本時計回りに引いて一周 
			 * →外積を使って比較対象の矩形の頂点が内部(右側)にあるか４回判定、あれば交差してる
			 * (回転した矩形同士で交差してる場合、必ずどちらかの頂点がもう一方の内部にある)
			 */
			// 参考http://marupeke296.com/COL_2D_No4_SquareToSquare.html
			// 多分お互いに判定すれば漏れなく判定できる
			Calculate_Point(my_object, target_object, -my_degree, -target_degree);// radiusはフィールドのを使う　-degreeにしておけば普通の座標軸を取ったときの感覚で使えるはず
			Calculate_Vector();
			Calculate_CrossProduct();
			IsInRegion(target_object);
		}
		private static void Calculate_Point(Object my_object, Object target_object, float my_degree, float target_degree)
		{
			Vector2 tmpPositionVector;

			my_object.radius = MathHelper.ToRadians(my_degree);
			target_object.radius = MathHelper.ToRadians(target_degree);
			/* 頂点の配置
			 * P1------------P2 
			 * |              |
			 * |              |
			 * P4------------P3
			 */

			// Point1:回転してないときの左上の頂点
			defPositionVector[1] = my_object.drawPos;//強制スクロール時：position if(!stage.isScrolled)
			//else defPositionVector[1] = myObject.position;

			// まず、回転してない状態の座標を求める
			// Point2:
			defPositionVector[2] = defPositionVector[1] + new Vector2(my_object.width, 0);
			// Point3:
			defPositionVector[3] = defPositionVector[1] + new Vector2(my_object.width, my_object.height);
			// Point4:
			defPositionVector[4] = defPositionVector[1] + new Vector2(0, my_object.height);

			/* 
			 * 回転行列をかけて、回転後の座標を求める(Point1中心の回転)
			 * (X) =（ cosθ -sinθ）(x)
			 * (Y)  （ sinθ cosθ ）(y)
			 * 
			 * そのままかけると原点中心の回転になってしまうので、Point1(矩形左上の点)が原点に位置するように4頂点を平行移動
			 * →回転行列をかけて回転→元の位置に戻す、という手順をとる。
			 */

			// Point1:
			myPositionVector[1] = defPositionVector[1];

			// 他の3つ
			for (int i = 2; i < 5; i++) {
				tmpPositionVector = defPositionVector[i] - defPositionVector[1];// *角度は後で考えて
				myPositionVector[i].X = (tmpPositionVector.X * (float)System.Math.Cos(target_object.radius)) - (tmpPositionVector.Y * (float)System.Math.Sin(target_object.radius));
				myPositionVector[i].Y = (tmpPositionVector.X * (float)System.Math.Sin(target_object.radius)) + (tmpPositionVector.Y * (float)System.Math.Cos(target_object.radius));
				myPositionVector[i] += defPositionVector[1];
			}

			// 比較対象矩形の頂点：自矩形と同様
			// def_position_vectorとtmp_position_vectorは、position_vector求めた後なので使いまわして大丈夫だろう

			// 無回転時
			// Point1:
			defPositionVector[1] = target_object.drawPos;//強制スクロール時：position
			// Point2:
			defPositionVector[2] = defPositionVector[1] + new Vector2(target_object.width, 0);
			// Point3:
			defPositionVector[3] = defPositionVector[1] + new Vector2(target_object.width, target_object.height);
			// Point4:
			defPositionVector[4] = defPositionVector[1] + new Vector2(0, target_object.height);

			// 回転
			// Point1:
			targetPositionVector[1] = defPositionVector[1];
			// 他の3つ
			for (int i = 2; i < 5; i++) {
				tmpPositionVector = defPositionVector[i] - defPositionVector[1];
				targetPositionVector[i].X = (tmpPositionVector.X * (float)System.Math.Cos(target_object.radius)) - (tmpPositionVector.Y * (float)System.Math.Sin(target_object.radius));
				targetPositionVector[i].Y = (tmpPositionVector.X * (float)System.Math.Sin(target_object.radius)) + (tmpPositionVector.Y * (float)System.Math.Cos(target_object.radius));
				targetPositionVector[i] += defPositionVector[1];
			}
		}
		private static void Calculate_Vector()
		{
			/*             vector1
			 *         P1→→→→→→P2 
			 * vector4 ↑            ↓vector2
			 *         ↑            ↓
			 *         P4←←←←←←P3
			 *            vector3
			 */

			// 自矩形の外周に沿って時計回りに引く
			for (int j = 1; j < 4; j++) {
				vectors[j] = myPositionVector[j + 1] - myPositionVector[j];
			}
			vectors[4] = myPositionVector[1] - myPositionVector[4];

			// 自Pointから対象Pointへ引く
			// 比較対象矩形の頂点ごとに
			for (int k = 1; k < 5; k++) {
				for (int l = 1; l < 5; l++) {
					targetVector[k, l] = targetPositionVector[k] - myPositionVector[l];
				}
			}
		}
		private static void Calculate_CrossProduct()
		{   // 外積の計算：1頂点につき4回調べる
			// |V×W| ＝ (V.x * W.y) - (V.y * W.x) = |V||W|sinθ

			//for(int i=1;i<crossProduct1.Length;i++) {
			//    crossProduct1[i] =(position[i].X * target_vector1.Y) - (target_vector1.X * vector1.Y);

			/* 比較対象オブジェクトの1つ目の頂点について自矩形の4つのベクトルとの外積を計算：
			 * |vector1×target_vector1_1| : この場合、プログラムの座標の取り方だと > 0
			 * vector1               ↑    ↗ target_vector1_1(to target_position_vector1)
			 * (to position_vector2) ↑   ↗
			 *                       ↑  ↗
			 *                       ↑ ↗
			 *                       ↑↗
			 *                       ↑
			 */
			for (int m = 1; m < 5; m++) {
				for (int n = 1; n < 5; n++) {
					crossProduct[m, n] = (vectors[n].X * targetVector[m, n].Y) - (targetVector[m, n].X * vectors[n].Y);
				}
			}
		}
		private static void IsInRegion(Object target_object)
		{
			target_object.isDamaged = false;

			// (新)
			if (crossProduct[1, 1] < 0) {
			} else if (crossProduct[1, 2] < 0) {
			} else if (crossProduct[1, 3] < 0) {
			} else if (crossProduct[1, 4] < 0) {
			} else {
				target_object.isDamaged = true;
			}
			if (crossProduct[2, 1] < 0) {
			} else if (crossProduct[2, 2] < 0) {
			} else if (crossProduct[2, 3] < 0) {
			} else if (crossProduct[2, 4] < 0) {
			} else {
				target_object.isDamaged = true;
			}
			if (crossProduct[3, 1] < 0) {
			} else if (crossProduct[3, 2] < 0) {
			} else if (crossProduct[3, 3] < 0) {
			} else if (crossProduct[3, 4] < 0) {
			} else {
				target_object.isDamaged = true;
			}
			if (crossProduct[4, 1] < 0) {
			} else if (crossProduct[4, 2] < 0) {
			} else if (crossProduct[4, 3] < 0) {
			} else if (crossProduct[4, 4] < 0) {
			} else {
				target_object.isDamaged = true;
			}

			// (旧)
			/*if(crossProduct[1,1] > 0 && crossProduct[1,2] > 0 && crossProduct[1,3] > 0 && crossProduct[1,4] > 0 ) {
				isDamaged = true;
			}
			if(crossProduct[2,1] > 0 && crossProduct[2,2] > 0 && crossProduct[2,3] > 0 && crossProduct[2,4] > 0 ) {
				isDamaged = true;
			}
			if(crossProduct[3,1] > 0 && crossProduct[3,2] > 0 && crossProduct[3,3] > 0 && crossProduct[3,4] > 0 ) {
				isDamaged = true;
			}
			if(crossProduct[4,1] > 0 && crossProduct[4,2] > 0 && crossProduct[4,3] > 0 && crossProduct[4,4] > 0 ) {
				isDamaged = true;
			}*/
		}
		#endregion

	}
}
