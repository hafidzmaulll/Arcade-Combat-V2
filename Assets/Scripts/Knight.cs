using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class Knight : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Damageable damageable;
    PhotonView photonView;

    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;
    public float walkSpeed = 3f;

    public enum WalkableDirection { Right, Left }

    private Vector2 walkDirectionVector = Vector2.right;
    private WalkableDirection _walkDirection;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                walkDirectionVector = value == WalkableDirection.Right ? Vector2.right : Vector2.left;
            }
            _walkDirection = value;
        }
    }

    public bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public float AttackCooldown
    {
        get { return animator.GetFloat(AnimationStrings.attackCooldown); }
        private set { animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0)); }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        HasTarget = attackZone.detectedColliders.Count > 0;

        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        }

        if (!damageable.LockVelocity)
        {
            if (CanMove && (touchingDirections.IsGrounded || cliffDetectionZone.detectedColliders.Count == 0))
                rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
            else
                rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void FlipDirection()
    {
        WalkDirection = WalkDirection == WalkableDirection.Right ? WalkableDirection.Left : WalkableDirection.Right;
    }

    public void InstantDeath()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_InstantDeath", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_InstantDeath()
    {
        damageable.InstantKill();
    }

    public void OnDeath()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_OnDeath", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_OnDeath()
    {
        rb.velocity = Vector2.zero; // Stop horizontal movement
        rb.gravityScale = 2f; // Increase gravity to make it fall

        animator.SetBool(AnimationStrings.canMove, false);
        enabled = false; // Optionally disable the entire script
    }
}
