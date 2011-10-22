using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DActionGame
{
    public class SlowMotionCounter
    {
        public bool isSlow { get; set; }
        public const int frequency = 2; // Update頻度　＝ (1 / Count)
        public int count = 1;

        public SlowMotionCounter(Stage stage)
        {
        }

        private bool Counter()
        {
            if (count % frequency == 0)
            {
                count = 1;
                return true;
            }
            else
            {
               return false;
            }
            count++;
        }

        public void StartSlowMotion()
        {
            isSlow = true;
        }

        public void FinishSlowMotion()
        {
            isSlow = false;
        }

    }
}
