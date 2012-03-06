using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
    public class Stage1 : Stage
    {
		public Stage1(Scene privousScene, bool isHighLvl)
			: base(privousScene, isHighLvl)
        {
            contentDirectory = "Stage01";
        }

        public override void Load()
        {
			if (!isHighLvl) {
                LoadMapData(1, "Stages\\Stage1_Easy.txt", 0, 0);
				boss = new Raijin(this, 14000, 100, 300, 280, 50);// (210, 210) (300, 280)
				characters.Add(boss);
			} else {
                LoadMapData(1, "Stages\\Stage1_Hard.txt", 0, 0);
				boss = new Raijin(this, 14300, 100, 300, 280, 50);
				characters.Add(boss);
			}
            SetTerrainDirection();
			SetWaterDirection();

            player = new Player(this, 100, 100, 48, 48, 10);
            characters.Add(player);
            sword = player.sword;
            weapons.Add(sword);
			AddObjects();
            
            base.Load();
			foreach (Weapon weapon in weapons) {
				if (weapon is Turret) weapon.Load(game.Content, "Object\\Turret&Bullet" + "\\" + "windTurret2");
			}

			SoundControl.Stop();
            SoundControl.IniMusic("Audio\\BGM\\forest", true);
			SoundControl.musicInstance.IsLooped = true;
            if (!game.isMuted) SoundControl.Play();
        }
    }
}
