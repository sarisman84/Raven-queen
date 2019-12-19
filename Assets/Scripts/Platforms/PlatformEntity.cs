using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlatformEntity : EntityController2D
{
    //Info on about the platform
    public float speed;
    public float waitTime;
    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;

    public bool cyclic;

    public Waypoint[] localWaypoints;
    Waypoint[] globalWaypoints;


    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, PlayerController2D> playerPassengers = new Dictionary<Transform, PlayerController2D>();
    HashSet<Transform> movedPassangers;

    protected override void Awake()
    {
        base.Awake();
        rayCollisions.horizontalRayCount = rayCollisions.verticalRayCount = 8;
        CalculateRaySpacing();


        globalWaypoints = new Waypoint[localWaypoints.Length];
        for (int i = 0; i < globalWaypoints.Length; i++)
        {
            globalWaypoints[i].waypointPosition = localWaypoints[i].waypointPosition + new Vector2(transform.position.x, transform.position.y);
        }

    }


    //Get collisions working

    protected override void Update()
    {
        UpdateRaycastOrigins();
        Vector2 velocity = CalculatePlatformMovement();
        transform.Translate(velocity * Time.deltaTime);
    }


    //Problem Isolated
    Vector2 CalculatePlatformMovement()
    {
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector2.Distance(globalWaypoints[fromWaypointIndex].waypointPosition, globalWaypoints[toWaypointIndex].waypointPosition);
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;

        Vector2 newPos = Vector2.Lerp(globalWaypoints[fromWaypointIndex].waypointPosition, globalWaypoints[toWaypointIndex].waypointPosition, percentBetweenWaypoints);
        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;
            if (!cyclic)
                if (globalWaypoints.Length - 1 <= fromWaypointIndex)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            nextMoveTime =  Time.time + globalWaypoints[toWaypointIndex].waitTime;
        }
        return newPos - new Vector2(transform.position.x, transform.position.y);

    }

    private void OnDrawGizmos()
    {
        if (localWaypoints == null) return;

        float size = .3f;

        for (int i = 0; i < localWaypoints.Length; i++)
        {
            Gizmos.color = localWaypoints[i].waypointColor;
            Vector2 globalWaypointPosition = (Application.isPlaying) ? globalWaypoints[i].waypointPosition : localWaypoints[i].waypointPosition + new Vector2(transform.position.x, transform.position.y);
            Gizmos.DrawLine(globalWaypointPosition - Vector2.up * size, globalWaypointPosition + Vector2.up * size);
            Gizmos.DrawLine(globalWaypointPosition - Vector2.left * size, globalWaypointPosition + Vector2.left * size);
        }
    }

    struct PassengerMovement
    {
        public Transform transform;
        public Vector2 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector2 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }

    }

    [System.Serializable]
    public struct Waypoint
    {
        public Color waypointColor;
        public Vector2 waypointPosition;
        public float waitTime;
    }
}


