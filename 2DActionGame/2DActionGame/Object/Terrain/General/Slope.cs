using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
	/// <summary>
	/// 斜面クラス。
	/// </summary>
    public class Slope : Terrain
    {
		/// <summary>
		/// 地面か天井か
		/// </summary>
        public bool onCeiling { get; private set; }
		/// <summary>
		/// 上りか下りか
		/// </summary>
        public bool isUp { get; private set; }
		/// <summary>
		/// 斜面が続いているところで右/左端の判定を切る
		/// </summary>
		public bool isLowerLeft { get; set; }
		public bool isLowerRight { get; set; }
        //public bool isLeftSlope { get; set; }
		public bool isRightBlock { get; set; }

        /// <summary>
		/// 辺を構成するVector
        /// </summary>
        public Vector2[] sideVector = new Vector2[3], sideVectorStart = new Vector2[3], sideVectorEnd = new Vector2[3];

        public Slope(Stage stage, float x, float y, int width, int height)
            : this(stage, x, y, width, height, true, 0)
        {
        }
        public Slope(Stage stage, float x, float y, int width, int height, bool isUp, int type)
            : this(stage, x, y, width, height, false, isUp, type)
        {
        }
        public Slope(Stage stage, float x, float y, int width, int height, bool onCeiling, bool isUp, int type)
            : base(stage, x, y, width, height)
        {
            this.onCeiling = onCeiling;
            this.isUp = isUp;
            this.type = type;
            if(!onCeiling) {
                switch (type) {
                    case 0: degree = 45; break;
                    case 1: degree = 22.5f; break;
                    case 2: degree = 11.25f; break;
                }
            }
            else{
                switch (type) {
                    case 0: degree = -45; break;
                    case 1: degree = -22.5f; break;
                    case 2: degree = -11.25f; break;
                }
            }

			if (isUp) {
				slopeVectorDown = position + new Vector2(0, height);      // 斜面下側
				slopeVectorUp = position + new Vector2(width, 0);         // 斜面上側
				slopeVector = (slopeVectorUp - slopeVectorDown);        // 斜面のベクトル
			}
			else {
				slopeVectorDown = position + new Vector2(width, height);
				slopeVectorUp = position;
				slopeVector = (slopeVectorDown - slopeVectorUp);
			}

			Load();
        }
		protected override void Load()
		{
			base.Load();
			if (!onCeiling)
				if (isUp)
					texture = content.Load<Texture2D>("Object\\Terrain\\Slopes\\0" + (game.stageNum - 1).ToString() + "\\Slope" + type);
				/*switch (movementType) {
				 * terrain.Load(content, "Object\\Terrain\\Slopes\\0" + (game.stageNum - 1).ToString() + "\\Slope0");
					case 0:
						texture = content.Load<Texture2D>("Object\\Terrain\\Slopes\\0" + (game.stageNum - 1).ToString() + "\\Slope0");
						break;
					case 1:
						texture = content.Load<Texture2D>("Object\\Terrain\\Slopes\\0" + (game.stageNum - 1).ToString() + "\\Slope1");
						break;
					case 2:
						break;
				}*/
				else
					texture = content.Load<Texture2D>("Object\\Terrain\\Slopes\\0" + (game.stageNum - 1).ToString() + "\\Slope-" + type);
			/*switch ((terrain as Slope).movementType) {
				case 0:
					terrain.Load(content, "Object\\Terrain\\Slopes\\0" + (game.stageNum - 1).ToString() + "\\Slope-0");
					break;
				case 1:
					terrain.Load(content, "Object\\Terrain\\Slopes\\0" + (game.stageNum - 1).ToString() + "\\Slope-1");
					break;
				case 2:
					terrain.Load(content, "Object\\Terrain\\Slopes\\0" + (game.stageNum - 1).ToString() + "\\Slope-2");
					break;
			}*/
			else {
				if (isUp)
					texture = content.Load<Texture2D>("Object\\Terrain\\Slopes\\0" + (game.stageNum - 1).ToString() + "\\Slope" + type);
				else
					texture = content.Load<Texture2D>("Object\\Terrain\\Slopes\\0" + (game.stageNum - 1).ToString() + "\\Slope-" + type);
			}
		}

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
		/// <summary>
		/// 外積
		/// </summary>
        private float[] crossProduct2 = new float [3];
        #region IsHit
        /// <summary>
        /// ベクトルを用いた斜面用の衝突判定メソッド
        /// 直角三角形単体で判定できて、かつ無駄な判定をしないメソッド....の予定だったが上手くいかず結局使っていない
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="targetObject"></param>
        public override void IsHit(Object targetObject)
        {
            CalculateVectors(targetObject);// 辺ではなく頂点で判定(方向によって判定すべき点を変更する)
            if(isUp) CalculateCrossProductT1(targetObject);
            else     CalculateCrossProductT2(targetObject);
        }
        private void CalculateVectors(Object targetObject)
        {
            // targetObjectの矩形の判定に使う点を求める
            defPositionVector[0] = targetObject.position;
            defPositionVector[1] = defPositionVector[0] + new Vector2(targetObject.width, 0);
            defPositionVector[2] = defPositionVector[0] + new Vector2(targetObject.width, targetObject.height);
            defPositionVector[3] = defPositionVector[0] + new Vector2(targetObject.width / 2, targetObject.height);
            defPositionVector[4] = defPositionVector[0] + new Vector2(0, targetObject.height);

            // 直角三角形の辺ベクトルを計算(時計回り)
            /*                         [0]End           |＼[0]Start
             *                        /|                |  ＼
             *                       / |                |    ＼
             *                      /  |                |      ＼
             *                     /   |                |        ＼
             *                    /    |                |          ＼   
             *   [0]Start([2]End)/_____|[1]End [2]Start |____________＼[1]Start
             *   
             */
            if(isUp) {
                sideVectorStart[0] = position + new Vector2(0, height);            // 斜面下側
                sideVectorEnd[0] = position + new Vector2(width, 0);               // 斜面上側

                sideVectorStart[1] = sideVectorEnd[0];                           // 右端上側
                sideVectorEnd[1] = position + new Vector2(width, height);          // 右端下側

                sideVectorStart[2] = sideVectorEnd[1];                           // 下端右側
                sideVectorEnd[2] = sideVectorStart[0];                           // 下端左側
            }
            else{
                sideVectorStart[0] = position;                                     // 斜面上側
                sideVectorEnd[0] = position + new Vector2(width, height);          // 斜面下側

                sideVectorStart[1] = sideVectorEnd[0];                           // 下端右側
                sideVectorEnd[1] = position + new Vector2(0, height);              // 下端左側

                sideVectorStart[2] = sideVectorEnd[1];                           // 左端下側
                sideVectorEnd[2] = sideVectorStart[0];                           // 左端上側
            }
            // foreachは割り当てには使えない
            for (int i = 0; i < sideVector.Length; i++) 
                sideVector[i] = sideVectorEnd[i] - sideVectorStart[i];
            
        }
        private void CheckDirection(Object targetObject)
        {
            //sideNum = 0;// とりあえず0に初期化..すると右/左端が反映されない...
            for(int i=2;i>=0;i--) {
                crossProduct2[i] = (sideVector[i].X * vectors[i].Y) - (vectors[i].X * sideVector[i].Y);
                //if (i == 0 && crossProduct2[i] <= 0) sideNum = i;
                if (crossProduct2[i] < 0) sideNum = i;
            }
        }
        private void CalculateCrossProductT1(Object targetObject)
        {
            //if(targetObject.position.X > position.X + width-1 && targetObject.position.X < position.X + width+1) vectors[0] = defPositionVector[3] - sideVectorStart[0];// 3
            //else 
            if (defPositionVector[3].X < position.X && targetObject.position.X + targetObject.width > position.X)
                vectors[0] = defPositionVector[2] - sideVectorStart[0];
            else vectors[0] = defPositionVector[3] - sideVectorStart[0];
            vectors[1] = defPositionVector[0] - sideVectorStart[1];// 0
            vectors[2] = defPositionVector[0] - sideVectorStart[2];
            

            CheckDirection(targetObject);

            for(int i=0;i<=2;i++)
                crossProduct[0, i] = (sideVector[i].X * vectors[i].Y) - (vectors[i].X * sideVector[i].Y);

            // 初期化
            targetObject.isHit = false;
            isHit = false;
            isUnderSomeone = false;
            targetObject.isOnSomething = false;

            // マップチップの矩形領域内に入っているか否かで大まかに絞る
            if (position.X + width < targetObject.position.X) {
            }else if (targetObject.position.X + targetObject.width < position.X) {
            }else if (targetObject.position.Y + targetObject.height < position.Y) {
            }else if (position.Y +　height < targetObject.position.Y) {
            }else{
                // targetObjectは矩形内にいる
                if(targetObject.speed.Y > 0 && sideNum == 0 && crossProduct[0,0] > 0) {
                    targetObject.isHit = true;
                    isHit = true;
                    // 着地処理
                    targetObject.isOnSomething = true;
                    targetObject.jumpCount = 0;
                    targetObject.isJumping = false;
                    isUnderSomeone = true;

                    adjustObjectT1(targetObject);
                }
                else if (targetObject.speed.X < 0 && sideNum == 1 && crossProduct[0, 1] > 0 && !isLowerLeft) {// 右側が壁の斜面チップを対象 if→elsif
                    targetObject.isHit = true;
                    isHit = true;
                    if (position.X + width - targetObject.position.X < maxLength) {
                        if (targetObject.position.Y + targetObject.height > position.Y  && targetObject.position.Y < position.Y + height) {// +5して判定を甘くする
                            targetObject.position.X = position.X + width;// 右に補正
                        }
                    }
                }
                else if(targetObject.speed.Y < 0 && sideNum == 2 && crossProduct[0,2] > 0) {
                    targetObject.isHit = true;
                    isHit = true;
                    if (targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width) {
                        if (position.Y + height - targetObject.position.Y < maxLength) {
                            targetObject.speed.Y = 0;
                            targetObject.position.Y = position.Y + height;// 下に補正
                        }
                    }
                }
            }

        }
        private void CalculateCrossProductT2(Object targetObject)
        {
            if (defPositionVector[3].X > position.X + width && targetObject.position.X < position.X + width)
                vectors[0] = defPositionVector[4] - sideVectorStart[0];
            else vectors[0] = defPositionVector[3] - sideVectorStart[0];
            vectors[1] = defPositionVector[1] - sideVectorStart[1];
            vectors[2] = defPositionVector[1] - sideVectorStart[2];
            CheckDirection(targetObject);

            for(int i=0;i<=2;i++)
                crossProduct[0, i] = (sideVector[i].X * vectors[i].Y) - (vectors[i].X * sideVector[i].Y);

            // 初期化
            targetObject.isHit = false;
            isHit = false;
            isUnderSomeone = false;
            targetObject.isOnSomething = false;

            // マップチップの矩形領域内に入っているか否かで大まかに絞る
            if (position.X + width < targetObject.position.X) {
            }else if (targetObject.position.X + targetObject.width < position.X) {
            }else if (targetObject.position.Y + targetObject.height < position.Y) {
            }else if (position.Y +　height < targetObject.position.Y) {
            }else{
                // targetObjectは矩形内にいる
                if(targetObject.speed.Y > 0 && sideNum == 0 && crossProduct[0,0] > 0) {
                    targetObject.isHit = true;
                    isHit = true;
                    // 着地処理
                    targetObject.isOnSomething = true;
                    targetObject.jumpCount = 0;
                    targetObject.isJumping = false;
                    isUnderSomeone = true;

                    adjustObjectT2(targetObject);
                }
                else if (targetObject.speed.X > 0 && sideNum == 2 && crossProduct[0, 2] > 0) {// 左側が壁の斜面チップを対象
                    targetObject.isHit = true;
                    isHit = true;
                    if (targetObject.position.Y > position.Y - targetObject.height && targetObject.position.Y < position.Y + height) {
                            targetObject.position.X = position.X - targetObject.width;  // 左に補正
                    }
                }
                else if(targetObject.speed.Y < 0 && sideNum == 1 && crossProduct[0, 1] > 0) {
                    targetObject.isHit = true;
                    isHit = true;
                    if (targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width) {
                        if (position.Y + height - targetObject.position.Y < maxLength) {
                            targetObject.speed.Y = 0;
                            targetObject.position.Y = position.Y + height;// 下に補正
                        }
                    }
                }
            }
        }
        private void adjustObjectT1(Object targetObject)
        {
            Vector2 criterionVector = defPositionVector[3];
            float adjustDistance = 0;

            targetObject.speed.Y = 0;

            if (criterionVector.X > position.X && criterionVector.X < position.X + width) {
                switch(type) {
                    case 0:
                        adjustDistance = targetObject.position.X + targetObject.width / 2 - position.X;
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
            // 上端付近での当たり判定
            else if(criterionVector.X > position.X + width && targetObject.position.X < position.X + width) {
                targetObject.position.Y = position.Y - targetObject.height;                
            }
            // 下端付近での当たり判定
            else if(criterionVector.X < position.X && targetObject.position.X + targetObject.width > position.X) {
                targetObject.position.Y = position.Y + height - targetObject.height;
            }
        }
        private void adjustObjectT2(Object targetObject)
        {
            Vector2 criterionVector = defPositionVector[3];
            float adjustDistance = 0;

            targetObject.speed.Y = 0;

            if (criterionVector.X > position.X && criterionVector.X < position.X + width) {
                switch(type) {
                    case 0:
                        adjustDistance = ((position.X + width) - (targetObject.position.X + targetObject.width/ 2));
                        targetObject.position.Y = (position.Y + height - adjustDistance) - targetObject.height;
                        break;
                    case 1:
                        adjustDistance = ((position.X + width) - (targetObject.position.X + targetObject.width / 2)) * 1 / 2;// typeの倍数表現でまとめてもよい
                        targetObject.position.Y = (position.Y + height - adjustDistance) - targetObject.height;
                        break;
                    case 2:
                        adjustDistance = ((position.X + width) - (targetObject.position.X + targetObject.width / 2)) * 1 / 4;
                        targetObject.position.Y = (position.Y + height - adjustDistance) - targetObject.height;
                        break;
                }
            }
            // 下端付近での当たり判定
            else if(criterionVector.X > position.X + width && targetObject.position.X < position.X + width) {
                //targetObject.position.Y = position.Y - targetObject.height;
                targetObject.position.Y = position.Y + height - targetObject.height;
                
            }
            // 上端付近での当たり判定
            else if(criterionVector.X < position.X && targetObject.position.X + targetObject.width > position.X) {
                //targetObject.position.Y = position.Y + height - targetObject.height;
                targetObject.position.Y = position.Y - targetObject.height;
            }
        }
        #endregion
        #region IsHitLite
        /// <summary>
        /// 重なったら斜面上に押し出すだけの簡易版メソッド。地形としてはこのほうが使えるし、実際こちらを使っている.
        /// ただ、配置を工夫する必要がある
        /// </summary>
        /// <param name="targetObjecy"></param>
        public override void IsHitLite(Object targetObject)
        {
            if (!onCeiling) 
				IsHitLiteG(targetObject);
            else 
				IsHitLiteC(targetObject);
        }
        private void IsHitLiteG(Object targetObject)
        {
            /*if(isUp) {
                slopeVectorDown = position + new Vector2(0, height);      // 斜面下側
                slopeVectorUp = position + new Vector2(width, 0);         // 斜面上側
                slopeVector = (slopeVectorUp - slopeVectorDown);        // 斜面のベクトル
            }
            else{
                slopeVectorDown = position + new Vector2(width, height);  // 斜面下側
                slopeVectorUp = position;                                 // 斜面上側
                slopeVector = (slopeVectorDown - slopeVectorUp);        // 斜面のベクトル
            }*/

            defPositionVector[0] = targetObject.position;
            //defPositionVector[1] = defPositionVector[0] + new Vector2(targetObject.width, 0);
            defPositionVector[3] = defPositionVector[0] + new Vector2(targetObject.width/2, targetObject.height);

            if(isUp)
				vectors[0] = defPositionVector[3] - slopeVectorDown;
            else
				vectors[0] = defPositionVector[3] - slopeVectorUp;
            crossProduct[0, 0] = (slopeVector.X * vectors[0].Y) - (vectors[0].X * slopeVector.Y);

            if (position.X + width < targetObject.position.X) {
            } else if (targetObject.position.X + targetObject.width < position.X) {
            } else if (targetObject.position.Y + targetObject.height < position.Y) {
            } else if (position.Y +　height < targetObject.position.Y) {
            } else {
                // ここまで到達 = targetObjectは矩形内にいる
                if(crossProduct[0,0] > 0) {
                    targetObject.isHit = true;
                    isHit = true;
                    // 着地処理
                    targetObject.isOnSomething = true;
                    targetObject.jumpCount = 0;
                    targetObject.isJumping = false;
                    isUnderSomeone = true;
                    
                    float adjustDistance;
                    targetObject.speed.Y = 0;

                    if(isUp) {// blockのほうで両隣にslopeがあったら端の当たり判定をしないようにしよう
                        if (defPositionVector[3].X > position.X && defPositionVector[3].X < position.X + width) {// 原因は範囲を絞ってなかったせい
                            switch(type) {
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
                        if(defPositionVector[3].X > position.X + width && targetObject.position.X < position.X + width)// スルー
                            targetObject.position.Y = position.Y - targetObject.height;
                    }
                    else {
                        if (defPositionVector[3].X > position.X && defPositionVector[3].X < position.X + width) {
                            switch(type) {
                                case 0:
                                    adjustDistance = ((position.X + width) - (targetObject.position.X + targetObject.width/ 2));
                                    targetObject.position.Y = ((position.Y + height) - adjustDistance) - targetObject.height;
                                    break;
                                case 1:
                                    adjustDistance = ((position.X + width) - (targetObject.position.X + targetObject.width / 2)) * 1 / 2;
                                    targetObject.position.Y = (position.Y + height - adjustDistance) - targetObject.height;
                                    break;
                                case 2:
                                    adjustDistance = ((position.X + width) - (targetObject.position.X + targetObject.width / 2)) * 1 / 4;
                                    targetObject.position.Y = (position.Y + height - adjustDistance) - targetObject.height;
                                    break;
                            }
                        }
                        //if(defPositionVector[3].X > position.X + width && targetObject.position.X < position.X + width) {
                        // 上端付近の処理
                        if (/*!isRightBlock && !isLeftSlope && */defPositionVector[3].X < position.X && targetObject.position.X + targetObject.width > position.X)
                            targetObject.position.Y = position.Y - targetObject.height;// +height(笑)
                        //}
                        // フラグは効いてるが判定されないで堕ちてる
                    }
                    
                }
            }

        }
        private void IsHitLiteC(Object targetObject)    //:ceiling:天井
        {
            /*if(isUp) {
                slopeVectorDown = position;                                    // 斜面下側
                slopeVectorUp = position + new Vector2(width, height);         // 斜面上側
                slopeVector = (slopeVectorUp - slopeVectorDown);             // 斜面のベクトル
            }
            else{
                slopeVectorDown = position + new Vector2(width, 0);           // 斜面下側
                slopeVectorUp = position + new Vector2(0, height);            // 斜面上側
                slopeVector = (slopeVectorDown - slopeVectorUp);            // 斜面のベクトル
            }*/

            defPositionVector[0] = targetObject.position;
            defPositionVector[3] = defPositionVector[0] + new Vector2(targetObject.width/2, 0); // 上辺の中点

            if(isUp)vectors[0] = defPositionVector[3] - slopeVectorDown;
            else    vectors[0] = defPositionVector[3] - slopeVectorUp;
            crossProduct[0, 0] = (slopeVector.X * vectors[0].Y) - (vectors[0].X * slopeVector.Y);// widthが全部32だった！

            if (position.X + width < targetObject.position.X) {
            }else if (targetObject.position.X + targetObject.width < position.X) {
            }else if (targetObject.position.Y + targetObject.height < position.Y) {
            }else if (position.Y +　height < targetObject.position.Y) {
            }else{
                if(crossProduct[0,0] < 0) {
                    targetObject.isHit = true;
                    isHit = true;
                    
                    float adjustDistance;
                    targetObject.speed.Y *= -0.40f;

                    if(isUp) {
                        if (defPositionVector[3].X > position.X && defPositionVector[3].X < position.X + width) {// 原因は範囲を絞ってなかったせい
                            switch(type) {
                                case 0:
                                    adjustDistance = (targetObject.position.X + targetObject.width / 2 - position.X) * 1.2f;//+targetObject.speed.X
                                    targetObject.position.Y = (position.Y + adjustDistance);
                                    break;
                                case 1:
                                    adjustDistance = (targetObject.position.X + targetObject.width / 2 - position.X) * 1 / 2 * 1.2f;
                                    targetObject.position.Y = (position.Y + adjustDistance);
                                    break;
                                case 2:
                                    adjustDistance = (targetObject.position.X + targetObject.width / 2 - position.X) * 1 / 4;
                                    targetObject.position.Y = (position.Y + adjustDistance);
                                    break;
                            }
                        }
                        if(defPositionVector[3].X > position.X + width && targetObject.position.X < position.X + width)// スルー
                            //targetObject.position.Y = position.Y - targetObject.height;
                            targetObject.position.Y = position.Y + height;
                    }
                    else {
                        if (defPositionVector[3].X > position.X && defPositionVector[3].X < position.X + width) {
                            switch(type) {
                                case 0:
                                    adjustDistance = ((position.X + width) - (targetObject.position.X + targetObject.width/ 2));
                                    targetObject.position.Y = ((position.Y ) + adjustDistance);
                                    break;
                                case 1:
                                    adjustDistance = ((position.X + width) - (targetObject.position.X + targetObject.width / 2)) * 1 / 2;
                                    targetObject.position.Y = (position.Y  + adjustDistance) ;
                                    break;
                                case 2:
                                    adjustDistance = ((position.X + width) - (targetObject.position.X + targetObject.width / 2)) * 1 / 4;
                                    targetObject.position.Y = (position.Y  + adjustDistance);
                                    break;
                            }
                        }
                        //if(defPositionVector[3].X > position.X + width && targetObject.position.X < position.X + width) {
                        // 上端付近の処理
                        if (/*!isRightBlock && !isLeftSlope && */defPositionVector[3].X < position.X && targetObject.position.X + targetObject.width > position.X )
                            //targetObject.position.Y = position.Y - targetObject.height;
                            targetObject.position.Y = position.Y + height;
                        //}
                    }
                    
                }
            }
        }
        #endregion

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (onCeiling) 
				spriteBatch.Draw(texture, position, null, Color.White, 0, new Vector2(),new Vector2(1,1), SpriteEffects.FlipVertically, 0f);
            else 
				spriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
        }

    }
}
