using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpawnerFromPool : MonoBehaviour
{
    [SerializeField] private ObjectPooler.PrefabTag prefabTag;
    [SerializeField] private GameObject[] spawningPositions; // If spawning using Vector2/3 is desired, then it should be wrapped as a GameObjection with that specific transform
    ObjectPooler objectPooler;
    [SerializeField] private float spawnRate;
    private float nextSpawn;

    private void Start()
    {
        objectPooler = ObjectPooler.Instance;
        nextSpawn = 0f;
    }

    /*
    void FixedUpdate()
    {
        objectPooler.SpawnFromPool(ObjectPooler.PrefabTag.Witch, Vector3.zero, Quaternion.identity);
    }
    */

    public void Update()
    {
        if ((Time.time > nextSpawn) && (spawningPositions != null) && (spawningPositions.Length > 0))
        {
            nextSpawn = Time.time + spawnRate;
            
            // Choose a random pool and spawn an object from this pool
            GameObject randomSpawnPlace = spawningPositions[UnityEngine.Random.Range(0, spawningPositions.Length)];
            objectPooler.SpawnFromPool(prefabTag, randomSpawnPlace.transform.position, randomSpawnPlace.transform.rotation);            
        }
    }
}
