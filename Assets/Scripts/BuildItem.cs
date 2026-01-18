using UnityEngine;

public class BuildItem : MonoBehaviour
{
    public GameObject itemPrefab;

    public void Build()
    {
        Debug.Log("Building item: " + itemPrefab.name);
        GameObject.Find("Player").GetComponent<Player>().BuildThis(itemPrefab);
    }
}
