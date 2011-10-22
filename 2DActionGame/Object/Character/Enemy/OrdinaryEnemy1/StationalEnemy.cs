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
    /// Stage1 動かない敵。Enemyでもいいかも
    /// </summary>
    public class StationalEnemy : Enemy
    {
        public StationalEnemy(Stage stage,  float x, float y, int width, int height, int HP)
            :base (stage, x, y, width, height, HP)
        {
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\StationalEnemy1");
		}

        public override void UpdateAnimation()
        {
            animation.Update(2, 0, width, height, 6, 1);
        }
    }
}
