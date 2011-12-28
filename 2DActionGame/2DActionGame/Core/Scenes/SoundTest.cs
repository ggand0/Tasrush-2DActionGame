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
		private SoundEffectInstance musicInstance;
		//private float volume;

		public SoundTest(Scene privousScene)
			: base(privousScene)
		{
			buttonNum = 5;
			button = new Button[buttonNum];
			musics = new SoundEffect[3];
			for (int i = 0; i < button.Length; i++) {
				button[i].color = Color.Blue;
			}
			Load();
        }

        public override void Load()
        {
			base.Load();
			button[0].texture = content.Load<Texture2D>("General\\Menu\\Option");
			musics[0] = content.Load<SoundEffect>("Audio\\BGM\\forest");
			musics[1] = content.Load<SoundEffect>("Audio\\BGM\\ice");
			musics[2] = content.Load<SoundEffect>("Audio\\BGM\\boss_nomal");
			musicInstance = musics[0].CreateInstance();
        }
		protected override void ButtonUpdate()
		{
			base.ButtonUpdate();

			if (button[0].isSelected && JoyStick.onStickDirectionChanged) {		// BGM
				if (JoyStick.stickDirection == Direction.RIGHT && SoundControl.volumeAll < 1.0f) {
					if (musicIndex < musics.Length - 1) musicIndex++;
				} else if (JoyStick.stickDirection == Direction.LEFT && SoundControl.volumeAll > 0f) {
					if (musicIndex > 0) musicIndex--;
				}
			}
			if (button[0].isSelected && JoyStick.IsOnKeyDown(3)) {
				SoundControl.Stop();
				musicInstance.Stop();
				musicInstance = musics[musicIndex].CreateInstance();
				musicInstance.Volume = 0.25f;
				musicInstance.Play();
			}
			if (button[1].isSelected && JoyStick.onStickDirectionChanged) {			// BGM volume
				if (JoyStick.stickDirection == Direction.RIGHT && musicInstance.Volume < 1.0f) {
					musicInstance.Volume += .05f;
				} else if (JoyStick.stickDirection == Direction.LEFT && musicInstance.Volume > 0f) {
					musicInstance.Volume -= .05f;
				}
			}
			if (button[2].isSelected && JoyStick.IsOnKeyDown(3)) {				// (SE)
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
			}
			if (button[4].isSelected && JoyStick.IsOnKeyDown(3)) {				// Back
				if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
				isEndScene = true;
				SoundControl.IniMusic("Audio\\BGM\\menu_new");
				SoundControl.Play();
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(backGround, new Vector2(0, 0), Color.White);
			spriteBatch.DrawString(game.Arial, "SoundTest", new Vector2(250, 0), Color.Orange);
			spriteBatch.DrawString(game.Arial, "Musics", new Vector2(200, 50), Color.Orange);
			spriteBatch.DrawString(game.Arial, "Index : " + musicIndex.ToString(), new Vector2(200, 100), button[0].color);
			spriteBatch.DrawString(game.Arial, "song name : " + musics[musicIndex].Name, new Vector2(200, 150), button[0].color);
			spriteBatch.DrawString(game.Arial, "song volume : " + musicInstance.Volume.ToString("F1"), new Vector2(200, 200), button[1].color);
			spriteBatch.DrawString(game.Arial, "Sound Effects", new Vector2(200, 300), button[2].color);
			spriteBatch.DrawString(game.Arial, "se volume : ", new Vector2(200, 350), button[3].color);
			spriteBatch.DrawString(game.Arial, "Back", new Vector2(200, 420), button[4].color);
		}
	}
}
