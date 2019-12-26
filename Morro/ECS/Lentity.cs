using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class Lentity
    {
        public int ID { get; private set; }
        public ComponentBag Components { get; private set; }

        public Lentity(int id)
        {
            ID = id;
            Components = new ComponentBag();
        }
    }
}
