using Item;
using UnityEngine;

namespace Layer
{
    public class LayerPlayBehaviour : BaseLayerBehaviour
    {
        private void Start()
        {
            ExpandLayer();
        }

        public override void ExpandLayer()
        {
            // set layer position
            this.RefreshLayerPosition();

            // instantiate items
            var itemPrefab = LoadComponent<ItemPlayBehaviour>(Consts.ItemPlay);
            foreach (var itemPosInfo in layerInfo.itemPosInfos)
            {
                var item = Instantiate(itemPrefab, transform);
                item.SetTransInfo(itemPosInfo.transInfo, true);

                var itemInfo = Resources.Load<ItemInfo>($"{Consts.ItemInfoRootPath}/{itemPosInfo.itemName}");
                item.itemInfo = itemInfo;
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
    }
}