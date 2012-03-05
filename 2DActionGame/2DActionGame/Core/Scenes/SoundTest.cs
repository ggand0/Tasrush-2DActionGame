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
	public class SoundTest : SelectScene
	{
		private int musicIndex, seIndex;
		private SoundEffect[] musics, soundEffects;
		private float musicVolume = .5f;
		private float seVolume = .5f;
		private SoundEffectInstance seInstance;

		private string[] musicString = {
			"menu_new", "forest", "forest_old", "cave", "ice", "machine", "boss", "boss_normal", "hard_last", "gameover", "clear", "ending_medley_newnew"
		};
		private string[] seString = {
			"bird", "blast", "blast_small", "cancel", "choose", "cloud", "critical", "damage", "damage2", "damaged", "explosion_small", "foot", "hit_big", "hit_small",
			"ice_SE", "item", "jump", "jump2", "katana", "last_explosion", "lift", "magic", "siren", "start", "syakiiin", "taiko", "taiko2", "TAS", "thunder_big", "thunder_small",
			"title", "tornado", "water", "wind", "zako_tama", "zangeki", "zangeki2", "zangeki3", "zimen"
		};

		public SoundTest(Scene privousScene)
			: base(privousScene)
		{
			sceneTitle = "SoundTest";
			buttonNum = 5;
			button = new Button[buttonNum];
			musics = new SoundEffect[musicString.Length];
			soundEffects = new SoundEffect[seString.Length];

			Load();

            for (int i = 0; i < button.Length; i++) {
                button[i].color = Color.Blue;
                button[i].name = menuString[i];
            }
        }

        public override void Load()
        {
			base.Load();

			button[0].texture = content.Load<Texture2D>("General\\Menu\\Option");
			for (int i = 0; i < musics.Length; i++) {
				musics[i] = content.Load<SoundEffect>("Audio\\BGM\\" + musicString[i]);
				musics[i].Name = musicString[i];
			}
			for (int i = 0; i < soundEffects.Length; i++) {
				soundEffects[i] = content.Load<SoundEffect>("Audio\\SE\\" + seString[i]);
				soundEffects[i].Name = seString[i];
			}
			musicInstance = musics[0].CreateInstance();
			seInstance = soundEffects[0].CreateInstance();

            menuString = new string[] {
				/*"Song index : "  + musicIndex.ToString() + "\r\n*/"song name : " + musics[musicIndex].Name,
				"song volume : ",
				/*"SoundEffect index : " + seIndex.ToString() + "\r\n*/"se name : " + soundEffects[seIndex].Name,
				"se volume : ",
				"Back"
			};
        }
		protected override void UpdateTexts()
		{
			base.UpdateTexts();
			button[0].name = /*"Song index : " + musicIndex.ToString() + "\r\n*/"song name : " + musics[musicIndex].Name;
			button[1].name = "song volume : " + musicVolume.ToString("F2");
			button[2].name = /*"SoundEffect index : " + seIndex.ToString() + "\r\n*/"se name : " + soundEffects[seIndex].Name;
			button[3].name = "se volume : " + seVolume.ToString("F2");
			musicInstance.Volume = musicVolume;
			seInstance.Volume = seVolume;
		}
		protected override void ButtonUpdate()
		{
			base.ButtonUpdate();

			// BGM
			if (button[0].isSelected && JoyStick.onStickDirectionChanged) {		// BGM
				if (JoyStick.stickDirection == Direction.RIGHT) {
					if (musicIndex < musics.Length - 1) musicIndex++;
				} else if (JoyStick.stickDirection == Direction.LEFT) {
					if (musicIndex > 0) musicIndex--;
				}
			}
			if (button[0].isSelected && JoyStick.IsOnKeyDown(1)) {
				SoundControl.Stop();
				musicInstance.Stop();
				musicInstance = musics[musicIndex].CreateInstance();
				musicInstance.Volume = musicVolume;
				musicInstance.Play();
			}
			if (button[1].isSelected && JoyStick.onStickDirectionChanged) {		// BGM volume
				if (JoyStick.stickDirection == Direction.RIGHT && musicVolume <= 0.95f) {
					musicVolume += .05f;
				} else if (JoyStick.stickDirection == Direction.LEFT && musicVolume >= 0.05f) {
					musicVolume -= .05f;
				}
			}

			// SE
			if (button[2].isSelected && JoyStick.onStickDirectionChanged) {		// SE
				if (JoyStick.stickDirection == Direction.RIGHT) {
					if (seIndex < soundEffects.Length - 1) seIndex++;
				} else if (JoyStick.stickDirection == Direction.LEFT) {
					if (seIndex > 0) seIndex--;
				}
			}
			if (button[2].isSelected && JoyStick.IsOnKeyDown(1)) {
				SoundControl.Stop();
				seInstance.Stop();
				seInstance = soundEffects[seIndex].CreateInstance();
				seInstance.Volume = seVolume;
				seInstance.Play();
			}
			if (button[3].isSelected && JoyStick.onStickDirectionChanged) {		// SE volume
				if (JoyStick.stickDirection == Direction.RIGHT && seVolume <= 0.95f) {
					seVolume += .05f;
				} else if (JoyStick.stickDirection == Direction.LEFT && seVolume >= 0.05f) {
					seVolume -= .05f;
				}
			}

			if (button[4].isSelected && JoyStick.IsOnKeyDown(1)) {				// Back
				musicInstance.Stop();
				seInstance.Stop();
				SoundControl.Stop();
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
				isEndScene = true;
				SoundControl.IniMusic("Audio\\BGM\\menu_new");
				SoundControl.Play(true);
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (drawBackGround) spriteBatch.Draw(backGround, Vector2.Zero, Color.White);

			Vector2 origin = game.titleFont.MeasureString(sceneTitle) / 2;
			spriteBatch.DrawString(game.titleFont, sceneTitle, TITLE_POSITION + new Vector2(0, origin.Y * 1), Color.DarkOrange, 0, origin, 1, SpriteEffects.None, 0);
			DrawTexts(spriteBatch, 1);
		}
	}
}