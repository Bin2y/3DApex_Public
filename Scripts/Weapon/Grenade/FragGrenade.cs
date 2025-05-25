using UnityEngine;

public class FragGrenade : Grenade
{
    
    private void OnCollisionEnter(Collision collision)
    {
        collideDamageMessage.amount = 10;
        if (collision.transform.gameObject.TryGetComponent<IDamageable>(out IDamageable component))
        {
            component.ApplyDamage(collideDamageMessage);
        }
    }
}


