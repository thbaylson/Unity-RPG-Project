using UnityEngine;
using RPG.Core;

public class Projectile : MonoBehaviour
{
    [SerializeField] float movementSpeed = 0;
    
    Health target = null;
    float damage = 0;

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        transform.LookAt(GetAimLocation());
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Health>() != target) return;
        
        target.TakeDamage(damage);
        Destroy(gameObject);
    }

    // This is looks like a constructor? Why not use an actual constructor?
    public void SetTarget(Health target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }

    private Vector3 GetAimLocation()
    {
        CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
        // If there isn't a CapsuleCollider, return raw position
        if (targetCapsule == null) return target.transform.position;

        // Characters with a CapsuleCollider will mess with the aim. We want to aim for their center
        return target.transform.position + Vector3.up * targetCapsule.height / 2;
    }
}
