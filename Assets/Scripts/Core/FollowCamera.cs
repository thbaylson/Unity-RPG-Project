using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform target;

        void LateUpdate()
        {
            /* We use LateUpdate here to prevent camera jitters. We only want the camera to move after the player
             * moves, otherwise the camera will attempt to make predictions and move before the character
               starts moving. For more info, see Unity's documentation.
            */

            transform.position = target.position;
        }
    }
}
