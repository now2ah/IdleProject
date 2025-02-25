using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public interface IPool
{
    Transform Parent { get; set; }

    Queue<GameObject> Pool { get; set; }

    GameObject GetGameObject(Action<GameObject> action = null);
    void ReturnObject(GameObject gameObject, Func<GameObject> action = null);
}

public class ObjectPool : IPool
{
    Transform _parent;
    Queue<GameObject> _poolQ;
    public Transform Parent { get { return _parent; } set { _parent = value; } }
    public Queue<GameObject> Pool { get { return _poolQ; } set { _poolQ = value; } }

    public void InitializePool(List<GameObject> objList)
    {
        _poolQ = new Queue<GameObject>();
        foreach (var obj in objList)
            ReturnObject(obj);
    }

    public GameObject GetGameObject(Action<GameObject> action = null)
    {
        GameObject obj = _poolQ.Dequeue();
        obj.SetActive(true);

        if (action != null)
        {
            action?.Invoke(obj);
        }

        return null;
    }

    public void ReturnObject(GameObject obj, Func<GameObject> action = null)
    {
        _poolQ.Enqueue(obj);
        obj.transform.SetParent(_parent);
        obj.SetActive(false);

        if (action != null)
        {
            action?.Invoke();
        }
    }
}

public class PoolManager : MonoBehaviour
{
    static PoolManager _instance = null;

    public static PoolManager Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindFirstObjectByType<PoolManager>();
            }

            return _instance;
        }
    }

    public Dictionary<string, IPool> poolDic;

    public IPool GetPoolObject(string path)
    {
        if (!poolDic.ContainsKey(path))
        {
            AddObjectPool(path);
        }

        return poolDic[path];
    }

    public GameObject AddObjectPool(string path)
    {
        GameObject obj = new GameObject(path + "Pool");
        ObjectPool objPool = new ObjectPool();

        poolDic.Add(path, objPool);

        objPool.Parent = obj.transform;

        return obj;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        poolDic = new Dictionary<string, IPool>();
    }
}
