using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    static class RandomManager
    {
        public static Random RNG { get; private set; }

        static RandomManager()
        {
            RNG = new Random(DateTime.Now.Millisecond);
        }
    }
}
