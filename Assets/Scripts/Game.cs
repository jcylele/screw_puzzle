using System.Collections.Generic;
using Stage;
using UnityEngine;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    [Tooltip("Pink,Red,Orange,Brown,Yellow,Green,Blue,Cyan,Purple,Gray")] [SerializeField]
    private Color[] screwColors =
    {
        Color.magenta,
        Color.red,
        new Color(1, 0.5f, 0),
        new Color(0.6f, 0.3f, 0),
        Color.yellow,
        Color.green,
        Color.blue,
        Color.cyan,
        new Color(0.5f, 0, 0.5f),
        Color.gray
    };
    public StagePlayBehaviour stagePlayPrefab;
    public int randomSeed = 0;
    public StageInfo stageInfo;
    
    private List<ScrewBag> screwBags;
    private SpareScrewBag spareSpareScrewBag;
    private readonly List<int> layerValues = new List<int>(Consts.MaxLayerCount);

    public static Game Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Random.InitState(randomSeed);
        for (int i = 0; i < Consts.MaxLayerCount; i++)
        {
            var layerVal = LayerMask.NameToLayer($"ItemLayer{i + 1}");
            layerValues.Add(layerVal);
        }
    }

    public int GetLayerValue(int layerIndex, bool isMask)
    {
        return isMask ? 1 << layerValues[layerIndex - 1] : layerValues[layerIndex - 1];
    }

    public ScrewColor GetNextColor()
    {
        return (ScrewColor)Random.Range(1, (int)ScrewColor.Max);
    }

    private void Start()
    {
        var stagePlayBehaviour = Instantiate(stagePlayPrefab, transform);
        stagePlayBehaviour.stageInfo = stageInfo;
        stagePlayBehaviour.ExpandStage();
        
        this.screwBags = new List<ScrewBag>(Consts.BagCount);
        for (int i = 0; i < Consts.BagCount; i++)
        {
            var bag = new ScrewBag();
            bag.Refresh(GetNextColor());
            screwBags.Add(bag);
        }

        this.spareSpareScrewBag = new SpareScrewBag();

        NotifyUI();
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

    public void OnGameEnd(bool isWin)
    {
        Debug.Log(isWin ? "Win" : "Lose");
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
            spareSpareScrewBag.PutScrew(screwColor);
            if (spareSpareScrewBag.IsFull)
            {
                OnGameEnd(false);
            }

            NotifyUI();
            return;
        }

        screwBag.PutScrew();
        // may happen multiple times
        while (screwBag.IsFull)
        {
            var newColor = GetNextColor();
            screwBag.Refresh(newColor);
            var spareCount = spareSpareScrewBag.TakeScrew(newColor);
            screwBag.PutScrew(spareCount);
        }

        NotifyUI();
    }

    private void Update()
    {
        // left mouse button down
        if (Input.GetMouseButtonDown(0))
        {
            var jointPlay = StagePlayBehaviour.Instance.RaycastJoint();
            if (jointPlay == null)
            {
                return;
            }

            this.OnScrewDrop(jointPlay.UnScrew());
        }
    }

    public Color GetScrewColor(ScrewColor color)
    {
        return color == ScrewColor.None ? Color.white : screwColors[(int)color - 1];
    }

    #region UI related

    public ScrewBag GetScrewBag(int index)
    {
        return screwBags[index];
    }

    public List<ScrewColor> GetSpareScrewList()
    {
        return spareSpareScrewBag.spareScrews;
    }

    private void NotifyUI()
    {
        GameUI.GameUI.Instance.Refresh();
    }

    #endregion
}