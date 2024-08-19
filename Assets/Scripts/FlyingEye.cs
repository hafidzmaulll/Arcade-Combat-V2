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

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
        networkPosition = transform.position;
        networkRotation = transform.rotation;
    }

    private void OnEnable()
    {
        damageable.damageableDeath.AddListener(OnDeath);
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            nextWaypoint = waypoints[waypointNum];
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            HasTarget = biteDetectionZone.detectedColliders.Count > 0;
        }
    }

    private void FixedUpdate()
    {
        if (!damageable.IsAlive) return;

        if (PhotonNetwork.IsMasterClient)
        {
            if (CanMove)
            {
                Flight();
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
        else
        {
            // Interpolate position and rotation for smoother movement on non-master clients
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.fixedDeltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.fixedDeltaTime * 10);
        }
    }

    private void Flight()
    {
        if (!damageable.IsAlive) return;

        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        rb.velocity = directionToWaypoint * flightSpeed;
        UpdateDirection();

        if (distance < waypointReachedDistance)
        {
            waypointNum++;

            if (waypointNum >= waypoints.Count)
            {
                waypointNum = 0;
            }

            nextWaypoint = waypoints[waypointNum];
        }
    }

    [PunRPC]
    private void ChangeDirection(int direction)
    {
        Vector3 locScale = transform.localScale;
        transform.localScale = new Vector3(direction * Mathf.Abs(locScale.x), locScale.y, locScale.z);
    }

    private void UpdateDirection()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 locScale = transform.localScale;

            if (transform.localScale.x > 0 && rb.velocity.x < 0)
            {
                photonView.RPC("ChangeDirection", RpcTarget.AllBuffered, -1);
            }
            else if (transform.localScale.x < 0 && rb.velocity.x > 0)
            {
                photonView.RPC("ChangeDirection", RpcTarget.AllBuffered, 1);
            }
        }
    }

    public void OnDeath()
    {
        photonView.RPC("HandleDeath", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void HandleDeath()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 2f;
        deathCollider.enabled = true;
        animator.SetBool(AnimationStrings.canMove, false);
        enabled = false;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(damageable.IsAlive);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            damageable.IsAlive = (bool)stream.ReceiveNext();
        }
    }
}
