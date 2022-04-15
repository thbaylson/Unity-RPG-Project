using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement
{
    /**<summary>Moves an object using the NavMeshAgent class.</summary>*/
    public class NavMeshMover : MonoBehaviour, IAction
    {
        Animator animator;
        ActionScheduler actionScheduler;
        Health health;
        
        NavMeshAgent agent;

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            health = GetComponent<Health>();
            
            agent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            agent.enabled = !health.IsDead;
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination)
        {
            actionScheduler.StartAction(this);
            MoveTo(destination);
        }

        /**<summary>Moves this GameObject to a worldspace Vector3 position.</summary>*/
        public void MoveTo(Vector3 destination)
        {
            agent.isStopped = false;
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
    }
}
