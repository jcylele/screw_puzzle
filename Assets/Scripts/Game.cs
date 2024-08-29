using System.Collections.Generic;
using Item;
using UnityEngine;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    private List<ScrewBag> screwBags;
    private ScrewLine spareScrewLine;

    private readonly RaycastHit2D[] rayHits = new RaycastHit2D[1];
    private readonly RaycastHit2D[] circleHits = new RaycastHit2D[1];

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

    private void OnScrewDrop(ScrewColor screwColor)
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

    private void Update()
    {
        // left mouse button down
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
    }

    private void OnClick()
    {
        // Debug.Log("Mouse0 Down");
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var rayHitCount = Physics2D.RaycastNonAlloc(mousePos, Vector2.zero, rayHits);
        if (rayHitCount == 0) return;

        // Debug.Log(hit.collider);
        var item = rayHits[0].collider.GetComponentInParent<ItemPlayBehaviour>();
        if (item == null) return;

        // Debug.Log(item);
        var jointPlay = item.GetJointByHit(rayHits[0].point);
        if (jointPlay == null) return;

        // check if the joint is covered by other items
        var circleHitCount = Physics2D.CircleCastNonAlloc(jointPlay.WorldPosition, Consts.JointCollisionRadius,
            Vector2.zero, circleHits);
        if (circleHitCount == 0)
        {
            Debug.LogError("WTF, circleHitCount == 0");
            return;
        }

        var item2 = circleHits[0].collider.GetComponentInParent<ItemPlayBehaviour>();
        // covered by other items
        if (item2 != item) return;

        this.OnScrewDrop(jointPlay.OnClick());
    }
}