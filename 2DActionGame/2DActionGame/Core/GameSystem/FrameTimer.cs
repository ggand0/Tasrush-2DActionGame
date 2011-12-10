using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DActionGame
{
    public class FrameTimer
    {
        //汎用フレームタイマー
        //ゲーム内での進行速度に合わせたカウンタは要りそうだとおもって作った
        public FrameTimer()
        {
            Count = 0;
            TimerState = 0;
        }

        private long timelimit;
        public long Count { get; private set; }

        public enum State
        {
            Ready = 0, Run, Stop, Finish
        }

        public State TimerState { get; private set; }

        public void Start(long timelimit)
        {
            this.timelimit = timelimit;
            Count = 0;
            TimerState = State.Run;
        }

        public void Update()
        {
            switch (TimerState)
            {
                case State.Run:
                    if (timelimit > Count)
                        Count += 1;
                    else
                        TimerState = State.Finish;
                    break;
            }

        }

        public void Stop()
        {
            TimerState = State.Stop;
        }
        public void Continue()
        {
            TimerState = State.Run;
        }

        public void Clear()
        {
            Count = 0;
            TimerState = State.Ready;
        }
    }
}
