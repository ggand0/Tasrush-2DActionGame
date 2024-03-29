﻿using System;
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
		private WinControl form;
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
            game.hasReachedCheckPoint = false;
            game.tmpGameStatus = new GameStatus();
            game.stageScores[game.stageNum - 1] += 10000;
        }

        public override void Load()
		{
			base.Load();

            button[0].texture = content.Load<Texture2D>("General\\Menu\\StageClear");
			SoundControl.IniMusic("Audio\\BGM\\clear", false);
			if (!game.isMuted) SoundControl.Play();

			// Scoreをcastして確定
			this.scoreToDisplay = (int)game.stageScores[game.stageNum-1];
			if (game.stageNum == 3) {
				form = new WinControl(game);
				form.Show();
			}
        }
		public override void Update(double dt)
		{
            if (JoyStick.IsOnKeyDown(1) || JoyStick.IsOnKeyDown(8)) {	// To Next Stage
				if (game.stageNum < Game1.maxStageNum) {
					game.stageNum++;
					game.hasReachedCheckPoint = false;
					SoundControl.Stop();
					game.ReloadStage(game.isHighLvl);
                    //SoundControl.Pause();//SoundControl.Stop();

					
					isEndScene = true;
					upperScene.isEndScene = true;
				} else if (game.stageNum == Game1.maxStageNum && (form.IsDisposed || form == null)) {// To Ending
					game.stageNum = 1;
					game.hasReachedCheckPoint = false;
					SoundControl.Stop();
					PushScene(new Ending(this, true));
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
            } else if (counter == ClearScene.displayTime) {
                /* sec2 = second;
                 * if (game.avilityNum == 2)
                    game.score *= (sec2 / 20);		// アクセルなら
                else if (sec2 / 20 != 0)
                    game.score /= (sec2 / 20);
                second = 0;
                minute = 0;
                sec = 0;*/

                if (game.avilityNum == 2) {
                    game.stageScores[game.stageNum - 1] *= Math.Log(1 / (double)gameStatus.time * 100000);//(gameStatus.time / 20.0);
                } else if (game.avilityNum == 1) {
                    game.stageScores[game.stageNum - 1] *= Math.Log(1 / (double)gameStatus.time * 10000);//(gameStatus.time / 40.0);
                } else {
                    game.stageScores[game.stageNum - 1] *= Math.Log(1 / (double)gameStatus.time * 1000);
                }/*else if (scoreCount / 20.0 != 0) {
                    game.stageScores[game.stageNum - 1] /= (gameStatus.time / 20.0);
                }*/
                // 再度castしつつ更新
                scoreToDisplay = (int)game.stageScores[game.stageNum - 1];

                scoreCount++;
            }

            spriteBatch.Draw(mask, new Rectangle(0, 0, 640, 480), new Color(0, 0, 0, 100));
			spriteBatch.Draw(button[0].texture, Vector2.Zero, Color.White);

            /*Vector2 origin = game.menuFont.MeasureString("GameTime:") / 2;
            Vector2 v = TEXT_POSITION;
            game.spriteBatch.DrawString(game.menuFont, "GameTime:",
                v, Color.White,
               0, origin, 1, SpriteEffects.None, 0);*/

			spriteBatch.DrawString(game.pumpDemi, "GameTime:" + ((int)(gameStatus.time / 60.0)).ToString() + ":" + ((int)(gameStatus.time % 60)).ToString(), /*origin */new Vector2(100, 280), Color.Orange);
			spriteBatch.DrawString(game.pumpDemi, "TotalScore:" + scoreToDisplay.ToString(), new Vector2(100, 350), Color.Orange);
        }
    }
}
