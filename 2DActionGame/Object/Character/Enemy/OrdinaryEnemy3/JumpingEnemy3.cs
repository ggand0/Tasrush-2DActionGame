using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
    /// <summary>
    /// 倒すと分裂するJumpingEnemy
    /// </summary>
    public class JumpingEnemy3 : JumpingEnemy
    {
        private bool isDivided;
		/// <summary>
		/// 子Object。リストにしてもいいかも
		/// </summary>
        private JumpingEnemy J1, J2;

        public JumpingEnemy3(Stage stage, float x, float y, int width, int height, int HP)
            : base(stage, x, y, width, height, HP)
        {
			//LoadXML("StationalEnemy3", "Xml\\Objects_Enemy_Stage3.xml");
        }

        public override void Update()
        {
            bool isDividing = false;

            if (!isAlive) isDividing = true;

			if (isDividing && !isDivided) {
				J1 = new JumpingEnemy(stage, defPos.X, defPos.Y, 20, 20, 3);
				J2 = new JumpingEnemy(stage, defPos.X + 20, defPos.Y, 20, 20, 3);
				J1.Load(game.Content, "Object\\Character\\JumpingEnemy3d");
				J2.Load(game.Content, "Object\\Character\\JumpingEnemy3d");
				stage.unitToAdd.Add(J1);
				stage.unitToAdd.Add(J2);
				J1.isDerived = true;
				J2.isDerived = true;
				isDivided = true;
			}

            base.Update();

            /*if (HP <= 0 && (J1.isAlive || J2.isAlive)) {
                isDividing = true;
                isDivided = true;
                isAlive = true;     // characters変更するとforeach使えなくなるから諦める
                isActive = false;
                HP = 1;
            }*/
        }
    }
}
