using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
	public class WindTurret : Enemy
	{
		private Turret turret;

		public WindTurret(Stage stage, float x, float y, int width, int height, int HP, Character user)
			: this(stage, x, y, width, height, HP, user, null)
		{
		}
		public WindTurret(Stage stage, float x, float y, int width, int height, int HP, Character user, Turret turret)
			: base(stage, x, y, width, height, HP, user)
		{
			//LoadXML("Enemy", "Xml\\Objects_Base.xml");
			if (turret == null) turret = new Turret(stage, this, position, 32, 32, 0, 1, 1, false, true, 0, 0, 5);
			Load();

			turret.isBeingUsed = true;
		}
		protected override void Load()
		{
			base.Load();

			texture = content.Load<Texture2D>("Object\\Turret&Bullet\\windTurret2");
		}

		public override void Update()
		{
			if (IsActive() && IsBeingUsed()) {
				
			}

			base.Update();
		}

		private void MovePattern0(Vector2 defaultPosition)
		{
			speed.X = -5;
			if (position.X < defaultPosition.X - 200) isBeingUsed = false;
		}
	}
}
