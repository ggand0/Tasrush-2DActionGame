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
	/// 左右に揺れ動きながら氷弾をPayerに向けて撃つ敵
	/// </summary>
    public class FlyingEnemy2 : FlyingEnemy
    {
		protected new readonly float defSpeed = 2;
		protected new readonly float shootDistance = 100;

		/// <summary>
		/// 自分と天井とを繋いぐ糸Object
		/// </summary>
        private Thread thread;

        public FlyingEnemy2(Stage stage, float x, float y, int width, int height, int HP)
            : this(stage, x, y, width, height, HP, 0)
        {
        }
        public FlyingEnemy2(Stage stage, float x, float y, int width, int height, int HP, int motionType)
            : base(stage, x, y, width, height, HP, motionType)
        {
			LoadXML("FlyingEnemy2", "Xml\\Objects_Enemy_Stage2.xml");
            //thread = new Thread(stage, 200, 100, 8, 32, 0, 45,150);
            thread = new Thread(stage, x, y - 150, 2, 150, 45, 150);// lengthは適当.あとで追加
            stage.dynamicTerrains.Add(thread);

            turret = new Turret(stage, this, Vector2.Zero/*new Vector2(width/2, height/2)*/, 8, 8, 0, 5, 2, false, true, 0, 3, 5, 180, 20, Vector2.Zero, false, false);
			//turret = new Turret(stage, this, new Vector2(width / 2, height / 2), 8, 8, 0, 5, 2, false, true, 0, 3, 2, 120, 20, Vector2.Zero, false, false); // こっちに変えても発射位置の問題は直らなかった
			stage.weapons.Add(turret);
            turret.isBeingUsed = true;
            //turret = new Turret(stage, this, shootPosition[0], 32, 32, 5, bulletType)
            
			Load();
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\FlyingEnemy2");
		}

        public override void Update()
        {
			if (isAlive) {
				if (!thread.isDivided) {
					//position = thread.position + thread.weightVector;
					position = thread.weightVector + thread.position - new Vector2(width / 2, height);// +new Vector2(0, thread.height);
					//gravity = 0;
				} else {
					gravity = defGravity;
				}

				//if (turret.isEnd) turret.Inicialize();    // 面白いけど失敗.
				//if (!turret.isBeingUsed) turret.isBeingUsed = true;

				//turret.Update();
				base.Update();
			} else {
				turret.isAlive = false;
			}
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
			if (game.inDebugMode) {
				spriteBatch.DrawString(game.menuFont, "t.pos : " + turret.position.ToString(), new Vector2(300, 50), Color.White);
				spriteBatch.DrawString(game.menuFont, "b[0].pos : " + turret.bullets[0].position.ToString(), new Vector2(300, 100), Color.White);
				spriteBatch.DrawString(game.menuFont, "this.pos : " + position.ToString(), new Vector2(300, 150), Color.White);
			}
        }

        public override void UpdateAnimation()
        {
			animation.Update(3, 0, texture.Width / 3, texture.Height, 12, 1);//w, h
        }

        public override void MovementUpdate()
        {
        }
        /// <summary>
        /// 振り子の動き
        /// </summary>
        public void MovementUpdate(float defaultDegree, float moveDegree)
        {
            // 左右の動きはGAR()に任せて、y方向のみ変更
            // y=sinθのパラメータで円弧の軌跡
            //float degree = 0;
            defaultDegree *= -1;
            //int movedDegree = 0;

			if (isMovingRight) {
				if (position.Y < (float)Math.Sin(defaultDegree) + (float)Math.Sin(moveDegree)) {
					degreeSpeed = 20;
					degree += degreeSpeed;
				} else degreeSpeed += -4;
			} else {
				if (position.Y < (float)Math.Sin(defaultDegree) + (float)Math.Sin(moveDegree)) {
					degreeSpeed = -20;
					degree += degreeSpeed;
				} else degreeSpeed += 4;
			}
            //movedDegree += degree;

            position.Y = (float)Math.Sin(degree);

			RoundTripMotion(defPos, moveDistance, defSpeed);
        }

        protected override void UpdateNumbers()
        {
            gravity = defGravity;
			speed.Y += (float)gravity * timeCoef;
            /*if (isOnSomething) scalarSpeed *= friction;
            else scalarSpeed *= frinctionAir;*/
            
            if (speed.X > 0) {
				speed.X += -(.40f * friction) * timeCoef;
                if(speed.X < 0) speed.X = 0;
            } else if (speed.X < 0) {
				speed.X += (.40f * friction) * timeCoef;
				if (speed.X > 0) speed.X = 0;
			}
            if (System.Math.Abs(speed.Y) > maxSpeed) speed.Y = maxSpeed;


			position += speed * timeCoef;			// 位置に加算
			if (position.Y < 0) position.Y = 0;		// 端

            locus.Add(this.drawPos);
            if (locus.Count > 2) locus.RemoveAt(0);
        }

    }
}
