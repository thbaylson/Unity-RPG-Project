using UnityEngine;
using RPG.Core;
using RPG.Movement;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float weaponDamage = 20f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;

        Animator animator;
        ActionScheduler actionScheduler;
        
        NavMeshMover mover;
        Health target;

        float timeSinceLastAttack = Mathf.Infinity;

        private void Start()
        {
            animator = GetComponent<Animator>();
            mover = GetComponent<NavMeshMover>();
            actionScheduler = GetComponent<ActionScheduler>();
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
                mover.MoveTo(target.transform.position);
            }
            else
            {
                // If the target is Not null AND we are within range, then stop moving and start attacking
                mover.Cancel();
                AttackBehavior();
            }
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
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

        /**<summary>Animation Event. Called from "Unarmed-Attack-L2" and "...-L3"</summary>**/
        private void Hit()
        {
            if (target == null || !GetIsInRange()) return;

            animator.SetBool("firstAttack", !animator.GetBool("firstAttack"));
            target.TakeDamage(DamageCalc());
        }

        private float DamageCalc()
        {
            return weaponDamage;
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
            StopAttack();
        }

        private void StopAttack()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }
    }
}
