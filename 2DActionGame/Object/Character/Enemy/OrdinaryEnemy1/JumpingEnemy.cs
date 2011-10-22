using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
    /// <summary>
    /// Stage1 跳ねる敵
    /// </summary>
    public class JumpingEnemy : Enemy
    {
		private SoundEffect jumpSound;
        private int jumpNum;
        public bool isDerived { get; set; }  // 分裂しているか否か：テクスチャのLoad時に用いる

        public JumpingEnemy(Stage stage,  float x, float y, int width, int height, int HP)
            : this(stage, x, y, width, height, HP, null)
        {
        }
        public JumpingEnemy(Stage stage, float x, float y, int width, int height, int HP, Character user)
			: base(stage, x, y, width, height, HP, user)
        {
            isMovingRight = true;
            isMovingAround = true;
            moveDistance = 80;
            speed.X = 5;
            delayTime = motionDelayTime + 1;

			Load();
        }
		protected override void Load()
		{
			base.Load();

			texture = content.Load<Texture2D>("Object\\Character\\JumpingEnemy1");
			jumpSound = content.Load<SoundEffect>("Audio\\SE\\jump2");
		}

        public override void Update()
        {
            //MotionDelay();
            if(isMovingAround && isAlive && isActive) MovementUpdate( -10, 40, 2);

            base.Update();
        }
        public override void UpdateAnimation()
        {
            if(!isDerived) {
                if(game.stageNum==6) {
                    if (speed.Y > 0)            animation.Update(0, 0, 64, 64, 0, 0);
                    else if (isOnSomething)     animation.Update(1, 0, 64, 64, 0, 0);
                    else if (speed.Y < 0)       animation.Update(2, 0, 64, 64, 0, 0);
                }
                else{
                    if (speed.Y > 0)            animation.Update(0, 0, 40, 40, 0, 0);
                    else if (isOnSomething)     animation.Update(1, 0, 40, 40, 0, 0);
                    else if (speed.Y < 0)       animation.Update(2, 0, 40, 40, 0, 0);
                }
            }
            else {
                if (speed.Y > 0)                animation.Update(0, 0, 20, 20, 0, 0);
                else if (isOnSomething)         animation.Update(1, 0, 20, 20, 0, 0);
                else if (speed.Y < 0)           animation.Update(2, 0, 20, 20, 0, 0);
            }
            /*
             * if (speed.Y > 0)             animation.Update(0, 0, textures.Width, textures.Height, 0, 0);
             else if (isOnSomething) animation.Update(1, 0, textures.Width, textures.Height, 0, 0);
             else if (speed.Y < 0) animation.Update(2, 0, textures.Width, textures.Height, 0, 0);
             */
        }
        public virtual void MovementUpdate(float jumpSpeed, int jumpTime, float moveRange)
        {
            jumpNum++;
            if (isOnSomething && jumpNum % jumpTime == 0) {
                speed.Y = jumpSpeed;// 40, -10, 2
                isJumping = true;
                if (!game.isMuted) jumpSound.Play(SoundControl.volumeAll, 0f, 0f);
            }
            else if (isOnSomething && jumpNum % jumpTime != 0)
                isJumping = false;

            // 規定範囲を往復する仕様 speed.Xには干渉し無い方針
            //RoundTripMotion(moveRange);
            isMovingRight = false;
            speed.X = -2.5f;

            // 機もい動き
            /*if (isMovingRight) {
                speed.X = 4;
                movedDistance += (float)speed.X;
            }
            else{
                speed.X = -4;
                movedDistance += (float)speed.X;
            }
            if (movedDistance > 20 || movedDistance < -20) {
                movedDistance = 0;
                if(isMovingRight) isMovingRight = false;
                else isMovingRight = true;
            }*/
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(isAlive && isActive) {
                if(!isMovingRight)  spriteBatch.Draw(texture, drawPos, animation.rect, Color.White);
                else                spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
                DrawComboCount(spriteBatch);
            }
        }
    }
}
