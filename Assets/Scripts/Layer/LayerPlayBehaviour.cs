using Item;
using UnityEngine;

namespace Layer
{
    public class LayerPlayBehaviour : BaseLayerBehaviour
    {
        public override void ExpandLayer()
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

        public override void ClearLayer()
        {
            // set layer position
            this.RefreshLayerPosition();

            foreach (var item in GetComponentsInChildren<ItemPlayBehaviour>())
            {
                Destroy(item.gameObject);
            }
        }

        public override void OnItemFallToGround(BaseItemBehaviour itemBehaviour)
        {
            this.ItemCount--;
            if (this.ItemCount == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}