using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
    public class UI_EnemyHP : UIObject
    {
        public bool visible { get; set; }

        public UI_EnemyHP(Stage stage, Vector2 position, int enemyHP)
            : base(stage, position)
        {
            this.stage = stage;
            this.position = position;
            this.data = enemyHP;
            this.visible = false;
        }

        public void Update(int enemyNumber)
        {
            this.position.X = stage.userInterface.enemyList[enemyNumber - 1].drawPos.X;
            this.position.Y = stage.userInterface.enemyList[enemyNumber - 1].drawPos.Y - 8;
            this.data = stage.userInterface.enemyList[enemyNumber - 1].HP;
            if (stage.defHP > data) {
                visible = true;
            }

        }
        override public void Draw(SpriteBatch sprite)
        {
            if (visible) {
                sprite.Draw(texture, position, new Rectangle((int)position.X, (int)position.Y, (32 * data), 8), new Color(255, 0, 0, 128));
            }
        }

    }
}
