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
    /// 水から飛び出して攻撃する魚
    /// </summary>
    public class RushingOutEnemy : FlyingOutEnemy
    {
		protected new readonly float defFlyingOutSpeed = -18;
		protected new readonly float defFlyingOutDistance = 200;

		private SoundEffect splashSound;
		/// <summary>
		/// (ジャンプして)頂点に辿り着いたか：一旦水面より上に飛んで欲しい
		/// </summary>
        private bool hasReached;
        private bool isEndingAttack;

        public RushingOutEnemy(Stage stage, float x, float y, int width, int height, float vx, int HP)
            : base(stage, x, y, width, height, HP)
        {
			LoadXML("RushingOutEnemy", "Xml\\Objects_Enemy_Stage2.xml");
			flyingOutSpeed = defFlyingOutSpeed;
            flyingOutDistance = defFlyingOutDistance;

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
				if (distance < flyingOutDistance) {
					if (counter == 0) {
						hasFlownOut = true;
						speed.Y = flyingOutSpeed;
						gravity = defGravity;
						if (!game.isMuted) splashSound.Play(SoundControl.volumeAll, 0f, 0f);
					}
					counter++;
				}
			} else if (hasFlownOut) {
				if (Math.Abs(speed.Y) < 5) {// 頂点付近 : 回転させる
					if (this.degree < 180) degree += 15;
					else degree = 180;
					hasReached = true;
				}

				// Waterの当たり判定に頼らず、元の位置を記憶しておいてそれを基準にフラグを立ててもいいかも.
				if (hasReached && (isInWater || Vector2.Distance(position, defPos/*+ new Vector2(0, -30)*/) < 5)) {// できればisInWAterで管理したいが...
					hasFlownOut = false;
					//degree = 0;   // 1frameで回転して不自然な動きになるので終了処理としてもう１段階必要だ.
					hasReached = false;
					counter = 0;
					speed = Vector2.Zero;
					gravity = 0;// 一応
					isEndingAttack = true;
				}
			}

			if (isEndingAttack) {
				if (this.degree < 360) degree += 15;
				else {
					degree = 0;
					isEndingAttack = false;
					speed = Vector2.Zero;
					position = defPos;
					gravity = 0;
				}
			}
        }
        

        public override void Draw(SpriteBatch spriteBatch)
        {
			if (IsActive() && IsBeingUsed()) {
				//base.DrawDamageBlink(spriteBatch, Color.Red, .60f);
				spriteBatch.Draw(texture, drawPos, animation.rect, Color.White, MathHelper.ToRadians(-degree), new Vector2(width / 2, height / 2), 1, SpriteEffects.None, 0);
			}
        }
    }
}
