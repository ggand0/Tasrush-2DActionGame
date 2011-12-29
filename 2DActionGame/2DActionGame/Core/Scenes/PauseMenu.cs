using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
    public class PauseMenu : SelectScene
    {
        public PauseMenu(Stage privousScene)
			: base(privousScene)
		{
			sceneTitle = "Pause";
			drawBackGround = false;
			buttonNum = 5;
			button = new Button[buttonNum];
			menuString = new string[] {
				"Resume",
				"Option", 
				"BackToMenu",
				"DebugMenu",
				"Exit"
			};
			for (int i = 0; i < buttonNum; i++) {
				button[i].color = Color.Blue;
				button[i].name = menuString[i];
			}
			SoundControl.Pause();

			Load();
        }

        protected override void ButtonUpdate()
        {
			base.ButtonUpdate();
			if (KeyInput.IsOnKeyDown(Microsoft.Xna.Framework.Input.Keys.T)) {
				if (!game.inDebugMode) game.inDebugMode = true;
				else if (game.inDebugMode) game.inDebugMode = false;
			}

			if (button[0].isSelected && JoyStick.IsOnKeyDown(3)) {		// Resume
				isEndScene = true;
				(upperScene as Stage).isPausing = false;

				if (!game.isMuted) SoundControl.Resume();
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
            }

			if (button[1].isSelected && JoyStick.IsOnKeyDown(3)) {		// Option
				PushScene(new Option(this));
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
            }

			if (button[2].isSelected && JoyStick.IsOnKeyDown(3)) {		// to Menu
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
				SoundControl.IniMusic("Audio\\BGM\\menu_new");
				if (!game.isMuted) SoundControl.Play();
				BackScene(4);
				
			}

			if (button[3].isSelected && JoyStick.IsOnKeyDown(3)) {		// to DebugMenu
				PushScene(new DebugMenu(this, (upperScene as Stage)));
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}

			if (button[4].isSelected && JoyStick.IsOnKeyDown(3)) {		// Exit
				game.Exit();
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
			upperScene.Draw(spriteBatch);
			spriteBatch.Draw(mask, new Rectangle(0, 0, 640, 480), new Color(0, 0, 0, 100));
			base.Draw(spriteBatch);
            

            /*spriteBatch.DrawString(game.Arial, "Pause", new Vector2(250, 100), Color.Orange);
            spriteBatch.DrawString(game.Arial, "Start", new Vector2(200, 150), button[0].color);
			spriteBatch.DrawString(game.Arial, "Option", new Vector2(200, 200), button[1].color);// 充実したオプション
			spriteBatch.DrawString(game.Arial, "Exit", new Vector2(200, 250), button[2].color);
			spriteBatch.DrawString(game.Arial, "BackToMenu", new Vector2(200, 300), button[3].color);
			spriteBatch.DrawString(game.Arial, "DebugMenu", new Vector2(200, 350), button[4].color);*/
        }
    }
}
