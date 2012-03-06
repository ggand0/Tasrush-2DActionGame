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
        private bool nowLoading, hasDisplayed;

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

			SoundControl.IniMusic("Audio\\BGM\\gameover", false);
			if (!game.isMuted) SoundControl.Play();
        }

        protected override void ButtonUpdate()
        {
            if (button[0].isSelected && (JoyStick.IsOnKeyDown(1) || JoyStick.IsOnKeyDown(8))) {		// Continue
				SoundControl.Stop();
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);

                
                //SoundControl.Pause();//SoundControl.Stop();
				//isEndScene = true;
                nowLoading = true;
            }
            if (button[1].isSelected && (JoyStick.IsOnKeyDown(1) || JoyStick.IsOnKeyDown(8))) {		// Back to Menu
                if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
				SoundControl.Stop();
                SoundControl.IniMusic("Audio\\BGM\\menu_new", true);
				//isEndScene = true;

				//BackScene(game.stageNum + 4);//5
				game.hasReachedCheckPoint = false;
				game.tmpGameStatus = new GameStatus();
				game.InitializeStack();
            }
            if ( (!game.twoButtonMode && JoyStick.KEY(2)||game.twoButtonMode&&JoyStick.KEY(5)) && game.avilityNum == 0 ) {
                if ((upperScene as Stage).reverse.ReduceTAS() != 0)
                {
                    SoundControl.Stop();
                    SoundControl.RestoreMusic();
                    SoundControl.Resume();
                    if (!game.isMuted) { (upperScene as Stage).reverse.PlaySound(); }
                    isEndScene = true;
                    (upperScene as Stage).player.isAlive = true;
                    (upperScene as Stage).toGameOver = false;
                    (upperScene as Stage).hasEffectedPlayerDeath = false;
                    (upperScene as Stage).reverse.StartReverse();
                    (upperScene as Stage).ResetDeathEffect();
                }
            }

            if (hasDisplayed) {
                game.ReloadStage(game.isHighLvl);
                isEndScene = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(backGround, Vector2.Zero, Color.White);

			for (int i = 0; i < buttonNum; i++)
				if (button[i].isSelected) spriteBatch.Draw(button[i].texture, Vector2.Zero, Color.White);

            if (nowLoading) {
                spriteBatch.DrawString(game.pumpDemi, "Now Loading...", new Vector2(0, 460), Color.Orange, 0, Vector2.Zero, new Vector2(.4f), SpriteEffects.None, 0f);
                hasDisplayed = true;
            }
        }
    }
}
