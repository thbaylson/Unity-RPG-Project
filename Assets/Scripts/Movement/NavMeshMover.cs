﻿using UnityEngine;
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

        // GetComponent should be called in Awake to avoid race conditions
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
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            // data should be a MoverSaveData or it'll throw an exception
            MoverSaveData data = (MoverSaveData)state;

            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = data.position.ToVector();
            transform.eulerAngles = data.rotation.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }

        // This annotation ensures the struct itself is serializable. Does NOT ensure the contained data is serializable
        [System.Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }
    }
}
