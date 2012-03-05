using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
    /// <summary>
    /// 線をたどって動く足場
    /// </summary>
    public class TracingFoothold : Foothold
    {
		private SoundEffect drivingSound;
        //private Vector2 startPosition;                              // トレース開始位置
        private List<Vector2> vectors = new List<Vector2>();
        private List<PointS> points = new List<PointS>();
        //private List<bool> goingSegments = new List<bool>();
        private PointS curPoint;                                    // 現在の行き先に相当する点

        private bool[] goingSegments;                               // pointsのCountが決まればリストにする理由もない
        private bool hasStarted;
        private bool hasEnded;

        public TracingFoothold(Stage stage, float x, float y, int width, int height, List<Vector2> vectors)
            : base(stage, x, y, width, height)
        {
            foreach (Vector2 vector in vectors)
                this.vectors.Add(vector);

            foreach (Vector2 vector in this.vectors)
                points.Add(new PointS(stage, vector.X, vector.Y, 32 ,32));

            foreach (PointS pS in points) {
                //stage.staticTerrains.Add(pS);
                stage.objects.Add(pS);
                stage.dynamicTerrains.Add(pS);// tF自体がdynamicなのを考えると合わせたほうがいい。融通も利く
                pS.Load(game.Content, "Object\\Terrain\\PointS");
            }

            goingSegments = new bool[points.Count];
            goingSegments[0] = true;

			Load();
        }
        protected override void Load()
        {
            texture = content.Load<Texture2D>("Object\\Terrain\\TracingFoothold");
			texture2 = content.Load<Texture2D>("Object\\Terrain\\PointS");
			drivingSound = content.Load<SoundEffect>("Audio\\SE\\lift");
        }

        public override void Update()
        {
            float distance; // vectorと移動先のpointsまでの距離
            
            if (isUnderSomeone || isHit) hasStarted = true;

            // 点を順に辿る
            if(hasStarted && !hasEnded) {
                for(int i=0;i<goingSegments.Length;i++) {
                    if(goingSegments[i]) {
                        Vector2 baseSpeed;

                        if(i < goingSegments.Length-1) {
                            curPoint = points[i+1];
                            speed = points[i+1].position - points[i].position;
                            baseSpeed = Vector2.Normalize(speed);
                            baseSpeed *= new Vector2(stage.camera.speed.Length(), stage.camera.speed.Length());

                            speed = baseSpeed;
                            position += speed;
                        }
                        else {
                            hasEnded = true;
                            break;
                        }
                    }

                    distance = Vector2.Distance(position, curPoint.position);

                    if(distance < 5) {
                        goingSegments[i] = false;
                        goingSegments[i + 1] = true;
                    }
                }

                if(counter % 5 == 0) {
                    if (!game.isMuted ) drivingSound.Play(SoundControl.volumeAll, 0f, 0f);
                }
                counter++;
                
            }
            if (hasEnded) {
                speed = Vector2.Zero;
            }
        }
        public override void IsHit(Object targetObject)
        {
            base.IsHit(targetObject);

            if (targetObject is RushingOutEnemy) {
                isUnderSomeone = isHit = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawPos, Color.White);
            spriteBatch.Draw(texture, drawPos + new Vector2(32, 0), Color.White);
            spriteBatch.Draw(texture, drawPos + new Vector2(64, 0), Color.White);
            // pointsは各個描画
        }
    }
}
