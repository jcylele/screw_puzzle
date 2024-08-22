using System.Collections.Generic;

public class ScrewLine
{
    private List<ScrewColor> spareScrews;

    public bool IsFull => this.spareScrews.Count == Consts.MaxSpareSlotCount;

    public ScrewLine()
    {
        this.spareScrews = new List<ScrewColor>(Consts.MaxSpareSlotCount);
    }

    public void PutScrew(ScrewColor screwColor)
    {
        this.spareScrews.Add(screwColor);
    }

    /// <summary>
    /// take screws by color, return taken count
    /// </summary>
    /// <returns>taken count, up to Consts.SlotPerBag</returns>
    public int TakeScrew(ScrewColor screwColor)
    {
        int takenCount = 0;
        for (int i = this.spareScrews.Count - 1; i >= 0 && takenCount < Consts.SlotPerBag; i++)
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