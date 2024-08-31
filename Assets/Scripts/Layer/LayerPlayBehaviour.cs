using System.Collections.Generic;
using Item;
using UnityEngine;

namespace Layer
{
    public class LayerPlayBehaviour : BaseLayerBehaviour
    {
        private readonly List<ItemPlayBehaviour> items = new List<ItemPlayBehaviour>();

        public void ExpandLayer()
        {
            if (LayerInfo == null)
            {
                Debug.LogError("layerInfo is null");
                return;
            }

            var playBehaviours = GetComponentsInChildren<ItemPlayBehaviour>(true);
            if (playBehaviours.Length > 0)
            {
                Debug.LogError("layer already expanded, please clear the layer first");
                return;
            }

            // set layer position
            this.RefreshLayerPosition();


            var layerColor = BelongStageBehaviour.GetLayerColor(LayerInfo.layerIndex);
            // instantiate items, prefab load only once, no need to cache
            var itemPrefab = LoadComponent<ItemPlayBehaviour>(Consts.ItemPlay);
            foreach (var itemPosInfo in LayerInfo.itemPosInfos)
            {
                var item = Instantiate(itemPrefab, transform);
                item.BelongLayerBehaviour = this;
                item.SetTransInfo(itemPosInfo.transInfo, true);
                item.Initialize(itemPosInfo.itemName);
                item.SetColor(layerColor);
                item.ExpandItem();

                items.Add(item);
            }
        }

        public void OnItemFallToGround(BaseItemBehaviour itemBehaviour)
        {
            items.Remove(itemBehaviour as ItemPlayBehaviour);
            if (items.Count > 0) return;
            this.BelongStageBehaviour.OnLayerComplete(this);
            Destroy(gameObject);
        }
    }
}