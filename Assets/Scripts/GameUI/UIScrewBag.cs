using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class UIScrewBag : MonoBehaviour
    {
        public RawImage bagImage;
        public Transform[] slotRoots;
        private List<UIScrewSlot> slots = new List<UIScrewSlot>(Consts.SlotPerBag);

        public void Initialize(UIScrewSlot slotPrefab)
        {
            for (int j = 0; j < Consts.SlotPerBag; j++)
            {
                var slot = Instantiate(slotPrefab, slotRoots[j]);
                slots.Add(slot);
            }
        }

        public void Refresh(ScrewColor newColor, int screwCount)
        {
            bagImage.color = Game.Instance.GetScrewColor(newColor);
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].Refresh(i < screwCount ? newColor : ScrewColor.None);
            }
        }
    }
}