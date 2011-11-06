using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
    /// <summary>
    /// Stage2 天井からPlayerに突進してくる敵
    /// </summary>
    public class FlyingOutEnemy2 : FlyingOutEnemy
    {
        public FlyingOutEnemy2(Stage stage,  float x, float y, int width, int height, int HP)
            : base(stage,x,y,width,height,HP)
        {   
            flyingOutspeed = 16;        // 下方向
            flyingOutDistance = 100;    // 割と遠めに設定

			Load();
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\FlyingOutEnemy2");
		}

        public override void UpdateAnimation()
        {
            if (!hasFlownOut)
				animation.Update(3, 0, texture.Width / 3, texture.Height, 6, 1);
            else
				animation.Update(3, 0, texture.Width / 3, texture.Height, 3, 1);
        }
        public override void MovementUpdate()
        {
            double degree;
            distance = Math.Abs(this.position.X - stage.player.position.X);

            if (!hasFlownOut) {
                isOnSomething = false;
                gravity = 0;
            }
            if(distance < flyingOutDistance && !hasFlownOut) {
                if(counter == 0) {
                    hasFlownOut=true;

                    // playerへの速度ベクトルを計算する
                    degree = Math.Atan2(stage.player.position.Y - position.Y, stage.player.position.X - position.X);
                    speed = 10 * Vector2.Normalize(new Vector2((float)Math.Cos(degree), (float)Math.Sin(degree)));
                }
                counter++;
            }

            if (hasFlownOut && isOnSomething) {
                isAlive = false;
                HP = -1;
                counter = 0;
            }
        }
    }
}
