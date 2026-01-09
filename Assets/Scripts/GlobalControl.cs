using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class GlobalControl : MonoBehaviour
{
    public List<GameObject> treePrefabs;
    public int treeCount = 100;
    public Terrain terrain;
    public int treeSpacing = 2;
    public Action treeSpawned;
    public Action nextWave;
    public int currentWave = 0;
    public TextMeshProUGUI zombieCountUI;
    public int spawnedZombies = 0;
    public NavMeshSurface navMeshSurface;
    public bool ongoingWave = false;
    public GameObject nextWaveButton;
    public BaseControl baseControl;
    public GameObject traderUI;
    public 

    void Start()
    {
        //navMeshSurface = GameObject.Find("NavMesh Surface").GetComponent<NavMeshSurface>();
        SpawnTrees();
        GameObject.Find("CharacterUI").SetActive(false);
    }
    public void AddZombies(int amount)
    {
        spawnedZombies += amount;
        zombieCountUI.text = $"Zombies: {spawnedZombies}";

    }
    public void RemoveZombies(int amount)
    {
        spawnedZombies -= amount;
        if (spawnedZombies < 0) spawnedZombies = 0;
        zombieCountUI.text = $"Zombies: {spawnedZombies}";
        if (spawnedZombies<=0)
        {
            WaveEnd();
        }
    }
    void WaveEnd()
    {
        ongoingWave = false;
        //nextWaveButton.SetActive(true);
        baseControl.money += currentWave * 100;
        ShowTrader();
    }

    void ShowTrader()
    {
        traderUI.SetActive(true);
    }


    public void OnNextWave()
    {
        currentWave++;
        spawnedZombies = 0;
        nextWave?.Invoke();
        ongoingWave = true;
        nextWaveButton.SetActive(false);
    }
    void SpawnTrees()
    {
        int count = 0;
        byte tryes = 0;
        if (tryes > 20)
        {
            
            treeSpawned?.Invoke();
            return;
        }
        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = terrain.GetPosition();

        for (int i = 0; i < treeCount; i++)
        {
            float x = UnityEngine.Random.Range(0f, 1f);
            float z = UnityEngine.Random.Range(0f, 1f);

            float posX = x * data.size.x + terrainPos.x;
            float posZ = z * data.size.z + terrainPos.z;
            float posY = terrain.SampleHeight(new Vector3(posX, 0, posZ)) + terrainPos.y;

            bool canSpawn = true;
            Collider[] colliderHits = Physics.OverlapSphere(new Vector3(posX, posY, posZ), treeSpacing);
            foreach (var  hit in colliderHits)
            {
                if (hit.gameObject.tag == "Tree")
                {
                    canSpawn = false;
                    break;
                }
            }

            if (canSpawn)
            {
                GameObject prefab = treePrefabs[UnityEngine.Random.Range(0, treePrefabs.Count)];
                Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

                Instantiate(prefab, new Vector3(posX, posY, posZ), rotation);
                count++;
                //Debug.Log($"Spawned {count} / {treeCount} trees.");
            }
            else
            {
                tryes++;
            }

        }
        treeSpawned?.Invoke();
        navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        navMeshSurface.BuildNavMesh();
    }
}
