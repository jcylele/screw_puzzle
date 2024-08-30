using UnityEngine;

namespace Layer
{
    public abstract class BaseLayerBehaviour : BaseBehaviour
    {
        public LayerInfo LayerInfo { get; private set; }

        /// <summary>
        /// child item count, only used in play mode
        /// </summary>
        protected int ItemCount { get; set; }

        public void SetLayerInfo(LayerInfo layerInfo)
        {
            this.LayerInfo = layerInfo;
            this.Rename();
        }

        /// <summary>
        /// set layer position
        /// </summary>
        public void RefreshLayerPosition()
        {
            this.ResetTransform();
            this.transform.position = new Vector3(0f, 0f, Consts.LayerZOffset * LayerInfo.layerIndex);
        }

        public void Rename()
        {
            this.gameObject.name = LayerInfo.ItemName;
        }
    }
}