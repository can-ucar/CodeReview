using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoolScript : MonoBehaviour {

    public static PoolScript Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PoolScript>();

            return instance;
        }
    }

    private static PoolScript instance;
    
    [SerializeField] private List<SpawnDetails> SpawnDetailsList = new List<SpawnDetails>();

    void Awake()
    {
        foreach (var spawnDetail in SpawnDetailsList)
        {
            InitializePool(spawnDetail);
        }
    }

    private void InitializePool(SpawnDetails details)
    {
        if (details.PooledObjects == null)
        {
            details.PooledObjects = new List<GameObject>();
        }
        for (var i = 0; i < details.Count; i++)
        {
            var go = Instantiate(details.Prefab, transform.position,Quaternion.identity,details.Parent);
            go.SetActive(false);
            details.PooledObjects.Add(go);
        }
    }

    public GameObject GetPooledObject(string name)
    {
        SpawnDetails spawnDetails = new SpawnDetails();

        foreach (var details in SpawnDetailsList)
        {
            if (!details.Prefab.name.Equals(name)) continue;

            spawnDetails = details;
            foreach (var pooledObject in spawnDetails.PooledObjects)
            {
                if (pooledObject != null)
                {
                    if (pooledObject.activeSelf) continue;
                    pooledObject.SetActive(true);
                    return pooledObject;
                }
            }
            break;
        }
        var newObj = Instantiate(spawnDetails.Prefab, transform.position,Quaternion.identity, spawnDetails.Parent);
        newObj.SetActive(true);
        spawnDetails.PooledObjects.Add(newObj);
        return newObj;
    }

    public void AddObjectToPool(GameObject _PoolObject, int _Count)
    {
        SpawnDetails spawnDetails = new SpawnDetails();
        spawnDetails.Prefab = _PoolObject;
        spawnDetails.Count = _Count;
        spawnDetails.Parent = transform;
        spawnDetails.PooledObjects = new List<GameObject>();
        for (var i = 0; i < _Count; i++)
        {
            var go = Instantiate(_PoolObject, transform);
            go.SetActive(false);
            spawnDetails.PooledObjects.Add(go);
        }
        SpawnDetailsList.Add(spawnDetails);
    }

    [System.Serializable]
    public struct SpawnDetails
    {
        public GameObject Prefab;
        public int Count;
        public Transform Parent;
        public List<GameObject> PooledObjects;
    }
}
