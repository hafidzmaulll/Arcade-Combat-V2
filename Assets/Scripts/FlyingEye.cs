using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FlyingEye : MonoBehaviourPun, IPunObservable
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
        if (waypoints.Count > 0)
        {
            nextWaypoint = waypoints[waypointNum];
        }
    }

    private void OnEnable()
    {
        damageable.damageableDeath.AddListener(OnDeath);
    }

    // Update is called once per frame
    void Update()
    {
        if(!damageable.IsAlive) return;

        HasTarget = biteDetectionZone.detectedColliders.Count > 0;
    }

    private void FixedUpdate()
    {
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
        if(!damageable.IsAlive) return;

        if (nextWaypoint == null) return;

        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        rb.velocity = directionToWaypoint * flightSpeed;
        UpdateDirection();

        if(distance < waypointReachedDistance)
        {
            waypointNum++;
            if(waypointNum >= waypoints.Count)
            {
                waypointNum = 0;
            }
            nextWaypoint = waypoints[waypointNum];
        }
    }

    private void UpdateDirection()
    {
        if(!damageable.IsAlive) return;

        Vector3 locScale = transform.localScale;

        if(transform.localScale.x > 0)
        {
            if(rb.velocity.x < 0)
            {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
        else
        {
            if(rb.velocity.x > 0)
            {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
    }

    public void OnDeath()
    {
        photonView.RPC("RPC_OnDeath", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_OnDeath()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 2f;
        deathCollider.enabled = true;
        animator.SetBool(AnimationStrings.canMove, false);
        enabled = false;
    }

    public void SetWaypoints(List<Transform> newWaypoints)
    {
        waypoints = newWaypoints;
        waypointNum = 0;
        if (waypoints.Count > 0)
        {
            nextWaypoint = waypoints[waypointNum];
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send relevant data to other players
            stream.SendNext(damageable.IsAlive);
        }
        else
        {
            // Receive data from other players
            bool isAlive = (bool)stream.ReceiveNext();
            if (!isAlive && damageable.IsAlive)
            {
                OnDeath();
            }
        }
    }
}
