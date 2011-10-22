﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
    /// <summary>
    /// Stage2 滑る敵
    /// </summary>
    public class SkatingEnemy : Enemy
    {
		protected float stopDistance = 50;
        protected float defaultSpeed;
        
        public SkatingEnemy(Stage stage, float x, float y, int width, int height, int HP)
            : this(stage, x, y, width, height, HP, -8)
        {
        }
        public SkatingEnemy(Stage stage, float x, float y, int width, int height,int HP, float defSpeed)
            : base(stage, x, y, width, height,HP)
        {
            this.defaultSpeed = defSpeed;
			friction = .25f;
            //animation = new Animation(40, 40);

			Load();
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\SkatingEnemy");
		}

        public override void Update()
        {
            if (!isWinced && position.X > stage.player.position.X + stopDistance) speed.X = defaultSpeed;
            base.Update();
        }

        protected override void MotionDelay()
        {
            if (delayTime < motionDelayTime) {
                if (stage.player.normalComboCount < 3) {
                    speed = Vector2.Zero;
                    Gravity = 0;
                }
                isMovingAround = false;
                isInDamageMotion = true;
                Gravity = 0;
                isWinced = true;
            }
            else {
                isMovingAround = true;
                isInDamageMotion = false;
                Gravity = .60;
                isWinced = false;
            }
        }
        public override void UpdateAnimation()
        {
            animation.Update(2, 0, width, height, 6, 1);
        }

    }
}