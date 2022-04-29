using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float movementSpeed = 0;
    [SerializeField] Transform target = null;

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        transform.LookAt(GetAimLocation());
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
    }

    private Vector3 GetAimLocation()
    {
        CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
        // If there isn't a CapsuleCollider, return raw position
        if (targetCapsule == null) return target.position;

        // Characters with a CapsuleCollider will mess with the aim. We want to aim for their center
        return target.position + Vector3.up * targetCapsule.height / 2;
    }
}
