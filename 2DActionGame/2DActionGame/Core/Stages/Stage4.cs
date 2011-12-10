using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace _2DActionGame
{
    public class Stage4 : Stage
    {
         public Stage4(Game1 game)
            : base(game)
        {
            this.game = game;
            //scrollingBackGround = new ScrollingBackground();
            contentDirectory = "Stage04";
            reverse = new Reverse(this);
            player = new Player(game, this, 200, 100, 32, 32, 0.0f);
            sword = new Sword(game, this, 200, 100, 64, 8, 0);
            stationalEnemy = new StationalEnemy(game, this, 300, 416, 32, 32, 3);

            //input_second = new string[500];
            //input_last = new string[500, 5];
            staticTerrains = new List<Terrain>();
            objects1 = new List<Object>();
            objects1.Add(player);

            staticTerrains.Clear();
            LoadMapData(1, "Flat_Map.txt", 0,0);
            //AddMapData(input_last);
        }
    }
}
