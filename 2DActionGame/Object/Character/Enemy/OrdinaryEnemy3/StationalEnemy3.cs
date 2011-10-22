using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
    /// <summary>
    /// Stage3 通常の状態で浮いており、(多分Gravity=0)時々開いたときに弾などで攻撃してくる敵。
    /// defaultPositionから特定のMovePaterrnで繰り返し動く仕様にする予定。
    /// </summary>
    public class StationalEnemy3 : Enemy
    {
		private static readonly int jumpInterval = 180;

		/// <summary>
		/// 隠れているか否か：開いているときに射撃＆Playerが攻撃可。
		/// </summary>
        private bool isHiding = true;
        private Vector2 defaultPosition;
        private Turret turret;

        public StationalEnemy3(Stage stage, float x, float y, int width, int height, int HP)
            : base(stage, x, y, width, height,HP)
        {
            defaultPosition = new Vector2(x, y); 
            turret = new Turret(stage, this, new Vector2(5, 5), 5, 5, 0, 1, 1, false, true, false, 3);

			Load();
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\StationalEnemy3");
		}

        public override void Update()
        {
            if(isAlive && isActive) {
                //MotionDelay();
                if (isMovingAround) MovementUpdate();
            }
            base.Update();
        }

        public void MovementUpdate()
        {
            // ※浮かない仕様になりました
            //Gravity = .10;
            //if (position.Y > defaultPosition.Y + 50) speed.Y = -4.5f;
            
            if(isHiding && time > jumpInterval) {
                isHiding = false;
                isAttacking = true;
                turret.isBeingUsed = true;
                turret.Inicialize();
                time = 0;
                turret.isActive = true;    // stageのListに追加してない(character完結のturretの実験)ので手動で。
            }
            if (isAttacking) turret.Update();

            if(!isHiding && time > jumpInterval) {
                isHiding = true;
                isAttacking = false;
                turret.isBeingUsed = false;
                time = 0;
            }

            RoundTripMotion(2);
            time++;
        }
        public override void UpdateAnimation()
        {
            if (isHiding) animation.Update(0, 0, width, height, 1, 0);
            else animation.Update(1, 0, width, height, 1, 0);
        }

    }
}
