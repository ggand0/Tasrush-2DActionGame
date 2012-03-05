using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
    /// <summary>
    /// 歩くと崩れる床:乗ってから数フレームの猶予の後消滅.壊れるときエフェクトがあっていい
    /// </summary>
    public class CollapsingFloor : CollapsingBlock
    {
        public CollapsingFloor(Stage stage, float x, float y, int width, int height)
            : this(stage, x, y, width, height, null, new Vector2())
        {
        }
        public CollapsingFloor(Stage stage, float x, float y, int width, int height, Object user, Vector2 localPosition)
            : base(stage, x, y, width, height, user, localPosition)
        {
        }
        protected override void Load()
        {
            base.Load();
            texture = content.Load<Texture2D>("Object\\Terrain\\CollapsingFloor");
        }

        public override void IsHit(Object targetObject)
        {
            if (targetObject is RushingOutEnemy) return;

            if (firstTimeInAFrame) {
                isHit = false;// 複数の敵と判定するので結局falseになってしまう
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
                if (targetObject is Player) isHitPlayer = true;
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
                        // ブロックの上端
                        if (targetObject.position.Y + targetObject.height - position.Y < maxLength) {
                            targetObject.speed.Y = 0;
                            targetObject.position.Y = position.Y - targetObject.height;  // 上に補正
                            targetObject.isOnSomething = true;
                            targetObject.jumpCount = 0;
                            targetObject.isJumping = false;
                            targetObject.position.X += this.speed.X;

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

            }
        }
    }
}
