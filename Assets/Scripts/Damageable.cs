using System.Collections;
using System.Collections.Generic;
using Photon.Pun; // Add this for Photon
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviourPun, IPunObservable // Implement IPunObservable
{
    Animator animator;

    public UnityEvent damageableDeath;
    public UnityEvent<float, Vector2> damageableHit;    
    public UnityEvent<float, float> healthChanged;

    [SerializeField]
    private float _maxHealth = 100;
    public float MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }

    [SerializeField]
    private float _health = 100;
    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth); // Clamp the health value between 0 and MaxHealth
            healthChanged?.Invoke(_health, MaxHealth);

            // if health drops below 0, character is no longer alive
            if (_health <= 0)
            {
                IsAlive = false;
                animator.SetTrigger(AnimationStrings.deathTrigger);
            }
        }
    }

    [SerializeField]
    private bool isInvincible = false;
    private float timeSinceHit = 0;
    public float invincibilityTime = 1f;

    [SerializeField]
    private bool _isAlive = true;
    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive set " + value);
        }
    }

    // The velocity should not be changed while this is true but needs to be respected by other physic components
    // like the player controller
    public bool LockVelocity 
    { 
        get 
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        } 
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                // Remove invincibility
                isInvincible = false;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        }
    }

    // Returns whether the damageable took damage or not
    public bool Hit(float damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            // Notify other subscribed components that the damageable was hit to handle the knockback and such
            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            damageableHit?.Invoke(damage, knockback);
            CharacterEvents.characterDamaged.Invoke(gameObject, damage);

            return true;
        }
        // Unable to hit
        return false;
    }

    public bool Heal(float healthRestore)
    {
        if (IsAlive && Health < MaxHealth)
        {
            float maxHeal = Mathf.Max(MaxHealth - Health, 0);
            float actualHeal = Mathf.Min(maxHeal, healthRestore);
            Health += actualHeal;
            CharacterEvents.characterHealed.Invoke(gameObject, actualHeal);
            return true;
        }
        return false;
    }

    public void InstantKill()
    {
        Health = 0;
        animator.SetTrigger(AnimationStrings.deathTrigger);
        IsAlive = false;
    }

    // Implement the IPunObservable interface to sync data
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // MasterClient sends data
            stream.SendNext(IsAlive);
            stream.SendNext(Health);
        }
        else
        {
            // Other clients receive data
            IsAlive = (bool)stream.ReceiveNext();
            Health = (float)stream.ReceiveNext();
        }
    }
}
