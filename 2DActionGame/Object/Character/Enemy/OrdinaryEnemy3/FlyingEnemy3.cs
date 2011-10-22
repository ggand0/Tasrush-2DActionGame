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
    /// Stgae3 "2回ほど跳ねて、空中で止まって真下にビームを放ち、また動く"
    /// </summary>
    public class FlyingEnemy3 : JumpingEnemy
    {
        private Turret turret;

        public FlyingEnemy3(Stage stage, float x, float y, int width, int height, int HP)
            : base(stage, x, y, width, height,HP)
        {
            //turret = new Turret(stage, this, new Vector2(5, 5), 5, 5, 0, 1);                               // ビームを放つタイプ：実装としてはThunderの方が近いのでそっちで.
            turret = new Turret(stage, this, new Vector2(5, 5), 5, 5, 0, 0, 1, false, true, false, 3, 0, 0);
            turret.bulletSpeed = new Vector2(0, 10);
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\FlyingEnemy3");
		}

        public override void UpdateAnimation()// 一瞬描画されなくなるのは多分ここのせいだな
        {
            if (speed.Y > 0) animation.Update(1, 0, width, height, 6, 1);
            else if (isOnSomething) animation.Update(1, 0, width, height, 0, 0);
            else if (speed.Y < 0) animation.Update(1, 0, width, height, 6, 1);
        }

        public override void MovementUpdate(float jumpSpeed, int jumpTime, float moveRange)
        {
            base.MovementUpdate(-16, 40, 3);
            turret.position = this.position;
            turret.isBeingUsed = true;
            /*if (isJumping) {
                turret.isBeingUsed = true;
                turret.Update();
            }*/
            if(isAlive) {
                if (Math.Abs(speed.Y) < 5) {// 頂点付近
                   //turret.isBeingUsed = true; // BulletのUpdateAni()とかはif(turret.isBeingUsed)で条件付けてるからtrueのままでいいかも
                   turret.UpdateShootingOnce(5);
                }
            }
        }

    }
}
