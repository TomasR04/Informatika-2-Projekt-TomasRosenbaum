using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;
using System;

public class CharacterInventory : MonoBehaviour
{
    
    public List<Item> items = new List<Item>();
    public int maxCapacity = 50;
    public Item equipedItem;
    public GameObject Hand;
    public PlayableControler playerController;
    public Action reloading;
    public Action reloadingDone;

    public void AddItem(Item item)
    {
        items.Add(item);
        item.gameObject.GetComponent<Collider>().enabled = false;
        item.gameObject.transform.SetParent(this.transform);
        item.GetComponent<Renderer>().enabled = false;
        Transform[] children = item.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child != item.transform)
            {
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    childRenderer.enabled = false;
                }
            }
        }
    }
    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }
    public bool HasItem(Item item)
    {
        return items.Contains(item);
    }
    public void EquipItem(Item item)
    {
        if (HasItem(item))
        {

            equipedItem = item;
            equipedItem.transform.SetParent(Hand.transform);
            equipedItem.GetComponent<Renderer>().enabled = true;
            Transform[] children = equipedItem.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child != equipedItem.transform)
                {
                    Renderer childRenderer = child.GetComponent<Renderer>();
                    if (childRenderer != null)
                    {
                        childRenderer.enabled = true;
                    }
                }
            }
        }
    }
    public void CheckEquiped()
    {
        if (equipedItem != null)
        {
            if (equipedItem.itemType == Item.ItemType.shortWeapon)
            {
                playerController.animator.SetBool("HasShortGun", true);
                Gun gun = equipedItem as Gun;
                if (gun != null)
                {
                    gun.OutOfAmmo = OnOutOfAmmo;
                }
            }
            else if (equipedItem.itemType == Item.ItemType.longWeapon)
            {
                playerController.animator.SetBool("HasLongGun", true);
                Gun gun = equipedItem as Gun;
                if (gun != null)
                {
                    //Debug.Log("Long");
                    gun.OutOfAmmo = OnOutOfAmmo;
                }
            }
            else
            {
                playerController.animator.SetBool("HasShortGun", false);
                playerController.animator.SetBool("HasLongGun", false);
            }
            equipedItem.transform.localPosition = equipedItem.handPosition;
            equipedItem.transform.localRotation = Quaternion.Euler(equipedItem.handRotation);

        }
        else
        {
            playerController.animator.SetBool("HasShortGun", false);
            playerController.animator.SetBool("HasLongGun", false);
        }
    }
    public void OnReloaded()
    {
        
        Gun gun = equipedItem as Gun;
        List<Item> magazines = GetItemsOfType(Item.ItemType.magazine);
        magazines.Add(equipedItem.gameObject.GetComponent<Gun>().magazine);
        foreach (Item item in magazines)
        {
            Magazine mag = item as Magazine;
            if (mag != null && mag.magazineType == gun.magazine.magazineType && mag.HasAmmo())
            {
                gun.ReloadGun(mag);
                mag.currentAmmo = 0;
                break;
            }
        }
        reloadingDone?.Invoke();
    }
    void OnOutOfAmmo()
    {
        
        ReloadGun();
    }
    void ReloadGun()
    {
        // Find a magazine in the inventory that matches the gun's magazine type
        Gun gun = equipedItem as Gun;
        if (gun != null)
        {
            List<Item> magazines = GetItemsOfType(Item.ItemType.magazine);
            magazines.Add(equipedItem.gameObject.GetComponent<Gun>().magazine);
            foreach (Item item in magazines)
            {
                Magazine mag = item as Magazine;
                if (mag != null && mag.magazineType == gun.magazine.magazineType && mag.HasAmmo())
                {
                    reloading?.Invoke();
                    
                    break;
                }
            }
        }
    }
    public List<Item> GetItemsOfType(Item.ItemType itemType)
    {
        List<Item> filteredItems = new List<Item>();
        foreach (Item item in items)
        {
            if (item.itemType == itemType)
            {
                filteredItems.Add(item);
            }
        }
        return filteredItems;
    }
    public void RefilMagazine(int availableAmmo)
    {
        int ammo = availableAmmo;
        List<Magazine> magazines = new List<Magazine>();
        foreach (Item item in items)
        {
            Magazine mag = item as Magazine;
            if (mag != null)
            {
                magazines.Add(mag);
            }
        }
        Magazine mostLowOnAmmo = magazines[0];
        foreach (Magazine mag in magazines)
        {
            if (mag.currentAmmo < mostLowOnAmmo.currentAmmo)
            {
                mostLowOnAmmo = mag;
            }
        }
        while (mostLowOnAmmo.Reload())
        {
            ammo--;
        }
    }

    public int RefilMagazineAndReturn(int availableAmmo)
    {
        Debug.Log("RefilMagazineAndReturn called with availableAmmo: " + availableAmmo);
        int ammo = availableAmmo;
        List<Magazine> magazines = new List<Magazine>();
        foreach (Item item in items)
        {
            Magazine mag = item as Magazine;
            if (mag != null)
            {
                magazines.Add(mag);
            }
        }
        Magazine mostLowOnAmmo = magazines[0];
        foreach (Magazine mag in magazines)
        {
            if (mag.currentAmmo < mostLowOnAmmo.currentAmmo)
            {
                mostLowOnAmmo = mag;
            }
        }
        while (mostLowOnAmmo.Reload())
        {
            ammo--;
        }
        return ammo;
    }

    void Start()
    {
        playerController = GetComponent<PlayableControler>();
        CheckEquiped();
    }
    public string GetAmmoStatsInString()
    {
        Gun gun = equipedItem as Gun;
        int totalAmmo = 0;
        int totalCapacity = 0;
        totalAmmo += gun.magazine.currentAmmo;
        totalCapacity += gun.magazine.capacity;
        foreach (Item item in items)
        {
            Magazine mag = item as Magazine;
            if (mag != null && mag.magazineType == gun.magazine.magazineType)
            {
                totalAmmo += mag.currentAmmo;
                totalCapacity += mag.capacity;
            }
        }
        return $"{totalAmmo}/{totalCapacity}";

    }
    public Item GetOneMagazine()
    {
        Gun gun = equipedItem as Gun;
        foreach (Item item in items)
        {
            Magazine mag = item as Magazine;
            if (mag != null && mag.magazineType == gun.magazine.magazineType)
            {
                items.Remove(item);
                return item;
            }
        }
        return null;
    }
    public GameObject ReturnItem(GameObject item)
    {
        foreach (Item invItem in items)
        {
            if (invItem.Name == item.GetComponent<Item>().Name)
            {
                items.Remove(invItem);
                return invItem.gameObject;
            }
        }
        return null;
    }

    public List<GameObject> ReturnAllMagazinesOfType(string type)
    {
        List<GameObject> mags = new List<GameObject>();
        foreach (Item invItem in items)
        {
            Magazine mag = invItem as Magazine;
            if (mag != null && mag.magazineType.ToString() == type)
            {
                mags.Add(invItem.gameObject);
            }
        }
        foreach (GameObject mag in mags)
        {
            items.Remove(mag.GetComponent<Item>());
        }
        return mags;
    }
}
