using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    internal class Magazine : Item
    {
        public int capacity;
        public int currentAmmo;
        public MagazineType magazineType;
        public bool Reload()
        {
            if (currentAmmo < capacity)
            {
                currentAmmo++;
                return true;
            }
            else
            {
                return false;
            }
        }
        public override bool UseItem()
        {
            
            if (currentAmmo > 0)
            {
                currentAmmo--;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool HasAmmo()
        {
            return currentAmmo > 0;
        }

        public enum MagazineType
        {
            pistol,
            rifle,
            shotgun,
            revolver
        }
        private void Start()
        {
            //currentAmmo = capacity;
        }
        private void Awake()
        {
            //currentAmmo = capacity;
        }

    }
}
