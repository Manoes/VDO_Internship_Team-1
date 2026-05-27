using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolRouter : Singleton<PoolRouter>
{
    [Serializable]
    public class PoolerRegistration
    {
        public string label;
        public ObjectPooler pooler;
    }

    [SerializeField] private PoolerRegistration[] poolers;

    private readonly Dictionary<GameObject, ObjectPooler> prefabToPooler = new();
    private bool registryBuilt;

    private void Start()
    {
        BuildRegistry();
    }

    [ContextMenu("Build Registry")]
    public void BuildRegistry()
    {
        prefabToPooler.Clear();
        registryBuilt = false;

        if (poolers == null || poolers.Length == 0)
        {
            Debug.LogWarning("[PoolRouter] No Poolers registered in the Pool Router.");
            return;
        }

        foreach (PoolerRegistration registration in poolers)
        {
            if (registration == null || registration.pooler == null)
                continue;

            IReadOnlyList<GameObject> registeredPrefabs = registration.pooler.GetRegisteredPrefabs();

            if (registeredPrefabs == null || registeredPrefabs.Count == 0)
            {
                Debug.LogWarning($"[PoolRouter] Pooler '{registration.label}' has no registered prefabs.");
                continue;
            }

            foreach (GameObject prefab in registeredPrefabs)
            {
                if (prefab == null)
                    continue;

                if (prefabToPooler.ContainsKey(prefab))
                {
                    Debug.LogWarning(
                        $"[PoolRouter] Prefab '{prefab.name}' is registered in multiple poolers. " +
                        $"Keeping first registration, ignoring duplicate in '{registration.label}'."
                    );
                    continue;
                }

                prefabToPooler[prefab] = registration.pooler;
            }
        }

        registryBuilt = true;
        Debug.Log($"[PoolRouter] Registry built with {prefabToPooler.Count} prefabs.");
    }

    private void EnsureRegistryBuilt()
    {
        if (!registryBuilt || prefabToPooler.Count == 0)
            BuildRegistry();
    }

    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[PoolRouter] Tried to get NULL Prefab from Pool.");
            return null;
        }

        EnsureRegistryBuilt();

        if (!prefabToPooler.TryGetValue(prefab, out ObjectPooler pooler) || pooler == null)
        {
            Debug.LogWarning($"[PoolRouter] No Pooler registered for Prefab: {prefab.name}");
            return null;
        }

        return pooler.GetFromPool(prefab, position, rotation);
    }

    public void ReturnToPool(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("[PoolRouter] Tried to return NULL Object to Pool.");
            return;
        }

        PooledObject pooledObject = obj.GetComponent<PooledObject>();
        if (pooledObject == null || pooledObject.SourcePrefab == null)
        {
            Debug.LogWarning($"[PoolRouter] Object '{obj.name}' has no valid PooledObject SourcePrefab. Destroying it.");
            Destroy(obj);
            return;
        }

        EnsureRegistryBuilt();

        GameObject sourcePrefab = pooledObject.SourcePrefab;

        if (!prefabToPooler.TryGetValue(sourcePrefab, out ObjectPooler pooler) || pooler == null)
        {
            Debug.LogWarning($"[PoolRouter] No Pooler registered for Prefab: {sourcePrefab.name}. Destroying object '{obj.name}'.");
            Destroy(obj);
            return;
        }

        pooler.ReturnToPool(obj);
    }

    public bool IsPrefabRegistered(GameObject prefab)
    {
        EnsureRegistryBuilt();
        return prefab != null && prefabToPooler.ContainsKey(prefab);
    }
}