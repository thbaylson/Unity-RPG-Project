using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;

namespace RPG.Movement
{
    /**<summary>Moves an object using the NavMeshAgent class.</summary>*/
    public class NavMeshMover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 6;

        Animator animator;
        ActionScheduler actionScheduler;
        Health health;
        
        NavMeshAgent agent;

        void Awake()
        {
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            health = GetComponent<Health>();
            
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            agent.enabled = !health.IsDead;
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        /**<summary>Moves this GameObject to a worldspace Vector3 position.</summary>*/
        public void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.isStopped = false;
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            agent.destination = destination;
        }

        public void Cancel()
        {
            agent.isStopped = true;
        }

        /**<summary>Updates this GameObject's Animator's "forwardSpeed" float value.</summary>*/
        private void UpdateAnimator()
        {
            // Get the global velocity from NavMeshAgent
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;

            // Convert global velocity into a local value relative to the character
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float forwardSpeed = localVelocity.z;

            // Set the Animator's blend value to the desired forward speed (on the Z axis)
            animator.SetFloat("forwardSpeed", forwardSpeed);
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            // position should be a SerializableVector3 or it'll throw an exception
            SerializableVector3 position = (SerializableVector3)state;

            // Alternative way to do the above statement. In this case, x may be a SerializableVector3 or null
            //SerializableVector3 x = state as SerializableVector3;

            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = position.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}
