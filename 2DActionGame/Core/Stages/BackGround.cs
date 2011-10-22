using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
    public class BackGround : Object
    {   // ScrollingBackGroundがあるから今のところ不要...だったが Blockの背景バッファとして利用することに
        public RenderTarget2D renderTarget { get; set; }
        public DepthStencilBuffer depthStencilBuffer { get; set; }

        public BackGround(Game1 game, Stage stage, int width, int height)
            :base(game,stage,width,height)
        {
            // OUtOfVideoMemory error.... 110個作成されていたorz
            renderTarget = new RenderTarget2D(
                game.graphics.GraphicsDevice, width, height, 1, SurfaceFormat.Color);// 予期しないエラーが発生しました(笑)→DepthStencilBufferを指定する必要があるらしい
            //depthStencilBuffer = new DepthStencilBuffer(game.GraphicsDevice, width, height, DepthFormat.Depth24Stencil8Single);// DepthFormatはよくわからない
            //depthStencilBuffer.
            //cameraにはじかせないために：
            this.activeDistance = width;
        }
        public override void Draw(SpriteBatch sprite)
        {
            //this.texture = renderTarget.GetTexture();
            //base.DrawEffect(sprite);
            
            sprite.Begin();
            sprite.Draw(texture, drawVector, Color.White);
            sprite.End();
            
        }
    }
}
