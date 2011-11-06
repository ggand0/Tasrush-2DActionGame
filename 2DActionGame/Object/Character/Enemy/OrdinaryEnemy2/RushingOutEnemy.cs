using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
    /// <summary>
    /// Stage2 魚
    /// </summary>
    public class RushingOutEnemy : FlyingOutEnemy
    {
		private SoundEffect splashSound;
        private bool hasReached;    // (ジャンプして)頂点に辿り着いたか：一旦水面より上に飛んで欲しいので.
        private bool isEndingAttack;

        public RushingOutEnemy(Stage stage, float x, float y, int width, int height, float vx, int HP)
            : base(stage, x, y, width, height, HP)
        {
            defaultPosition = new Vector2(x, y);
            flyingOutspeed = -18;
            flyingOutDistance = 200;

			Load();
        }
		protected override void Load()
		{
			base.Load();

			texture = content.Load<Texture2D>("Object\\Character\\RushingOutEnemy");
			splashSound = content.Load<SoundEffect>("Audio\\SE\\water");
			animation = new Animation(texture.Width, texture.Height);
		}

        public override void UpdateAnimation()
        {
            animation.Update(2, 0, 48, 48, 9, 1);
        }
        public override void MovementUpdate()
        {
            distance = Math.Abs(this.position.X - stage.player.position.X);
            if (!hasFlownOut && !isEndingAttack) {
                isOnSomething = false;
                gravity = 0; 
            }

            if(distance < flyingOutDistance && !hasFlownOut&& !isEndingAttack) {
                if(counter == 0) {
                    hasFlownOut=true;
                    speed.Y = flyingOutspeed;
                    gravity = defGravity;
					if (!game.isMuted) splashSound.Play(SoundControl.volumeAll, 0f, 0f);
                }
                counter++;
            }
            if(hasFlownOut && Math.Abs(speed.Y) < 5) {// 頂点付近 : 回転させる
                if (this.degree < 180) degree += 15;
                else degree = 180;
                hasReached = true;
            }

            // Waterの当たり判定に頼らず、元の位置を記憶しておいてそれを基準にフラグを立ててもいいかも.
            if(hasFlownOut && hasReached && (isInWater || Vector2.Distance(position, defaultPosition /*+ new Vector2(0, -30)*/) < 5)) {// できればisInWAterで管理したいが...
                hasFlownOut = false;
                //degree = 0;   // 1frameで回転して不自然な動きになるので終了処理としてもう１段階必要だ.
                hasReached = false;
                counter = 0;
                speed = Vector2.Zero;
                gravity = 0;// 一応
                isEndingAttack = true;
            }
            if(isEndingAttack)
                if (this.degree < 360) degree += 15;
                else {
                    degree = 0;
                    isEndingAttack = false;
                    speed = Vector2.Zero;
                    position = defaultPosition;
                    gravity = 0;
                }
        }
        

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isActive && isAlive)
                spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, MathHelper.ToRadians(-degree), new Vector2(width/2, height/2), 1, SpriteEffects.None, 0);
        }
    }
}
