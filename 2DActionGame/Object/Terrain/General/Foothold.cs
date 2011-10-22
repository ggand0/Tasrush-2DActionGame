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
	/// 足場クラス。Playerは下から通り抜けることができる
	/// </summary>
    public class Foothold : Terrain
    {
        public Foothold()
        {
        }
        public Foothold(Stage stage,  float x, float y, int width, int height)
            : base(stage,x,y,width,height)
        {
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Terrain\\Foothold");
		}

        public override void IsHit(Object targetObject)
        {
            if(targetObject.isFirstTimeInAFrame) { //3/14 もう何かに乗ってると判断されてるなら飛ばしてもいいかもしれない
                isHit = false;// 複数の敵と判定するので結局falseになってしまう
                //(targetObject as Character).onConveyor = false;
                targetObject.isFirstTimeInAFrame = false;
                isHitCB = false;
                //isOnSomething = false;
            }
            //isHitCB = false;
            //if(targetObject.speed.Y != 0)targetObject.isOnSomething = false; //ここにそのまま書くと常にfalse
            
            if (targetObject.position.X + targetObject.width < position.X) {
            } else if (position.X + width < targetObject.position.X) {
            } else if (targetObject.position.Y + targetObject.height < position.Y) {
            } else if (position.Y + height < targetObject.position.Y) {
            } else {
                Vector2 criterionVector = targetObject.position + new Vector2(targetObject.width / 2, targetObject.height);

                // 当たりあり
                isHit = true;// collapsingBlock:objectsのはtrueになってもdynamicTerrrainsのほうはtrueにならない(解決)
                targetObject.isHit = true;
                isFirstTimeInAFrame = false;

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
                            targetObject.isOnSomething = true;
                            targetObject.jumpCount = 0;      // Playerに限定したかったが諦めた
                            targetObject.isJumping = false;　// 着地したらJumpできるように

                            targetObject.position += this.speed;

                            if (type == 0) targetObject.friction = .40f;
                            else if (type == 1) targetObject.friction = .05f;
                            
                            isUnderSomeone = true;
                        }
                    }
                }
                /*// 上に移動中
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
                        }
                    }
                } 
                // 左に移動中
                else if (targetObject.speed.X < 0 && !isLeft && !isLeftSlope) {
                    if (targetObject.position.Y + targetObject.height> position.Y  && targetObject.position.Y < position.Y + height) {
                        // ブロックの右端
                        if ((position.X + width) - targetObject.position.X < maxLength) {
                            targetObject.position.X = position.X + width;   // 右に補正
                        }
                    }
                }*/
            }

        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sprite)
        {
            sprite.Draw(texture, position, Color.White);
        }
    }
}
