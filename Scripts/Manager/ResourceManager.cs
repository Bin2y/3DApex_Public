using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public Dictionary<string, object> AssetPools = new Dictionary<string, object>();

    protected override void Awake()
    {
        _isDontDestroyOnLoad = true;
        base.Awake();
    }
    /// <summary>
    /// 로드 할 때 캐싱된 리소스가 있으면 리턴하고 없으면 로드한다
    /// </summary>
    public T Load<T>(string path) where T : Object
    {

        if (AssetPools.ContainsKey(path))
        {
            return AssetPools[path] as T;
        }
        else
        {
            T asset = Resources.Load<T>(path);
            if (asset == null)
            {
                throw new System.NullReferenceException();
            }

            AssetPools.Add(path, asset);
            return asset;
        }
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            throw new System.NullReferenceException();
        }
        return Instantiate(prefab, parent);
    }
}