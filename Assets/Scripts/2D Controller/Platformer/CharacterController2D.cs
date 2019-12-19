// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class CharacterController2D : EntityController
// {
//     public LayerMask collisionMask;
//     [Header("Jump")]
//     public float jumpHeight = 4;
//     public float timeToJumpApex = .4f;
//     float jumpVelocity;

//     [Header("Movement")]
//     public float moveSpeed = 6;
//     public float slideTriggerSpeed = 5;
//     public float slidingAmount = 0.1f;

//     [Header("Crouch")]
//     public float crouchSpeed = 3;
//     public float collisionBoxHeight = 0.5f;


//     [Header("Slope Angles")]
//     public float maxClimbAngle = 80f;
//     public float maxDecendAngle = 75f;



//     public bool StandingOnGround => detections.collision.below;
//     public bool HittingACeiling => detections.collision.above;
//     public bool HittingObjectOnLeft => detections.collision.left;
//     public bool HittingObjectOnRight => detections.collision.right;


//     public bool IsCharacterSliding => isSliding;
//     public bool IsCharacterCrouching => isCrouching;


//     private bool isSliding;
//     private bool isCrouching;
//     private float velocityXSmoothing;
//     private float finalSpeed;


//     Vector2 size;
//     Vector2 offset;

//     BoxCollider2D boxCollider;


//     protected override void Awake()
//     {
//         base.Awake();
//         boxCollider = GetComponent<BoxCollider2D>();
//         CalculateGravityAndJumpVelocity();
//         StoreCollisionData();
//     }

//     private void StoreCollisionData()
//     {
//         size = boxCollider.size;
//         offset = boxCollider.offset;
//     }

//     private void CalculateGravityAndJumpVelocity()
//     {
//         gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
//         jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
//         //print($"Gravity: {gravity} & Jump Velocity: {jumpVelocity}");
//     }

//     private void OnValidate()
//     {
//         CalculateGravityAndJumpVelocity();
//     }

//     public void ControlCharacter(Vector2 input, bool jump, bool crouch)
//     {

//         if (detections.collision.above || detections.collision.below) vel.y = 0;

//         isSliding = (crouch && Mathf.Abs(vel.x) >= slideTriggerSpeed) ? true : (Mathf.Abs(vel.x) <= 0.1f) ? false : isSliding;
//         //Debug.Log($"Currently sliding: {isSliding}");
//         ChangeCollisionSize(false);
//         if (isSliding || crouch)
//         {
//             ChangeCollisionSize(true);
//         }


//         if (jump && StandingOnGround)
//         {
//             vel.y = jumpVelocity;
//         }
//         finalSpeed = (!isSliding && crouch) ? crouchSpeed : moveSpeed;

//         isCrouching = !isSliding && crouch;
//         Move(input);
//     }

//     private void ChangeCollisionSize(bool changeSize)
//     {
//         if (!changeSize)
//         {
//             boxCollider.size = size;
//             boxCollider.offset = offset;
//             detections.CalculateRaySpacing();
//             return;
//         }

//         boxCollider.size = new Vector2(1, collisionBoxHeight);
//         boxCollider.offset = new Vector2(0, -(0.5f - (boxCollider.size.y / 2)));
//         detections.CalculateRaySpacing();
//     }

//     new public void Move(Vector2 input)
//     {
//         float desiredVelocityX = input.x * finalSpeed;
//         vel.x = (isSliding) ? Mathf.SmoothDamp(velocity.x, desiredVelocityX, ref velocityXSmoothing, slidingAmount) : desiredVelocityX;
//         vel.y += gravity * Time.deltaTime;


//         Vector2 value = vel * Time.deltaTime;

//         detections.UpdateRaycastOrigins();
//         detections.collision.Reset();

//         detections.collision.velocityOld = value;

//         if (value.y < 0)
//         {
//             DecendSlope(ref value);
//         }
//         if (value.y != 0)
//             VerticalCollisions(ref value);
//         if (value.x != 0)
//             HorizontalCollisions(ref value);
//         transform.Translate(value);
//     }

//     public void RawMove(Vector2 _velocity)
//     {
//         detections.UpdateRaycastOrigins();
//         detections.collision.Reset();

//         detections.collision.velocityOld = _velocity;

//         if (_velocity.y < 0)
//         {
//             DecendSlope(ref _velocity);
//         }
//         if (_velocity.y != 0)
//             VerticalCollisions(ref _velocity);
//         if (_velocity.x != 0)
//             HorizontalCollisions(ref _velocity);
//         transform.Translate(_velocity);


//     }

   


//     protected override void VerticalCollisions(ref Vector2 velocity)
//     {
//         float directionY = Mathf.Sign(velocity.y);
//         float rayLength = Mathf.Abs(velocity.y) + RayCollisions2D.skinWidth;
//         for (int i = 0; i < detections.verticalRayCount; i++)
//         {
//             Vector2 rayOrigin = (directionY == -1) ? detections.collisionOrigin.bottomLeft : detections.collisionOrigin.topLeft;
//             rayOrigin += Vector2.right * (detections.verticalRaySpacing * i + velocity.x);
//             RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
//             Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, lineColor);

//             if (hit)
//             {
//                 velocity.y = (hit.distance - RayCollisions2D.skinWidth) * directionY;
//                 rayLength = hit.distance;
//                 if (detections.collision.climbingSlope)
//                 {
//                     velocity.x = velocity.y / Mathf.Tan(detections.collision.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
//                 }
//                 detections.collision.below = directionY == -1;
//                 detections.collision.above = directionY == 1;
//             }

//         }
//         if (detections.collision.climbingSlope)
//         {
//             float directionX = Mathf.Sign(velocity.x);
//             rayLength = Mathf.Abs(velocity.x) + RayCollisions2D.skinWidth;
//             Vector2 rayOrigin = ((directionX == -1) ? detections.collisionOrigin.bottomLeft : detections.collisionOrigin.bottomRight) * Vector2.up * velocity.y;
//             RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

//             if (hit)
//             {
//                 float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
//                 if (slopeAngle != detections.collision.slopeAngle)
//                 {
//                     velocity.x = (hit.distance - RayCollisions2D.skinWidth) * directionX;
//                     detections.collision.slopeAngle = slopeAngle;
//                 }
//             }



//         }
//     }

//     protected override void HorizontalCollisions(ref Vector2 velocity)
//     {
//         float directionX = Mathf.Sign(velocity.x);
//         float rayLength = Mathf.Abs(velocity.x) + RayCollisions2D.skinWidth;
//         for (int i = 0; i < detections.horizontalRayCount; i++)
//         {
//             Vector2 rayOrigin = (directionX == -1) ? detections.collisionOrigin.bottomLeft : detections.collisionOrigin.bottomRight;
//             rayOrigin += Vector2.up * (detections.horizontalRaySpacing * i);
//             RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
//             Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, lineColor);

//             if (hit)
//             {
//                 if (hit.distance == 0) continue;
//                 float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

//                 if (i == 0 && slopeAngle <= maxClimbAngle)
//                 {
//                     if (detections.collision.decendingSlope)
//                     {
//                         detections.collision.decendingSlope = false;
//                         velocity = detections.collision.velocityOld;
//                     }
//                     float distanceToSlopeStart = 0;
//                     if (slopeAngle != detections.collision.slopeAngleOld)
//                     {
//                         distanceToSlopeStart = hit.distance - RayCollisions2D.skinWidth;
//                         velocity.x -= distanceToSlopeStart * directionX;
//                     }
//                     ClimbSlope(ref velocity, slopeAngle);
//                     velocity.x += distanceToSlopeStart * directionX;
//                 }

//                 if (!detections.collision.climbingSlope || slopeAngle > maxClimbAngle)
//                 {
//                     velocity.x = (hit.distance - RayCollisions2D.skinWidth) * directionX;
//                     rayLength = hit.distance;
//                     if (detections.collision.climbingSlope)
//                     {

//                         velocity.y = Mathf.Tan(detections.collision.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);

//                     }
//                     detections.collision.left = directionX == -1;
//                     detections.collision.right = directionX == 1;
//                 }

//             }
//         }
//     }

//     private void ClimbSlope(ref Vector2 velocity, float slopeAngle)
//     {
//         float moveDistance = Mathf.Abs(velocity.x);
//         float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
//         if (velocity.y <= climbVelocityY)
//         {
//             velocity.y = climbVelocityY;
//             velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
//             detections.collision.below = true;
//             detections.collision.climbingSlope = true;
//             detections.collision.slopeAngle = slopeAngle;
//         }


//     }

//     private void DecendSlope(ref Vector2 velocity)
//     {
//         float directionX = Mathf.Sign(velocity.x);
//         Vector2 rayOrigin = (directionX == -1) ? detections.collisionOrigin.bottomRight : detections.collisionOrigin.bottomLeft;
//         RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

//         if (hit)
//         {
//             float slopeAnlge = Vector2.Angle(hit.normal, Vector2.up);
//             if (slopeAnlge != 0 && slopeAnlge <= maxDecendAngle)
//             {
//                 if (Mathf.Sign(hit.normal.x) == directionX)
//                 {
//                     if (hit.distance - RayCollisions2D.skinWidth <= Mathf.Tan(slopeAnlge * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
//                     {
//                         float moveDistance = Mathf.Abs(velocity.x);
//                         float decendVelocityY = Mathf.Sin(slopeAnlge * Mathf.Deg2Rad) * moveDistance;

//                         velocity.x = Mathf.Cos(slopeAnlge * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
//                         velocity.y -= decendVelocityY;

//                         detections.collision.slopeAngle = slopeAnlge;
//                         detections.collision.decendingSlope = true;
//                         detections.collision.below = true;
//                     }
//                 }
//             }
//         }
//     }

// }

