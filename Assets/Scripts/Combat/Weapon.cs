using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        const string weaponName = "Weapon";

        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject equippedPrefab = null;

        [SerializeField] float weaponDamage = 20f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] bool isRightHanded = true;

        // Not every weapon has a projectile. 
        // TODO: Separate Weapon into MeleeWeapon and RangedWeapon
        [SerializeField] Projectile projectile = null;

        public void Spawn(Transform leftHand, Transform rightHand, Animator animator)
        {
            DestroyOldWeapon(leftHand, rightHand);

            if(equippedPrefab != null)
            {
                GameObject weapon = Instantiate(equippedPrefab, (isRightHanded ? rightHand : leftHand));
                weapon.name = weaponName;
            }
            if(animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
        }

        private void DestroyOldWeapon(Transform leftHand, Transform rightHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            // If there is nothing in the rightHand, check the leftHand
            if(oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            // If oldWeapon was not null, then we have to destroy it
            // Change the to-be-destroyed weapon's name to avoid weird race conditions
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        // TODO: MeleeWeapons don't need this. Should be refactored into a RangedWeapon class
        public void LaunchProjectile(Transform leftHand, Transform rightHand, Health target)
        {
            // Instantiate a projectile instance
            Projectile projectileInstance = Instantiate(projectile, (isRightHanded ? rightHand : leftHand).position, Quaternion.identity);
            // Invoke SetTarget of the newly instantiated projectile
            projectileInstance.SetTarget(target, weaponDamage);
        }

        // TODO: This is unecessary if Weapon gets split into MeleeWeapon and RangedWeapon
        public bool HasProjectile()
        {
            return projectile != null;
        }

        public float Damage { get { return weaponDamage; } }
        public float Range { get { return weaponRange; } }
    }
}