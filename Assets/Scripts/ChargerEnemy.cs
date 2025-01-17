using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class ChargerEnemy : MonoBehaviourPunCallbacks, IPunObservable
{
    Animator animator;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Damageable damageable;

    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;
    public float walkSpeed = 3f;
    public float walkSpeedOnAttack = 6f; // Speed when charging towards the player
    public enum WalkableDirection { Right, Left }
    public GameObject bossDefeatedCanvas;

    private Vector2 walkDirectionVector = Vector2.right;
    private WalkableDirection _walkDirection;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                // Direction flipped
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value;
        }
    }

    private bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            if (animator != null)
            {
                animator.SetBool(AnimationStrings.hasTarget, value);
            }
            else
            {
                Debug.LogError("Animator component is not assigned.");
            }
        }
    }

    public bool CanMove
    {
        get
        {
            return animator != null && animator.GetBool(AnimationStrings.canMove);
        }
    }

    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat(AnimationStrings.attackCooldown);
        }
        private set
        {
            animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();

        // Null checks for essential components
        if (animator == null) Debug.LogError("Animator component is missing.");
        if (rb == null) Debug.LogError("Rigidbody2D component is missing.");
        if (touchingDirections == null) Debug.LogError("TouchingDirections component is missing.");
        if (damageable == null) Debug.LogError("Damageable component is missing.");
        if (attackZone == null) Debug.LogError("AttackZone is not assigned.");
        if (cliffDetectionZone == null) Debug.LogError("CliffDetectionZone is not assigned.");
    }

    private void Start()
    {
        if(bossDefeatedCanvas != null)
        {
            bossDefeatedCanvas.SetActive(false);
        }
    }

    void Update()
    {
        if (attackZone != null)
        {
            HasTarget = attackZone.detectedColliders.Count > 0;
        }
        else
        {
            Debug.LogError("AttackZone is not assigned.");
        }

        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return; // Only allow the owner to update the state

        if (!damageable.IsAlive)
        {
            // Stop all movement and direction changes when the enemy is dead
            rb.velocity = Vector2.zero;

            // Show the pop-up when the boss is defeated
            if (bossDefeatedCanvas != null && photonView.IsMine)
            {
                bossDefeatedCanvas.SetActive(true);
            }
            
            return;
        }

        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall || cliffDetectionZone.detectedColliders.Count == 0)
        {
            FlipDirection();
        }

        if (!damageable.LockVelocity)
        {
            if (CanMove && touchingDirections.IsGrounded)
            {
                // Adjust speed based on whether a target is detected
                float currentSpeed = HasTarget ? walkSpeedOnAttack : walkSpeed;
                rb.velocity = new Vector2(currentSpeed * walkDirectionVector.x, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }

        photonView.RPC("RPC_FlipDirection", RpcTarget.OthersBuffered, WalkDirection);
    }

    [PunRPC]
    private void RPC_FlipDirection(WalkableDirection newDirection)
    {
        WalkDirection = newDirection;
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    public void OnCliffDetected()
    {
        if (touchingDirections != null && touchingDirections.IsGrounded)
        {
            FlipDirection();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.velocity);
            stream.SendNext(transform.localScale);
        }
        else
        {
            rb.position = (Vector2)stream.ReceiveNext();
            rb.velocity = (Vector2)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();
        }
    }
}
