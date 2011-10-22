using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DActionGame
{
    /// <summary>
    /// 歩くと崩れる床:乗ってから数フレームの猶予の後消滅.壊れるときエフェクトがあっていい
    /// </summary>
    public class CollapsingFloor : CollapsingBlock
    {
        public CollapsingFloor(Stage stage,  float x, float y, int width, int height)
            : this( stage, x, y, width, height, null, new Vector2())
        {
        }
        public CollapsingFloor(Stage stage, float x, float y, int width, int height, Object user, Vector2 localPosition)
            : base(stage, x, y, width, height, user, localPosition)
        {
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Terrain\\CollapsingFloor");
		}
    }
}
