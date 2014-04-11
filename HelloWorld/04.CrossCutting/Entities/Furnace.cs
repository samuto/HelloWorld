using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities.Blocks;
using WindowsFormsApplication7.Business.Recipes;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Furnace : Entity
    {
        public event EventHandler Progress;
        public event EventHandler Changed;

        public Slot Input = new Slot();
        public Slot Fuel = new Slot();
        public Slot Product = new Slot();

        private int combustionCounter = 0;
        private int energyCounter = 0;
        private int lastInputId = 0;
        private int lastFuelId = 0;

        private bool on;

        public Furnace()
        {
            this.on = false;
            EntityType = EntityTypeEnum.BlockFullUpdate;
        }

        internal override void OnUpdate()
        {
            // assume that we are turned off
            Recepe recipe = FurnaceRecipes.Instance.Find(Input);
            if (recipe == null ||
               !Product.Content.Compatible(recipe.Product) ||
               Fuel.Content.IsEmpty)
            {
                TurnOff();
                return;
            }
            TurnOn();
            if (Product.Content.IsEmpty)
                Product.Content.ReplaceWithEmptyCompatibleStack(recipe.Product);
            if (Fuel.Content.Id != lastFuelId ||
                Input.Content.Id != lastInputId)
            {
                // check for recipe...
                combustionCounter = 0;
                energyCounter = 0;
            }

            // ok we are burning!
            TurnOn();
            lastFuelId = Fuel.Content.Id;
            lastInputId = Input.Content.Id;

            combustionCounter++;
            if (combustionCounter >= Entity.FromId(Fuel.Content.Id).HeatOfCombustion)
            {
                combustionCounter = 0;
                Fuel.Content.Remove(1);
            }
            energyCounter++;
            if (energyCounter >= Entity.FromId(Input.Content.Id).EnergyToTransform)
            {
                energyCounter = 0;
                Input.Content.Remove(1);
                Product.Content.Add(1);
                if (Changed != null)
                    Changed(this, new EventArgs());
            }
        }

        private void TurnOn()
        {
            if (on == true)
                return;
            // change block to furnaceon
            on = true; 
            Parent.SetLocalBlock(PositionBlock.X, PositionBlock.Y, PositionBlock.Z, BlockRepository.FurnaceOn.Id, false);
            Parent.Invalidate();
        }

        private void TurnOff()
        {
            if (on == false)
                return;
            // change block to furnace (off)
            on = false; 
            Parent.SetLocalBlock(PositionBlock.X, PositionBlock.Y, PositionBlock.Z, BlockRepository.FurnaceOff.Id, false);
            Parent.Invalidate();
        }

        public bool IsRecipeValid()
        {
            return true;
        }

        internal void SaveMetaData()
        {
        }

        internal void Save()
        {

        }
    }
}
