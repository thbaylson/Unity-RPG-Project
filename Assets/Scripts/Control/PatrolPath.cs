using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        const float waypointGizmoRadius = 0.1f;
        int childCount;
        public int ChildCount { get { return childCount; } }

        private void Start()
        {
            childCount = transform.childCount;
        }

        private void OnDrawGizmos()
        {
            Vector3 pos;
            for(int i = 0; i < transform.childCount; i++)
            {
                pos = GetWaypoint(i);

                Gizmos.color = i == 0 ? Color.green : Color.cyan;
                Gizmos.DrawSphere(pos, waypointGizmoRadius);
                Gizmos.DrawLine(Vector3.up / 2 + pos, pos);

                Gizmos.DrawLine(pos, GetWaypoint(GetNextIndex(i)));

                //GetWaypoint(i).name = $"Waypoint {i}";
            }
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }

        public int GetNextIndex(int i)
        {
            return (i + 1) % transform.childCount;
        }
    }
}