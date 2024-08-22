using System;
using System.Collections.Generic;
using Item;

namespace Layer
{
    [Serializable]
    public class ItemPosInfo
    {
        public string itemName;
        public TransInfo transInfo;
    }

    [Serializable]
    public class LayerInfo
    {
        public string ItemName => $"layer {layerIndex} ({uuid})";
        public int layerIndex;
        public int uuid;
        public List<ItemPosInfo> itemPosInfos = new List<ItemPosInfo>();

        public override string ToString()
        {
            return $"{ItemName} - {itemPosInfos.Count} items";
        }
    }
}