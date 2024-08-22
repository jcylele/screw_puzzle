using System;
using System.Collections.Generic;
using Item;
using UnityEngine;

namespace Stage
{
    [Serializable]
    public class ItemPosInfo
    {
        public string itemName;
        public TransInfo transInfo;
    }

    [CreateAssetMenu(fileName = "StageInfo", menuName = "Screw/StageInfo", order = 2)]
    public class StageInfo : ScriptableObject
    {
        public List<ItemPosInfo> itemPosInfos = new List<ItemPosInfo>();
    }
}