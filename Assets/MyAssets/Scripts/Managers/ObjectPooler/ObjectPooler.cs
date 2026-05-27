using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [Serializable]
    public class PoolEntry
    {
        public GameObject prefab;
        public int initialSize = 10;
        public bool canExpand = true;
    }

    [SerializeField] private PoolEntry[] pools;

    private readonly Dictionary<GameObject, Queue<GameObject>> poolDictionary = new();
    private readonly Dictionary<GameObject, PoolEntry> poolConfigDictionary = new();
    private readonly List<GameObject> registeredPrefabs = new();

    private void Awake()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary.Clear();
        poolConfigDictionary.Clear();
        registeredPrefabs.Clear();

        if (pools == null || pools.Length == 0)
        {
            Debug.LogWarning("[ObjectPooler] No Pools defined in the Object Pooler.");
            return;
        }

        foreach (PoolEntry entry in pools)
        {
            if (entry == null || entry.prefab == null) continue;

            if (poolDictionary.ContainsKey(entry.prefab))
            {
                Debug.LogWarning($"[ObjectPooler] Duplicate prefab '{entry.prefab.name}' found in pool configuration. Skipping duplicate entry.");
                continue;
            }

            Queue<GameObject> queue = new();
            poolDictionary[entry.prefab] = queue;
            poolConfigDictionary[entry.prefab] = entry;
            registeredPrefabs.Add(entry.prefab);

            for (int i = 0; i < entry.initialSize; i++)
            {
                GameObject obj = CreateNewInstance(entry.prefab);
                ReturnToPoolInternal(entry.prefab, obj);
            }
        }

        Debug.Log($"[ObjectPooler:{name}] Registered {registeredPrefabs.Count} prefabs.");
    }

    #region Public API

    public IReadOnlyList<GameObject> GetRegisteredPrefabs()
    {
        return registeredPrefabs;
    }

    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[ObjectPooler] Tried to get NULL Prefab from Pool");
            return null;
        }

        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning($"[ObjectPooler] No Pool exists for Prefab: {prefab.name}");
            return null;
        }

        Queue<GameObject> queue = poolDictionary[prefab];
        GameObject obj = null;

        while (queue.Count > 0 && obj == null)
            obj = queue.Dequeue();

        if (obj == null)
        {
            PoolEntry config = poolConfigDictionary[prefab];

            if (!config.canExpand)
            {
                Debug.LogWarning($"[ObjectPooler] Pool Exhausted and cannot expand for: {prefab.name}");
                return null;
            }

            obj = CreateNewInstance(prefab);
        }

        Transform t = obj.transform;
        t.SetParent(null);
        t.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;

        PooledObject pooledObject = obj.GetComponent<PooledObject>();
        if (pooledObject == null || pooledObject.SourcePrefab == null)
        {
            Debug.LogWarning($"[ObjectPooler] Object {obj.name} has no valid PooledObject Source Prefab. Destroying it.");
            Destroy(obj);
            return;
        }

        ReturnToPoolInternal(pooledObject.SourcePrefab, obj);
    }

    #endregion

    #region Object Pooler Helpers

    private void ReturnToPoolInternal(GameObject prefab, GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);

        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning($"[ObjectPooler] No Pool found for Prefab {prefab.name}. Destroying Returned Object.");
            Destroy(obj);
            return;
        }

        poolDictionary[prefab].Enqueue(obj);
    }

    private GameObject CreateNewInstance(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);

        PooledObject pooledObject = obj.GetComponent<PooledObject>();
        if (pooledObject == null)
            pooledObject = obj.AddComponent<PooledObject>();

        pooledObject.SetSourcePrefab(prefab);
        return obj;
    }

    #endregion
}