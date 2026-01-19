using Assets.Scripts;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseControl : MonoBehaviour
{
    public int baseRadius = 50;
    public GlobalControl globalControl;
    public int money = 0;
    public List<GameObject> characters;
    [SerializeField]
    private List<GameObject> baseInventory = new List<GameObject>();
    public List<GameObject> listOfGuns = new List<GameObject>();


    void Start()
    {
        globalControl.treeSpawned += OnTreeSpawned;
    }
    public List<GameObject> GetInventory()
    {         
        return baseInventory;
    }
    public void AddToInventory(GameObject item)
    {
        item.GetComponent<Renderer>().enabled = false;
        if (item.GetComponent<Collider>() != null)
            item.GetComponent<Collider>().enabled = false;
        baseInventory.Add(item);
        if (item.GetComponent<Gun>() != null)
        {
            listOfGuns.Add(item);
        }
        
    }
    public void RemoveFromInventory(GameObject item)
    {
        foreach (var invItem in baseInventory)
        {
            if (invItem.GetComponent<Item>().Name == item.GetComponent<Item>().Name)
            {
                baseInventory.Remove(invItem);
                if (item.GetComponent<Gun>() != null)
                {
                    listOfGuns.Remove(item);
                }
                break;
            }
        }
        globalControl.GetComponent<GlobalControl>().CheckUI();
    }

    private void Awake()
    {
        globalControl.treeSpawned += OnTreeSpawned;
    }
    // Update is called once per frame
    

    public void OnTreeSpawned()
    {
        //Debug.Log("Tree spawned, clearing area around base.");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, baseRadius);
        List<GameObject> treesInRange = new List<GameObject>();
        foreach (var hitCollider in hitColliders)
        {
            
            if (hitCollider.CompareTag("Tree"))
            {
                //Debug.Log("Tree found within base radius: " + hitCollider.gameObject.name);
                treesInRange.Add(hitCollider.gameObject);
            }
        }
        
        foreach (var tree in treesInRange)
        {
            Destroy(tree);
        }


    }
    public int GetAllAmmoOfType(string Type)
    {
        Debug.Log("Checking for ammo of type: " + Type);
        int totalAmmo = 0;
        List<GameObject> bullets = new List<GameObject>();
        foreach (var item in baseInventory)
        {
            Bullet bullet = item.GetComponent<Bullet>();
            if (bullet != null && bullet.caliber.ToString() == Type)
            {
                totalAmmo++;

            }
        }
        return totalAmmo;
    }
    public void ReciveAmmoOfType(string Type, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject bulletPrefab = globalControl.GetBulletOfType(Type);
            if (bulletPrefab != null)
            {
                GameObject newBullet = Instantiate(bulletPrefab);
                AddToInventory(newBullet);
            }
        }

    }
    public void SetAmmoOfTypeTo(string Type, int amount)
    {
        GameObject bulletPrefab = globalControl.GetBulletOfType(Type);
        if (bulletPrefab == null) return;
        List<GameObject> bulletsToRemove = new List<GameObject>();
        foreach (Item item in baseInventory.ConvertAll(i => i.GetComponent<Item>()))
        {
            Bullet bullet = item as Bullet;
            if (bullet != null && bullet.caliber.ToString() == Type)
            {
                bulletsToRemove.Add(item.gameObject);
            }
        }
        foreach (var bullet in bulletsToRemove)
        {
            RemoveFromInventory(bullet);
            Destroy(bullet);
        }
        ReciveAmmoOfType(Type, amount);

    }

    public Item GetMagazineOfType(string Type)
    {
        foreach (var item in baseInventory)
        {
            Magazine mag = item.GetComponent<Magazine>();
            if (mag != null && mag.magazineType.ToString() == Type)
            {
                baseInventory.Remove(item);
                mag.GetComponent<Renderer>().enabled = false;
                if (mag.GetComponent<Collider>() != null)
                    mag.GetComponent<Collider>().enabled = true;
                return mag;
            }
        }
        return null;
    }
    public byte SwitchCharacterWeapon(GameObject character, byte listing)
    {
        PlayableControler controler = character.GetComponent<PlayableControler>();
        Gun currentGun = controler.inventory.equipedItem.GetComponent<Gun>();
        bool looking = true;
        while (looking)
        {
            if (listOfGuns[listing].GetComponent<Gun>().Name != currentGun.Name)
            {
                currentGun.transform.position = new Vector3(0, -1000, 0);
                currentGun.transform.parent = null;
                Item newGun = listOfGuns[listing].GetComponent<Item>();
                AddToInventory(controler.inventory.ReturnItem(currentGun.gameObject));
                List<GameObject> invItems = controler.inventory.ReturnAllMagazinesOfType(currentGun.magazine.magazineType.ToString());
                foreach (var mag in invItems)
                {
                    AddToInventory(mag);
                }
                controler.inventory.AddItem(newGun);
                controler.inventory.EquipItem(newGun);
                RemoveFromInventory(newGun.gameObject);


                looking = false;
                return listing;
            }
            else
            {
                listing++;
                if (listing >= listOfGuns.Count)
                {
                    return 0;
                }
            }
        }
        return 0;

    }

    public void AddCharacter(GameObject character)
    {
        characters.Add(character);
        bool hasNotBeenPositioned = true;
        int safetyCounter = 0;
        while (hasNotBeenPositioned && safetyCounter<20)
        {
            safetyCounter++;
            Vector2 randomPoint = Random.insideUnitCircle * baseRadius;
            Vector3 spawnPosition = new Vector3(transform.position.x + randomPoint.x, transform.position.y, transform.position.z + randomPoint.y);
            spawnPosition.y = Terrain.activeTerrain.SampleHeight(spawnPosition);
            Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, 1f);
            Debug.Log("Checking position for new character, found " + hitColliders.Length + " colliders at position "+ spawnPosition.ToString() ); 
            if (hitColliders.Length < 2)
            {
                character.transform.position = spawnPosition;
                hasNotBeenPositioned = false;
            }

        }
        
    }
    /*public void AddCharacter(GameObject character)
    {
        characters.Add(character);

        NavMeshAgent agent = character.GetComponent<NavMeshAgent>();
        agent.enabled = false; 

        for (int i = 0; i < 20; i++)
        {
            Vector2 randomPoint = Random.insideUnitCircle * baseRadius;
            Vector3 roughPos = new Vector3(
                transform.position.x + randomPoint.x,
                transform.position.y + 10f, 
                transform.position.z + randomPoint.y
            );

            NavMeshHit hit;
            if (NavMesh.SamplePosition(roughPos, out hit, 5f, NavMesh.AllAreas))
            {
                character.transform.position = hit.position;
                agent.enabled = true;
                return;
            }
        }

        Debug.LogError("Nepodaøilo se umístit postavu na NavMesh");
    }*/
}
