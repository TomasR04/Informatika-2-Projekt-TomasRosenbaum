using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    internal class Gun : Item
    {
        //public ItemType Type;
        
        public Magazine magazine;
        public float range = 10f;

        public float fireDelay = 1f;
        float nextFireTime = 0f;

        public ParticleSystem muzzleFlash;
        public Action OutOfAmmo;
        public Action ShotsFired;

        public override bool UseItem()
        {
            
            if (Time.time < nextFireTime)
                return false;

            if (magazine != null && magazine.HasAmmo())
            {
                magazine.UseItem();
                muzzleFlash.Play();
                ShotsFired?.Invoke();
                nextFireTime = Time.time + fireDelay;
                return true;
            }

            OutOfAmmo?.Invoke();
            return false;
        }
        private void Start()
        {
            
            muzzleFlash.Stop();
        }
        

        public void ReloadGun(Magazine newMagazine)
        {
            magazine.currentAmmo = newMagazine.currentAmmo;



        }
    }
}
