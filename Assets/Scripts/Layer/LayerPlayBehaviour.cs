using Item;
using Stage;
using UnityEngine;

namespace Layer
{
    public class LayerPlayBehaviour : BaseLayerBehaviour
    {
        public StagePlayBehaviour BelongStageBehaviour { get; set; }

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

            this.ItemCount = 0;

            // instantiate items, prefab load only once, no need to cache
            var itemPrefab = LoadComponent<ItemPlayBehaviour>(Consts.ItemPlay);
            foreach (var itemPosInfo in LayerInfo.itemPosInfos)
            {
                var item = Instantiate(itemPrefab, transform);
                item.BelongLayerBehaviour = this;
                this.ItemCount++;
                item.SetTransInfo(itemPosInfo.transInfo, true);

                var itemInfo = Resources.Load<ItemInfo>($"{Consts.ItemInfoRootPath}/{itemPosInfo.itemName}");
                item.itemInfo = itemInfo;

                item.ExpandItem();
            }
        }

        public void OnItemFallToGround(BaseItemBehaviour itemBehaviour)
        {
            this.ItemCount--;
            if (this.ItemCount != 0) return;
            this.BelongStageBehaviour.OnLayerComplete(this);
            Destroy(gameObject);
        }
    }
}