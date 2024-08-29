using System.Collections.Generic;
using Item;
using UnityEngine;

public class BaseBehaviour : MonoBehaviour
{
    private readonly Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();

    protected void ResetTransform()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Set transform position and rotation by TransInfo
    /// </summary>
    /// <param name="transInfo">information</param>
    /// <param name="isLocal">true for local space, false for world space</param>
    /// <param name="trans">default as comp.transform</param>
    public void SetTransInfo(TransInfo transInfo, bool isLocal, Transform trans = null)
    {
        if (trans == null)
        {
            trans = transform;
        }

        if (isLocal)
        {
            trans.localPosition = transInfo.position;
            trans.localRotation = transInfo.rotation;
        }
        else
        {
            trans.position = transInfo.position;
            trans.rotation = transInfo.rotation;
        }
    }

    protected TransInfo GetTransInfo(bool isLocal, Transform trans = null)
    {
        if (trans == null)
        {
            trans = transform;
        }

        return new TransInfo
        {
            position = isLocal ? trans.localPosition : trans.position,
            rotation = isLocal ? trans.localRotation : trans.rotation
        };
    }

    private GameObject LoadPrefab(string prefabPath)
    {
        if (prefabDict.TryGetValue(prefabPath, out var prefab))
        {
            return prefab;
        }

        prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("Load prefab failed: " + prefabPath);
            return null;
        }

        prefabDict.Add(prefabPath, prefab);
        return prefab;
    }

    public T LoadComponent<T>(string prefabPath) where T : Component
    {
        var prefab = LoadPrefab(prefabPath);
        if (prefab == null)
        {
            return null;
        }

        var component = prefab.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError("Load component failed: " + prefabPath);
            return null;
        }

        return component;
    }
}