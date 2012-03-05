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
        public int counter;

        public UI_BossHP(Stage stage, Vector2 position)
            : base(stage, position)
        {
            this.stage = stage;
            this.position = position;
            this.visible = true;
            this.counter = 0;
            this.data = 0;
        }

        public void Update()
        {
            if (stage.toBossScene && counter % 20 == 0) {
                if (data < stage.boss.HP) {
                    data += (int)(stage.boss.HP / 10.0f);
                    if (data >= stage.boss.HP) data = stage.boss.HP;
                } else {
                    data = stage.boss.HP;
                }
            }
            counter++;
            if (stage != null && stage.inBossBattle) {
                data = stage.boss.HP;
            }
            position.X = 600;
            position.Y = 400 - data * 6;
            


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
            if (stage.inBossBattle || stage.toBossScene)
                sprite.Draw(texture, position, new Rectangle((int)position.X, (int)position.Y, 20, (6*data)), new Color(255, 30, 30, 200));
        }

    }
}
