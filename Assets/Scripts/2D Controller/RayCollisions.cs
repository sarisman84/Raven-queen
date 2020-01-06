using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCollisions2D
{
    public const float skinWidth = .015f;
    public RaycastOrigins collisionOrigin;
    public CollisionInfo collision;
    public LayerMask collisionMask;
    public int horizontalRayCount, verticalRayCount;

    public Collider2D col;

    public float horizontalRaySpacing, verticalRaySpacing;

    public RayCollisions2D(Collider2D _col)
    {
        col = _col;
        CalculateRaySpacing(horizontalRayCount, verticalRayCount);
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = col.bounds;
        bounds.Expand(skinWidth * -2);

        collisionOrigin.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        collisionOrigin.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        collisionOrigin.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        collisionOrigin.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing(int horizontalRayCount, int verticalRayCount)
    {
        Bounds bounds = col.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
    public void CalculateRaySpacing()
    {
        CalculateRaySpacing(horizontalRayCount, verticalRayCount);
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public float slopeAngle, slopeAngleOld;
        public bool decendingSlope;

        public Vector3 velocityOld;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            decendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
