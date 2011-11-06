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
    /// 触れるとダメージを受けるトゲBlock
    /// </summary>
    public class DamageBlock : Block
    {
        public DamageBlock(Stage stage,  float x, float y, int width, int height)
            : this(stage, x, y, width, height, null, Vector2.Zero)
        {
        }
        public DamageBlock(Stage stage, float x, float y, int width, int height, Object user, Vector2 localPosition)
            : base(stage, x, y, width, 0, height,user, localPosition)
        {
        }
		protected override void Load()
		{
			//base.Load();
			texture = content.Load<Texture2D>("Object\\Terrain\\damageBlock");
		}

        public override void Update()
        {
            if (isActive && isAlive)
                if (user is SnowBall) UpdateNumbers();
        }

        public override void IsHit(Object targetObject)
        {
			if (targetObject.firstTimeInAFrame) { //3/14 もう何かに乗ってると判断されてるなら飛ばしてもいいかもしれない
				isHit = false;// 複数の敵と判定するので結局falseになってしまう
				//(targetObject as Character).onConveyor = false;
				targetObject.firstTimeInAFrame = false;
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
				isHit = true;// collapsingBlock:objectsのはtrueになってもdynamicTerrrainsのほうはtrueにならないでござる(解決)
				targetObject.isHit = true;
				//targetObject.isDamaged = true; // 上だけにしたい
				//targetObject.isOnSomething ~ 
				firstTimeInAFrame = false;
				ontop = false; onleft = false; onright = false;// 初期化(デバッグ用)

				// targetObjectが下に移動中
				if (targetObject.speed.Y > 0 && !isUnder) {
					//if(isLeftSlope && criterionVector.X > position.X &&  criterionVector.X < position.X + width) {
					//}
					if ((!isRightSlope && !isLeftSlope && targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width)
						|| (!isLeftSlope && isRightSlope && criterionVector.X > position.X && targetObject.position.X < position.X + width)
						|| (!isRightSlope && isLeftSlope && targetObject.position.X + targetObject.width > position.X && criterionVector.X < position.X + width)
						|| (isLeftSlope && isRightSlope && criterionVector.X > position.X && criterionVector.X < position.X + width)) {
						// 座標を指定しないとBlockに接してジャンプしたときにｷﾞﾘｷﾞﾘで乗ってしまう
						// ブロックの上端
						if (targetObject.position.Y + targetObject.height - position.Y < maxLength) {
							targetObject.speed.Y = 0;
							targetObject.position.Y = position.Y - targetObject.height;				// 上に補正
							targetObject.isOnSomething = true;
							targetObject.jumpCount = 0;												// Playerに限定したかったが諦めた
							targetObject.isJumping = false;　										// 着地したらJumpできるように

							targetObject.position.X += this.speed.X;								// これで慣性を再現できるか！？

							if (type == 0) targetObject.friction = defFriction;
							else if (type == 1) targetObject.friction = .05f;

							ontop = true;
							isUnderSomeone = true;
							targetObject.isDamaged = true;
						}
					}
				}
					// 上に移動中
				else if (targetObject.speed.Y < 0 && !isOn) {
					if (targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width) {
						// ブロックの下端
						if (position.Y + height - targetObject.position.Y < maxLength) {
							targetObject.speed.Y = 0;
							targetObject.position.Y = position.Y + height;				// 下に補正
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
					if (targetObject.position.Y + targetObject.height > position.Y && targetObject.position.Y < position.Y + height) {
						// ブロックの右端
						if ((position.X + width) - targetObject.position.X < maxLength) {
							targetObject.position.X = position.X + width;   // 右に補正
							onright = true;
						}
					}
				}
			}
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
           /* if (user == null) {// == staticTerrain
                spriteBatch.Draw(texture, position, Color.White);
			} else if (isActive && isBeingUsed) {
				spriteBatch.Draw(texture, drawPos, Color.White);
			}*/
			spriteBatch.Draw(texture, position/*drawVectorvector*/, Color.White);
       }
    }
}
