using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public PrefabTag tag;
        public GameObject prefab;
        public int size;
    }
    
    public enum PrefabTag
    {
        Witch
    }
    
    public List<Pool> pools;
    public Dictionary<PrefabTag, Queue<GameObject>> poolDictionary;

    #region Singleton

    public static ObjectPooler Instance;
    private void Awake() 
    {
        Instance = this;
    }

    #endregion

    void Start()
    {
        poolDictionary = new Dictionary<PrefabTag, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(PrefabTag tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.\nTherefore, couldn't spawn an object...");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();

        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }

        // Put it back to the Queue in the Dictionary so that we can use it again if needed
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    /*
    public GameObject SpawnFromPool(PrefabTag tag, Vector2 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.\nTherefore, couldn't spawn an object...");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Put it back to the Queue in the Dictionary so that we can use it again if needed
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public GameObject SpawnFromPool(PrefabTag tag, GameObject spawningPosition)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.\nTherefore, couldn't spawn an object...");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = spawningPosition.transform.position;
        objectToSpawn.transform.rotation = spawningPosition.transform.rotation;

        // Put it back to the Queue in the Dictionary so that we can use it again if needed
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
    */

}
