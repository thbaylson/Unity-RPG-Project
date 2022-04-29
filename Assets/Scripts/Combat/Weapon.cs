using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] float weaponDamage = 20f;
        public float Damage { get { return weaponDamage; } }
        [SerializeField] float weaponRange = 2f;
        public float Range { get { return weaponRange; } }
        
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] bool isRightHanded = true;

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
    }
}