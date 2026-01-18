using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class BaseControl : MonoBehaviour
{
    public int baseRadius = 50;
    public GlobalControl globalControl;
    public int money = 0;
    public List<GameObject> characters;
    private List<GameObject> baseInventory = new List<GameObject>();
    

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
        
    }
    public void RemoveFromInventory(GameObject item)
    {
        foreach (var invItem in baseInventory)
        {
            if (invItem.GetComponent<Item>().Name == item.GetComponent<Item>().Name)
            {
                baseInventory.Remove(invItem);
                Destroy(invItem);
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
}
