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
			buttonNum = 4;
			button = new Button[buttonNum];

			for (int i = 0; i < button.Length; i++) {
				button[i].color = Color.Blue;
			}

			Load();
        }

        public override void Load() 
        {
			base.Load();
			button[0].texture = content.Load<Texture2D>("General\\Menu\\Option");
        }
        protected override void ButtonUpdate()
        {
			base.ButtonUpdate();

            if (button[0].isSelected && Controller.IsOnKeyDown(3)) {						// KeyConfig
                if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
            }
			if (button[1].isSelected && Controller.IsOnKeyDown(3)) {						// FullScreen / Window
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
				game.graphics.ToggleFullScreen();
            }
			if (button[2].isSelected && Controller.KEY(3) && counter % 10 == 0) {			// Volume Control
				game.wholeVolume = SoundControl.volumeAll += .05f;
				if (SoundControl.volumeAll > 1.0f) 
					game.wholeVolume = SoundControl.volumeAll = 0;
            }
			if (button[3].isSelected && Controller.IsOnKeyDown(3)) {						// Back To Menu
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
				isEndScene = true;
			}

        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(backGround, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(game.Arial, "Option", new Vector2(250, 100), Color.Orange);
            spriteBatch.DrawString(game.Arial, "KeyConfig(making)", new Vector2(200, 150), button[0].color);
			spriteBatch.DrawString(game.Arial, "Full Screen / Window", new Vector2(200, 200), button[1].color);
			spriteBatch.DrawString(game.Arial, "BGMVolume", new Vector2(200, 250), button[2].color);
			spriteBatch.DrawString(game.Arial, "volume : " + SoundControl.volumeAll.ToString(), new Vector2(200, 300), button[2].color);
			spriteBatch.DrawString(game.Arial, "Back", new Vector2(200, 400), button[3].color);
        }
    }
}
