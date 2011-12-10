using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace _2DActionGame
{
    public class UIObject
    {

        protected Vector2 position;
        protected int data { get; set; }
        public Stage stage;
        public Texture2D texture;
        public bool Visible { get; set; }
        //表示する部分を指定する矩形
        protected Rectangle rectangle;
        protected Color color { get; set; }


        public UIObject(Stage stage, Vector2 position)
        {
            this.stage = stage;
            this.position = position;

            Visible = true;
        }

        public virtual void Load(ContentManager content, string texture_name)
        {
            texture = content.Load<Texture2D>(texture_name);
        }


        virtual public void Draw(SpriteBatch sprite)
        {
            sprite.Draw(texture, position, new Rectangle(0, 0, 640, 32), Color.White);
        }
    }
}
