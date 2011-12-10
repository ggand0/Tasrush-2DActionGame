using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
    public class SizeTest : Stage
    {
        public SizeTest(Game1 game) : base(game)
        {
            contentDirectory = "SizeTest";
            frontalScrollingBackGround = new ScrollingBackground(new Vector2(0, 0));

            player = new Player(game, this, 100, 100, 32, 48, 0);
            characters.Add(player);
            characters.Add(new StationalEnemy(game, this, 300, 100, 92, 92, 5));
            characters.Add(new StationalEnemy(game, this, 450, 100, 92, 92, 5));
            characters.Add(new StationalEnemy(game, this, 600, 100, 92, 92, 5));
            characters.Add(new StationalEnemy(game, this, 670, 100, 92, 92, 5));
            characters.Add(new JumpingEnemy(game, this, 800, 100, 64, 64, 5));
            sword = new Sword(game, this, player, 200, 100, 64, 8, 0);

            //input_second = new string[500];
            //input_last = new string[500, 5];
            LoadMapData(1, "Flat_Map3840.txt", 0, 0);
        }
    }
}
