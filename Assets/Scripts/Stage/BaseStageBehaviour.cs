using Layer;
using UnityEngine;

namespace Stage
{
    public abstract class BaseStageBehaviour : BaseBehaviour
    {
        public StageInfo stageInfo;

        [Tooltip("Color for each layer, from top to bottom.")] [SerializeField]
        private Color[] layerColors = new Color[10]
        {
            Color.magenta,
            Color.green,
            Color.blue,
            Color.red,
            Color.cyan,
            new Color(1, 0.5f, 0),
            new Color(0.5f, 0, 0.5f),
            Color.yellow,
            Color.gray,
            new Color(0.6f, 0.3f, 0),
        };

        [SerializeField]
        private float layerAlpha = 0.5f;

        public Color GetLayerColor(int layerIndex)
        {
            var color = layerColors[layerIndex - 1];
            color.a = layerAlpha;
            return color;
        }

        public abstract void OnLayerComplete(BaseLayerBehaviour layer);
    }
}