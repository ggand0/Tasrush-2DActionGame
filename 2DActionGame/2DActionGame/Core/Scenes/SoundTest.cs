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
		private int musicIndex;
		private SoundEffect[] musics;
		private float musicVolume;
		private string[] musicString = {
			"menu_new", "forest", "forest_old", "cave", "ice", "machine", "boss", "boss_normal", "hard_last", "gameover", "clear", "ending_medley_newnew"
		};

		public SoundTest(Scene privousScene)
			: base(privousScene)
		{
			sceneTitle = "SoundTest";
			buttonNum = 5;
			button = new Button[buttonNum];
			musics = new SoundEffect[musicString.Length];

			menuString = new string[] { 
				"Index : "  + musicIndex.ToString() + "\r\nsong name : ",
				"song volume : ",
				"Sound Effects",
				"se volume : ",
				"Back" 
			};
			for (int i = 0; i < button.Length; i++) {
				button[i].color = Color.Blue;
				button[i].name = menuString[i];
			}
			
			Load();
        }

        public override void Load()
        {
			base.Load();
			button[0].texture = content.Load<Texture2D>("General\\Menu\\Option");
			/*musics[0] = content.Load<SoundEffect>("Audio\\BGM\\forest");
			musics[1] = content.Load<SoundEffect>("Audio\\BGM\\ice");
			musics[2] = content.Load<SoundEffect>("Audio\\BGM\\boss_normal");*/
			for (int i = 0; i < musics.Length; i++) {
				musics[i] = content.Load<SoundEffect>("Audio\\BGM\\" + musicString[i]);
				musics[i].Name = musicString[i];
			}
			musicInstance = musics[0].CreateInstance();
			
        }
		protected override void UpdateTexts()
		{
			base.UpdateTexts();
			button[0].name =  "Index : "  + musicIndex.ToString() + "\r\nsong name : " + musics[musicIndex].Name;
			button[1].name = "song volume : " + musicVolume.ToString("F2");
			musicInstance.Volume = musicVolume;
		}
		protected override void ButtonUpdate()
		{
			base.ButtonUpdate();

			if (button[0].isSelected && JoyStick.onStickDirectionChanged) {		// BGM
				if (JoyStick.stickDirection == Direction.RIGHT && SoundControl.volumeAll <= .95f) {
					if (musicIndex < musics.Length - 1) musicIndex++;
				} else if (JoyStick.stickDirection == Direction.LEFT && SoundControl.volumeAll >= 0.05f) {
					if (musicIndex > 0) musicIndex--;
				}
			}
			if (button[0].isSelected && JoyStick.IsOnKeyDown(3)) {
				SoundControl.Stop();
				musicInstance.Stop();
				musicInstance = musics[musicIndex].CreateInstance();
				musicInstance.Volume = musicVolume;
				musicInstance.Play();
			}
			if (button[1].isSelected && JoyStick.onStickDirectionChanged) {			// BGM volume
				if (JoyStick.stickDirection == Direction.RIGHT && musicVolume <= 0.95f) {
					musicVolume += .05f;
				} else if (JoyStick.stickDirection == Direction.LEFT && musicVolume >= 0.05f) {
					musicVolume -= .05f;
				}
			}
			if (button[2].isSelected && JoyStick.IsOnKeyDown(3)) {				// (SE)
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[4].isSelected && JoyStick.IsOnKeyDown(3)) {				// Back
				SoundControl.Stop();
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
				isEndScene = true;
				SoundControl.IniMusic("Audio\\BGM\\menu_new");
				SoundControl.Play();
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			/*spriteBatch.DrawString(game.Arial, "Musics", new Vector2(200, 50), Color.Orange);
			spriteBatch.DrawString(game.Arial, "Index : " + musicIndex.ToString(), new Vector2(200, 100), button[0].color);
			spriteBatch.DrawString(game.Arial, "song name : " + musics[musicIndex].Name, new Vector2(200, 150), button[0].color);
			spriteBatch.DrawString(game.Arial, "song volume : " + musicInstance.Volume.ToString("F1"), new Vector2(200, 200), button[1].color);
			spriteBatch.DrawString(game.Arial, "Sound Effects", new Vector2(200, 300), button[2].color);
			spriteBatch.DrawString(game.Arial, "se volume : ", new Vector2(200, 350), button[3].color);
			spriteBatch.DrawString(game.Arial, "Back", new Vector2(200, 420), button[4].color);*/
		}
	}
}
