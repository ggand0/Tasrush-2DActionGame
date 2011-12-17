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
		public readonly float defSpeed;//-2.5f
		public readonly float jumpSpeed;//-10
		public new readonly byte defMovePattern = 1;
		public static readonly byte maxSoundEffectNum = 3;

		private SoundEffect jumpSound;
		/// <summary>
		/// ジャンプ回数。途中で行動を変えるときに使えるかも
		/// </summary>
        private int jumpNum;
		/// <summary>
		/// 分裂しているか否か：テクスチャのLoad時に用いる
		/// </summary>
        public bool isDerived { get; set; }
		public bool canPlayse { get; set; }

		public int JumpingEnemyNumInScreen()
		{
			return stage.activeCharacters.FindAll((x) => x is JumpingEnemy).Count;
		}

        public JumpingEnemy(Stage stage,  float x, float y, int width, int height, int HP)
            : this(stage, x, y, width, height, HP, null)
        {
        }
        public JumpingEnemy(Stage stage, float x, float y, int width, int height, int HP, Character user)
			: base(stage, x, y, width, height, HP, user)
        {
			
            isMovingAround = true;
            moveDistance = 80;
            speed.X = defSpeed;
			movePattern = defMovePattern;
			if (defSpeed > 0) {
				isMovingRight = true;
				turnsRight = true;
			}
            //delayTime = motionDelayTime + 1;

			Load();
        }
		protected override void Load()
		{
			base.Load();

			texture = content.Load<Texture2D>("Object\\Character\\JumpingEnemy1");
			jumpSound = content.Load<SoundEffect>("Audio\\SE\\jump2");
			LoadXML("JumpingEnemy", "Xml\\Objects_Enemy_Stage1.xml");
		}

        public override void Update()
        {
			if (isMovingAround && IsActive()) {
				//canPlayse = JumpingEnemyNumInScreen() <= maxSoundEffectNum;				
				MovementUpdate(jumpSpeed, 40, defSpeed);
			}
            base.Update();
        }
        public override void UpdateAnimation()
        {
			if (!isDerived) {
				if (game.stageNum == 6) {
					if (speed.Y > 0) animation.Update(0, 0, 64, 64, 0, 0);
					else if (isOnSomething) animation.Update(1, 0, 64, 64, 0, 0);
					else if (speed.Y < 0) animation.Update(2, 0, 64, 64, 0, 0);
				} else {
					if (speed.Y > 0) animation.Update(0, 0, 40, 40, 0, 0);
					else if (isOnSomething) animation.Update(1, 0, 40, 40, 0, 0);
					else if (speed.Y < 0) animation.Update(2, 0, 40, 40, 0, 0);
				}
			} else {
				if (speed.Y > 0) animation.Update(0, 0, 20, 20, 0, 0);
				else if (isOnSomething) animation.Update(1, 0, 20, 20, 0, 0);
				else if (speed.Y < 0) animation.Update(2, 0, 20, 20, 0, 0);
			}
        }
        public virtual void MovementUpdate(float jumpSpeed, int jumpTime, float roundTriipMoveSpeed)
        {
			switch (movePattern) {
				case 0:// ←に移動するだけ
					speed.X = defSpeed;
					break;
				case 1:// 往復移動
					//RoundTripMotion(defPos, moveDistance, roundTriipMoveSpeed);
					RoundTripMotion(defSpeed, defPos.X - moveDistance, defPos.X + moveDistance);
					turnsRight = isMovingRight;
					break;
				case 2:
					// 機もい動き
					if (isMovingRight) {
						speed.X = 4;
						moveDistance += (float)speed.X;
					} else {
						speed.X = -4;
						moveDistance += (float)speed.X;
					}
					if (moveDistance > 20 || moveDistance < -20) {
						moveDistance = 0;
						if (isMovingRight) isMovingRight = false;
						else isMovingRight = true;
					}
					break;
			}

			
			// 周期的にジャンプ
            if (isOnSomething && jumpNum % jumpTime == 0) {
                speed.Y = jumpSpeed;// 40, -10, 2
                isJumping = true;
				jumpNum++;
                if (!game.isMuted && canPlayse) jumpSound.Play(SoundControl.volumeAll, 0f, 0f);
			} else if (isOnSomething && jumpNum % jumpTime != 0) {
				isJumping = false;
			}
			jumpNum++;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
			base.Draw(spriteBatch);
			if (IsActive()) {
				DrawComboCount(spriteBatch);
			}
        }

    }
}
