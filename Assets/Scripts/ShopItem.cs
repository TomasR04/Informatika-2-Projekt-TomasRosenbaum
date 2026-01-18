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
        if (itemPrefab.GetComponent<Gun>()&&itemName == null)
        {
            itemName = itemPrefab.GetComponent<Gun>().Name;

        }
        
        gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = itemName;
        gameObject.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = $"${itemPrice}";
        if (itemIcon != null)
            gameObject.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>().sprite = itemIcon;
        gameObject.transform.Find("Type").GetComponent<TextMeshProUGUI>().text = itemType.ToString();
    }
    /*public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Item clicked");
        BuyItem();
    }*/
    

    public void BuyItem()
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

}
