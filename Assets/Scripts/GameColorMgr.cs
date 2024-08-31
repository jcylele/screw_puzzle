using System.Collections.Generic;
using UnityEngine;

public class GameColorMgr
{
    private class ColorInfo
    {
        private readonly ScrewColor color;
        private int usedCount;
        private int leftCount;

        public bool CanUse => leftCount > 0;
        public bool IsUsed => usedCount > 0;

        public ColorInfo(ScrewColor color)
        {
            this.color = color;
            this.usedCount = 0;
            this.leftCount = 1;
        }

        public void Add()
        {
            leftCount++;
        }

        public void Use()
        {
            // Debug.Log($"use {this.color}");
            usedCount++;
            leftCount--;
        }

        public bool Unuse()
        {
            // Debug.Log($"unuse {this.color}");
            if (usedCount == 0)
            {
                return false;
            }

            usedCount--;
            return true;
        }
    }

    private readonly Dictionary<ScrewColor, ColorInfo> colorInfos = new Dictionary<ScrewColor, ColorInfo>();
    private int leftColorCount;
    private List<ScrewColor> jointColors;
    private int nextJointColorIndex = 0;

    private readonly Game game;

    // cast for performance
    private readonly List<ScrewColor> colorCandidates = new List<ScrewColor>();

    public GameColorMgr(Game game)
    {
        this.game = game;
        jointColors = new List<ScrewColor>();
        nextJointColorIndex = 0;
        leftColorCount = 0;
    }

    public bool UnuseColor(ScrewColor color)
    {
        if (!colorInfos.TryGetValue(color, out var info)) return false;
        return info.Unuse();
    }

    private delegate bool ColorInfoFilter(ColorInfo info);

    private ScrewColor InnerGetNextColor(ColorInfoFilter filter)
    {
        colorCandidates.Clear();
        foreach (var pair in colorInfos)
        {
            if (filter(pair.Value))
            {
                colorCandidates.Add(pair.Key);
            }
        }

        if (colorCandidates.Count <= 0) return ScrewColor.None;

        var index = game.RandomNext(0, colorCandidates.Count);
        var color = colorCandidates[index];
        return color;
    }

    public ScrewColor UseColor()
    {
        if (leftColorCount <= 0)
        {
            return ScrewColor.None;
        }

        ScrewColor color = ScrewColor.None;
        // 1st choice is unused colors
        color = InnerGetNextColor(info => info.CanUse && !info.IsUsed);
        if (color == ScrewColor.None)
        {
            // 2nd choice is used colors
            color = InnerGetNextColor(info => info.CanUse);
        }

        if (color == ScrewColor.None)
        {
            // no color can be used
            Debug.LogError($"no color can be used");
            return ScrewColor.None;
        }

        colorInfos[color].Use();
        leftColorCount--;
        return color;
    }

    public void AddColor(ScrewColor color)
    {
        if (colorInfos.TryGetValue(color, out var info))
        {
            info.Add();
        }
        else
        {
            colorInfos[color] = new ColorInfo(color);
        }

        for (int i = 0; i < Consts.SlotPerBag; i++)
        {
            jointColors.Add(color);
        }

        leftColorCount++;
    }

    public void ShuffleJointColors()
    {
        // Fisher–Yates shuffle
        for (int i = jointColors.Count - 1; i > 0; i--)
        {
            var j = game.RandomNext(0, i + 1);
            (jointColors[i], jointColors[j]) = (jointColors[j], jointColors[i]);
        }
    }


    public ScrewColor GetNextJointColor()
    {
        if (nextJointColorIndex >= jointColors.Count)
        {
            Debug.LogError($"joint color index out of range");
            return ScrewColor.None;
        }

        return jointColors[nextJointColorIndex++];
    }
}