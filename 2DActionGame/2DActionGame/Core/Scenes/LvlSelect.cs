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
    public class LvlSelect : SelectScene
    {

		public LvlSelect(Scene privousScene)
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

			for (int i = 0; i < buttonNum; i++) {
				button[i].texture = content.Load<Texture2D>("General\\Menu\\LvlSelect" + i);
			}
        }
        protected override void ButtonUpdate()
        {
			base.ButtonUpdate();

			if (button[0].isSelected && JoyStick.IsOnKeyDown(3)) {
				if (game.isHighLvl) game.hasReachedCheckPoint = false;

                game.isHighLvl = false;
				if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
				PushScene(new AvilitySelect(this));
            }
			if (button[1].isSelected && JoyStick.IsOnKeyDown(3)) {
				if (!game.isHighLvl) game.hasReachedCheckPoint = false;

                game.isHighLvl = true;
                if (!game.isMuted) choose.Play(SoundControl.volumeAll, 0f, 0f);
				PushScene(new AvilitySelect(this));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backGround, Vector2.Zero, Color.White);

            for(int i = 0; i < buttonNum; i++)
				if (button[i].isSelected) spriteBatch.Draw(button[i].texture, Vector2.Zero, Color.White);
        }
    }
}
