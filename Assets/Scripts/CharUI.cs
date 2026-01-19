using Assets.Scripts;
using System;
using TMPro;
using UnityEngine;

public class CharUI : MonoBehaviour
{
    public GameObject character;
    byte listing;
    public void Heal(Item medkit) 
    { 
        var pc = character.GetComponent<PlayableControler>();
        pc.Heal(medkit);
    }

    public void RefilMagazine()
    {
        Debug.Log("Refilling magazine from UI");
        character.GetComponent<PlayableControler>().RefilMagazine();
    }
    private void Awake()
    {
        listing = 0;
    }
    public void AddMagazine()
    {
        Gun gun = character.GetComponent<PlayableControler>().inventory.equipedItem as Gun;     
        Item mag = character.GetComponent<PlayableControler>().baseControl.GetMagazineOfType(gun.magazine.magazineType.ToString());
        if (mag != null)
        {
            character.GetComponent<PlayableControler>().inventory.AddItem(mag);
        }
        
        ActualizeUI();
    }
    public void RemoveMagazine()
    {
        Item mag = character.GetComponent<PlayableControler>().inventory.GetOneMagazine();
        if (mag != null)
        {
            character.GetComponent<PlayableControler>().baseControl.AddToInventory(mag.gameObject);
        }
        ActualizeUI();
        

    }
    public void ChangeWeapon()
    {
        listing = character.GetComponent<PlayableControler>().baseControl.SwitchCharacterWeapon(character, listing);
        ActualizeUI();
        character.GetComponent<PlayableControler>().inventory.CheckEquiped();

    }
    public void ActualizeUI()
    {
        transform.Find("Name").GetComponent<TextMeshProUGUI>().text = character.GetComponent<PlayableControler>().charName;
        transform.Find("Health").GetComponent<TextMeshProUGUI>().text = $"Health: {character.GetComponent<PlayableControler>().health}";
        transform.Find("Gun").GetComponent<TextMeshProUGUI>().text = $"Gun: {character.GetComponent<PlayableControler>().inventory.equipedItem?.Name ?? "None"}";
        string ammoStats = character.GetComponent<PlayableControler>().inventory.GetAmmoStatsInString();
        transform.Find("Ammo").GetComponent<TextMeshProUGUI>().text = $"Ammo: {ammoStats}";
        GameObject.Find("Global Control").GetComponent<GlobalControl>().ShowBaseInventory();
        
    }
}
