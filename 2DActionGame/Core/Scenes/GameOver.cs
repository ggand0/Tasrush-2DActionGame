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
    public class GameOver : SelectScene
    {
        public GameOver(Scene privousScene)
			: base(privousScene)
        {
			buttonNum = 2;
			button = new Button[buttonNum];
			for (int i = 0; i < buttonNum; i++)
				button[i].color = Color.Blue;

			Load();
        }
        public override void Load()
        {
			base.Load();

			for (int i = 0; i < buttonNum; i++)
				button[i].texture = content.Load<Texture2D>("General\\Menu\\GameOver" + i);

			SoundControl.IniMusic("Audio\\BGM\\gameover");
			if (!game.isMuted) SoundControl.Play();
        }

        protected override void ButtonUpdate()
        {
            if (button[0].isSelected && (Controller.IsOnKeyDown(3) || Controller.IsOnKeyDown(8))) {		// Continue
                game.ReloadStage(game.isHighLvl);
				isEndScene = true;
                
                if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
            }
            if (button[1].isSelected && (Controller.IsOnKeyDown(3) || Controller.IsOnKeyDown(8))) {		// Back to Menu
                if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
				SoundControl.Stop();
				SoundControl.IniMusic("Audio\\BGM\\menu_new");
				BackScene(5);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(backGround, Vector2.Zero, Color.White);

			for (int i = 0; i < buttonNum; i++)
				if (button[i].isSelected) spriteBatch.Draw(button[i].texture, Vector2.Zero, Color.White);
        }
    }
}
