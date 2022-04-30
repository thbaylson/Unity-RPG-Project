using UnityEngine;
using RPG.Core;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {   
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
            if(equippedPrefab != null)
            {
                Instantiate(equippedPrefab, (isRightHanded ? rightHand : leftHand));
            }
            if(animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
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