using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{



    protected float horizontalRaySpacing;


    protected float verticalRaySpacing;
    [Header("Collision")]
    public LayerMask collisionMask;
    public CollisionInfo collisions;
    protected BoxCollider2D col;
    protected BoxCollider2D defaultCollider;
    public BoxCollider2D crouchCollider;
    protected RaycastOrigins raycastOrigins;

    [Tooltip("Determines the amount of checks (rays) the horizontal collisions will have.")] public int horizontalRayCount = 4;
    [Tooltip("Determines the amount of checks (rays) the vertical collisions will have.")] public int verticalRayCount = 4;
    protected const float skinWidth = .015f;

    protected virtual void Start()
    {
        defaultCollider = GetComponent<BoxCollider2D>();
        col = defaultCollider;
        if (crouchCollider != null)
            crouchCollider.enabled = false;
        CalculateRaySpacing();
    }
    protected void UpdateRaycastOrigins()
    {
        Bounds bounds = col.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    protected void UpdatingCollider(bool value)
    {
        if (crouchCollider == null) return;
        if (value)
        {
            col.enabled = false;
            crouchCollider.enabled = true;
            col = crouchCollider;
            CalculateRaySpacing();
            return;
        }
        crouchCollider.enabled = false;
        defaultCollider.enabled = true;
        col = defaultCollider;
        CalculateRaySpacing();
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = col.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    protected struct RaycastOrigins
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
