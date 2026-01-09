using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.AI.Navigation;
public class ZombieSpawnArea : MonoBehaviour
{
    public float radius = 50f;
    GlobalControl globalControl;
    public List<GameObject> zombiePrefabs;
    
    void Start()
    {
        
        globalControl = GameObject.Find("Global Control").GetComponent<GlobalControl>();
        globalControl.treeSpawned += OnTreesSpawned;
        globalControl.nextWave += () => SpawnWave(globalControl.currentWave);
    }
    public void OnTreesSpawned()
    {
        List<GameObject> treePositions = new List<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.tag == "Tree")
            {
                treePositions.Add(collider.gameObject);
            }
        }
        foreach (var tree in treePositions)
        {
            Destroy(tree);
        }
        

    }
    public void OnDied(ZombieControler zombie)
    {

        zombie.Died -= OnDied;
        globalControl.RemoveZombies(1);
    }
    public void SpawnWave(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 randomPoint = Random.insideUnitCircle * radius;
            Vector3 spawnPosition = new Vector3(transform.position.x + randomPoint.x, transform.position.y, transform.position.z + randomPoint.y);
            spawnPosition.y = Terrain.activeTerrain.SampleHeight(spawnPosition);
            GameObject prefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Count)];
            GameObject newZombie = Instantiate(prefab, spawnPosition, Quaternion.identity);
            newZombie.GetComponent<ZombieControler>().Died += OnDied;
            
        }
        globalControl.AddZombies(amount);
        Debug.Log(gameObject.name + " wave spawned");

    }
}
