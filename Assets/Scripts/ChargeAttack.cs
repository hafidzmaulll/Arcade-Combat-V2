using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttack : MonoBehaviour
{
    public float attackDamage = 10;
    public Vector2 knockback = Vector2.zero;

    private void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek Apakah Bisa Diserang
        Damageable damageable = collision.GetComponent<Damageable>();

        if(damageable != null)
        {
            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            // Serangan Mengenai Target
            bool gotHit = damageable.Hit(attackDamage, deliveredKnockback);

            if(gotHit)
                Debug.Log(collision.name + " hit for " + attackDamage);
        }
    }
}

