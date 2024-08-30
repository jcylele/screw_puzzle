using Layer;
using UnityEngine;

namespace Item
{
    public class ItemEditContainer : BaseBehaviour
    {
        public string itemName;

        public void LoadItem(BaseLayerBehaviour layerBehaviour)
        {
            var itemEditPrefab = LoadComponent<ItemEditBehaviour>(Consts.ItemEdit);

            var item = Instantiate(itemEditPrefab, transform);
            item.itemInfo = Resources.Load<ItemInfo>($"{Consts.ItemInfoRootPath}/{itemName}");
            item.ExpandItem();

            item.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
    }
}