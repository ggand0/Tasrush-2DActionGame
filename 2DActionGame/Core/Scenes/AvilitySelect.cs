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

			for(int i = 0; i < buttonNum; i++)
				button[i].texture = content.Load<Texture2D>("General\\Menu\\AvilitySelect" + i);
			start = content.Load<SoundEffect>("Audio\\SE\\start");
        }
        protected override void ButtonUpdate()
        {
			base.ButtonUpdate();

            if (button[0].isSelected && Controller.IsOnKeyDown(3)) {// reverse
                //game.stageNum = 1; // StageSelectでstageNumを変えるため
                game.avilityNum = 0;
				SoundControl.Stop();
                //game.ReloadStage(game.isHighLvl);
				PushStage();
                if (!game.isMuted) start.Play(SoundControl.volumeAll, 0f, 0f);
            }
			if (button[1].isSelected && Controller.IsOnKeyDown(3)) {// slow
                //game.stageNum = 1;
                game.avilityNum = 1;
				SoundControl.Stop();
				PushStage(); //PushScene(new Stage1(this, game.isHighLvl));
				if (!game.isMuted) start.Play(SoundControl.volumeAll, 0f, 0f);
            }
			if (button[2].isSelected && Controller.IsOnKeyDown(3)) {// accel
                //game.stageNum = 1;
                game.avilityNum = 2;
				SoundControl.Stop();
				PushStage(); //PushScene(new Stage1(this, game.isHighLvl));
				if (!game.isMuted) start.Play(SoundControl.volumeAll, 0f, 0f);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            for(int i = 0; i < buttonNum; i++)
				if (button[i].isSelected) spriteBatch.Draw(button[i].texture, Vector2.Zero, Color.White);
        }
    }
}
