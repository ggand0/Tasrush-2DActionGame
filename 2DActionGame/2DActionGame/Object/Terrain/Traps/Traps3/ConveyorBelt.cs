using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
    /// <summary>
    /// ベルトコンベア
    /// </summary>
    public class ConveyorBelt : Block
    {
		/// <summary>
		/// ベルトの回す向き
		/// </summary>
        private bool toRight;
        private List<Object> ridingObjects = new List<Object>();

        public ConveyorBelt(Stage stage, float x, float y, int width, int height)
            : base(stage, x, y, width, height)
        {
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Terrain\\ConveyorBelt");
		}

        public override void Update()
        {
            if(isUnderSomeone) {// 誰かに乗られていたら
                    
            }
            foreach(Object obj in ridingObjects) {
                    if(toRight) obj.speed.X = 2.5f;
                    else obj.speed.X = -2.5f;
            }
            UpdateAnimation();

            base.Update();

            ridingObjects.Clear();// よくわからんので(isInFirstFrameが機能してるかかなり怪しいので)とりあえずUpdate()の最後に.
        }
        public override void UpdateAnimation()
        {
            animation.Update(3, 0, width, height, 6, 1);
        }

        public override void IsHit(Object targetObject)
        {
            
            if(targetObject.isFirstTimevsCB) {
                (targetObject as Character).onConveyor = false;
                targetObject.isFirstTimevsCB = false;
                targetObject.isHitCB = false;
            }
            //targetObject.isHitCB = false;

            //if(isFirstTimevsCB &&)
            if (targetObject.position.X + targetObject.width < position.X) {
            } else if (position.X + width < targetObject.position.X) {
            } else if (targetObject.position.Y + targetObject.height < position.Y) {
            } else if (position.Y + height < targetObject.position.Y) {
            } else {
                Vector2 criterionVector = targetObject.position + new Vector2(targetObject.width / 2, targetObject.height);

                // 当たりあり
                isHit = true;// collapsingBlock:objectsのはtrueになってもdynamicTerrrainsのほうはtrueにならないでござる(解決)
                //targetObject.isHit = true;
                targetObject.isHitCB = true;

                //firstTimeInAFrame = false;
                ontop = false; onleft = false; onright = false;// 初期化(デバッグ用)

                // ↓――――――――――――――――――――――当たり判定処理――――――――――――――――――――――――↓
                // targetObjectが下に移動中
                if (targetObject.speed.Y > 0 && !isUnder) {
                    //if(isLeftSlope && criterionVector.X > position.X &&  criterionVector.X < position.X + width) {
                    //}
                    if ((!isRightSlope && !isLeftSlope && targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width)
                        || (!isLeftSlope && isRightSlope && criterionVector.X > position.X && targetObject.position.X < position.X + width)
                        || (!isRightSlope && isLeftSlope && targetObject.position.X + targetObject.width > position.X && criterionVector.X < position.X + width)
                        || (isLeftSlope && isRightSlope && criterionVector.X > position.X && criterionVector.X < position.X + width))
                    {
                        //座標を指定しないとBlockに接してジャンプしたときにｷﾞﾘｷﾞﾘで乗ってしまう
                        // ブロックの上端
                        if (targetObject.position.Y + targetObject.height - position.Y < maxLength) {
                            targetObject.speed.Y = 0;
                            targetObject.position.Y = position.Y - targetObject.height;  // 上に補正
                            if (targetObject is Player)
                            { }
                            targetObject.isOnSomething = true;
                            targetObject.jumpCount = 0;      // Playerに限定したかったが諦めた
                            targetObject.isJumping = false;　// 着地したらJumpできるように

                            targetObject.position.X += this.speed.X;  // これで慣性を再現できるか！？

                            if (type == 0) targetObject.friction = defFriction;
                            else if (type == 1) targetObject.friction = .05f;
                            
                            ontop = true;
                            isUnderSomeone = true;
                        }
                    }
                }
                // 上に移動中
                else　if(targetObject.speed.Y < 0 && !isOn) {
                    if (targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width) {
                        // ブロックの下端
                        if (position.Y + height - targetObject.position.Y < maxLength) {
                            targetObject.speed.Y = 0;
                            targetObject.position.Y = position.Y + height;   // 下に補正
                        }
                    }
                }
                // 右に移動中
                if (targetObject.speed.X > 0 && !isRight && !isRightSlope) {//(stage.game.isScrolled ? 0 : this.scalarSpeed))
                    if (targetObject.position.Y > position.Y - targetObject.height && targetObject.position.Y < position.Y + height) {
                        // ブロックの左端
                        if ((targetObject.position.X + targetObject.width) - position.X < maxLength) {
                            targetObject.position.X = position.X - targetObject.width;  // 左に補正
                            onleft = true;
                        }
                    }
                } 
                // 左に移動中
                else if (targetObject.speed.X < 0 && !isLeft && !isLeftSlope) {
                    if (targetObject.position.Y + targetObject.height> position.Y  && targetObject.position.Y < position.Y + height) {
                        // ブロックの右端
                        if ((position.X + width) - targetObject.position.X < maxLength) {
                            targetObject.position.X = position.X + width;   // 右に補正
                            onright = true;
                        }
                    }
                }
            }


            //if (targetObject.isHit) targetObject.isHitCB = true;// これで"CBに当たったら"ができる？

            if (targetObject.isHitCB && isUnderSomeone) {//(targetObject.isHit || this.isUnderSomeone) { cBにisHitしてなきゃ意味ないのよね...
                ridingObjects.Add(targetObject);
                (targetObject as Character).onConveyor = true;
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);
        }
    }
}
