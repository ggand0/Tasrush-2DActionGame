using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DActionGame
{
    /// <summary>
    /// ネジのような敵。埋まっているときは無敵（頭の一部が見える状態）、
	/// RushingOutEnemyのように一度飛び出した後も死ぬまでは繰り返す。
    /// 飛び出し時に射撃する。
    /// </summary>
    public class FlyingOutEnemy3 : FlyingOutEnemy
    {
		protected new readonly float defFlyingOutDistance = 300;
		protected new readonly float defFlyingOutSpeed = -12;

        // 戻る時に地形に引っかかるので当たり判定から外そうか
		/// <summary>
		/// 頂点に達したかどうか
		/// </summary>
        private bool hasReached;

        public FlyingOutEnemy3(Stage stage,  float x, float y, int width, int height, int HP)
            : base(stage, x, y, width, height, HP)
        {
			LoadXML("FlyingOutEnemy3", "Xml\\Objects_Enemy_Stage3.xml");
			flyingOutDistance = defFlyingOutDistance;
			flyingOutSpeed = defFlyingOutSpeed;
			speed.X = -defSpeed;
        }
		protected override void Load()
		{
			base.Load();
			texture = content.Load<Texture2D>("Object\\Character\\FlyingOutENemy3");
		}

        public override void UpdateAnimation()
        {
             animation.Update(0, 0, width, height, 0, 0);
        }
        public override void MovementUpdate()
        {
            distance = Math.Abs(this.position.X - stage.player.position.X);

			if (!hasFlownOut) {
				isOnSomething = false;
				gravity = 0;

				if (distance < flyingOutDistance) {
					if (counter == 0) {
						hasFlownOut = true;
						speed.Y = flyingOutSpeed;
					}
					counter++;
				}
			} else if (hasFlownOut) {
				gravity = defGravity;
				if (Math.Abs(speed.Y) < 5) {// 頂点付近
					hasReached = true;
				}

				// 元の位置に戻って初期化する
				if (Vector2.Distance(position, defPos) <= speed.Y && hasReached) {//<= 5
					hasFlownOut = false;
					hasReached = false;
					isOnSomething = false;
					gravity = 0;
					speed = Vector2.Zero;
					position = defPos;
					counter = 0;
				}
			}
        }
            
    }
}
