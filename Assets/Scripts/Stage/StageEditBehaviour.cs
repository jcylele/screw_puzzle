using System.Collections.Generic;
using Item;
using UnityEngine;

namespace Stage
{
    public class StageEditBehaviour : BaseStageBehaviour
    {
        private void Awake()
        {
            Debug.LogError("StageEditBehaviour should not run in play mode");
            this.gameObject.SetActive(false);
        }

        public override void ExpandStage()
        {
            if (stageInfo == null)
            {
                Debug.LogError("stageInfo is null");
                return;
            }

            var containers = GetComponentsInChildren<ItemEditContainer>(true);
            if (containers.Length > 0)
            {
                Debug.LogError("stage already expanded, please clear the stage first");
                return;
            }

            var itemEditContainer = LoadComponent<ItemEditContainer>(Consts.ItemEditContainer);
            foreach (var itemPosInfo in stageInfo.itemPosInfos)
            {
                var container = Instantiate(itemEditContainer, transform);
                container.gameObject.name = itemPosInfo.itemName;
                container.itemName = itemPosInfo.itemName;
                container.LoadItem();
                SetTransInfo(itemPosInfo.transInfo, true, container.transform);
            }
        }

        public override void ClearStage()
        {
            var containers = GetComponentsInChildren<ItemEditContainer>(true);
            foreach (var container in containers)
            {
                DestroyImmediate(container.gameObject);
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

            var containers = GetComponentsInChildren<ItemEditContainer>(false);
            if (containers.Length == 0)
            {
                Debug.LogError("no item found in the stage, nothing to serialize");
                return;
            }

            this.ResetTransform();

            stageInfo.itemPosInfos = new List<ItemPosInfo>(containers.Length);
            foreach (var container in containers)
            {
                var itemPosInfo = new ItemPosInfo
                {
                    itemName = container.itemName,
                    transInfo = GetTransInfo(true, container.transform)
                };
                stageInfo.itemPosInfos.Add(itemPosInfo);
            }

            UnityEditor.EditorUtility.SetDirty(stageInfo);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}