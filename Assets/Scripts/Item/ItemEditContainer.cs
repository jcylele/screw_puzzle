using Layer;
using UnityEngine;

namespace Item
{
    public class ItemEditContainer : BaseBehaviour
    {
        public string itemName;
        private ItemEditBehaviour itemEditBehaviour;

        public void LoadItem(BaseLayerBehaviour layerBehaviour)
        {
            var itemEditPrefab = LoadComponent<ItemEditBehaviour>(Consts.ItemEdit);

            itemEditBehaviour = Instantiate(itemEditPrefab, transform);
            itemEditBehaviour.Initialize(itemName);
            itemEditBehaviour.SetColor(layerBehaviour.GetLayerColor());
            itemEditBehaviour.ExpandItem();

            itemEditBehaviour.gameObject.hideFlags = HideFlags.HideInHierarchy;

            this.gameObject.name = $"{itemName} Container";
        }
    }
}