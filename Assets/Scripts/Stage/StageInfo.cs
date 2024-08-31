using System.Collections.Generic;
using Item;
using Layer;
using UnityEngine;

namespace Stage
{
    [CreateAssetMenu(fileName = "StageInfo", menuName = "Screw/StageInfo", order = 2)]
    public class StageInfo : ScriptableObject
    {
        [ReadOnly]
        public int jointCount;

        public List<LayerInfo> layerInfos = new List<LayerInfo>();

        public void RemoveLayer(string layerName)
        {
            for (int i = 0; i < layerInfos.Count; i++)
            {
                if (layerInfos[i].layerName != layerName) continue;
                layerInfos.RemoveAt(i);
                return;
            }
        }

        public int FindLayerIndex(string layerName)
        {
            for (int i = 0; i < layerInfos.Count; i++)
            {
                if (layerInfos[i].layerName == layerName)
                {
                    return i;
                }
            }

            return -1;
        }

#if UNITY_EDITOR

        private bool CheckDuplicate()
        {
            var indices = new HashSet<int>();
            var names = new HashSet<string>();
            foreach (var info in layerInfos)
            {
                if (!indices.Add(info.layerIndex))
                {
                    Debug.LogError($"Duplicate layer index {info.layerIndex}");
                    return false;
                }

                if (!names.Add(info.layerName))
                {
                    Debug.LogError($"Duplicate uuid {info.layerName}");
                    return false;
                }
            }

            return true;
        }

        private void CalcJointCount()
        {
            var itemNameCount = new Dictionary<string, int>();
            foreach (var layerInfo in layerInfos)
            {
                foreach (var itemPosInfo in layerInfo.itemPosInfos)
                {
                    if (!itemNameCount.ContainsKey(itemPosInfo.itemName))
                    {
                        itemNameCount[itemPosInfo.itemName] = 0;
                    }

                    itemNameCount[itemPosInfo.itemName]++;
                }
            }

            jointCount = 0;
            foreach (var pair in itemNameCount)
            {
                var itemInfo = Resources.Load<ItemInfo>($"{Consts.ItemInfoRootPath}/{pair.Key}");
                if (itemInfo == null)
                {
                    Debug.LogError($"ItemInfo not found: {pair.Key}");
                }

                jointCount += pair.Value * itemInfo.GetTotalJointCount();
            }

            if (jointCount % Consts.SlotPerBag != 0)
            {
                Debug.LogError($"Joint count {jointCount} is not a multiple of {Consts.SlotPerBag}");
            }
        }

        public void SaveAsset()
        {
            CheckDuplicate();
            CalcJointCount();

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}