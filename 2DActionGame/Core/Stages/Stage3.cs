﻿using System;
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
				LoadMapData(1, "Stage3_Easy.txt", 0, 0);
			} else {
				LoadMapData(1, "Stage3_Hard.txt", 0, 0);
			}

            player = new Player(this, 100, 100, 48, 48, 10);
            characters.Add(player);
            sword = player.sword;
            weapons.Add(sword);
            staticTerrains.Add(new Block(this, 100, 200, 32, 32));
            boss = new Rival(this, 15000, 300, 60, 60, 50, 0, 1, 1); 
            characters.Add(boss);
            AddObjects();

            SetTerrainDirection();
			SetWaterDirection();

            base.Load();
            foreach (Weapon weapon in weapons) {
                if(weapon is Turret) weapon.Load(game.Content, "Object\\Turret&Bullet" + "\\" + "windTurret2");
            }

			if (!game.isHighLvl)
				SoundControl.IniMusic("Audio\\BGM\\machine");
			else
				SoundControl.IniMusic("Audio\\BGM\\boss");
			if (!game.isMuted) SoundControl.Play();
        }
		public override void Update(double dt)
        {
            base.Update(dt);

            /*if (!toBossScene && BGM.IsStopped && !inBossBattle && !game.isOvered&& !game.inMenu) {
                if (!game.isHighLvl)
                    BGM = game.soundBank.GetCue("machine");
                else
                    BGM = game.soundBank.GetCue("hard_last");
                BGM.Play(SoundControl.volumeAll, 0f, 0f);
            }
            else if (!toBossScene && BGM.IsPaused && !inBossBattle && !game.isOvered && !game.inMenu)
                BGM.Resume();// BGM.Play(SoundControl.volumeAll, 0f, 0f);*/

        }

    }
}
