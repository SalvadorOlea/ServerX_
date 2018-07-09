using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class Misc
    {

        private static Stopwatch sw;

        public static void Reset()
        {
            if (sw != null)
            {
                sw.Stop();
                sw.Reset();
            }

            sw = new Stopwatch();
            sw.Start();
        }
        public static uint UnixTime
        {
            get
            {
                TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                return (uint)ts.TotalSeconds;
            }
        }
        public static ulong Now
        {
            get { return (ulong)sw.ElapsedMilliseconds + 900000; }
        }
    }
}
