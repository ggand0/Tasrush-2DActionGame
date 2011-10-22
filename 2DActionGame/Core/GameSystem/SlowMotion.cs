using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DActionGame
{
    public class SlowMotion
    {
        //実質このメンバーしかいなくなってしまった可哀想な子
        public bool isSlow { get; private set; }


        public SlowMotion()
        {
        }


        public void Update()
        {
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
