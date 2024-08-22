using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    private List<ScrewBag> screwBags;
    private ScrewLine spareScrewLine;

    public static Game Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Random.InitState(0);
    }

    public ScrewColor GetNextColor()
    {
        return (ScrewColor)Random.Range(1, (int)ScrewColor.Max);
    }

    private void Start()
    {
        this.screwBags = new List<ScrewBag>(Consts.BagCount);
        for (int i = 0; i < Consts.BagCount; i++)
        {
            var bag = new ScrewBag();
            bag.Refresh(GetNextColor());
            screwBags.Add(bag);
        }

        this.spareScrewLine = new ScrewLine();
    }

    private ScrewBag GetScrewBag(ScrewColor color)
    {
        foreach (var screwBag in screwBags)
        {
            if (screwBag.BagColor == color)
            {
                return screwBag;
            }
        }

        return null;
    }

    public void OnScrewDrop(ScrewColor screwColor)
    {
        if (screwColor == ScrewColor.None)
        {
            Debug.LogError("ScrewColor is None");
            return;
        }

        var screwBag = GetScrewBag(screwColor);
        if (screwBag == null)
        {
            spareScrewLine.PutScrew(screwColor);
            if (spareScrewLine.IsFull)
            {
                Debug.Log("Game Over!");
            }

            return;
        }

        screwBag.PutScrew();
        // may happen multiple times
        while (screwBag.IsFull)
        {
            var newColor = GetNextColor();
            screwBag.Refresh(newColor);
            var spareCount = spareScrewLine.TakeScrew(newColor);
            screwBag.PutScrew(spareCount);
        }
    }
}