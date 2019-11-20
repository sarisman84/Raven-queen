using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision2DController {

    public const float skinWidth = .015f;
    protected RaycastOrigins raycastOrigins;
    protected CollisionInfo collision;
    protected LayerMask collisionMask;
    protected int horizontalRayCount, verticalRayCount;

    Collider2D col;

    protected float horizontalRaySpacing, verticalRaySpacing;

    public Collision2DController (Collider2D _col) {
        col = _col;
        CalculateRaySpacing (horizontalRayCount, verticalRayCount);
    }

    public void UpdateRaycastOrigins () {
        Bounds bounds = col.bounds;
        bounds.Expand (skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing (int horizontalRayCount, int verticalRayCount) {
        Bounds bounds = col.bounds;
        bounds.Expand (skinWidth * -2);

        horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public float slopeAngle, slopeAngleOld;
        public bool decendingSlope;

        public Vector3 velocityOld;

        public void Reset () {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            decendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}