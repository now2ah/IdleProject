using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    ObjectPool _pool;
    Collider _collider;

    public ObjectPool Pool { get { return _pool; } set { _pool = value; } }

    public float SpawnTime;
    public GameObject MonsterPrefab;

    void _OnGameStart(object o, EventArgs e)
    {
        StartCoroutine(SpawnMonsterCoroutine());
    }

    void _SpawnMonster()
    {
        Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(_collider.bounds.min.x, _collider.bounds.max.x),
            UnityEngine.Random.Range(_collider.bounds.min.y, _collider.bounds.max.y),
            UnityEngine.Random.Range(_collider.bounds.min.z, _collider.bounds.max.z));

        _pool.GetGameObject((obj) => 
        {
            obj.transform.position = spawnPos;
            obj.SetActive(true);
        });
    }

    IEnumerator SpawnMonsterCoroutine()
    {
        while(GameManager.Instance.IsRunning)
        {
            _SpawnMonster();
            yield return new WaitForSeconds(SpawnTime);
        }
    }

    private void Awake()
    {
        //_monsterList = new List<Monster>();
        _collider = GetComponent<Collider>();
        SpawnTime = 1f;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.OnGameStart += _OnGameStart;
        GameManager.Instance.Spawner = this;
        PoolManager.Instance.AddObjectPool("Monster");

        List<GameObject> monsterList = new List<GameObject>();
        int maxMonsterNumber = 100;
        for (int i=0; i<maxMonsterNumber; i++)
        {
            monsterList.Add(Instantiate(MonsterPrefab));
        }

        _pool = PoolManager.Instance.GetPoolObject("Monster") as ObjectPool;
        _pool.InitializePool(monsterList);
    }
}
