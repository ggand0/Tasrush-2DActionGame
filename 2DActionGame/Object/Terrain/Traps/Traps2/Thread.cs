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
    /// ObjectやTerrainで代用してもいいが、触れたときに切れる仕様や動きを考えてオブジェクト化。
    /// </summary>
    public class Thread : Terrain
    {
        public bool isDivided { get; set; }
        private Vector2 startingPoint;
        private float length;                       // "糸"だから.
        float Phi, PhiSpeed, PhiNew, PhiSpeedNew;
        internal Vector2 weightVector;              // おもりの位置

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
        /// 角度の指定範囲で回転.　ソース：忘れた
        /// </summary>
        private void MovementUpdate(/*float defaultDegree, float degreeDistance*/)
        {
            // EulerMethodで単振り子の動きの再現を目指す.それがだめならハードコーディングで.
            if(length == 0)length = 100.0f;
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
            // おもりの位置の計算
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
            PhiVt = PhiSpeed - dT * (float)Math.Sin(MathHelper.ToRadians(Phi)) * (float)Gravity / length;

            // 修正オイラー法
            PhiNew = Phi + 0.5f * dT * (PhiSpeed + PhiSpeed);
            PhiSpeedNew = PhiSpeed - 0.5f * dT *(float)(Math.Sin(MathHelper.ToRadians(Phi)) + Math.Sin(MathHelper.ToRadians(PhiT))) * (float)Gravity / length;
        }

        public override void MotionUpdate()
        {
            if (isDamaged) isDivided = true;
        }
        /// <summary>
        /// Playerに引っかかるので何もしない
        /// </summary>
        /// <param name="targetObject"></param>
        public override void IsHit(Object targetObject)
        {
            //base.IsHit(targetObject);
        }

        public override void Draw(SpriteBatch sprite)
        {
            if (isActive) {// 垂れ下がっている状態からの角度分を傾けて描画する
                sprite.Draw(texture, drawPos,new Rectangle(0,0,width,height), Color.White, MathHelper.ToRadians(-0-Phi), new Vector2(0, 0)/*originVector*/, new Vector2(1, 1), SpriteEffects.None, 0);
                //-degreeで引数に持たせると普通の座標系で使う感覚で使える
            }
            //base.DrawEffect(spriteBatch);
        }
    }
}
