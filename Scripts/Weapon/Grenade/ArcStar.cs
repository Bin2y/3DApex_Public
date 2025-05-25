
using UnityEngine;

public class ArcStar : Grenade
{
    private void OnCollisionEnter(Collision collision)
    {
        collideDamageMessage.amount = 10;

        if (collision.transform.gameObject.TryGetComponent<IDamageable>(out IDamageable component))
        {
            if (collision.gameObject.CompareTag("Player")) return;
            component.ApplyDamage(collideDamageMessage);
        }

        ContactPoint contact = collision.contacts[0];
        transform.position = contact.point;

        transform.SetParent(collision.transform);

        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            Destroy(rb);
        }

        if (TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
        }
    }
}


