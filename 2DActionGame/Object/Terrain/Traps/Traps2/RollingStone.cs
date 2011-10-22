using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace _2DActionGame
{
    /// <summary>
    /// 回転させる仕様にしたいが実装がだるい
    /// もう面倒なのでこのクラスは後回しにしよう. どうせ間に合わんがね...
    /// </summary>
    public class RollingStone : Terrain
    {
        private float defaultSpeed;
        public RollingStone(Game1 game, Stage stage,  float x, float y, int width, int height)
            :base(game,stage,x,y,width,height)
        {
            this.width = 96;
            this.height = 96;
            defaultSpeed = -8;
        }

        public override void Update()
        {
            //defaultSpeed = -1;
            if (vector.X > stage.player.vector.X + 50) speed.X = defaultSpeed;
            degree += 1;
            if (degree == 360) degree = 0;
            //base.Update();
            UpdateNumbers();
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sprite)
        {
            //base.Draw(sprite);
            sprite.Begin();
            sprite.Draw(texture, drawVector, Color.White);
            sprite.Draw(texture, drawVector + new Vector2(width/2, height/2), null, Color.White, -degree, new Vector2(width/2, height/2), new Vector2(1, 1), SpriteEffects.None, 0);
            //sprite.Draw(texture, drawVector + new Vector2(-48,-48), null, Color.White, -degree, new Vector2(48,48), new Vector2(1, 1), SpriteEffects.None, 0);
            sprite.End();
        }

    }
}
