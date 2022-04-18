using UnityEngine;
using RPG.Core;
using RPG.Movement;
using RPG.Combat;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;
        NavMeshMover mover;
        Fighter combat;

        // Start is called before the first frame update
        void Start()
        {
            health = GetComponent<Health>();
            mover = GetComponent<NavMeshMover>();
            combat = GetComponent<Fighter>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (health.IsDead) return;

            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
            //print("Noting to do.");
        }

        /**<summary>Helper method to handle combat logic.</summary>**/
        private bool InteractWithCombat()
        {
            // TODO: Can this be done with Linq?
            // This casts a ray through the Scene and returns all hits. Note that order of the results is undefined.
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            bool interacted = false;

            CombatTarget target;
            foreach (RaycastHit hit in hits)
            {
                target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;

                // If the target can't be attacked, skip it and continue looping
                if (!combat.CanAttackTarget(target.gameObject)) continue;

                // If (Left-Mouse is clicked)...
                if (Input.GetMouseButton(0))
                {
                    combat.Attack(target.gameObject);
                }
                interacted = true;
            }

            return interacted;
        }

        /**<summary>Attempts to set this NavMeshAgent's destination to the last screen point clicked.</summary>*/
        private bool InteractWithMovement()
        {
            // The out parameter of Physics.Raycast()
            RaycastHit hit = new RaycastHit();
            
            // Determine if the mouse is hovering over a walkable area
            bool canMove = false;

            // Do the raycast and return if we actually hit anything or not
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            // If the mouse clicked something that has collision...
            if (hasHit)
            {
                // Draw a debug line from the camera to the point of impact
                //Debug.DrawRay(ray.origin, ray.direction * 100, Color.white, 1f);

                if (Input.GetMouseButton(0))
                {
                    // Move the agent's destination to the RaycastHit's impact point
                    mover.StartMoveAction(hit.point, 1f);
                }

                canMove = true;
            }

            return canMove;
        }

        /**<summary>Helper method for getting a Ray from the mouse position.</summary>**/
        private static Ray GetMouseRay()
        {
            // The ray that is shot from the camera to the point the player clicks
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}