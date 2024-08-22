using UnityEngine;

namespace Layer
{
    public abstract class BaseLayerBehaviour : BaseBehaviour
    {
        public LayerInfo layerInfo = null;

        /// <summary>
        /// set layer position
        /// </summary>
        public void RefreshLayerPosition()
        {
            this.ResetTransform();
            this.transform.position = new Vector3(0f, 0f, Consts.LayerZOffset * layerInfo.layerIndex);
        }

        public void Rename()
        {
            this.gameObject.name = layerInfo.ItemName;
        }

        public abstract void ExpandLayer();
        public abstract void ClearLayer();
    }
}