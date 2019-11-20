using System;
using UnityEngine;

public class Entity : Collision2DController {
    public float maxClimbAngle = 80, maxDecendAngle = 75;
    float gravity;
    bool airborneControl;

    Transform entity;

    EntityInfo info;

    public EntityInfo UpdateInfo {
        set {
            info = value;
            horizontalRayCount = info.horizontalRayCount;
            verticalRayCount = info.verticalRayCount;
            collisionMask = info.collisionMask;
            maxClimbAngle = info.maxClimbAngle;
            maxDecendAngle = info.maxDecendAngle;
            airborneControl = info.isAirborne;
        }
    }

    public Entity (Collider2D _col, Transform _entity, EntityInfo _info) : base (_col) { entity = _entity; UpdateInfo = _info; }

    private void CalculateJumpVelocityAndGravity () {
        gravity = -(2 * jumpHeight) / Mathf.Pow (jumpSpeed, 2);
        jumpForce = Mathf.Abs (gravity) * jumpSpeed;
        print ($"Gravity {gravity}, Jump Force: {jumpForce}");
    }

    public void MoveEntity (Vector2 velocity, bool useGravity, bool standingOnPlatform = false) {
        UpdateRaycastOrigins ();
        collision.Reset ();

        if (collision.below || airborneControl) {

        }

        if (useGravity) {
            velocity.y += gravity * Time.deltaTime;
        }

        collision.velocityOld = velocity;
        if (velocity.y < 0) {
            DecendSlope (ref velocity);
        }
        if (velocity.x != 0) {
            HorizontalCollisions (ref velocity);
        }
        if (velocity.y != 0) {
            VerticalCollisions (ref velocity);
        }
        entity.Translate (velocity);
        if (standingOnPlatform) {
            collision.below = true;
        }

    }

    private void VerticalCollisions (ref Vector2 velocity) {
        float directionY = Mathf.Sign (velocity.y);
        float rayLength = Mathf.Abs (velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++) {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay (rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit) {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collision.climbingSlope) {
                    velocity.x = velocity.y / Mathf.Tan (collision.slopeAngle * Mathf.Deg2Rad * Mathf.Sign (velocity.x));
                }

                collision.below = directionY == -1;
                collision.above = directionY == 1;
            }
        }

        if (collision.climbingSlope) {
            float directionX = Mathf.Sign (velocity.x);
            rayLength = Mathf.Abs (velocity.x) + skinWidth;

            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            if (hit) {
                float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
                if (slopeAngle != collision.slopeAngle) {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collision.slopeAngle = slopeAngle;
                }
            }
        }
    }

    private void HorizontalCollisions (ref Vector2 velocity) {
        float directionX = Mathf.Sign (velocity.x);
        float rayLength = Mathf.Abs (velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++) {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay (rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit) {

                if (hit.distance == 0) {
                    continue;
                }
                float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle) {
                    if (collision.decendingSlope) {
                        collision.decendingSlope = false;
                        velocity = collision.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collision.slopeAngleOld) {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope (ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (!collision.climbingSlope || slopeAngle > maxClimbAngle) {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collision.climbingSlope) {
                        velocity.y = Mathf.Tan (collision.slopeAngle * Mathf.Deg2Rad * Math.Abs (velocity.x));
                    }

                    collision.left = directionX == -1;
                    collision.right = directionX == 1;
                }

            }
        }
    }

    private void ClimbSlope (ref Vector2 velocity, float slopeAngle) {
        float moveDistance = Mathf.Abs (velocity.x);
        float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelocityY) {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
            collision.below = true;

            collision.climbingSlope = true;
            collision.slopeAngle = slopeAngle;
        }
    }

    private void DecendSlope (ref Vector2 velocity) {
        float directionX = Mathf.Sign (velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit) {
            float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDecendAngle) {
                if (Math.Sign (hit.normal.x) == directionX) {
                    if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad * Mathf.Abs (velocity.x))) {
                        float moveDistance = Mathf.Abs (velocity.x);
                        float decendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
                        velocity.y -= decendVelocityY;
                        collision.slopeAngle = slopeAngle;
                        collision.decendingSlope = true;
                        collision.below = true;
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class EntityInfo {
    public float maxClimbAngle = 80, maxDecendAngle = 75;
    public int horizontalRayCount, verticalRayCount;

    public LayerMask collisionMask;

    public bool useGravity;
    public float gravity;

    public bool isAirborne;

}