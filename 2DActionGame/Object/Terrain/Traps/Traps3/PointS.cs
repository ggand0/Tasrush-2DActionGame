using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
    /// <summary>
    /// 動く足場の軌跡をつなぐ点クラス。
	/// TF内では、ScrollUpdateに例外処理を加えたくないがためにカバーしきれないので仕方なくオブジェクト化。
    /// </summary>
    public class PointS : Terrain
    {
		/// <summary>
		/// どの足場の軌跡か
		/// </summary>
        private TracingFoothold tF;

        public PointS(Stage stage, float x, float y, int width, int height)
            : this(stage, x, y, width, height,  null)
        {
        }
        public PointS(Stage stage, float x, float y, int width, int height, TracingFoothold tF)
            : base(stage, x, y, width, height)
        {
            this.tF = tF;
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Terrain\\PointS");
		}

        /// <summary>
        /// 何もしない : 点だから
        /// </summary>
        public override void IsHit(Object targetObject)
        {
        }
    }
}
