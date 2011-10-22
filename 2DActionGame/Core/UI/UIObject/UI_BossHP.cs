using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
    public class UI_BossHP : UIObject
    {
        public bool visible { get; set; }

        public UI_BossHP(Stage stage, Vector2 position)
            : base(stage, position)
        {
            this.stage = stage;
            this.position = position;
            this.visible = true;
        }

        public void Update()
        {
            if (stage != null) {
                if (stage.boss is Raijin)
                    position.X = stage.boss.drawPos.X + 100;
                else if (stage.boss is Fuujin)
                    position.X = stage.boss.drawPos.X + 100;
                else if (stage.boss is Rival)
                    position.X = stage.boss.drawPos.X - 50;
                if (stage.boss is Raijin)
                    position.Y = stage.boss.drawPos.Y;
                else if (stage.boss is Fuujin)
                    position.Y = stage.boss.drawPos.Y;
                else if (stage.boss is Rival)
                    position.Y = stage.boss.drawPos.Y - 16;

                data = stage.boss.HP;
            }


            //foreach (Boss boss in stage.characters)
            //{
            //    this.position.X = boss.drawPos.X;
            //    this.position.Y = boss.drawPos.Y;
            //    this.data = boss.HP;
            //}

            //this.position.X = stage.characters.First<Boss>.drawPos.X;
            //this.position.Y = stage.characters.First<Boss>.drawPos.Y - 8;
            //this.data = (stage.characters.OfType<Boss>).;
        }

        public override void Draw(SpriteBatch sprite)
        {
            if (stage.inBossBattle)
                sprite.Draw(texture, position, new Rectangle((int)position.X, (int)position.Y, (4 * data), 8), new Color(255, 0, 255, 127));
        }

    }
}
