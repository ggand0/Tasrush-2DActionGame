using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
    /// <summary>
    /// 静的地形の背景バッファ
    /// </summary>
    public class BackGround : Object
    {
        public RenderTarget2D renderTarget { get; set; }
        public DepthStencilBuffer depthStencilBuffer { get; set; }

        public BackGround(Stage stage, int width, int height)
            : base(stage,width,height)
        {
            // OUtOfVideoMemory error.... 110個作成されていたorz
            renderTarget = new RenderTarget2D(game.graphics.GraphicsDevice, width, height, 1, SurfaceFormat.Color);// 予期しないエラーが発生しました(笑)→DepthStencilBufferを指定する必要があるらしい
            //depthStencilBuffer = new DepthStencilBuffer(game.GraphicsDevice, width, height, DepthFormat.Depth24Stencil8Single);// DepthFormatはよくわからない
            //depthStencilBuffer.
            //cameraにはじかせないために：
            this.activeDistance = width;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            //this.textures = renderTarget.GetTexture();
            //base.DrawEffect(spriteBatch);

            //spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);

            //spriteBatch.Draw(textures, drawPos, Color.White);
            //if(textures != null)
            spriteBatch.Draw(texture, drawPos, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .5f);
            //spriteBatch.End();
            
        }
    }
}
