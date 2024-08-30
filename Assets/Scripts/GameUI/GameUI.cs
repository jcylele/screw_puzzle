using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
    public class GameUI : MonoBehaviour
    {
        public Transform bagRoot;
        public UIScrewBag uiScrewBagPrefab;
        public Transform spareSlotRoot;
        public UIScrewSlot uiScrewSlotPrefab;

        public static GameUI Instance { get; private set; }

        private List<UIScrewBag> bags;
        private List<UIScrewSlot> spareSlots;

        private void Awake()
        {
            Instance = this;

            bags = new List<UIScrewBag>(Consts.BagCount);
            for (int i = 0; i < Consts.BagCount; i++)
            {
                var bag = Instantiate(uiScrewBagPrefab, bagRoot);
                bag.Initialize(uiScrewSlotPrefab);
                bags.Add(bag);
            }

            spareSlots = new List<UIScrewSlot>(Consts.SpareSlotCount);
            for (int i = 0; i < Consts.SpareSlotCount; i++)
            {
                var slot = Instantiate(uiScrewSlotPrefab, spareSlotRoot);
                spareSlots.Add(slot);
            }
        }

        public void Refresh()
        {
            var gameInstance = Game.Instance;
            for (int i = 0; i < Consts.BagCount; i++)
            {
                var bag = gameInstance.GetScrewBag(i);
                bags[i].Refresh(bag.BagColor, bag.ScrewCount);
            }

            var spareScrewList = gameInstance.GetSpareScrewList();
            for (int i = 0; i < Consts.SpareSlotCount; i++)
            {
                spareSlots[i].Refresh(i < spareScrewList.Count ? spareScrewList[i] : ScrewColor.None);
            }
        }
    }
}