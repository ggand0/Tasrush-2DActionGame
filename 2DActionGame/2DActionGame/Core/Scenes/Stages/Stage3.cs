using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace _2DActionGame
{
    public class Stage3 : Stage
    {
		public Stage3(Scene privousScene, bool isHighLvl)
			: base(privousScene, isHighLvl)
         {
            contentDirectory = "Stage03";
         }

        public override void Load()
        {
			if (!isHighLvl) {
                LoadMapData(1, "Stages\\Stage3_Easy.txt", 0, 0);
			} else {
                LoadMapData(1, "Stages\\Stage3_Hard.txt", 0, 0);
			}

            player = new Player(this, 100, 100, 48, 48, 10);
            characters.Add(player);
            sword = player.sword;
            weapons.Add(sword);
            staticTerrains.Add(new Block(this, 100, 200, 32, 32));
            boss = new Rival(this, 15000, 300, 60, 60, game.isHighLvl ? 50 : 40, 0, 1, 1); 
            characters.Add(boss);
            AddObjects();

            SetTerrainDirection();
			SetWaterDirection();

            base.Load();
            foreach (Weapon weapon in weapons) {
                if(weapon is Turret) weapon.Load(game.Content, "Object\\Turret&Bullet" + "\\" + "windTurret2");
            }

			if (!game.isHighLvl) {
				SoundControl.IniMusic("Audio\\BGM\\machine", true);
			} else {
                SoundControl.IniMusic("Audio\\BGM\\boss", true);
			} SoundControl.musicInstance.IsLooped = true;
			if (!game.isMuted) SoundControl.Play();
        }
    }
}
