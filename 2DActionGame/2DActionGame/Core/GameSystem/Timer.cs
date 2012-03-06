using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace _2DActionGame
{
    public class Timer
    {
        public Stopwatch sw = new Stopwatch();
        public bool IsStarted { get { return sw.IsRunning; } }
        public double CurrentTime { get { return (double)sw.ElapsedTicks / (double)Stopwatch.Frequency; } }
        public double PrevTime { get; set; }
        public long Tick { get { return sw.ElapsedTicks; } }

        public void Start()
        {
            sw.Start();
        }
        public void Update()
        {
            PrevTime = CurrentTime;
        }

        /// <summary>
        /// CurrentTime - PrevTimeによる１フレーム分の経過時間を測定します。実行した次の行でUpdateしてください。
        /// </summary>
        public double FrameTime()
        {
            return CurrentTime - PrevTime;
        }

        /// <summary>
        /// タイマーが始まってからの総時間
        /// </summary>
        /// <returns></returns>
        public float TotalTime()
        {
            return (float)sw.ElapsedMilliseconds / 1000.0f; 
        }
    }
}
