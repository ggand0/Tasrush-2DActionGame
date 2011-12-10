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
				LoadMapData(1, "Stage1_Easy.txt", 0, 0);
				boss = new Raijin(this, 14000, 100, 210, 210, 50);
				characters.Add(boss);
			} else {
				LoadMapData(1, "Stage1_Hard.txt", 0, 0);
				boss = new Raijin(this, 14300, 100, 210, 210, 50);
				characters.Add(boss);
			}
            SetTerrainDirection();
			SetWaterDirection();

            player = new Player(this, 100, 100, 48, 48, 10);
            characters.Add(player);
            sword = player.sword;
            weapons.Add(sword);
			//this.dynamicTerrains.Add(new SnowBall(this, 100, 100, 128, 128));
			AddObjects();
            
            base.Load();
			foreach (Weapon weapon in weapons) {
				if (weapon is Turret) weapon.Load(game.Content, "Object\\Turret&Bullet" + "\\" + "windTurret2");
			}

            //BGM = game.soundBank.GetCue("forest");
            //if (!game.isMuted) BGM.Play(SoundControl.volumeAll, 0f, 0f);
			SoundControl.Stop();
			SoundControl.IniMusic("Audio\\BGM\\forest");
			SoundControl.musicInstance.IsLooped = true;
            if (!game.isMuted) SoundControl.Play();
        }

		public override void Update(double dt)
        {
            base.Update(dt);

            //if (!pauseMenu.Pausing && game.isInStage && (BGM.IsStopped || BGM.IsPaused) && !inBossBattle) {
			/*if (!toBossScene && BGM.IsStopped && !inBossBattle && !game.isOvered && !game.inMenu) {// game.inStageならでいいじゃん !pauseMenu.Pausing &&
                if (!game.isHighLvl)
                    BGM = game.soundBank.GetCue("forest");
                else
                    BGM = game.soundBank.GetCue("forest");
                BGM.Play(SoundControl.volumeAll, 0f, 0f);
            }
			else if (!toBossScene && BGM.IsPaused && !inBossBattle && !game.isOvered && !game.inMenu)//!pauseMenu.Pausing && 
                BGM.Resume();// BGM.Play(SoundControl.volumeAll, 0f, 0f);*/
        }
    }
}
