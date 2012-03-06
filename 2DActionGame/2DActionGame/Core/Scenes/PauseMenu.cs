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
			
			menuString = new string[] {
				"Resume",
				//"Option", 
				"BackToMenu",
				"DebugMenu",
				//"Exit"
			};
            buttonNum = menuString.Length;
            button = new Button[buttonNum];
			for (int i = 0; i < buttonNum; i++) {
				button[i].color = Color.Blue;
				button[i].name = menuString[i];
			}
			SoundControl.Pause();

			Load();
        }

        protected override void ButtonUpdate()
        {
			if (JoyStick.IsOnKeyDown(3) || JoyStick.IsOnKeyDown(8)) {
				isEndScene = true;
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
			}

			if (KeyInput.IsOnKeyDown(Microsoft.Xna.Framework.Input.Keys.T)) {
				if (!game.inDebugMode) game.inDebugMode = true;
				else if (game.inDebugMode) game.inDebugMode = false;
			}

			if (button[0].isSelected && JoyStick.IsOnKeyDown(1)) {		// Resume
				isEndScene = true;
				(upperScene as Stage).isPausing = false;

				if (!game.isMuted) SoundControl.Resume();
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
            }

			/*if (button[1].isSelected && JoyStick.IsOnKeyDown(1)) {		// Option
				PushScene(new Option(this));
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
            }*/

			if (button[1].isSelected && JoyStick.IsOnKeyDown(1)) {		// to Menu
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
				SoundControl.IniMusic("Audio\\BGM\\menu_new", true);
				if (!game.isMuted) SoundControl.Play();
				BackScene(4);
				
			}

			if (button[2].isSelected && JoyStick.IsOnKeyDown(1)) {		// to DebugMenu
				PushScene(new DebugMenu(this, (upperScene as Stage)));
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}

			/*if (button[3].isSelected && JoyStick.IsOnKeyDown(1)) {		// Exit
				game.Exit();
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}*/

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
			upperScene.Draw(spriteBatch);
			spriteBatch.Draw(mask, new Rectangle(0, 0, 640, 480), new Color(0, 0, 0, 100));
			base.Draw(spriteBatch);
        }
    }
}
