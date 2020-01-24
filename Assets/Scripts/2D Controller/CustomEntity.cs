using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public delegate void MoveBehaivour (ref Vector2 velocity);

[RequireComponent (typeof (BoxCollider2D))]
public class CustomEntity : MonoBehaviour {
    [Header ("Debug")]
    public bool drawRays = true;
    public Color rayColor = Color.red;
    protected RayCollisions2D rayCollisions;
    protected BoxCollider2D boxCollider2D;

    protected const float skinWidth = RayCollisions2D.skinWidth;

    public bool IsGrounded {
        get => rayCollisions.collision.below;
        set => rayCollisions.collision.below = value;
    }

    public bool IsCollidingAbove => rayCollisions.collision.above;
    public bool IsCollidingLeft => rayCollisions.collision.left;
    public bool IsCollidingRight => rayCollisions.collision.right;

    public bool IsClimbingASlope { get => rayCollisions.collision.climbingSlope; set => rayCollisions.collision.climbingSlope = value; }
    public bool IsDecendingASlope { get => rayCollisions.collision.decendingSlope; set => rayCollisions.collision.decendingSlope = value; }
    public float CurrentSlopeAngle { get => rayCollisions.collision.slopeAngle; set => rayCollisions.collision.slopeAngle = value; }
    public float PreviousSlopeAngle { get => rayCollisions.collision.slopeAngleOld; }

    public Vector2 OldVelocity { get => rayCollisions.collision.velocityOld; set => rayCollisions.collision.velocityOld = value; }

    protected void ResetInfo () {
        rayCollisions.collision.Reset ();
    }
    protected virtual void Awake () {
        //Get RayCollision Info

        GetRayCollisions ();
        CalculateRaySpacing ();

    }

    private void GetRayCollisions () {
        boxCollider2D = GetComponent<BoxCollider2D> ();
        rayCollisions = new RayCollisions2D (boxCollider2D);
        rayCollisions.horizontalRayCount = rayCollisions.verticalRayCount = 4;
    }

    public void Move (Vector2 velocity) {
        UpdateRaycastOrigins ();
        ResetInfo ();
        rayCollisions.collision.velocityOld = velocity;

        if (velocity.x != 0)
            BaseHorizontalCollisions (ref velocity);
        if (velocity.y != 0)
            BaseVerticalCollisions (ref velocity);

        transform.Translate (velocity);
    }

    public void Move (Vector2 velocity, MoveBehaivour method) {
        UpdateRaycastOrigins ();
        ResetInfo ();

        if (velocity.y < 0) method (ref velocity);
        if (velocity.x != 0)
            BaseHorizontalCollisions (ref velocity);
        if (velocity.y != 0)
            BaseVerticalCollisions (ref velocity);

        transform.Translate (velocity);
    }

    //The base behaivour of vertical collisions
    protected void BaseVerticalCollisions (ref Vector2 velocity) {
        float directionY = Mathf.Sign (velocity.y);
        float rayLength = Mathf.Abs (velocity.y) + skinWidth;

        for (int i = 0; i < rayCollisions.verticalRayCount; i++) {
            Vector2 rayOrigin = (directionY == -1) ? rayCollisions.collisionOrigin.bottomLeft : rayCollisions.collisionOrigin.topLeft;
            rayOrigin += Vector2.right * (rayCollisions.verticalRaySpacing * i + velocity.x);
            if (drawRays)
                Debug.DrawRay (rayOrigin, Vector2.up * directionY * rayLength, rayColor);
            RaycastHit2D[] hits = Physics2D.RaycastAll (rayOrigin, Vector2.up * directionY, rayLength);
            foreach (var hit in hits) {
                if (!hit || hit.transform.gameObject == gameObject) continue;
                VerticalCollisionBehaivour (i, hit, directionY, ref rayLength, ref velocity);
                rayCollisions.collision.below = directionY == -1;
                rayCollisions.collision.above = directionY == 1;
                break;
            }

        }
        VerticalCollisionBehaivour (directionY, ref rayLength, ref velocity);
    }

    protected void BaseHorizontalCollisions (ref Vector2 velocity) {
        float directionX = Mathf.Sign (velocity.x);
        float rayLength = Mathf.Abs (velocity.x) + skinWidth;

        for (int i = 0; i < rayCollisions.horizontalRayCount; i++) {
            Vector2 rayOrigin = (directionX == -1) ? rayCollisions.collisionOrigin.bottomLeft : rayCollisions.collisionOrigin.bottomRight;
            rayOrigin += Vector2.up * (rayCollisions.horizontalRaySpacing * i);
            if (drawRays)
                Debug.DrawRay (rayOrigin, Vector2.right * directionX * rayLength, rayColor);
            RaycastHit2D[] hits = Physics2D.RaycastAll (rayOrigin, Vector2.right * directionX, rayLength);
            foreach (var hit in hits) {
                if (!hit || hit.transform.gameObject == gameObject) continue;
                HorizontalCollisionBehaivour (i, hit, directionX, ref rayLength, ref velocity);
                rayCollisions.collision.left = directionX == -1;
                rayCollisions.collision.right = directionX == 1;
                break;
            }

        }
    }

    /// <summary>
    /// Determines how the horizontal collisions behave on the object. The calculation is created using raycasts.
    /// </summary>
    /// <param name="hit">Current Information of the detected Object</param>
    /// <param name="curDirection">The current horizontal direction.</param>
    /// <param name="rayLength">How long the ray itself is.</param>
    /// <param name="velocity">The current velocity of the object.</param>
    protected virtual void HorizontalCollisionBehaivour (int i,
        RaycastHit2D hit,
        float curDirection,
        ref float rayLength,
        ref Vector2 velocity) {
        velocity.x = (hit.distance - skinWidth) * curDirection;
        rayLength = hit.distance;
    }

    /// <summary>
    /// Determines how the vertical collisions behave on the object. The calculation is created using raycasts.
    /// </summary>
    /// <param name="hit">Current Information of the detected Object</param>
    /// <param name="curDirection">The current vertical direction.</param>
    /// <param name="rayLength">How long the ray itself is.</param>
    /// <param name="velocity">The current velocity of the object.</param>
    protected virtual void VerticalCollisionBehaivour (
        int i, RaycastHit2D hit, float curDirection, ref float rayLength, ref Vector2 velocity) {
        velocity.y = (hit.distance - skinWidth) * curDirection;
        rayLength = hit.distance;
    }

    protected virtual void VerticalCollisionBehaivour (float curDirection, ref float rayLength, ref Vector2 velocity) {

    }

    protected void UpdateRaycastOrigins () {
        rayCollisions.UpdateRaycastOrigins ();
    }

    protected void CalculateRaySpacing () {
        rayCollisions.CalculateRaySpacing ();
    }
}