using System.Collections.Generic;
using Item;
using UnityEngine;
using Object = UnityEngine.Object;

public class BaseBehaviour : MonoBehaviour
{
    private readonly Dictionary<string, Object> prefabDict = new Dictionary<string, Object>();

    private void OnDestroy()
    {
        prefabDict.Clear();
    }

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

    protected Object LoadObject(string prefabPath, bool cache = true)
    {
        if (prefabDict.TryGetValue(prefabPath, out var prefab))
        {
            return prefab;
        }

        prefab = Resources.Load(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("LoadObject failed: " + prefabPath);
            return null;
        }

        if (cache)
        {
            prefabDict.Add(prefabPath, prefab);
        }

        return prefab;
    }

    protected T LoadComponent<T>(string prefabPath, bool cache = true) where T : Component
    {
        var prefab = LoadObject(prefabPath, cache) as GameObject;
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