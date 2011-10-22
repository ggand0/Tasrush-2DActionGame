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
    /// TracingFoothold用の、辿るべき線クラス.矩形内のTFに決まった方向の速度を与えて移動させる.
    /// </summary>
    public class Line : Terrain
    {
        //private Vector2 tmpSpeed;

        public Line(Stage stage, float x, float y, int width, int height)
            : base(stage, x, y, width, height)
        {
        }

        /// 方向を変えるときはdegreeで指定させる:斜めは当面想定していないので4方向*2方向で向きを決定.
        /// type0:degree=0, type1:degree=90, type2:degree=180, type3:degree= 270 
        public Line(Stage stage, float x, float y, int width, int height, int type)
            : base(stage, x, y, width, height)
        {
            degree = type * 90;
        }

        public override void IsHit(Object targetObject)
        {
            if (isFirstTimeInAFrame)
                isHit = false;
             targetObject.isHit = false;

            //if (targetObject.speed.Y != 0) targetObject.isOnSomething = false; //ここにそのまま書くと常にfalse

            if (targetObject.position.X + targetObject.width < position.X) {
            }else if (position.X + width < targetObject.position.X) {
            }else if (targetObject.position.Y + targetObject.height < position.Y) {
            }else if (position.Y + height < targetObject.position.Y) {
            }else{   // 当たりあり
                isHit = true;
                targetObject.isHit = true;
                isFirstTimeInAFrame = false;

                // 動かす方向へ速度を与える
                //targetObject.speed = tmpSpeed;
            }

        }
        /// <summary>
        /// デバッグ用.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
        }

    }

}
