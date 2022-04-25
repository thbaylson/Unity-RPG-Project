using UnityEngine;
using RPG.Saving;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float maxHealth = 100f;
        [SerializeField] float currentHealth;

        bool isDead;
        public bool IsDead { get { return isDead; } }

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            // These puts a lower bound of 0 on currentHealth. Neat!
            currentHealth = Mathf.Max((currentHealth - damage), 0f);

            if (currentHealth <= 0) Die();
        }

        private void Die()
        {
            // If we're dead, then do nothing
            if (isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public object CaptureState()
        {
            return currentHealth;
        }

        public void RestoreState(object state)
        {
            currentHealth = (float)state;

            if (currentHealth <= 0) Die();
        }
    }
}