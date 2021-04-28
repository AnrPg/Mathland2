using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSimpleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private GameObject[] spawningPositions; // If spawning using Vector2/3 is desired, then it should be wrapped as a GameObjection with that specific transform
    [SerializeField] private float spawnRate;
    private float nextSpawn;
    private bool isActive;

    public bool IsActive { get => isActive; set => isActive = value; }

    private void Start()
    {
        nextSpawn = 0f;
        IsActive = false;
    }

    public void Update()
    {
        if (isActive)
        {
            bool isProperlyInitialized = (prefabs != null) && (prefabs.Length > 0) && (spawningPositions != null) && (spawningPositions.Length > 0);
            if ((Time.time > nextSpawn) && isProperlyInitialized)
            {
                nextSpawn = Time.time + spawnRate;
                SpawnRandomly();
            }
        }        
    }

    // TODO: Trigger event when an object is spawned, so that the newly spawned object is registered at some list (if so needed)
    public GameObject SpawnRandomly()
    {
        GameObject randomSpawnPlace = spawningPositions[UnityEngine.Random.Range(0, spawningPositions.Length)];
        GameObject randomSpawnPrefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
        GameObject obj = Instantiate(randomSpawnPrefab, randomSpawnPlace.transform.position, randomSpawnPlace.transform.rotation);
        obj.SetActive(false);
        
        object[] parameters = new object[2];
        parameters[0] = obj;
        parameters[1] = obj.GetType();
        SendMessage("objectIsSpawned", parameters); // Receiver function should be in the same GameObject
        return obj;
    }
}
