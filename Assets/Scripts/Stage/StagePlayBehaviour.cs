using Layer;

namespace Stage
{
    public class StagePlayBehaviour : BaseStageBehaviour
    {
        public override void ExpandStage()
        {
            var layerPrefab = LoadComponent<LayerPlayBehaviour>(Consts.LayerPlay);
            foreach (var layerInfo in stageInfo.layerInfos)
            {
                var layer = Instantiate(layerPrefab, transform);
                layer.layerInfo = layerInfo;
                layer.ExpandLayer();
            }
        }

        public override void ClearStage()
        {
            var layers = GetComponentsInChildren<LayerPlayBehaviour>(true);
            foreach (var layer in layers)
            {
                DestroyImmediate(layer.gameObject);
            }
        }

        private void Start()
        {
            ExpandStage();
        }
    }
}