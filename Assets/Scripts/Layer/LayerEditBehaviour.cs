using System.Collections.Generic;
using Item;
using Stage;
using UnityEngine;

namespace Layer
{
    public class LayerEditBehaviour : BaseLayerBehaviour
    {
        private void Awake()
        {
            // Debug.LogWarning("LayerEditBehaviour should not run in play mode");
            this.gameObject.SetActive(false);
        }

        public void ExpandLayer()
        {
            if (LayerInfo == null)
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
            foreach (var itemPosInfo in LayerInfo.itemPosInfos)
            {
                var container = Instantiate(itemEditContainer, transform);
                container.gameObject.name = itemPosInfo.itemName;
                container.itemName = itemPosInfo.itemName;
                container.LoadItem(this);
                SetTransInfo(itemPosInfo.transInfo, true, container.transform);
            }
        }

        public void ClearLayer()
        {
            this.RefreshLayerPosition();

            var containers = GetComponentsInChildren<ItemEditContainer>(true);
            foreach (var container in containers)
            {
                DestroyImmediate(container.gameObject);
            }
        }

        public void RefreshLayerColor()
        {
            var layerColor = BelongStageBehaviour.GetLayerColor(LayerInfo.layerIndex);
            var itemEditBehaviours = GetComponentsInChildren<ItemEditBehaviour>(true);
            foreach (var itemEditBehaviour in itemEditBehaviours)
            {
                itemEditBehaviour.SetColor(layerColor);
            }
        }

#if UNITY_EDITOR
        public void SerializeLayer(bool saveAsset)
        {
            if (LayerInfo == null)
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

            LayerInfo.itemPosInfos = new List<ItemPosInfo>(items.Length);
            foreach (var item in items)
            {
                var itemPosInfo = new ItemPosInfo
                {
                    itemName = item.itemName,
                    transInfo = GetTransInfo(true, item.transform)
                };
                // Debug.Log($"itemPosInfo: {itemPosInfo.itemName} {itemPosInfo.transInfo.position}");
                LayerInfo.itemPosInfos.Add(itemPosInfo);
            }

            var stage = GetComponentInParent<StageEditBehaviour>();
            var stageInfo = stage.stageInfo;

            var index = stageInfo.FindLayerIndex(LayerInfo.layerName);
            if (index == -1)
            {
                stageInfo.layerInfos.Add(LayerInfo);
                Debug.Log($"Add {LayerInfo.ItemName} to {stageInfo.name}");
            }
            else
            {
                stageInfo.layerInfos[index] = LayerInfo;
                Debug.Log($"Update {LayerInfo.ItemName} of {stageInfo.name}");
            }

            if (saveAsset)
            {
                stageInfo.SaveAsset();
            }
        }

        public void RemoveLayer()
        {
            var stage = GetComponentInParent<StageEditBehaviour>();
            stage.stageInfo.RemoveLayer(LayerInfo.layerName);
            stage.stageInfo.SaveAsset();

            DestroyImmediate(gameObject);
        }
#endif
    }
}