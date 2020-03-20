using Morro.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Systems
{
    class SBoidPhysics : SPhysics
    {
        public SBoidPhysics(Scene scene) : base(scene, Integrator.SemiImplictEuler, 4, 60)
        {
            Depend(typeof(SFlocking));
        }
    }
}
