using System.Collections.Generic;
using Stage;
using UnityEngine;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    [Tooltip("Pink,Red,Orange,Brown,Yellow,Green,Blue,Cyan,Purple,Gray")] [SerializeField]
    private Color[] screwColors =
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.gray,
        Color.yellow,
        Color.magenta,
        new Color(0.5f, 0, 0.5f), // purple
        new Color(1, 0.5f, 0), // brown
        Color.cyan,
        new Color(0.6f, 0.3f, 0), // orange
    };

    public StagePlayBehaviour stagePlayPrefab;
    public StageInfo stageInfo;
    public int randomSeed = 0;

    [Range(Consts.BagCount, (int)(ScrewColor.Max - 1))] [Tooltip("Color count in game")]
    public int colorCount = 10;

    private StagePlayBehaviour stagePlayBehaviour;
    private List<ScrewBag> screwBags;
    private SpareScrewBag spareScrewBag;
    private GameColorMgr gameColorMgr;
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

        if (!RandomColors())
        {
            gameObject.SetActive(false);
            return;
        }
    }

    public int GetLayerValue(int layerIndex, bool isMask)
    {
        return isMask ? 1 << layerValues[layerIndex - 1] : layerValues[layerIndex - 1];
    }


    public int RandomNext(int min, int max)
    {
        return Random.Range(min, max);
    }

    private void Start()
    {
        stagePlayBehaviour = Instantiate(stagePlayPrefab, transform);
        stagePlayBehaviour.stageInfo = stageInfo;
        stagePlayBehaviour.ExpandStage();

        this.screwBags = new List<ScrewBag>(Consts.BagCount);
        for (int i = 0; i < Consts.BagCount; i++)
        {
            var bag = new ScrewBag();
            var bagColor = gameColorMgr.UseColor();
            bag.Refresh(bagColor);
            screwBags.Add(bag);
        }

        this.spareScrewBag = new SpareScrewBag();

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
        // put to spare bag
        if (screwBag == null)
        {
            spareScrewBag.PutScrew(screwColor);
            if (spareScrewBag.IsFull)
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
            // use nww color before unuse current color, can avoid the same color
            var newColor = gameColorMgr.UseColor();
            
            // unuse current color
            if (!gameColorMgr.UnuseColor(screwBag.BagColor))
            {
                return;
            }
            
            // refresh bag color
            screwBag.Refresh(newColor);

            // put spare screws of the new color into new bag
            var spareCount = spareScrewBag.TakeScrew(newColor);
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

    #region Color Random related

    private bool RandomColors()
    {
        if (stageInfo.jointCount % Consts.SlotPerBag != 0)
        {
            Debug.LogError("Joint count is not multiple of slot per bag");
            return false;
        }

        var bagCount = stageInfo.jointCount / Consts.SlotPerBag;
        if (bagCount < colorCount)
        {
            Debug.LogError($"joint count {stageInfo.jointCount} is not enough for color count {colorCount}");
            return false;
        }

        gameColorMgr = new GameColorMgr(this);
        // ensure each color has at least one bag
        for (int i = 0; i < colorCount; i++)
        {
            var color = (ScrewColor)(i + 1);
            gameColorMgr.AddColor(color);
        }

        // random the rest bags
        bagCount -= colorCount;
        for (var i = 0; i < bagCount; i++)
        {
            var bagColor = (ScrewColor)RandomNext(1, colorCount + 1);
            gameColorMgr.AddColor(bagColor);
        }

        gameColorMgr.ShuffleJointColors();

        return true;
    }

    public ScrewColor GetNextJointColor()
    {
        return gameColorMgr.GetNextJointColor();
    }

    #endregion

    #region UI related

    public ScrewBag GetScrewBag(int index)
    {
        return screwBags[index];
    }

    public List<ScrewColor> GetSpareScrewList()
    {
        return spareScrewBag.spareScrews;
    }

    private void NotifyUI()
    {
        GameUI.GameUI.Instance.Refresh();
    }

    #endregion
}