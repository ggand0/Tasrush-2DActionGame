﻿#region File Description
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
    public class ScrollingBackground : Object
    {
        // 背景スクロール　http://msdn.microsoft.com/ja-jp/library/bb203868.aspx　をちょっと変更しただけ
        public  Vector2 screenpos, origin, texturesize;
        private int screenwidth;
        public bool isFrontal { get; set; }

        public ScrollingBackground(Vector2 screenpos)
            :this(null, screenpos)
        {
        }
        public ScrollingBackground(Stage stage, Vector2 screenpos)
        {
            this.stage = stage;
            this.screenpos = screenpos;
        }

        public void Load( GraphicsDevice device, /*Texture2D backgroundTexture*/ContentManager content, string texture_name )
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
            texturesize = new Vector2(texture.Width,0);
        }

        public void Update(float deltaX)
        {
            if (stage != null && stage.isScrolled) {
                screenpos.X -= deltaX;
                if (screenpos.X < 0) screenpos.X = texture.Width;
            }
            //screenpos.X = screenpos.X % textures.Width;　正の向きに動かすときはこう書くとｽﾏｰﾄ

            /*screenpos.X -= deltaX; //振動する
            screenpos.X = -(screenpos.X % textures.Width);*/
        }
		// 補助メソッド
		public void ScrollUpdateBoss(Vector2 criteriaPosition)
		{
			if (isFrontal) {
				drawPos.X = position.X - stage.player.position.X * .10f + Player.screenPosition.X;
			} else {
				drawPos.X = position.X - stage.player.position.X * .05f + Player.screenPosition.X;
			}

			drawPos.Y = position.Y;
			//targetObject.drawPos.X = targetObject.position.X - this.position.X;
			distanceToCamara = Math.Abs(this.position.X - position.X);
		}

        /// <summary>
        /// 背景なのでlayerは一番奥
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            /*spriteBatch.Begin(SpriteSortMode.FrontToBack,    // 3.1ではBlendStateクラスが無いようなので
                       BlendState.Opaque,
                       null,
                       DepthStencilState.Default,
                       null);*/
            //spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);

            // DrawEffect the textures, if it is still onscreen.
			if (stage != null && (!stage.isScrolled /*|| stage.inBossBattle*/)) {
				if (isFrontal) {
					spriteBatch.Draw(texture, drawPos, null, Color.White, 0, origin, 1, SpriteEffects.None, 0f);
					spriteBatch.Draw(texture, drawPos - new Vector2(-640, 0), null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);// 背景の深度値は.0fに統一すべきか？ 3/28 drawVec=0
					spriteBatch.Draw(texture, drawPos - new Vector2(640, 0), null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
					spriteBatch.Draw(texture, drawPos - new Vector2(-1280, 0), null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
					spriteBatch.Draw(texture, drawPos - new Vector2(1280, 0), null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
				} else {
					spriteBatch.Draw(texture, drawPos, null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
					spriteBatch.Draw(texture, drawPos - new Vector2(-640, 0), null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
					spriteBatch.Draw(texture, drawPos - new Vector2(640, 0), null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
				}
			} else {
				if (screenpos.X < screenwidth) {
					spriteBatch.Draw(texture, screenpos, null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
					// DrawEffect the textures a second time, behind the first,
					// to create the scrolling illusion.
					spriteBatch.Draw(texture, screenpos - texturesize, null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
				}
			}
            //spriteBatch.End();
        }
    }
}