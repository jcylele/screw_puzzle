using System.Collections.Generic;
using Layer;
using UnityEngine;

namespace Stage
{
    [CreateAssetMenu(fileName = "StageInfo", menuName = "Screw/StageInfo", order = 2)]
    public class StageInfo : ScriptableObject
    {
        public List<LayerInfo> layerInfos = new List<LayerInfo>();

        public void RemoveLayerByUuid(int uuid)
        {
            for (int i = 0; i < layerInfos.Count; i++)
            {
                if (layerInfos[i].uuid != uuid) continue;
                layerInfos.RemoveAt(i);
                return;
            }
        }

        public int FindLayerIndex(int uuid)
        {
            for (int i = 0; i < layerInfos.Count; i++)
            {
                if (layerInfos[i].uuid == uuid)
                {
                    return i;
                }
            }

            return -1;
        }

        public bool CheckConflict()
        {
            HashSet<int> indices = new HashSet<int>();
            HashSet<int> uuids = new HashSet<int>();
            foreach (var info in layerInfos)
            {
                if (!indices.Add(info.layerIndex))
                {
                    Debug.LogError($"Duplicate layer index {info.layerIndex}");
                    return false;
                }

                if (!uuids.Add(info.uuid))
                {
                    Debug.LogError($"Duplicate uuid {info.uuid}");
                    return false;
                }
            }

            return true;
        }
    }
}