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
    public class AvilitySelect : SelectScene
    {
        private SoundEffect start;
        private bool nowLoading, hasDisplayed;

        public AvilitySelect(Scene privousScene)
            : base(privousScene)
        {
            buttonNum = 3;
            button = new Button[buttonNum];

            for (int i = 0; i < button.Length; i++)
                button[i].color = Color.Blue;

            Load();
        }
        private void PushStage()
        {
            switch (game.stageNum) {
                default:
                    PushScene(new Stage1(this, game.isHighLvl));
                    break;
                case 2:
                    PushScene(new Stage2(this, game.isHighLvl));
                    break;
                case 3:
                    PushScene(new Stage3(this, game.isHighLvl));
                    break;
            }
        }

        public override void Load()
        {
            base.Load();

            for (int i = 0; i < buttonNum; i++)
                button[i].texture = content.Load<Texture2D>("General\\Menu\\AvilitySelect" + i);
            start = content.Load<SoundEffect>("Audio\\SE\\start");
        }
        protected override void ButtonUpdate()
        {
            base.ButtonUpdate();



            if (button[0].isSelected && JoyStick.IsOnKeyDown(1)) {// reverse
                if (!game.isMuted) start.Play(SoundControl.volumeAll, 0f, 0f);
                //game.stageNum = 1; // StageSelectでstageNumを変えるため
                game.avilityNum = 0;
                SoundControl.Stop();
                nowLoading = true;
                //game.ReloadStage(game.isHighLvl);
                //PushStage();

            }
            if (button[1].isSelected && JoyStick.IsOnKeyDown(1)) {// slow
                if (!game.isMuted) start.Play(SoundControl.volumeAll, 0f, 0f);
                //game.stageNum = 1;
                game.avilityNum = 1;
                SoundControl.Stop();
                nowLoading = true;
                //PushStage(); //PushScene(new Stage1(this, game.isHighLvl));

            }
            if (button[2].isSelected && JoyStick.IsOnKeyDown(1)) {// accel
                if (!game.isMuted) start.Play(SoundControl.volumeAll, 0f, 0f);
                //game.stageNum = 1;
                game.avilityNum = 2;
                nowLoading = true;
                SoundControl.Stop();
                //PushStage(); //PushScene(new Stage1(this, game.isHighLvl));

            }
            if (hasDisplayed) PushStage();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < buttonNum; i++)
                if (button[i].isSelected) spriteBatch.Draw(button[i].texture, Vector2.Zero, Color.White);


            if (nowLoading) {
                spriteBatch.DrawString(game.pumpDemi, "Now Loading...", new Vector2(0, 460), Color.Orange, 0, Vector2.Zero, new Vector2(.4f), SpriteEffects.None, 0f);
                hasDisplayed = true;
            }
        }
    }
}
