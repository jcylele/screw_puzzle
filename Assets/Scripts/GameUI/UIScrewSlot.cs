using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class UIScrewSlot : MonoBehaviour
    {
        public RawImage screwImage;

        public void Refresh(ScrewColor newColor)
        {
            if (newColor == ScrewColor.None)
            {
                screwImage.enabled = false;
            }
            else
            {
                screwImage.enabled = true;
                screwImage.color = Game.Instance.GetScrewColor(newColor);
            }
        }
    }
}