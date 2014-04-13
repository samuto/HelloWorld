using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using SlimDX;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;
using SlimDX.DirectInput;
using WindowsFormsApplication7.Frontend.Gui;
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business.Profiling;
using WindowsFormsApplication7.CrossCutting.Entities.Items;
using WindowsFormsApplication7.CrossCutting.Entities.Blocks;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Entity
    {
        public int Id;
        public float HeatOfCombustion = 0f;
        public float EnergyToTransform = 0f;
        public string Name;
        public bool Consumable = false;
        public bool IsOpaque = true;
        public int Density = 0;
        public EntityTypeEnum EntityType = EntityTypeEnum.NoUpdate;

        public Chunk Parent;
        public PositionBlock PositionBlock = new PositionBlock(-1,-1,-1);

        public enum EntityTypeEnum
        {
            NoUpdate,
            BlockFullUpdate,
            BlockRandomUpdate,
            EntityStackFullUpdate
        }

        internal static Entity FromId(int id)
        {
            if (IsItem(id))
                return Item.FromId(id);
            else if(IsBlock(id))
                return Block.FromId(id);
            throw new Exception("Unknown entity type");
                    
        }

        internal Entity SetHeatOfCombustion(float heatOfCombustion)
        {
            HeatOfCombustion = heatOfCombustion;
            return this;
        }

        internal static bool IsBlock(int id)
        {
                return id > 0 && id < ItemRepository.ItemIdOffset;
            
        }

        internal static bool IsItem(int id)
        {
                return id >= ItemRepository.ItemIdOffset;
        }

        internal virtual void OnUpdate()
        {
            
        }

        internal void AddToParent()
        {
            Parent.AddEntity(this);
        }

        internal void RemoveFromParent()
        {
            Parent.RemoveEntity(this);
        }

        internal virtual void OnInitialize()
        {

        }
    }
}