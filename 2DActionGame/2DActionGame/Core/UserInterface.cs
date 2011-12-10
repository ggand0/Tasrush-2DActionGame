using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame.Core
{
    public class UserInterface
    {
        /*
         * ゲージ、スコアなどを管理するクラス。
         * 基本的にはデータ元からプロパティ引っ張ってきて、画面に表示するだけだろう。
         */

        public Stage stage;
        public Game1 game;
        public static SpriteBatch sprite;
        public Texture2D texture;
        public UIData uidata;
        //場所および描画範囲を指定する矩形。配列とかで並べるほうがいいのか？
        private Rectangle reverseRectangle;

        //いろんな数値を集めるdata群。意味あるか微妙だけど・・・。
        private struct UIData
        {
            public int reversePower;
        }

        public UserInterface(Stage stage)
        {
            this.stage = stage;
            uidata = new UIData();
            reverseRectangle = new Rectangle(500,10,0,10);
        }

        public void Load(ContentManager content, string texture_name)
        {
            texture = content.Load<Texture2D>(texture_name);
        }

        public void Update()
        {
            uidata.reversePower = stage.reverse.ReversePower;
            reverseRectangle.Width = uidata.reversePower;
        }

        public void Draw(SpriteBatch sprite)
        {
            sprite.Begin();
            sprite.Draw(texture,reverseRectangle,Color.White);
            sprite.End();
 
        }

        rec
    }
}
