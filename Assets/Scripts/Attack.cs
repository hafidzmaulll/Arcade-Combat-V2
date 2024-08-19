using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // See if it can be hit
        Damageable damageable = collision.GetComponent<Damageable>();

        if(damageable != null)
        {
            // Get the parent (Knight) and its direction
            Knight knight = damageable.GetComponent<Knight>();
            Vector2 deliveredKnockback = knockback;

            // Check if the hit object is a Knight and apply knockback in the direction it's moving
            if (knight != null)
            {
                deliveredKnockback.x = knight.WalkDirection == Knight.WalkableDirection.Right ? Mathf.Abs(knockback.x) : -Mathf.Abs(knockback.x);
            }
            else
            {
                // If it's not a Knight, use the usual knockback logic
                deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);
            }

            // Hit the target
            bool gotHit = damageable.Hit(attackDamage, deliveredKnockback);

            if(gotHit)
                Debug.Log(collision.name + " hit for " + attackDamage);
        }
    }
}
