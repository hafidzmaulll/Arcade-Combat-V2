using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    public float damage = 10;
    public Vector2 moveSpeed = new Vector2(3f, 0);
    public Vector2 knockback = new Vector2(0, 0);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // If you want the projectile to be affected by gravity by default, make it a dynamic mode Rigidbody
        rb.velocity = new Vector2(moveSpeed.x * transform.localScale.x, moveSpeed.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // See if it can be hit
        Damageable damageable = collision.GetComponent<Damageable>();

        if(damageable != null)
        {
            Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            // Hit the target
            bool gotHit = damageable.Hit(damage, deliveredKnockback);

            if(gotHit)
            {
                Debug.Log(collision.name + " hit for " + damage);
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            // If the collision object is not damageable (e.g., ground), destroy the projectile
            Destroy(gameObject);
        }
    }
}
