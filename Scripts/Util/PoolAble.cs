using System.CodeDom;
using UnityEngine;
using UnityEngine.Pool;

public class PoolAble : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }
    public IObjectPool<GameObject> DynamicPool { get; set; }

    public void ReleaseObject()
    {
        if (Pool != null)
        {
            Pool.Release(gameObject);
        }

    }

    public void ReleaseDynamicObject()
    {
        if (DynamicPool != null)
        {
            DynamicPool.Release(gameObject);
        }
    }
}
