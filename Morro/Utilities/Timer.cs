using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Utilities
{
    class Timer
    {
        public bool Active { get; private set; }        
        public float Duration { get; set; }
        public float ElapsedTime { get; private set; }

        private bool done;

        public Timer(float duration) : this(duration, true)
        {

        }

        public Timer(float duration, bool active)
        {
            Active = active;
            Duration = duration;            
        }


        public void Start()
        {
            Active = true;
        }

        public void Stop()
        {
            Active = false;
        }

        public void Reset()
        {
            ElapsedTime = 0;
            done = false;
        }

        public bool Done()
        {
            if (!Active)
                return false;

            ElapsedTime += Engine.DeltaTime * 1000;
            done = ElapsedTime >= Duration;

            return done;
        }
    }
}
