
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
    /// 動かずに弾を撃つ敵
    /// </summary>
    public class ShootingEnemy : Enemy
    {
        private static readonly Vector2 shootPosition=  new Vector2(10);
		private readonly float shootDistance;
        private Turret turret;

        public ShootingEnemy(Stage stage,  float x, float y, int width, int height, int HP)
            : base(stage, x, y, width, height, HP)
        {
			LoadXML("ShootingEnemy", "Xml\\Objects_Enemy_Stage1.xml");
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

			if (IsActive()) {
				turret.isBeingUsed = true;

				if (isWinced) turret.isBeingUsed = false;
				else turret.isBeingUsed = true;

				distance = Vector2.Distance(stage.player.position, position);
				if (distance < shootDistance) turret.isBeingUsed = false;
				distance = stage.player.position.X - position.X;
				if (distance > shootDistance * 2) turret.isBeingUsed = false;
			} else {
				turret.isBeingUsed = false;// 毎回falseにするのは無駄だ
			}

			base.Update();
        }

        public override void UpdateAnimation()
        {
            animation.Update(2, 0, width, texture.Height, 6, 3,turret.hasShot);
        }
    }
}
