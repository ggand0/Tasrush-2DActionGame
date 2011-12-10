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
    /// 糸クラス。ObjectやTerrainで代用してもいいが、
	/// 触れたときに切れる仕様や動きを考えてクラス化
    /// </summary>
    public class Thread : Terrain
    {
		private Vector2 startingPoint;
		/// <summary>
		/// 位置の計算に使う。
		/// </summary>
		private float Phi, PhiSpeed, PhiNew, PhiSpeedNew;
		private float length;
		/// <summary>
		/// (Playerが触れて)糸が切れたかどうか
		/// </summary>
        public bool isDivided { get; private set; }
		/// <summary>
		/// おもりの位置
		/// </summary>
        internal Vector2 weightVector;

        public Thread(Stage stage, float x, float y, int width, int height)
            : this(stage, x, y, width, height, 0, 40)
        {
        }
        public Thread(Stage stage, float x, float y, int width, int height, float startingDegree, float length)
            : base(stage, x, y, width, height)
        {
            this.length = length;
            canBeDestroyed = true;
            startingPoint = new Vector2(x, y);
            Phi = startingDegree;
            //PhiSpeed = 5;
            //Create(210);

			Load();
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Terrain\\Thread");
		}

        public override void Update()
        {
            MovementUpdate();
            base.Update();
        }
        /// <summary>
        /// 角度の指定範囲で回転させるメソッド。ソース：忘れた
		/// EulerMethodで単振り子の動きを再現する。
        /// </summary>
		private void MovementUpdate(/*float defaultDegree, float degreeDistance*/)
		{
			if (length == 0) length = 100.0f;
			//float vθ = 0;
			//float Phi, PhiSpeed, PhiNew, PhiSpeedNew;

			//EulerMethod(1,Phi,PhiNew,PhiSpeed,PhiSpeedNew);
			EulerMethod(1, Phi, PhiSpeed);
			Phi = PhiNew;
			PhiSpeed = PhiSpeedNew;

			/*position.X = -length * (float)Math.Sin(MathHelper.ToRadians(Phi));
			position.Y = -length * (float)Math.Cos(MathHelper.ToRadians(Phi));
			position.Y *= -1;
			//position += startingPoint;*/

			// 錘の位置の計算
			weightVector.X = -length * (float)Math.Sin(MathHelper.ToRadians(Phi));
			weightVector.Y = -length * (float)Math.Cos(MathHelper.ToRadians(Phi));
			weightVector.Y *= -1;
			weightVector.X *= -1;

		}
        private void Inicialize(float defaultDegree)
        {
            position.X = -length * (float)Math.Sin(MathHelper.ToRadians(defaultDegree));
            position.Y = -length * (float)Math.Cos(MathHelper.ToRadians(defaultDegree));
        }
        private void EulerMethod(float dT, float Phi, float PhiSpeed)
        {
            float PhiT, PhiVt;

            // オイラー法
            PhiT = Phi + dT * PhiSpeed;
            PhiVt = PhiSpeed - dT * (float)Math.Sin(MathHelper.ToRadians(Phi)) * (float)gravity / length;

            // 修正オイラー法
            PhiNew = Phi + 0.5f * dT * (PhiSpeed + PhiSpeed);
            PhiSpeedNew = PhiSpeed - 0.5f * dT *(float)(Math.Sin(MathHelper.ToRadians(Phi)) + Math.Sin(MathHelper.ToRadians(PhiT))) * (float)gravity / length;
        }

        public override void MotionUpdate()
        {
            if (isDamaged) isDivided = true;
        }
        /// <summary>
        /// Playerに引っかかるので何もしない(切れる仕様にするなら追加する)
        /// </summary>
        public override void IsHit(Object targetObject)
        {
            //base.IsHit(targetObject);
        }

		/// <summary>
		/// 垂れ下がっている状態からの角度分を傾けて描画する
		/// </summary>
		/// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive()) {
				//-degreeで引数に持たせると普通の座標系で使う感覚で使える
                spriteBatch.Draw(texture, drawPos,new Rectangle(0,0,width,height), Color.White, MathHelper.ToRadians(-0 - Phi), Vector2.Zero, 1, SpriteEffects.None, 0);
            }
        }
    }
}
