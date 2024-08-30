using Layer;
using UnityEngine;

namespace Stage
{
    public class StageEditBehaviour : BaseStageBehaviour
    {
        private void Awake()
        {
            // Debug.LogError("StageEditBehaviour should not run in play mode");
            this.gameObject.SetActive(false);
        }

        public void ExpandStage()
        {
            if (stageInfo == null)
            {
                Debug.LogError("stageInfo is null");
                return;
            }

            var layers = GetComponentsInChildren<LayerEditBehaviour>(true);
            if (layers.Length > 0)
            {
                Debug.LogError("stage already expanded, please clear the stage first");
                return;
            }

            var layerEditPrefab = LoadComponent<LayerEditBehaviour>(Consts.LayerEdit);
            foreach (var layerInfo in stageInfo.layerInfos)
            {
                AddLayer(layerInfo, layerEditPrefab);
            }
        }

        public LayerEditBehaviour AddLayer(LayerInfo layerInfo, LayerEditBehaviour layerEditPrefab = null)
        {
            if (layerEditPrefab == null)
            {
                layerEditPrefab = LoadComponent<LayerEditBehaviour>(Consts.LayerEdit);
            }

            var layer = Instantiate(layerEditPrefab, transform);
            layer.SetLayerInfo(layerInfo);
            layer.ExpandLayer();

            return layer;
        }

        public void ClearStage()
        {
            var layers = GetComponentsInChildren<LayerEditBehaviour>(true);
            foreach (var layer in layers)
            {
                DestroyImmediate(layer.gameObject);
            }
        }

#if UNITY_EDITOR
        public void SerializeStage()
        {
            if (stageInfo == null)
            {
                Debug.LogError("stageInfo is null");
                return;
            }

            var layers = GetComponentsInChildren<LayerEditBehaviour>(false);
            if (layers.Length == 0)
            {
                Debug.LogError("no layer found in the stage, nothing to serialize");
                return;
            }

            this.ResetTransform();

            foreach (var layer in layers)
            {
                layer.SerializeLayer(false);
            }

            stageInfo.layerInfos.Sort((a, b) => a.layerIndex.CompareTo(b.layerIndex));

            SaveAsset();
        }

        private void SaveAsset()
        {
            UnityEditor.EditorUtility.SetDirty(stageInfo);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}