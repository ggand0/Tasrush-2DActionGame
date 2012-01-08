using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
    /// <summary>
    /// 滑って突進してくる敵
    /// </summary>
    public class SkatingEnemy : Enemy
    {
		protected readonly float defSpeed;
		protected readonly float spFriction = .25f;
		protected readonly float defStopDistance = 50;
		protected float stopDistance;
        
        public SkatingEnemy(Stage stage, float x, float y, int width, int height, int HP)
            : this(stage, x, y, width, height, HP, -8)
        {
        }
        public SkatingEnemy(Stage stage, float x, float y, int width, int height,int HP, float defSpeed)
            : base(stage, x, y, width, height,HP)
        {
			LoadXML("SkatingEnemy", "Xml\\Objects_Enemy_Stage2.xml");
            this.defSpeed = defSpeed;
			friction = spFriction;
			stopDistance = defStopDistance;

			Load();
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\SkatingEnemy");
		}

        public override void Update()
        {
			if (!isWinced && position.X > stage.player.position.X + stopDistance) {
				speed.X = defSpeed;
			}

            base.Update();
        }

        protected override void MotionDelay()
        {
			if (delayTime < motionDelayTime) {
				if (stage.player.normalComboCount < 3) {
					speed = Vector2.Zero;
					gravity = 0;
				}
				isMovingAround = false;
				isInDamageMotion = true;
				gravity = 0;
				isWinced = true;
			} else {
				isMovingAround = true;
				isInDamageMotion = false;
				gravity = defGravity;
				isWinced = false;
			}
        }
        public override void UpdateAnimation()
        {
            animation.Update(2, 0, width, height, 6, 1);
        }
    }
}