using System.Collections.Generic;
using System.Linq.Expressions;
using Item;
using Stage;
using UnityEngine;

namespace Layer
{
    public class LayerEditBehaviour : BaseLayerBehaviour
    {
        private void Awake()
        {
            Debug.LogWarning("LayerEditBehaviour should not run in play mode");
            this.gameObject.SetActive(false);
        }

        public override void ExpandLayer()
        {
            if (layerInfo == null)
            {
                Debug.LogError("layerInfo is null");
                return;
            }

            var containers = GetComponentsInChildren<ItemEditContainer>(true);
            if (containers.Length > 0)
            {
                Debug.LogError("layer already expanded, please clear the layer first");
                return;
            }

            this.RefreshLayerPosition();

            var itemEditContainer = LoadComponent<ItemEditContainer>(Consts.ItemEditContainer);
            foreach (var itemPosInfo in layerInfo.itemPosInfos)
            {
                var container = Instantiate(itemEditContainer, transform);
                container.gameObject.name = itemPosInfo.itemName;
                container.itemName = itemPosInfo.itemName;
                container.LoadItem();
                SetTransInfo(itemPosInfo.transInfo, true, container.transform);
            }
        }

        public override void ClearLayer()
        {
            this.RefreshLayerPosition();

            var containers = GetComponentsInChildren<ItemEditContainer>(true);
            foreach (var container in containers)
            {
                DestroyImmediate(container.gameObject);
            }
        }

#if UNITY_EDITOR
        public void SerializeLayer(bool saveAsset)
        {
            if (layerInfo == null)
            {
                Debug.LogError($"layerInfo of {gameObject.name} is null");
                return;
            }

            var items = GetComponentsInChildren<ItemEditContainer>(false);
            if (items.Length == 0)
            {
                Debug.LogError($"no item found in {gameObject.name}, nothing to serialize");
                return;
            }

            this.RefreshLayerPosition();

            layerInfo.itemPosInfos = new List<ItemPosInfo>(items.Length);
            foreach (var item in items)
            {
                var itemPosInfo = new ItemPosInfo
                {
                    itemName = item.itemName,
                    transInfo = GetTransInfo(true, item.transform)
                };
                layerInfo.itemPosInfos.Add(itemPosInfo);
            }

            if (!saveAsset) return;

            var stage = GetComponentInParent<StageEditBehaviour>();
            var stageInfo = stage.stageInfo;

            var index = stageInfo.FindLayerIndex(layerInfo.uuid);
            if (index == -1)
            {
                stageInfo.layerInfos.Add(layerInfo);
            }
            else
            {
                stageInfo.layerInfos[index] = layerInfo;
            }

            SaveAsset(stageInfo);
        }

        private void SaveAsset(StageInfo stageInfo)
        {
            if (!stageInfo.CheckConflict())
            {
                return;
            }

            UnityEditor.EditorUtility.SetDirty(stageInfo);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        public void RemoveLayer()
        {
            var stage = GetComponentInParent<StageEditBehaviour>();
            stage.stageInfo.RemoveLayerByUuid(layerInfo.uuid);
            SaveAsset(stage.stageInfo);

            DestroyImmediate(gameObject);
        }
#endif
    }
}