using System.Collections.Generic;
using Layer;
using UnityEngine;

namespace Stage
{
    [CreateAssetMenu(fileName = "StageInfo", menuName = "Screw/StageInfo", order = 2)]
    public class StageInfo : ScriptableObject
    {
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

        public bool CheckConflict()
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
    }
}