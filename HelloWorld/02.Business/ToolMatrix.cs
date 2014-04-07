using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Business
{
    class ToolMatrix
    {
        private static Dictionary<int, Dictionary<int, float>> efficiencyMatrix = new Dictionary<int, Dictionary<int, float>>();

        static ToolMatrix()
        {
            float excellent = 8f;
            float good = 4f;
            float medium = 2f;
            RegisterEfficiency(ItemRepository.StoneShovel.Id, BlockRepository.Dirt.Id, excellent);
            RegisterEfficiency(ItemRepository.StoneShovel.Id, BlockRepository.DirtWithGrass.Id, excellent);
            RegisterEfficiency(ItemRepository.StoneShovel.Id, BlockRepository.Sand.Id, excellent);
            RegisterEfficiency(ItemRepository.StonePickAxe.Id, BlockRepository.Stone.Id, good);
            RegisterEfficiency(ItemRepository.StonePickAxe.Id, BlockRepository.CobbleStone.Id, good);
            RegisterEfficiency(ItemRepository.StoneAxe.Id, BlockRepository.Wood.Id, good);
            RegisterEfficiency(ItemRepository.StoneAxe.Id, BlockRepository.Plank.Id, good);
        }

        private static void RegisterEfficiency(int itemIdInHand, int itemIdToDestroy, float efficiency)
        {
            if (!efficiencyMatrix.ContainsKey(itemIdInHand))
            {
                efficiencyMatrix.Add(itemIdInHand, new Dictionary<int, float>());
            }
            efficiencyMatrix[itemIdInHand].Add(itemIdToDestroy, efficiency);
        }

        internal static float GetEfficiency(int itemIdInHand, int itemIdToDestroy)
        {
            if (!efficiencyMatrix.ContainsKey(itemIdInHand))
                return 1f;
            if (!efficiencyMatrix[itemIdInHand].ContainsKey(itemIdToDestroy))
                return 1f;
            return efficiencyMatrix[itemIdInHand][itemIdToDestroy];
        }
    }
}
