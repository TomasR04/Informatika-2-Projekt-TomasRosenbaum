using UnityEngine;
using Assets.Scripts;


internal class Bullet : Item
{
    [SerializeField]
    public Magazine.MagazineType caliber;
    public override bool UseItem()
    {
        return true;
    }

}

