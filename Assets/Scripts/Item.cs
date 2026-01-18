using Unity.VisualScripting;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public int size = 1;
    public int value = 20;
    //public float range = 10f;
    public string Name;
    public ItemType itemType;
    public Vector3 handPosition = Vector3.zero;
    public Vector3 handRotation = Vector3.zero;
    public enum ItemType
    {
        food,
        medicine,
        shortWeapon,
        longWeapon,
        magazine,
        structure,
        ammo
    }

    public abstract bool UseItem();
    


}
