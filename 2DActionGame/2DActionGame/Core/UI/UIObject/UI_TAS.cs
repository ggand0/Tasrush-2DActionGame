using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
    public class UI_TAS : UIObject
    {

        public UI_TAS(Stage stage, Vector2 position)
            : base(stage, position)
        {
            this.stage = stage;
            rectangle = new Rectangle(0, 0, 150, 16);
            this.position = position;
        }

        public void Update()
        {
            if (data != stage.player.TASpower) {
                data = stage.player.TASpower;
                position.X = ((data / (float)stage.player.defMAXTAS) * 150) - 150;
            }

        }


    }
}
