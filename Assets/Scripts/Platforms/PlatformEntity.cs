using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlatformEntity : EntityController2D
{
    [Header("Platform Information")]
    public float speed;
    [Range(0, 2)]
    public float easeAmount;
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

    protected override void Update()
    {
        UpdateRaycastOrigins();
        Vector2 velocity = CalculatePlatformMovement();
        CalculatePassengerMovement(velocity);


        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    float Ease(float x)
    {
        float a = easeAmount + 1;

        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    Vector2 CalculatePlatformMovement()
    {
        cyclic = (globalWaypoints.Length == 2 && !cyclic) ? true : cyclic;
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector2.Distance(globalWaypoints[fromWaypointIndex].waypointPosition, globalWaypoints[toWaypointIndex].waypointPosition);
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector2 newPos = Vector2.Lerp(globalWaypoints[fromWaypointIndex].waypointPosition, globalWaypoints[toWaypointIndex].waypointPosition, easedPercentBetweenWaypoints);
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
            nextMoveTime = Time.time + localWaypoints[toWaypointIndex].waitTime;
        }
        //Debug.Log($"{newPos - new Vector2(transform.position.x, transform.position.y)} where newPos is : {newPos}, currentPos is: {transform.position} and easedPercent is {easedPercentBetweenWaypoints}");
        return newPos - new Vector2(transform.position.x, transform.position.y);

    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach (PassengerMovement passenger in passengerMovement)
        {
            if (!playerPassengers.ContainsKey(passenger.transform))
            {
                PlayerController2D player = passenger.transform.GetComponent<PlayerController2D>();
                if (player == null) continue;
                playerPassengers.Add(passenger.transform, player);
            }
            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {

                PlayerController2D controller = playerPassengers[passenger.transform];
                controller.Move(passenger.velocity);
                controller.IsGrounded = true;
            }
        }
    }

    void CalculatePassengerMovement(Vector2 velocity)
    {

        movedPassangers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        //Vertically moving platform
        if (velocity.y != 0)
        {
            BaseVerticalCollisions(ref velocity);
        }

        //Horizontally moving platform
        if (velocity.x != 0)
        {
            BaseHorizontalCollisions(ref velocity);
        }

        //Passenger on top of a horizontally or downward moving platform
        float directionY = Mathf.Sign(velocity.y);
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = RayCollisions2D.skinWidth * 2;
            for (int i = 0; i < rayCollisions.verticalRayCount; i++)
            {
                Vector2 rayOrigin = rayCollisions.collisionOrigin.topLeft + Vector2.right * (rayCollisions.verticalRaySpacing * i);
                RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.up, rayLength);
                if (drawRays)
                    Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, rayColor);
                foreach (var hit in hits)
                {
                    if (hit && hit.transform.gameObject != gameObject && !movedPassangers.Contains(hit.transform))
                    {
                        movedPassangers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), true, false));
                        break;
                    }
                }

            }
        }
    }

    protected override void VerticalCollisionBehaivour(int i, RaycastHit2D hit, float curDirection, ref float rayLength, ref Vector2 velocity)
    {
        if (hit || movedPassangers.Contains(hit.transform)) return;

        movedPassangers.Add(hit.transform);
        float pushX = (curDirection == 1) ? velocity.x : 0;
        float pushY = velocity.y - (hit.distance - skinWidth) * curDirection;
        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), curDirection == 1, true));


    }

    protected override void HorizontalCollisionBehaivour(int i, RaycastHit2D hit, float curDirection, ref float rayLength, ref Vector2 velocity)
    {
        if (hit || movedPassangers.Contains(hit.transform)) return;

        movedPassangers.Add(hit.transform);
        float pushX = velocity.x - (hit.distance - skinWidth) * curDirection;
        float pushY = -skinWidth;

        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), false, true));
    }

    // protected void VerticalCollisions(ref Vector2 velocity)
    // {
    //     float directionY = Mathf.Sign(velocity.y);
    //     float rayLength = Mathf.Abs(velocity.y) + skinWidth;
    //     for (int i = 0; i < rayCollisions.verticalRayCount; i++)
    //     {
    //         Vector2 rayOrigin = (directionY == -1) ? rayCollisions.collisionOrigin.bottomLeft : rayCollisions.collisionOrigin.topLeft;
    //         rayOrigin += Vector2.right * (rayCollisions.verticalRaySpacing * i);
    //         RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.up * directionY, rayLength);
    //         if (drawRays)
    //             Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, rayColor);
    //         foreach (var hit in hits)
    //         {
    //             if (hit || hit.transform.gameObject == gameObject || movedPassangers.Contains(hit.transform)) continue;

    //             movedPassangers.Add(hit.transform);
    //             float pushX = (directionY == 1) ? velocity.x : 0;
    //             float pushY = velocity.y - (hit.distance - skinWidth) * directionY;
    //             passengerMovement.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), directionY == 1, true));
    //             break;


    //         }

    //     }
    // }

    // protected void HorizontalCollisions(ref Vector2 velocity)
    // {
    //     float directionX = Mathf.Sign(velocity.x);
    //     float rayLength = Mathf.Abs(velocity.x) + skinWidth;
    //     for (int i = 0; i < rayCollisions.horizontalRayCount; i++)
    //     {
    //         Vector2 rayOrigin = (directionX == -1) ? rayCollisions.collisionOrigin.bottomLeft : rayCollisions.collisionOrigin.bottomRight;
    //         rayOrigin += Vector2.up * (rayCollisions.horizontalRaySpacing * i);
    //         RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.right * directionX, rayLength);
    //         if (drawRays)
    //             Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, rayColor);
    //         foreach (var hit in hits)
    //         {
    //             if (hit || hit.transform.gameObject == gameObject || movedPassangers.Contains(hit.transform)) continue;

    //             movedPassangers.Add(hit.transform);
    //             float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
    //             float pushY = -skinWidth;

    //             passengerMovement.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), false, true));
    //             break;


    //         }

    //     }




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


