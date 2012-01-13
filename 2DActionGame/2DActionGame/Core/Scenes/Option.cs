using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
    /// <summary>
    /// 充実したオプション()
    /// </summary>
    public class Option : SelectScene
    {
		public Option(Scene privousScene)
			: base(privousScene)
		{
			sceneTitle = "Option";
			buttonNum = 8;
			button = new Button[buttonNum];
			menuString = new string[] { 
				"KeyConfig",
				"Full Screen / Window",
				"BGM volume : " + SoundControl.volumeAll.ToString("F1"),
				"Mute all sound " + (game.isMuted ? "On" : "Off"),
				"SoundTest",
				"ViewRanking",
				"StageSelect",
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
        }
		protected override void UpdateTexts()
		{
			base.UpdateTexts();
			button[2].name = "BGM volume : " + SoundControl.volumeAll.ToString("F2");
			button[3].name = "Mute all sound " + (game.isMuted ? "On" : "Off");
		}
        protected override void ButtonUpdate()
        {
			base.ButtonUpdate();

            if (button[0].isSelected && JoyStick.IsOnKeyDown(3)) {						// KeyConfig
				game.PushScene(new KeyConfig(this));
                if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
            }
			if (button[1].isSelected && JoyStick.IsOnKeyDown(3)) {						// FullScreen / Window
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
				game.graphics.ToggleFullScreen();
            }
			if (button[2].isSelected && JoyStick.onStickDirectionChanged/*JoyStick.KEY(3) && counter % 5 == 0*/) {			// Volume Control
				if (JoyStick.stickDirection == Direction.RIGHT && SoundControl.volumeAll <= .95f) {
					game.wholeVolume = SoundControl.volumeAll += .05f;
					SoundControl.musicInstance.Volume += .05f;
				} else if (JoyStick.stickDirection == Direction.LEFT && SoundControl.volumeAll >= 0.05f) {
					game.wholeVolume = SoundControl.volumeAll -= .05f;
					SoundControl.musicInstance.Volume -= .05f;
				}
            }
			if (button[3].isSelected && JoyStick.IsOnKeyDown(3)) {						// Mute
				if (game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);			// ここだけはわかりやすさのため他のSEの条件の逆にする
				if (!game.isMuted) {
					game.isMuted = true;
					SoundControl.Stop();
				} else {
					game.isMuted = false;
					SoundControl.Play();
				}
			}
			if (button[4].isSelected && JoyStick.IsOnKeyDown(3)) {													// Sound Test
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
				PushScene(new SoundTest(this));
			}


			if (button[5].isSelected && JoyStick.IsOnKeyDown(3)) {						// Ranking
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
				PushScene(new Ranking(this));
			}
            if (button[6].isSelected && JoyStick.IsOnKeyDown(3)) {						// StageSelect
                if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
                PushScene(new StageSelect(this));
            }
			if (button[7].isSelected && JoyStick.IsOnKeyDown(3)) {						// Back To Menu
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
				isEndScene = true;
			}
        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
			base.Draw(spriteBatch);
			spriteBatch.DrawString(game.japaneseFont, "てすと", new Vector2(200, 50), button[0].color);
            /*spriteBatch.DrawString(game.Arial, "KeyConfig", new Vector2(200, 50), button[0].color);
			spriteBatch.DrawString(game.Arial, "Full Screen / Window", new Vector2(200, 100), button[1].color);
			spriteBatch.DrawString(game.Arial, "BGM volume : " + SoundControl.volumeAll.ToString("F1"), new Vector2(200, 150), button[2].color);
			spriteBatch.DrawString(game.Arial, "Mute all sound : " + (game.isMuted ? "On" : "Off"), new Vector2(200, 200), button[3].color);
			spriteBatch.DrawString(game.Arial, "SoundTest", new Vector2(200, 250), button[4].color);
			spriteBatch.DrawString(game.Arial, "ViewRanking", new Vector2(200, 300), button[5].color);
            spriteBatch.DrawString(game.Arial, "StageSelect", new Vector2(200, 350), button[6].color);
			spriteBatch.DrawString(game.Arial, "Back", new Vector2(200, 420), button[7].color);*/
        }
    }
}
