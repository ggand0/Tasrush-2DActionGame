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
    public class ClearScene : SelectScene
    {
		private GameStatus gameStatus;
		private static readonly int displayTime = 120;

		private int scoreCount;
		public int scoreToDisplay { get; private set; }
		/*/// <summary>
		/// スコア情報
		/// </summary>
		private int second, minute, sec;
		/// <summary>
		/// スコア情報
		/// </summary>
		private float sec2;*/

		public ClearScene(Stage stage)
			: base(stage)
        {
			buttonNum = 1;
			button = new Button[buttonNum];
			this.gameStatus = stage.gameStatus;

			for (int i = 0; i < button.Length; i++)
				button[i].color = Color.Blue;

			Load();
        }

        public override void Load()
		{
			base.Load();

            button[0].texture = content.Load<Texture2D>("General\\Menu\\StageClear");
			SoundControl.IniMusic("Audio\\BGM\\clear");
			if (!game.isMuted) SoundControl.Play();

			// Scoreをcastして確定
			this.scoreToDisplay = (int)game.score;
			game.hasReachedCheckPoint = false;
        }
		public override void Update(double dt)
		{
            if (JoyStick.IsOnKeyDown(3) || JoyStick.IsOnKeyDown(8)) {	// To Next Stage
				if (game.stageNum < Game1.maxStageNum) {
					game.stageNum++;
					game.ReloadStage(game.isHighLvl);
					isEndScene = true;
				} else {
					PushScene(new Ending(this));
				}
            }

            counter++;
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            upperScene.Draw(spriteBatch);

            if (counter < ClearScene.displayTime) {
                //minute = second / 60;
                //sec = second % 60;
            } else if (counter == ClearScene.displayTime){
				/* sec2 = second;
				 * if (game.avilityNum == 2)
					game.score *= (sec2 / 20);		// アクセルなら
				else if (sec2 / 20 != 0)
					game.score /= (sec2 / 20);
				second = 0;
				minute = 0;
				sec = 0;*/
				
				if (game.avilityNum == 2) {
					game.score *= (gameStatus.time / 20.0);// 20.0:補正値
				} else if (scoreCount / 20.0 != 0) {
					game.score /= (gameStatus.time / 20.0);
				}
				// 再度castしつつ更新
				scoreToDisplay = (int)game.score;
				
				scoreCount++;
			}

            spriteBatch.Draw(mask, new Rectangle(0, 0, 640, 480), new Color(0, 0, 0, 100));
			spriteBatch.Draw(button[0].texture, Vector2.Zero, Color.White);
			spriteBatch.DrawString(game.pumpDemi, "GameTime:" + ((int)(gameStatus.time / 60.0)).ToString() + ":" + ((int)(gameStatus.time % 60)).ToString(), new Vector2(50, 250), Color.Orange);
			spriteBatch.DrawString(game.pumpDemi, "TotalScore:" + scoreToDisplay.ToString(), new Vector2(50, 300), Color.Orange);
        }
    }
}
