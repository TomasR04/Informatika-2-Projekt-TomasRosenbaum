using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour
{
    public List<GameObject> treePrefabs;
    public int treeCount = 100;
    public Terrain terrain;
    public int treeSpacing = 2;
    public Action treeSpawned;
    void Start()
    {
        SpawnTrees();
        GameObject.Find("CharacterUI").SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }
}
