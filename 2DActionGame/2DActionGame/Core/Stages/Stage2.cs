using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _2DActionGame
{
    public class Stage2 : Stage
    {
        public Stage2(Scene privousScene, bool isHighLvl)
			: base(privousScene, isHighLvl)
        {
            contentDirectory = "Stage02";
        }

        public override void Load()
        {
			if (!isHighLvl) {
				LoadMapData(1, "Stage2_Easy.txt", 0, 0);
				SoundControl.IniMusic("Audio\\BGM\\cave");
			} else {
				LoadMapData(1, "Stage2_Hard.txt", 0, 0);
				SoundControl.IniMusic("Audio\\BGM\\ice");
			} SoundControl.musicInstance.IsLooped = true;
			
            player = new Player(this, 100, 100, 48, 48, 10);
            characters.Add(player);
            sword = player.sword;
            weapons.Add(sword);
            boss = new Fuujin(this, 14500, 100, 380, 310, 50, 0, 0, 1);//210, 210
            characters.Add(boss);

            SetTerrainDirection();
			SetWaterDirection();
            AddObjects();
            base.Load();

			if (!game.isMuted) SoundControl.Play(true);
        }
    }    
}
