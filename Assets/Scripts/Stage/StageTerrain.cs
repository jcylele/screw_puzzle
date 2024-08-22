using Item;
using UnityEngine;

namespace Stage
{
    public class StageTerrain : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D col)
        {
            var item = col.collider.GetComponentInParent<ItemPlayBehaviour>();
            if (item == null) return;
            item.OnFallToGround();
        }
    }
}