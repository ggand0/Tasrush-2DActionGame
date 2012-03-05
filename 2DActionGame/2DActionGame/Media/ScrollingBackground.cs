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
	/// <summary>
	/// 背景スクロール
	/// </summary>
	/// <see cref="http://msdn.microsoft.com/ja-jp/library/bb203868.aspx"/>
    public class ScrollingBackground : Object
    {
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

		public void Load(GraphicsDevice device, ContentManager content, string texture_name)
		{
			texture = content.Load<Texture2D>(texture_name);
			screenwidth = 1280;
			origin = new Vector2(0, 0);
			texturesize = new Vector2(texture.Width, 0);
		}

        public void Update(float deltaX)
        {
            if (stage != null && stage.isScrolled) {
				if (isFrontal) {
					screenpos.X -= deltaX;
					if (screenpos.X < 0) screenpos.X = texture.Width;
				} else {
					screenpos.X += deltaX * 0.4f;// 0.5f
					if (screenpos.X >  texture.Width) screenpos.X = 0;
				}
            }
            // screenpos.X = screenpos.X % textures.Width;
        }
		// 補助メソッド
		public void ScrollUpdateBoss(Vector2 criteriaPosition)
		{
			if (isFrontal) {
				//if (texture != null) drawPos.X = stage.bossScreenEdgeLeft + (texture.Width - stage.bossScreenEdgeLeft % texture.Width)/**/ + (Game1.Width - Player.screenPosition.X) - criteriaPosition.X - 32;//48;
				if (texture != null) drawPos.X = stage.bossScreenEdgeLeft + (texture.Width - stage.bossScreenEdgeLeft % texture.Width)/**/ + (Game1.Width - Player.screenPosition.X) + Player.screenPosition.X - criteriaPosition.X - 32;//48;
				//drawPos.X = position.X - stage.player.position.X * .10f/**/ + Player.screenPosition.X;
				//drawPos.X = position.X - stage.player.position.X + Player.screenPosition.X;
			} else {
				drawPos.X = position.X - stage.player.position.X * .05f + Player.screenPosition.X;// .05f
			}

			drawPos.Y = position.Y;
			//targetObject.drawPos.X = targetObject.position.X - this.position.X;
			distanceToCamara = Math.Abs(this.position.X - position.X);
		}

        /// <summary>
        /// 背景なのでlayerは一番奥
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
			if (stage != null && (!stage.isScrolled)) {
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
					spriteBatch.Draw(texture, screenpos - texturesize, null, Color.White, 0, origin, 1, SpriteEffects.None, .0f);
				}
			}
        }
    }
}
