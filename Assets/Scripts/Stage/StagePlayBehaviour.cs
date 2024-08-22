using Item;
using UnityEngine;

namespace Stage
{
    public class StagePlayBehaviour : BaseStageBehaviour
    {
        private readonly RaycastHit2D[] rayHits = new RaycastHit2D[1];
        private readonly RaycastHit2D[] circleHits = new RaycastHit2D[1];
        
        public override void ExpandStage()
        {
            var itemPrefab = LoadComponent<ItemPlayBehaviour>(Consts.ItemPlay);
            foreach (var itemPosInfo in stageInfo.itemPosInfos)
            {
                var item = Instantiate(itemPrefab, transform);
                item.SetTransInfo(itemPosInfo.transInfo, true);

                var itemInfo = Resources.Load<ItemInfo>($"{Consts.ItemInfoRootPath}/{itemPosInfo.itemName}");
                item.itemInfo = itemInfo;
            }
        }

        public override void ClearStage()
        {
            foreach (var item in GetComponentsInChildren<ItemPlayBehaviour>())
            {
                Destroy(item.gameObject);
            }
        }

        private void Start()
        {
            ExpandStage();
        }

        private void Update()
        {
            // left mouse button down
            if (Input.GetMouseButtonDown(0))
            {
                OnClick();
            }
        }

        private void OnClick()
        {
            // Debug.Log("Mouse0 Down");
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var rayHitCount = Physics2D.RaycastNonAlloc(mousePos, Vector2.zero, rayHits);
            if (rayHitCount == 0) return;
            
            // Debug.Log(hit.collider);
            var item = rayHits[0].collider.GetComponentInParent<ItemPlayBehaviour>();
            if (item == null) return;
            
            // Debug.Log(item);
            var joint = item.GetJointByHit(rayHits[0].point);
            if (joint == null) return;
            
            // check if the joint is covered by other items
            var circleHitCount = Physics2D.CircleCastNonAlloc(joint.connectedAnchor, Consts.JointCollisionRadius, Vector2.zero, circleHits);
            if (circleHitCount == 0)
            {
                Debug.LogError("WTF, circleHitCount == 0");
                return;
            }
            
            var item2 = circleHits[0].collider.GetComponentInParent<ItemPlayBehaviour>();
            if (item2 != item) return;
            
            item.OnJointClick(joint);
        }
    }
}