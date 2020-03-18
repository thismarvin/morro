﻿using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    abstract class PartitioningSystem : UpdateSystem
    {
        private IComponent[] positions;
        private IComponent[] dimensions;

        protected Partitioner<PartitionerEntry> partitioner;

        internal PartitioningSystem(Scene scene, int targetFPS) : base(scene, 0, targetFPS)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CPartitionable));
        }

        public List<int> Query(Rectangle bounds)
        {
            return partitioner.Query(bounds);
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];

            partitioner.Insert(new PartitionerEntry(entity, position, dimension));
        }

        public override void Update()
        {
            partitioner.Clear();

            positions = scene.GetData<CPosition>();
            dimensions = scene.GetData<CDimension>();

            base.Update();
        }

        protected struct PartitionerEntry : IPartitionable
        {
            public int Identifier { get; set; }
            public Rectangle Bounds { get; set; }

            public PartitionerEntry(int entity, CPosition position, CDimension dimension)
            {
                Identifier = entity;
                Bounds = new Rectangle(position.X, position.Y, dimension.Width, dimension.Height);
            }
        }
    }
}
