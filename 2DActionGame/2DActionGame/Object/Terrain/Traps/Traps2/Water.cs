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
    /// Stage2の水地形
    /// </summary>
    public class Water : Terrain
    {
		private bool hasSynced;
		/// <summary>
		/// アニメーションカウントを監視してアニメーションを合わせるために、
		/// 隣の水チップへの参照を持っておく
		/// </summary>
		public Water neighborWater { get; set; }
		/// <summary>
		/// アニメーションを合わせるメソッド。Stage.SetTerrainDirection()実行時に呼ばれる
		/// </summary>
		public void SyncAnimation()
		{
			if (neighborWater != null) {
				if (this.distanceToCamara < 640/* - 200/*/) {
					// isActive != 画面内に入ってる、だからまだずれてる
					this.animation.poseCount = neighborWater.animation.poseCount/* - 1*/;
					this.animation.count = neighborWater.animation.count;
				}
			}
		}

        public Water(Stage stage, float x, float y, int width, int height)
            : this(stage, x, y, width, height, new Vector2())
        {
        }
        public Water(Stage stage, float x, float y, int width, int height, Vector2 localPosition)
            : this(stage, x, y, width, height, null, localPosition)
        {
        }
        public Water(Stage stage, float x, float y, int width, int height, Object user, Vector2 localPosition)
            : base(stage, x, y, width, height)
        {
            this.user = user;
            this.localPosition = localPosition;

			Load();
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Terrain\\Water");
		}
        public override void Update()
        {
			if (IsActive()) {
				if (!hasSynced) {
					SyncAnimation();
					hasSynced = true;
				}
				UpdateAnimation();
			}
        }
        public override void UpdateAnimation()
        {
            animation.Update(3, 0, width, height, 12, 1);
        }

        public override void IsHit(Object targetObject)
        {
            if (firstTimeInAFrame) {// 1人or1匹でも触れていたらisHit=trueのままでいい
                isHit = false;
                //targetObject.isInWater = false;
            }

            targetObject.isInWater = false;
            targetObject.isHit = false;

            //if (targetObject.speed.Y != 0) targetObject.isOnSomething = false; //ここにそのまま書くと常にfalse

            if (targetObject.position.X + targetObject.width < position.X) {
            }else if (position.X + width < targetObject.position.X) {
            }else if (targetObject.position.Y + targetObject.height < position.Y) {
            }else if (position.Y + height < targetObject.position.Y) {
            }else{
                // 当たりあり
                if(targetObject is RushingOutEnemy &&(targetObject as RushingOutEnemy).hasFlownOut)
                {}

                isHit = true;
                targetObject.isHit = true;
                targetObject.hasTouchedWater = true;
                firstTimeInAFrame = false;
                targetObject.isInWater = true;
                // 追加
                if (/*isOn &&*/ targetObject.position.Y > position.Y || targetObject.position.Y + targetObject.height > position.Y) { // isUNderでもWAterの下にいるとは限らないがとりあえず. 
                    targetObject.isInWater = true;// もっと確実な条件があればよいが...  
                    isInWater = true;
                }

                /*
                // ↓――――――――――――――――――――――当たり判定処理――――――――――――――――――――――――↓
                // 下に移動中
                if (targetObject.speed.Y > 0 && !isUnder) {
                    if (targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width)                    {
                        //座標を指定しないとBlockに接してジャンプしたときにｷﾞﾘｷﾞﾘで乗ってしまう
                        // ブロックの上端
                        if (targetObject.position.Y + targetObject.height - position.Y < maxLength)                        {
                            if (!stage.reverse.isReversed)                            {
                                targetObject.speed.Y = 0;
                                targetObject.position.Y = position.Y - targetObject.height;  // 上に補正
                            }

                            targetObject.isOnSomething = true;
                            targetObject.jumpCount = 0;      // Playerに限定したかったが諦めた
                            targetObject.isJumping = false;　// 着地したらJumpできるように
                        }
                    }
                }
                // 上に移動中
                else if (targetObject.speed.Y < 0 && !isOn)  {
                    if (!stage.reverse.isReversed) {
                        if (targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width)  {
                            // ブロックの下端
                            if (position.Y + height - targetObject.position.Y < maxLength) {
                                targetObject.speed.Y = 0;
                                targetObject.position.Y = position.Y + height;   // 下に補正
                            }
                        }
                    }
                }
                // 右に移動中
                if (targetObject.speed.X > 0 && !isRight) {
                    if (!stage.reverse.isReversed) {
                        if (targetObject.position.Y > position.Y - targetObject.height && targetObject.position.Y < position.Y + height)  {
                            // ブロックの左端
                            if ((targetObject.position.X + targetObject.width) - position.X < maxLength) {
                                targetObject.position.X = position.X - targetObject.width;  // 左に補正
                            }
                        }
                    }
                }
                // 左に移動中
                else if (targetObject.speed.X < 0 && !isLeft) {
                    if (!stage.reverse.isReversed) {
                        if (targetObject.position.Y + targetObject.height > position.Y && targetObject.position.Y < position.Y + height)  {
                            // ブロックの右端
                            if ((position.X + width) - targetObject.position.X < maxLength) {
                                targetObject.position.X = position.X + width;   // 右に補正
                            }
                        }
                    }
                }*/

                
            }
        }

        /// <summary>
        /// 忘れがちだが、最初に背景に描画してしまうようなstaticな地形はif(isActive)のような条件を加えないように.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // 水面だけ描画&アニメーションする
            if ((isOn && !isUnder) || (!isOn && !isUnder))
                spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);// 必ずvectorで.(staticの場合)
        }

    }
}
