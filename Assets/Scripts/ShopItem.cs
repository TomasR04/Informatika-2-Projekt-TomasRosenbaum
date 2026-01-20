using UnityEngine;
using TMPro;
using Assets.Scripts;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopItem : MonoBehaviour
{
    public string itemName;
    public int itemPrice;
    public string itemDescription;
    public ItemType itemType;
    public GameObject itemPrefab;
    public Sprite itemIcon;
    public enum ItemType
    {
        Weapon,
        Ammo,
        Health,
        Structure,
        Magazine
    }
    void Start()
    {
        if (itemPrefab.tag == "Item")
        {
            if (itemPrefab.GetComponent<Gun>() && itemName == null)
            {
                itemName = itemPrefab.GetComponent<Gun>().Name;

            }

            gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = itemName;
            gameObject.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = $"${itemPrice}";
            if (itemIcon != null)
                gameObject.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>().sprite = itemIcon;
            gameObject.transform.Find("Type").GetComponent<TextMeshProUGUI>().text = itemType.ToString();
        }
        else
        {
            gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = "Nová postava";
            gameObject.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = $"${itemPrice}";
            if (itemIcon != null)
                gameObject.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>().sprite = itemIcon;
            gameObject.transform.Find("Type").GetComponent<TextMeshProUGUI>().text = "Character";
        }
        
    }
    /*public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Item clicked");
        BuyItem();
    }*/
    public void FixUI()
    {
        Debug.Log("Fixing UI");
        if (itemPrefab.tag == "Item")
        {
            if (itemPrefab.GetComponent<Gun>() && itemName == null)
            {
                itemName = itemPrefab.GetComponent<Gun>().Name;

            }

            gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = itemName;
            gameObject.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = $"${itemPrice}";
            if (itemIcon != null)
                gameObject.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>().sprite = itemIcon;
            gameObject.transform.Find("Type").GetComponent<TextMeshProUGUI>().text = itemType.ToString();
        }
        else
        {
            gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = "Nová postava";
            gameObject.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = $"${itemPrice}";
            if (itemIcon != null)
                gameObject.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>().sprite = itemIcon;
            gameObject.transform.Find("Type").GetComponent<TextMeshProUGUI>().text = "Character";
        }
    }

    public void BuyItem()
    {
        if (itemPrefab.tag == "Item")
        {
            GlobalControl globalControl = GameObject.Find("Global Control").GetComponent<GlobalControl>();
            if (globalControl.baseControl.money >= itemPrice)
            {
                globalControl.baseControl.money -= itemPrice;
                GameObject newItem = Instantiate(itemPrefab);
                globalControl.baseControl.AddToInventory(newItem);
                globalControl.ShowTrader();

            }
            else
            {
                Debug.Log($"Not enough money to buy {itemName}. You need ${itemPrice - globalControl.baseControl.money} more.");
            }
        }
        else
        {
            GlobalControl globalControl = GameObject.Find("Global Control").GetComponent<GlobalControl>();
            if (globalControl.baseControl.money >= itemPrice && globalControl.baseControl.characterLimit >= globalControl.baseControl.characters.Count +1)
            {
                globalControl.baseControl.money -= itemPrice;

                GameObject newCharacter = GetNewRandomCharacter();
                globalControl.baseControl.AddCharacter(newCharacter);
                newCharacter.GetComponent<PlayableControler>().PlaceOnNavMesh();
                globalControl.ShowTrader();
            }
            else
            {
                Debug.Log($"Not enough money to buy {itemName}. You need ${itemPrice - globalControl.baseControl.money} more.");
            }

        }
    }
    GameObject GetNewRandomCharacter()
    {
        string[] characterNames = { "Franta", "Pepa", "Pat", "Mat"};
        string randomName = characterNames[Random.Range(0, characterNames.Length)];
        int speed = Random.Range(5, 20);
        int aim = Random.Range(5, 20);
        GameObject newCharacter = Instantiate(itemPrefab);
        newCharacter.GetComponent<PlayableControler>().speed = speed;
        newCharacter.GetComponent<PlayableControler>().aiming = aim;

        newCharacter.GetComponent<PlayableControler>().charName = randomName;
        return newCharacter;
    }

}
