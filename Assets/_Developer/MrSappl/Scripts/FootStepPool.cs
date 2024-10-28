using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FootStepPool : NetworkBehaviour
{
    private ObjectPool<GameObject> _pool;
    [SerializeField] private int _startPoolCoint;
    [SerializeField] private int _maxPoolSize;
    [SerializeField] private GameObject _preab;

    private void Start()
    {
        _pool = new ObjectPool<GameObject>(
            createFunc: CreatePoolObjects,
            actionOnGet: ActionOnGet,
            actionOnRelease: ActionOnRelease,
            actionOnDestroy: ActionOnDestroy,
            collectionCheck: false,
            defaultCapacity: _startPoolCoint,
            maxSize: _maxPoolSize
            );
    }

    private GameObject CreatePoolObjects()
    {
        GameObject pref = GetObjectFromNetworkManager(_preab);
        GameObject objectForPool = Instantiate(pref);
        NetworkServer.Spawn(objectForPool);
        return objectForPool;
    }

    private void ActionOnGet(GameObject objectFromPool)
    {
        objectFromPool.SetActive(true);
    }

    private void ActionOnRelease(GameObject objectFromPool)
    {
        objectFromPool.SetActive(false);
    }

    private void ActionOnDestroy(GameObject objectFromPool)
    {
        Destroy(objectFromPool);
    }

    public GameObject GetObjectFromPool()
    {
        return _pool.Get();
    }
    public void ReturnObjectInPool(GameObject relesedObject)
    {
        _pool.Release(relesedObject);
    }
    private void OnDestroy()
    {
        _pool.Dispose();
    }

    private GameObject GetObjectFromNetworkManager(GameObject gameObject)
    {
        for (int i = 0; i < NetworkManager.singleton.spawnPrefabs.Count; i++)
        {
            if (NetworkManager.singleton.spawnPrefabs[i] == gameObject)
            {
                return NetworkManager.singleton.spawnPrefabs[i];
            }
        }
        return null;
    }
}
