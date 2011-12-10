#region File Description
//-----------------------------------------------------------------------------
// ScrollingBackground.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DActionGame
{
    public class ScrollingTASEffect : Object
    {
        //scrollingBackgroundをさらにコピー

        public  Vector2 screenpos, origin, texturesize;
        private int screenwidth;
        public bool isFrontal { get; set; }
        public Color color;
        public byte colorSpeed { get; set; }
        public byte defcolor { get; set; }

        public ScrollingTASEffect(Vector2 screenpos)
            :this(null, screenpos)
        {
        }
        public ScrollingTASEffect(Stage stage, Vector2 screenpos)
        {
            
            this.stage = stage;
            this.screenpos = screenpos;
            this.color = new Color(255, 255, 255, 191);
            colorSpeed = 3;
        }

        public void Load(GraphicsDevice device, /*Texture2D backgroundTexture*/ContentManager content, string texture_name)
        {
            //textures = backgroundTexture;
            texture = content.Load<Texture2D>(texture_name);
            //screenheight = 960;// device.Viewport.Height;// screenheight = Height
            screenwidth = 1280;//device.Viewport.Width;
            // Set the origin so that we're drawing from the 
            // center of the top edge.
            origin = new Vector2(0, 0);// textures.Width / 2
            // Set the screen position to the center of the screen.
            //↓最初から削ってったっけ？削らないと動かない状態だったが...
            //screenpos = new Vector2(screenwidth , 0);//screenwidth / 2, screenheight / 2
            // Offset to draw the second textures, when necessary.
            texturesize = new Vector2(texture.Width, 0);
        }

        public void Update(float deltaX)
        {
            if (stage != null && (stage.reverse.isReversed || stage.isAccelerated || stage.slowmotion.isSlow) )
            {
                screenpos.X -= deltaX;
                if (screenpos.X < 0) screenpos.X = texture.Width;
            }
            if (color.A < 10)
                color.A = (byte)191;
            else
                color.A = (byte)((color.A) - 6);


            //screenpos.X = screenpos.X % textures.Width;　正の向きに動かすときはこう書くとｽﾏｰﾄ

            /*screenpos.X -= deltaX; //振動する
            screenpos.X = -(screenpos.X % textures.Width);*/
        }


        //layerは一番前

        public override void Draw(SpriteBatch spriteBatch)
        {
            /*spriteBatch.Begin(SpriteSortMode.FrontToBack,    // 3.1ではBlendStateクラスが無いようなので
                       BlendState.Opaque,
                       null,
                       DepthStencilState.Default,
                       null);*/
           // spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);

            // DrawEffect the textures, if it is still onscreen.
            if (stage != null && !stage.reverse.isReversed && !stage.slowmotion.isSlow && !stage.isAccelerated)
            {
                if (isFrontal)
                {
                    spriteBatch.Draw(texture, drawPos, null, Color.White, 0, origin, 1, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, drawPos - new Vector2(-640, 0), null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);// 背景の深度値は.0fに統一すべきか？
                    spriteBatch.Draw(texture, drawPos - new Vector2(640, 0), null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
                    spriteBatch.Draw(texture, drawPos - new Vector2(-1280, 0), null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
                    spriteBatch.Draw(texture, drawPos - new Vector2(1280, 0), null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
                }
                else
                {
                    spriteBatch.Draw(texture, drawPos, null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
                    spriteBatch.Draw(texture, drawPos - new Vector2(-640, 0), null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
                    spriteBatch.Draw(texture, drawPos - new Vector2(640, 0), null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
                }
            }
            else
            {
                if (screenpos.X < screenwidth)
                {
                    spriteBatch.Draw(texture, screenpos, null, color, 0, origin, 1, SpriteEffects.None, .0f);
                    // DrawEffect the textures a second time, behind the first,
                    // to create the scrolling illusion.
                    spriteBatch.Draw(texture, screenpos - texturesize, null, color, 0, origin, 1, SpriteEffects.None, .0f);
                }
            }
            //spriteBatch.End();
        }

    }
}
