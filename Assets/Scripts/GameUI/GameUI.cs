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
        private bool initialized;

        private void Awake()
        {
            Instance = this;
            initialized = false;
        }

        private void Initialize()
        {
            var bagCount = Game.Instance.bagCount;
            bags = new List<UIScrewBag>(bagCount);
            for (int i = 0; i < bagCount; i++)
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
            if (!initialized)
            {
                Initialize();
                initialized = true;
            }

            var gameInstance = Game.Instance;
            for (int i = 0; i < gameInstance.bagCount; i++)
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