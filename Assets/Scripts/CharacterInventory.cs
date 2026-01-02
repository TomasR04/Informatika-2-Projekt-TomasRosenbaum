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
                    gun.OutOfAmmo += OnOutOfAmmo;
                }
            }
            else if (equipedItem.itemType == Item.ItemType.longWeapon)
            {
                playerController.animator.SetBool("HasLongGun", true);
                Gun gun = equipedItem as Gun;
                if (gun != null)
                {
                    gun.OutOfAmmo += OnOutOfAmmo;
                }
            }
        }
    }
    public void OnReloaded()
    {
        
        Gun gun = equipedItem as Gun;
        foreach (Item item in items)
        {
            Magazine mag = item as Magazine;
            if (mag != null && mag.magazineType == gun.magazine.magazineType)
            {
                gun.ReloadGun(mag);
                items.Remove(mag);
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
            foreach (Item item in items)
            {
                Magazine mag = item as Magazine;
                if (mag != null && mag.magazineType == gun.magazine.magazineType)
                {
                    reloading?.Invoke();
                    
                    break;
                }
            }
        }
    }

    void Start()
    {
        playerController = GetComponent<PlayableControler>();
        CheckEquiped();
    }
}
