using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviourPun, IPunObservable
{   
    Rigidbody2D rb;
    Animator animator;
    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;
    PhotonView view;

    public float jumpImpulse = 8f;
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    private Checkpoint currentCheckpoint;
    public GameObject Mark;

    // Dash variables
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    [SerializeField] private TrailRenderer tr;

    // Player facing direction
    [SerializeField]
    private bool _isFacingRight = true;
    public bool IsFacingRight 
    {
        get { return _isFacingRight; }
        private set 
        {
            if (_isFacingRight != value)
            {
                //Flip the local scale to make the player face the opposite direction
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                _isFacingRight = value;

                // Synchronize the facing direction across the network
                if (view.IsMine)
                {
                    photonView.RPC("SyncFacingDirection", RpcTarget.OthersBuffered, _isFacingRight);
                }
            }
        }
    }

    // Player movement and state
    public bool CanMove 
    { 
        get { return animator.GetBool(AnimationStrings.canMove); }
        private set { animator.SetBool(AnimationStrings.canMove, value); }
    }

    public bool IsAlive
    {
        get { return animator.GetBool(AnimationStrings.isAlive); }
        private set { animator.SetBool(AnimationStrings.isAlive, value); }
    }

    public float CurrentMoveSpeed 
    { 
        get
        {
            if(CanMove)
            {
                if(IsMoving && !touchingDirections.IsOnWall)
                {
                    return IsRunning ? runSpeed : walkSpeed;
                }
                else
                {
                    // Idle speed is 0
                    return 0;
                }
            }
            else
            {
                // Movement locked
                return 0;
            }
        }
    }

    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving 
    { 
        get { return _isMoving; }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    [SerializeField]
    private bool _isRunning = false;
    public bool IsRunning
    {
        get { return _isRunning; }
        private set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }
    
    private void Awake()
    {  
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
        view = GetComponent<PhotonView>();

        damageable.healthChanged.AddListener(OnHealthChanged);
    }

    void Update()
    {
        if(view.IsMine)
        {
            if(isDashing)
            {
                return;
            }

            if(Input.GetKeyDown(KeyCode.LeftControl) && canDash)
            {
                StartCoroutine(Dash());
            }
        }
    }

    private void FixedUpdate()
    {
        if(view.IsMine)
        {
            if(isDashing)
            {
                return;
            }
        
            if(!damageable.LockVelocity)
                rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);

            animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if(view.IsMine)
        {
            if(moveInput.x > 0 && !IsFacingRight)
            {
                //Face the right
                IsFacingRight = true;
            }
            else if(moveInput.x < 0 && IsFacingRight)
            {
                //Face the left
                IsFacingRight = false;
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(view.IsMine)
        {
            moveInput = context.ReadValue<Vector2>();

            if(IsAlive)
            {
                IsMoving = moveInput != Vector2.zero;
                SetFacingDirection(moveInput);
            } 
            else
            {
                IsMoving = false;
            }
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if(view.IsMine)
        {
            if(context.started)
            {
                IsRunning = true;
            }
            else if(context.canceled)
            {
                IsRunning = false;
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(view.IsMine)
        {
            if(context.started && touchingDirections.IsGrounded && CanMove)
            {
                animator.SetTrigger(AnimationStrings.jumpTrigger);
                rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(view.IsMine)
        {
            if(context.started)
            {
                animator.SetTrigger(AnimationStrings.attackTrigger);
            }
        }
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if(view.IsMine)
        {
            if(context.started)
            {
                animator.SetTrigger(AnimationStrings.rangedAttackTrigger);
            }
        }
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        if(view.IsMine)
        {
            rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void OnHealthChanged(float health, float MaxHealth)
    {
        if(view.IsMine)
        {
            if (health <= 0)
            {
                StartCoroutine(RespawnAfterDelay());
            }
        }
    }

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        if(photonView.IsMine)
        {
            currentCheckpoint = checkpoint;
            Debug.Log("Checkpoint set at: " + checkpoint.transform.position);
        }
    }

    [PunRPC]
    public void InstantDeath()
    {
        if(view.IsMine)
        {
            damageable.InstantKill();
            StartCoroutine(RespawnAfterDelay());
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        if(view.IsMine)
        {
            yield return new WaitForSeconds(2f); // Adjust this delay as necessary

            Respawn();
        }
    }

    private void Respawn()
    {
        if(view.IsMine)
        {
            if(currentCheckpoint != null && photonView.IsMine)
            {
                transform.position = currentCheckpoint.transform.position;
                damageable.Health = damageable.MaxHealth;
                rb.velocity = Vector2.zero;

                damageable.IsAlive = true;
                CanMove = true;
                IsMoving = false;
                IsRunning = false;

                Debug.Log("Player respawned at checkpoint");
            }
            else
            {
                Debug.LogWarning("No checkpoint set");
            }
        }
    }

    // New Teleport Method
    public void Teleport(Vector3 newPosition)
    {
        if(view.IsMine)
        {
            transform.position = newPosition;
            rb.velocity = Vector2.zero;  // Reset velocity to prevent carryover momentum
        }
    }

    // Synchronize the facing direction across the network
    [PunRPC]
    private void SyncFacingDirection(bool isFacingRight)
    {
        if (isFacingRight != IsFacingRight)
        {
            IsFacingRight = isFacingRight;
        }
    }

    // Photon serialization for synchronization
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send facing direction to other players
            stream.SendNext(IsFacingRight);
        }
        else
        {
            // Receive facing direction from the network
            IsFacingRight = (bool)stream.ReceiveNext();
        }
    }
}
