using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
    /// <summary>
    /// 天井からPlayerに突進してくる敵
    /// </summary>
    public class FlyingOutEnemy2 : FlyingOutEnemy
    {
		// baseクラスで使われるときはbaseクラスに定義してある同名の定数が使われてしまうので非常に危険な気がしてきた
		// base.MovementUpdate();とかやると一発アウトな気がしてならない
		protected new readonly float defSpeed = 2;
		protected new readonly float defFlyingOutDistance　= 300;
		protected new readonly float defFlyingOutSpeed= -12;

		private SoundEffect flyingOutSound;
		private SoundEffectInstance flyingOutSoundInstance;

        public FlyingOutEnemy2(Stage stage,  float x, float y, int width, int height, int HP)
            : base(stage,x,y,width,height,HP)
        {
			LoadXML("FlyingOutEnemy2", "Xml\\Objects_Enemy_Stage2.xml");
			flyingOutSpeed = defFlyingOutSpeed;
            flyingOutDistance = defFlyingOutDistance;

			Load();
        }
		protected override void Load()
		{
			
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\FlyingOutEnemy2");
			flyingOutSound = content.Load<SoundEffect>("Audio\\SE\\wind");
			flyingOutSoundInstance = flyingOutSound.CreateInstance();
			flyingOutSoundInstance.Volume = .5f;
		}

        public override void UpdateAnimation()
        {
			if (!hasFlownOut) {
				animation.Update(3, 0, texture.Width / 3, texture.Height, 6, 1);
			} else {
				animation.Update(3, 0, texture.Width / 3, texture.Height, 3, 1);
			}
        }
        public override void MovementUpdate()
        {
            double degree;
			distance = Math.Abs(this.position.X - stage.player.position.X);
			/*float distanceY = Math.Abs(this.position.Y - stage.player.position.Y);
			flyingOutDistance = defFlyingOutDistance - distanceY;*/

			if (!hasFlownOut) {
				isOnSomething = false;
				gravity = 0;

				if (distance < flyingOutDistance) {
					if (flowCount == 0) {
						hasFlownOut = true;
						if (!game.isMuted) flyingOutSoundInstance.Play();
						// playerへの速度ベクトルを計算する
						degree = Math.Atan2(stage.player.position.Y - position.Y, stage.player.position.X - position.X);
						speed = 10 * Vector2.Normalize(new Vector2((float)Math.Cos(degree), (float)Math.Sin(degree)));
					}
					flowCount++;
				}
			} else if (hasFlownOut && isOnSomething) {
                isAlive = false;
                HP = -1;
                flowCount = 0;
            }
        }
    }
}
