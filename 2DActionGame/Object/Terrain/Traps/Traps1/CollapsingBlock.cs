using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
    public class CollapsingBlock : Block
    {
        private int timeToFall = 10;

        public CollapsingBlock()
        { 
        }
        public CollapsingBlock(Stage stage,  float x, float y, int width, int height)
            : this( stage, x, y, width, height, null, Vector2.Zero)
        {
        }
        public CollapsingBlock(Stage stage, float x, float y, int width, int height, Object user, Vector2 localPosition)
            : base(stage, x, y, width, height, 0, user, localPosition)
        {
            gravity = .40;
			Load();
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Terrain\\Slopes\\0" + (game.stageNum - 1).ToString() + "\\Block02");
		}

        public override void Update()
        {
            if(isUnderSomeone) UpdateNumbers();
        }
        protected override void UpdateNumbers()
        {
			if (counter > timeToFall) {
				//gravity = .40;
				speed.Y += (float)gravity * timeCoef;

				if (speed.X > 0) {
					speed.X += -(.40f * friction) * timeCoef;// 要調整箇所(敵の挙動的な意味で)0.5 accel
					if (speed.X < 0) speed.X = 0;
				} else if (speed.X < 0) {
					speed.X += (.40f * friction) * timeCoef;
					if (speed.X > 0) speed.X = 0;
				}

				if (System.Math.Abs(speed.Y) > maxSpeed) speed.Y = maxSpeed;
				// 位置に加算
				position.X += speed.X * timeCoef;
				position.Y += speed.Y * timeCoef;
				// 端
				if (position.Y < 0) position.Y = 0;
			}

            counter++;
        }
        public override void IsHit(Object targetObject)
        {
			if (firstTimeInAFrame) {
				isHit = false;// 複数の敵と判定するので結局falseになってしまう
			}
            targetObject.isHit = false;

            if (targetObject.position.X + targetObject.width < position.X) {
            }else if (position.X + width < targetObject.position.X) {
            }else if (targetObject.position.Y + targetObject.height < position.Y) {
			} else if (position.Y + height < targetObject.position.Y) {
			} else {
				Vector2 criterionVector = targetObject.position + new Vector2(targetObject.width / 2, targetObject.height);
				// 当たりあり
				isHit = true;// collapsingBlock:objectsのはtrueになってもdynamicTerrrainsのほうはtrueにならないでござる(解決)
				targetObject.isHit = true;
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
						//座標を指定しないとBlockに接してジャンプしたときにｷﾞﾘｷﾞﾘで乗ってしまう
						// ブロックの上端
						if (targetObject.position.Y + targetObject.height - position.Y < maxLength) {
							targetObject.speed.Y = 0;
							targetObject.position.Y = position.Y - targetObject.height;  // 上に補正
							targetObject.isOnSomething = true;
							targetObject.jumpCount = 0;             // Playerに限定したかったが諦めた
							targetObject.isJumping = false;　       // 着地したらJumpできるように
							targetObject.position.X += this.speed.X;  // これで慣性を再現できるか！？

							if (type == 0) targetObject.friction = defFriction;
							else if (type == 1) targetObject.friction = .05f;

							ontop = true;
							if (!(targetObject is SkatingEnemy)) isUnderSomeone = true;
						}
					}
				}
					// 上に移動中
				else if (targetObject.speed.Y < 0 && !isOn) {
					if (targetObject.position.X + targetObject.width > position.X && targetObject.position.X < position.X + width) {
						// ブロックの下端
						if (position.Y + height - targetObject.position.Y < maxLength) {
							targetObject.speed.Y = 0;
							targetObject.position.Y = position.Y + height;   // 下に補正
						}
					}
				}

				// 一旦横の判定を切ってみる
				// 右に移動中
				/*if (targetObject.speed.X > 0 && !isRight && !isRightSlope) {
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
				}*/
			}
        } 

        public override void Draw(SpriteBatch sprite)
        {
            if (isActive) {
                sprite.Draw(texture, drawPos, Color.White);
            }
        }
    }
}
