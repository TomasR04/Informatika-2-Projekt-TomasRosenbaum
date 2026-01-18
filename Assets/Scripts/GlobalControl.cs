using Assets.Scripts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    public GameObject traderBTN;
    public BaseControl baseControl;
    public GameObject traderUI;
    List<PlayableControler> players = new List<PlayableControler>();
    public GameObject charUIPrefab;
    public GameObject baseItemUIPrefab;
    public GameObject BuilderUI;
    public GameObject CurrentOpenUI;
    public GameObject buildUI;
    public List<GameObject> bulletsList = new List<GameObject>();
    public GameObject builderBTN;

    public bool constructedSomething = false;

    void Start()
    {
        //navMeshSurface = GameObject.Find("NavMesh Surface").GetComponent<NavMeshSurface>();
        SpawnTrees();
        GameObject.Find("CharacterUI").SetActive(false);
        traderUI.SetActive(false);
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
        nextWaveButton.SetActive(true);
        traderBTN.SetActive(true);
        builderBTN.SetActive(true);
        baseControl.money += currentWave * 100;
        zombieCountUI.gameObject.SetActive(false);
        foreach (GameObject player in baseControl.characters)
        {
            players.Add(player.GetComponent<PlayableControler>());
        }
        
    }

    public void ShowTrader()
    {
        traderUI.SetActive(true);
        CurrentOpenUI = traderUI;
        FixShopUI();
        ShowBaseInventory();
        ShowPlayers();
        TextMeshProUGUI moneyTxT = traderUI.transform.Find("MoneyTXT").GetComponent<TextMeshProUGUI>();
        moneyTxT.text = baseControl.money.ToString();       
    }
    void ShowPlayers()
    {
        Transform panel = traderUI.transform.Find("CharacterPanel");
        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);
        }
        foreach (var player in baseControl.characters)
        {
            GameObject charUI = Instantiate(charUIPrefab, panel, false);
            charUI.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = player.GetComponent<PlayableControler>().charName;
            charUI.transform.Find("Health").GetComponent<TextMeshProUGUI>().text = $"Health: {player.GetComponent<PlayableControler>().health}";
            charUI.transform.Find("Gun").GetComponent<TextMeshProUGUI>().text = $"Gun: {player.GetComponent<PlayableControler>().inventory.equipedItem?.Name ?? "None"}";
            string ammoStats = player.GetComponent<PlayableControler>().inventory.GetAmmoStatsInString();
            charUI.transform.Find("Ammo").GetComponent<TextMeshProUGUI>().text = $"Ammo: {ammoStats}";
            charUI.GetComponent<CharUI>().character = player;
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                traderUI.transform.Find("CharacterPanel") as RectTransform
            );
            panel = traderUI.transform.Find("CharacterPanel");
        }

    }
    void FixShopUI()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            traderUI.transform.Find("ShopPanel") as RectTransform
            );
    }
    public void ShowBaseInventory()
    {
        Transform panel = traderUI.transform.Find("InventoryPanel");

        foreach (Transform child in panel)
            Destroy(child.gameObject);

        var inventory = baseControl.GetInventory();

        Dictionary<string, (int count, Item item)> grouped = new();

        foreach (var go in inventory)
        {
            var item = go.GetComponent<Item>();

            if (!grouped.ContainsKey(item.Name))
                grouped[item.Name] = (0, item);

            grouped[item.Name] = (grouped[item.Name].count + 1, item);
        }

        foreach (var pair in grouped.Values)
        {
            GameObject itemUI = Instantiate(baseItemUIPrefab, panel, false);
            itemUI.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = pair.item.Name;
            itemUI.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = pair.count.ToString();
            itemUI.transform.Find("Type").GetComponent<TextMeshProUGUI>().text = pair.item.itemType.ToString();

            
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(panel as RectTransform);
    }


    public void OnNextWave()
    {
        zombieCountUI.gameObject.SetActive(true);
        currentWave++;
        spawnedZombies = 0;
        nextWave?.Invoke();
        ongoingWave = true;
        nextWaveButton.SetActive(false);
        traderBTN.SetActive(false);
        builderBTN.SetActive(false);
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
    public void OnOpenBuilder()
    {
        Debug.Log("Open Builder UI");
        BuilderUI.SetActive(true);
        nextWaveButton.SetActive(false);
        traderBTN.SetActive(false);
        CurrentOpenUI = BuilderUI;
        Transform panel = BuilderUI.transform.Find("Inventory");

        foreach (Transform child in panel)
            Destroy(child.gameObject);

        var inventory = baseControl.GetInventory();

        Dictionary<string, (int count, Item item)> grouped = new();

        foreach (var go in inventory)
        {
            Debug.Log("Processing item: " + go.name + " " + go.GetComponent<Item>().itemType.ToString());
            var item = go.GetComponent<Item>();
            if (item.itemType == Item.ItemType.structure)
            {
                if (!grouped.ContainsKey(item.Name))
                    grouped[item.Name] = (0, item);

                grouped[item.Name] = (grouped[item.Name].count + 1, item);
            }
                

            
        }
        Debug.Log("Grouped items count: " + grouped.Count);
        foreach (var pair in grouped.Values)
        {
            
            GameObject itemUI = Instantiate(buildUI, panel, false);
            itemUI.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = pair.item.Name;
            itemUI.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = pair.count.ToString();
            itemUI.GetComponent<BuildItem>().itemPrefab = pair.item.gameObject;

        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(panel as RectTransform);
    }

    public void OnEsc() {         
        if (CurrentOpenUI != null)
        {
            if (CurrentOpenUI == BuilderUI)
            {
                if (constructedSomething)
                {
                    navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
                    navMeshSurface.BuildNavMesh();
                    constructedSomething = false;
                    
                }
            }
            Debug.Log("Closing UI: " + CurrentOpenUI.name);
            CurrentOpenUI.SetActive(false);
            nextWaveButton.SetActive(true);
            traderBTN.SetActive(true);
            builderBTN.SetActive(true);
            CurrentOpenUI = null;
        }
    }   
    public void CheckUI()
    {
               if (CurrentOpenUI == traderUI)
        {
            ShowTrader();
        }
        else if (CurrentOpenUI == BuilderUI)
        {
            OnOpenBuilder();
        }
    }
    public GameObject GetBulletOfType(string type)
    {
        foreach (var bullet in bulletsList)
        {
            if (bullet.GetComponent<Bullet>().caliber.ToString() == type)
            {
                return bullet;
            }
        }
        return null;

    }
    public void Refiled()
    {
        if (CurrentOpenUI == traderUI)
        {
            ShowTrader();
        }
    }
}
