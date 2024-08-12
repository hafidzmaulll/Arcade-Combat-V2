using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;
    Transform nextWaypoint;

    public float flightSpeed = 2f;
    public float waypointReachedDistance = 0.1f;

    public DetectionZone biteDetectionZone;
    public Collider2D deathCollider;
    public List<Transform> waypoints;

    int waypointNum = 0;

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

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        nextWaypoint = waypoints[waypointNum];
    }

    private void OnEnable()
    {
        damageable.damageableDeath.AddListener(OnDeath);
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure no movement logic occurs if the enemy is dead
        if(!damageable.IsAlive) return;

        HasTarget = biteDetectionZone.detectedColliders.Count > 0;
    }

    private void FixedUpdate()
    {
        // Ensure no movement logic occurs if the enemy is dead
        if(!damageable.IsAlive) return;

        if(CanMove)
        {
            Flight();
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void Flight()
    {
        // Ensure no movement logic occurs if the enemy is dead
        if(!damageable.IsAlive) return;

        // Fly to next waypoint
        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        // Check if we have reached the waypoint already
        float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        rb.velocity = directionToWaypoint * flightSpeed;
        UpdateDirection();

        // See if we need to switch waypoints
        if(distance < waypointReachedDistance)
        {
            // Switch to next waypoint
            waypointNum++;

            if(waypointNum >= waypoints.Count)
            {
                // Loop back to original waypoint
                waypointNum = 0;
            }

            nextWaypoint = waypoints[waypointNum];
        }
    }

    private void UpdateDirection()
    {
        // Ensure no movement logic occurs if the enemy is dead
        if(!damageable.IsAlive) return;

        Vector3 locScale = transform.localScale;

        if(transform.localScale.x > 0)
        {
            // Facing the right
            if(rb.velocity.x < 0)
            {
                // Flip
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
        else
        {
            // Facing the left
            if(rb.velocity.x > 0)
            {
                // Flip
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
    }

    public void OnDeath()
    {
        // Stop all movement and fall to the ground
        rb.velocity = Vector2.zero; // Stop horizontal movement
        rb.gravityScale = 2f; // Increase gravity to make it fall
        deathCollider.enabled = true; // Enable the collider for death

        // Disable the ability to move further
        animator.SetBool(AnimationStrings.canMove, false);
        enabled = false; // Optionally disable the entire script
    }
}
