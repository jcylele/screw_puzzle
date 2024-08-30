using System.Collections.Generic;
using Item;
using Layer;
using UnityEngine;

namespace Stage
{
    public class StagePlayBehaviour : BaseStageBehaviour
    {
        public static StagePlayBehaviour Instance { get; private set; }

        private readonly List<LayerPlayBehaviour> layers = new List<LayerPlayBehaviour>(Consts.MaxLayerCount);

        private Camera mainCamera;

        private void Awake()
        {
            Instance = this;
            mainCamera = Camera.main;
        }

        public void ExpandStage()
        {
            var layerPrefab = LoadComponent<LayerPlayBehaviour>(Consts.LayerPlay);
            foreach (var layerInfo in stageInfo.layerInfos)
            {
                var layer = Instantiate(layerPrefab, transform);
                layer.SetLayerInfo(layerInfo);
                layer.BelongStageBehaviour = this;
                layer.ExpandLayer();

                layers.Add(layer);
            }
        }

        public void OnLayerComplete(LayerPlayBehaviour layer)
        {
            layers.Remove(layer);
            if (layers.Count == 0)
            {
                Game.Instance.OnGameEnd(true);
            }
        }

        #region Raycast Related

        /// <summary>
        /// point raycast, so 1 is enough
        /// </summary>
        private readonly RaycastHit2D[] raycastHits = new RaycastHit2D[1];

        /// <summary>
        /// get item by raycast
        /// </summary>
        /// <returns></returns>
        private ItemPlayBehaviour RaycastItem()
        {
            var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            foreach (var layer in layers)
            {
                var layerMask = Game.Instance.GetLayerValue(layer.LayerInfo.layerIndex, true);
                var hitCount = Physics2D.RaycastNonAlloc(mousePos, Vector2.zero, raycastHits, 10, layerMask);
                if (hitCount == 0) continue;

                var item = raycastHits[0].collider.GetComponentInParent<ItemPlayBehaviour>();
                if (item != null) return item;

                Debug.LogError($"ItemPlayBehaviour is null, {raycastHits[0].collider}");
            }

            return null;
        }

        /// <summary>
        /// circle cast, but only to check exist or not, so 1 is enough
        /// </summary>
        private readonly RaycastHit2D[] circleCastHits = new RaycastHit2D[1];

        /// <summary>
        /// use circle to check if any item is covering the pos of target layer
        /// </summary>
        /// <returns>yes if not covered</returns>
        private bool CircleCastCheck(int targetLayerIndex, Vector2 pos)
        {
            foreach (var layer in layers)
            {
                var layerIndex = layer.LayerInfo.layerIndex;
                if (layerIndex == targetLayerIndex)
                {
                    return true;
                }

                var layerMask = Game.Instance.GetLayerValue(layerIndex, true);
                var hitCount = Physics2D.CircleCastNonAlloc(pos, Consts.JointCollisionRadius,
                    Vector2.zero, circleCastHits, 10, layerMask);
                if (hitCount > 0) return false;
            }

            Debug.LogError($"WTF, no layer found, targetLayerIndex: {targetLayerIndex}");
            return false;
        }

        /// <summary>
        /// raycast check if any joint is hit and not covered by other items
        /// </summary>
        /// <returns></returns>
        public JointPlayBehaviour RaycastJoint()
        {
            var item = RaycastItem();
            if (item == null) return null;

            // Debug.Log(item);
            var jointPlay = item.GetJointByHit(raycastHits[0].point);
            if (jointPlay == null) return null;

            // check if the joint is covered by other items
            return CircleCastCheck(item.LayerIndex, jointPlay.WorldPosition) ? jointPlay : null;
        }

        #endregion
    }
}