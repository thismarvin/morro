using Microsoft.Xna.Framework;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    public enum ShapeType
    {
        Square
    }

    static class Geometry
    {
        public static ResourceHandler<ShapeData> Shapes;

        static Geometry()
        {
            Shapes = new ResourceHandler<ShapeData>();

            ShapeData squareData = new ShapeData
            (
                new List<Vector3>() 
                { 
                    new Vector3(0, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(0, 1, 0),
                },
                new List<short>() 
                { 
                    0, 1, 2,
                    0, 2, 3
                }
            );

            Shapes.Register("Square", squareData);
        }
    }
}
