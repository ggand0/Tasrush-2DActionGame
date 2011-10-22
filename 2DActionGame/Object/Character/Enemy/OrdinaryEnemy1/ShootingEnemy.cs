
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;


namespace _2DActionGame
{
    /// <summary>
    /// Stage1 動かずに弾を撃つ敵
    /// </summary>
    public class ShootingEnemy : Enemy
    {
        private static readonly Vector2 shootPosition=  new Vector2(10, 10);
        private Turret turret;

        public ShootingEnemy(Stage stage,  float x, float y, int width, int height, int HP)
            : base(stage, x, y, width, height, HP)
        {
            //animation = new Animation(48, 48);
            turret = new Turret(stage, this, shootPosition, 32, 32, 0, 1 ,1, false, true, false, 3, 0, 0, 4);
            stage.weapons.Add(turret);

			Load();
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\ShootingEnemy1");
		}

        public override void Update()
        {
            float distance;

            if(isActive && isAlive) {
				turret.isBeingUsed = true;// shootPositionは毎回Updateしないとダメらしい
                base.Update();
                
                if (isWinced) turret.isBeingUsed = false;
                else turret.isBeingUsed = true;
                distance = Vector2.Distance(stage.player.position, position);
                if (distance < 100) turret.isBeingUsed = false;
                distance = stage.player.position.X - position.X;
                if (distance > 200) turret.isBeingUsed = false;
            }
            else
				turret.isBeingUsed = false;// 毎回falseにするのは無駄だ
        }

        /*private void Shoot(Object targetObject)
        {
            double rad = Math.Atan2(targetObject.position.Y - position.Y, targetObject.position.X - position.X);
            float speed = 20;
            
            if(counter == 0) {
                bullet1.speed.X = (float)Math.Cos(rad) * speed;	// x方向の移動量を計算
                bullet1.speed.Y = (float)Math.Sin(rad) * speed;
                bullet1.isShot = true;
            }	// y方向の移動量を計算
        }
        private void ShootingUpdate1()
        {
             timeInterval++;
            if (timeInterval % 120 == 0) {
                hasShot = true;
                bullet1.isShot = true;
            }
            if(hasShot)Shoot(stage.player);
            if (hasShot) counter++;
            if (!bullet1.isShot && hasShot && counter > 120) {// 終了処理
                hasShot = false;
                counter = 0;
                //timeInterval = 0;
            }
        }*/

        public override void UpdateAnimation()
        {
            animation.Update(2, 0, width, texture.Height, 6, 3,turret.hasShot);
        }
    }
}
