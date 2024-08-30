using System.Collections.Generic;
using UnityEngine;

public class SpareScrewBag
{
    public List<ScrewColor> spareScrews { get; }

    public bool IsFull => this.spareScrews.Count == Consts.SpareSlotCount;

    public SpareScrewBag()
    {
        this.spareScrews = new List<ScrewColor>(Consts.SpareSlotCount);
    }

    public void PutScrew(ScrewColor screwColor)
    {
        if (screwColor == ScrewColor.None)
        {
            Debug.Log($"put none to spare screw bag");
            return;
        }

        this.spareScrews.Add(screwColor);
    }

    /// <summary>
    /// take screws by color, return taken count
    /// </summary>
    /// <returns>taken count, up to Consts.SlotPerBag</returns>
    public int TakeScrew(ScrewColor screwColor)
    {
        var takenCount = 0;
        for (int i = this.spareScrews.Count - 1; i >= 0 && takenCount < Consts.SlotPerBag; i--)
        {
            if (this.spareScrews[i] != screwColor)
            {
                continue;
            }

            this.spareScrews.RemoveAt(i);
            takenCount++;
        }

        return takenCount;
    }
}