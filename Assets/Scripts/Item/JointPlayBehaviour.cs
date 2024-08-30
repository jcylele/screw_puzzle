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
            props.SetColor(ColorID, Game.Instance.GetScrewColor(sc));
            screwRenderer.SetPropertyBlock(props);
        }

        public Vector2 WorldPosition => transform.position;

        public ScrewColor UnScrew()
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