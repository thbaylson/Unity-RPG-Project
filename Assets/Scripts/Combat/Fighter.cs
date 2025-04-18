﻿using UnityEngine;
using RPG.Core;
using RPG.Movement;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform leftHand = null;
        [SerializeField] Transform rightHand = null;
        [SerializeField] Weapon defaultWeapon = null;// This is a scriptable object

        Animator animator;
        ActionScheduler actionScheduler;
        
        NavMeshMover mover;
        Health target;
        Weapon currentWeapon = null;

        float timeSinceLastAttack = Mathf.Infinity;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            mover = GetComponent<NavMeshMover>();
            actionScheduler = GetComponent<ActionScheduler>();
        }

        private void Start()
        {
            EquipWeapon(defaultWeapon);
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            // If there is no target, then leave Update()
            if (target == null) return;
            if (target.IsDead) return;

            // If we're not within range to attack the target, then move towards the target
            if (!GetIsInRange())
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                // If the target is Not null AND we are within range, then stop moving and start attacking
                mover.Cancel();
                AttackBehavior();
            }
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;

            // The Weapon Scriptable Object handles spawning
            weapon.Spawn(leftHand, rightHand, animator);
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.Range;
        }

        private void AttackBehavior()
        {
            transform.LookAt(target.transform);

            // If the amount of time since our last attack is greater than our attack cooldown, then attack
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            // Clear the "stopAttack" trigger now before transitioning to the attack animation
            animator.ResetTrigger("stopAttack");

            // This will start the animation that eventually calls the Hit() event
            animator.SetTrigger("attack");
        }

        /**<summary>Animation Event. Called from melee based attacking animations.</summary>**/
        private void Hit()
        {
            if (target == null || !GetIsInRange()) return;

            if (currentWeapon.HasProjectile())// currentWeapon is a ranged weapon
            {
                currentWeapon.LaunchProjectile(leftHand, rightHand, target);
            }
            else// currentWeapon must be a melee weapon
            {
                animator.SetBool("firstAttack", !animator.GetBool("firstAttack"));
                target.TakeDamage(DamageCalc());
            }
        }

        /**<summary>Animation Event. Called from range based attacking animations.</summary>**/
        void Shoot()
        {
            Hit();
        }

        private float DamageCalc()
        {
            return currentWeapon.Damage;
        }

        /**<summary>Returns true if the given CombatTarget is NOT null and is NOT dead.</summary>**/
        public bool CanAttackTarget(GameObject combatTarget)
        {
            if (combatTarget == null) return false;

            Health targetToTest = combatTarget.GetComponent<Health>();
            return (targetToTest != null) && (!targetToTest.IsDead);
        }

        /**<summary></summary>**/
        public void Attack(GameObject combatTarget)
        {
            actionScheduler.StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        /**<summary></summary>**/
        public void Cancel()
        {
            target = null;
            mover.Cancel();
            StopAttack();
        }

        private void StopAttack()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }
    }
}
