using UnityEngine;

namespace Item
{
    public class JointPlayBehaviour : BaseBehaviour
    {
        [SerializeField]
        private ScrewColor screwColor;

        public ScrewColor ScrewColor
        {
            get => screwColor;
            set => SetScrewColor(value);
        }

        public HingeJoint2D joint;

        [SerializeField]
        private MeshRenderer screwRenderer;

        [Tooltip("Pink,Red,Orange,Brown,Yellow,Green,Blue,Cyan,Purple,Gray")]
        public Color[] screwColors =
        {
            Color.magenta,
            Color.red,
            new Color(1, 0.5f, 0),
            new Color(0.6f, 0.3f, 0),
            Color.yellow,
            Color.green,
            Color.blue,
            Color.cyan,
            new Color(0.5f, 0, 0.5f),
            Color.gray
        };

        private MaterialPropertyBlock props;
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        private void SetScrewColor(ScrewColor sc)
        {
            screwColor = sc;
            screwRenderer.gameObject.SetActive(sc != ScrewColor.None);
            if (sc == ScrewColor.None)
            {
                screwRenderer.gameObject.SetActive(false);
                return;
            }

            screwRenderer.gameObject.SetActive(true);

            props ??= new MaterialPropertyBlock();
            props.SetColor(ColorID, screwColors[(int)sc - 1]);
            screwRenderer.SetPropertyBlock(props);
        }

        public Vector2 WorldPosition => transform.position;

        public ScrewColor OnClick()
        {
            if (screwColor == ScrewColor.None)
            {
                Debug.LogError("ScrewColor is None");
                return ScrewColor.None;
            }

            var ret = screwColor;

            SetScrewColor(ScrewColor.None);
            Destroy(joint);
            joint = null;

            return ret;
        }
    }
}