using UnityEngine;

public class ScrewBag
{
    public ScrewColor BagColor { get; private set; }
    private int ScrewCount { get; set; }

    public bool IsFull => this.ScrewCount == Consts.SlotPerBag;

    public ScrewBag()
    {
        this.Refresh(ScrewColor.None);
    }

    public void Refresh(ScrewColor newColor)
    {
        this.BagColor = newColor;
        this.ScrewCount = 0;
    }

    public void PutScrew(int screwCount = 1)
    {
        if (screwCount > Consts.SlotPerBag)
        {
            Debug.Assert(false, "Screw count is too big");
        }

        this.ScrewCount += screwCount;
    }
}