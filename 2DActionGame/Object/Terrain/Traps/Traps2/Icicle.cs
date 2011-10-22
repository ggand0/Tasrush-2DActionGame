using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
    /// <summary>
    /// つららクラス。上から落下して来て、Playerは当たるとダメージ。
	/// Obstacle.MV2()などのためにフラグ処理等を抽象化したい
    /// </summary>
    public class Icicle : CollapsingBlock
    {
		private SoundEffect dropSound;
		/// <summary>
		/// Playerとの距離
		/// </summary>
        protected float distance;
		/// <summary>
		/// 反応する距離
		/// </summary>
        protected float distanceToFallDown = 100;
        public bool isFallingDown { get; internal set; }
        public bool hasPlayedSE2{ get; internal set; }
        public bool hasFalled { get; internal set; }

        public Icicle(Stage stage,  float x, float y, int width, int height)
            : this(stage, x, y, width, height, null, Vector2.Zero)
        {
        }
        public Icicle(Stage stage, float x, float y, int width, int height, Object user, Vector2 localPosition)
            : base(stage, x, y, width, height, user, localPosition)
        {
            if (user != null) hasFalled = true;

			Load();
        }
		protected override void Load()
		{
			base.Load();

			texture = content.Load<Texture2D>("Object\\Terrain\\Icicle");
			dropSound = content.Load<SoundEffect>("Audio\\SE\\ice_SE");
		}


		public override void Initialize()
		{
			base.Initialize();
			hasFalled = false;
			isFallingDown = false;
			hasPlayedSE = false;
		}
        public override void Update()
        {
            distance = Math.Abs(this.position.X - stage.player.position.X);
			if (user == null) {// || (user != null && isBeingUsed)) {
				if (distance < distanceToFallDown)
					isFallingDown = true;
			} else if (isBeingUsed) {
				isFallingDown = true;
			} else {
				if (counter > 120) isFallingDown = true;
			}

            //if (!isFallingDown) hasFalled = false;
            if (isFallingDown) {
                if(!hasPlayedSE) {
                    if (!game.isMuted) dropSound.Play(SoundControl.volumeAll, 0f, 0f);
                    hasPlayedSE = true;
                }
                UpdateNumbers();// ここでもcounter++されてる
                if (position.Y > 640 + 64) hasFalled = true;// 10/1:ここまで来てない：実際に落ちてない
            }

            counter++;
        }


        public override void IsHit(Object targetObject)
        {
            base.IsHit(targetObject);

			if (targetObject.isHit) {
				targetObject.isDamaged = true;
			}
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if ((user != null && isBeingUsed) || user == null) base.Draw(spriteBatch);
        }
    }
}
