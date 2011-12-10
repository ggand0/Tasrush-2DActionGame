using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
    public class UI_HP : UIObject
    {
        Color GaugeColor { get; set; }
        public UI_HP(Stage stage, Vector2 position)
            : base(stage, position)
        {
            this.stage = stage;
            rectangle = new Rectangle(0, 0, 150, 16);
            this.position = position;
            this.data = 10;

        }

        public void Update()
        {
            if (data != stage.player.HP) {
                data = stage.player.HP;
                position.X = 0 - ((10 - data) * 15);
            }
            //if (data < 4) GaugeColor = Color.Red;
            //else GaugeColor = Color.White;
        }

        override public void Draw(SpriteBatch sprite)
        {
            if (data < 4)
                sprite.Draw(texture, position, new Color(30, 150, 150, 255));
            else
                sprite.Draw(texture, position, Color.White);
        }




    }
}
