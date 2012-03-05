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
			
			
			menuString = new string[] { 
				"KeyConfig",
				"Full Screen / Window",
				"BGM volume : " + SoundControl.volumeAll.ToString("F1"),
				"Mute all sound " + (game.isMuted ? "On" : "Off"),
				"SoundTest",
				"ViewRanking",
				"StageSelect",
                "Credit",
				"Back" 
			};
            buttonNum = menuString.Length;
            button = new Button[buttonNum];

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
            SoundControl.Stop();
            SoundControl.IniMusic("Audio//BGM//menu-b");
            SoundControl.Play(true);
        }
		protected override void UpdateTexts()
		{
			base.UpdateTexts();
			button[2].name = "BGM volume : " + SoundControl.volumeAll.ToString("F2");
			button[3].name = "Mute all sound " + (game.isMuted ? "On" : "Off");
		}
        protected override void ButtonUpdate()
        {
            if (button[0].isSelected && JoyStick.IsOnKeyDown(1)) {						// KeyConfig
				game.PushScene(new KeyConfig(this));
                if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
            }
			if (button[1].isSelected && JoyStick.IsOnKeyDown(1)) {						// FullScreen / Window
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
			if (button[3].isSelected && JoyStick.IsOnKeyDown(1)) {						// Mute
				if (game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);			// ここだけはわかりやすさのため他のSEの条件の逆にする
				if (!game.isMuted) {
					game.isMuted = true;
					SoundControl.Stop();
				} else {
					game.isMuted = false;
					SoundControl.Play(true);
				}
			}
			if (button[4].isSelected && JoyStick.IsOnKeyDown(1)) {													// Sound Test
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
				PushScene(new SoundTest(this));
			}


			if (button[5].isSelected && JoyStick.IsOnKeyDown(1)) {						// Ranking
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
				PushScene(new Ranking(this));
			}
            if (button[6].isSelected && JoyStick.IsOnKeyDown(1)) {						// StageSelect
                if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
                PushScene(new StageSelect(this));
            }
            if (button[7].isSelected && JoyStick.IsOnKeyDown(1)) {						// Credit
                if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
                PushScene(new Ending(this, false));
            }
            if (button[8].isSelected && JoyStick.IsOnKeyDown(1) || JoyStick.IsOnKeyDown(3)) {						// Back To Menu
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
				isEndScene = true;

                if (upperScene is MainMenu) {
                    SoundControl.Stop();
                    SoundControl.IniMusic("Audio\\BGM\\menu_new");
                    SoundControl.Play(true);
                }
			}
        }

    }
}
