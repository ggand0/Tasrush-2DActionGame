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
    /// ２回ほど跳ねて、空中で止まって真下にビームを放ち、また動く敵
    /// </summary>
    public class FlyingEnemy3 : JumpingEnemy
    {
		protected new readonly float defSpeed = 2;
		protected new readonly float jumpSpeed = -16;
		public new readonly byte defMovePattern = 1;

        private Turret turret;

        public FlyingEnemy3(Stage stage, float x, float y, int width, int height, int HP)
            : base(stage, x, y, width, height,HP)
        {
			LoadXML("FlyingEnemy2", "Xml\\Objects_Enemy_Stage2.xml");
            //turret = new Turret(stage, this, new Vector2(5, 5), 5, 5, 0, 1);// ビームを放つタイプ：実装としてはThunderの方が近いのでそっちで.
            turret = new Turret(stage, this, new Vector2(5, 5), 5, 5, 0, 0, 1, false, true, 0, 0);
            turret.bulletSpeed = new Vector2(0, 10);
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\FlyingEnemy3");
		}

        public override void UpdateAnimation()
        {
			if (speed.Y > 0) {
				animation.Update(1, 0, width, height, 6, 1);
			} else if (isOnSomething) {
				animation.Update(1, 0, width, height, 0, 0);
			} else if (speed.Y < 0) {
				animation.Update(1, 0, width, height, 6, 1);
			}
        }

        public override void MovementUpdate(float jumpSpeed, int jumpTime, float roundTripSpeed)
        {
            base.MovementUpdate(jumpSpeed, jumpTime, roundTripSpeed);// -16, 40
            turret.position = this.position;
            turret.isBeingUsed = true;

			if (IsActive()) {// isAlive
				if (Math.Abs(speed.Y) < 5) {// 頂点付近なら
					//turret.isBeingUsed = true; // BulletのUpdateAni()とかはif(turret.isBeingUsed)で
					//条件付けてるからtrueのままでいいかも
					turret.UpdateShootingOnce(5);
				}
			}
        }

    }
}
