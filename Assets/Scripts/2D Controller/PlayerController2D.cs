using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

enum InputControl { Active, Disable }
public enum FacingDirection { Left, Right }

public class PlayerController2D : EntityController2D {
    [Header ("Horizontal Movement")]
    public float moveSpeed;
    public float sprintSpeed;

    public float accelerationTimeAirborne = 0.2f, accelerationTimeGrounded = 0.1f;
    public bool airborneControl;

    [Header ("Jump")]
    public float jumpHeight;
    public float timeToJumpApex;

    [Header ("Crouch Movement")]
    public float crouchHeight;
    public float crouchSpeedMultiplier;

    [Header ("Sliding")]
    public float slideTrigger = 5f;
    public float slidingRate = 3f;

    [Header ("Slopes")]
    public float maxClimbAngle = 80f;
    public float maxDecendAngle = 70f;

    [Header ("Main Player Input")]
    public InputAction horizontalInput;
    public InputAction jumpInput, crouchInput;

    [Header ("Extra Player Inputs")]
    public InputInformation[] inputArray;

    [Header ("Model")]
    public Transform playerModel;

    [Header ("Unity Events")]
    public EventInformation[] eventArray;

    float CalculateGravity => -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
    float CalculateJumpVelocity => Mathf.Abs (gravity) * timeToJumpApex;

    FacingDirection direction;
    public FacingDirection CurrentFacingDirection => direction;

    protected Vector2 vel;

    float jumpVelocity = 8;
    private float velocityXSmoothing;
    private bool isSliding, isCrouching;
    private Vector2 defaultSize;
    private Vector2 defaultOffset;

    private Animator animatorController;

    protected override void Awake () {
        base.Awake ();
        UpdateJumpVariables ();
        Debug.Log ($"Gravity: {gravity}|| Jump Velocity: {jumpVelocity}");

        defaultSize = boxCollider2D.size;
        defaultOffset = boxCollider2D.offset;
        animatorController = playerModel.GetComponent<Animator> ();

    }

    private void OnValidate () {
        UpdateJumpVariables ();
    }

    private void UpdateJumpVariables () {
        gravity = CalculateGravity;
        jumpVelocity = CalculateJumpVelocity;
    }

    #region Input Setup
    private void OnEnable () {
        SetInputTo (InputControl.Active);
    }

    private void OnDisable () {
        SetInputTo (InputControl.Disable);
    }

    private void SetInputTo (InputControl inputControl) {
        switch (inputControl) {

            case InputControl.Active:
                horizontalInput.Enable ();
                jumpInput.Enable ();
                crouchInput.Enable ();
                foreach (var item in inputArray) {
                    item.inputAction.Enable ();
                }
                break;

            case InputControl.Disable:
                horizontalInput.Disable ();
                jumpInput.Disable ();
                crouchInput.Disable ();
                foreach (var item in inputArray) {
                    item.inputAction.Disable ();
                }
                break;
        }

    }

    #endregion

    protected override void Update () {
        float targetVelocityX = ((IsGrounded || airborneControl) && !isSliding) ? horizontalInput.ReadValue<float> () * moveSpeed : vel.x;
        float desiredAccelerationTimeGrounded = accelerationTimeGrounded;
        CrouchAndSlidingDefinitions (ref targetVelocityX, ref desiredAccelerationTimeGrounded);

        vel.x = Mathf.SmoothDamp (vel.x, targetVelocityX, ref velocityXSmoothing, (IsGrounded) ? desiredAccelerationTimeGrounded : accelerationTimeAirborne);

        //Change Facing Direction
        direction = ChangeFacingDirection ();
        SetAnimationVariables ();
        EventCallbacks (this);
        if (IsGrounded || IsCollidingAbove) vel.y = 0;
        Jump ();
        vel.y += gravity * Time.deltaTime;
        Move (vel * Time.deltaTime, DecendSlope);
    }

    private void EventCallbacks (PlayerController2D playerController2D) {
        if (eventArray.Length == 0 || eventArray == null) return;
        if (inputArray.Length == 0 || inputArray == null) return;

        for (int i = 0; i < eventArray.Length; i++) {
            InputAction action = Array.Find (inputArray, p => eventArray[i].eventName.ToLower ().Contains (p.inputName.ToLower ())).inputAction;
            if (action != null && action.ReadValue<float> () == 1)
                eventArray[i].Event.Invoke ();
        }

    }

    private void SetAnimationVariables () {
        animatorController.SetFloat ("VelocityX", Mathf.Abs (vel.x));
        animatorController.SetFloat ("VelocityY", vel.y);
        animatorController.SetBool ("IsGrounded", IsGrounded);
        animatorController.SetBool ("Slide", isSliding);
        animatorController.SetBool ("Crouch", isCrouching);
    }

    private void Jump () {
        if (jumpInput.ReadValue<float> () == 1 && IsGrounded) {
            vel.y = jumpVelocity;
            animatorController.SetTrigger ("Jump");
        }
    }

    private FacingDirection ChangeFacingDirection () {
        Vector2 size = transform.localScale;
        if (Mathf.Abs (vel.x) != 0)
            size.x = Mathf.Sign (vel.x);

        transform.localScale = size;
        return (Mathf.Sign (vel.x) == -1) ? FacingDirection.Left : FacingDirection.Right;
    }

    private void CrouchAndSlidingDefinitions (ref float targetVelocityX, ref float desiredAccelerationTimeGrounded) {
        Slide (ref targetVelocityX, ref desiredAccelerationTimeGrounded);
        ChangeCollisionSize (false);

        //Updating Hitbox size when either sliding or crouching
        if (crouchInput.ReadValue<float> () == 1 || isSliding) {
            ChangeCollisionSize (true);
        }

        Crouch (ref targetVelocityX, ref desiredAccelerationTimeGrounded);
    }

    private void Crouch (ref float targetVelocityX, ref float desiredAccelerationTimeGrounded) {
        isCrouching = crouchInput.ReadValue<float> () == 1;
        //If the player is not fast enough, is on a surface and has pressed the crouch button; Crouch.
        if (Mathf.Abs (vel.x) < slideTrigger && IsGrounded && isCrouching) {
            targetVelocityX = (IsGrounded || airborneControl) ? horizontalInput.ReadValue<float> () * moveSpeed * crouchSpeedMultiplier : vel.x;
            desiredAccelerationTimeGrounded = accelerationTimeGrounded / 2f;
        }

    }

    private void Slide (ref float targetVelocityX, ref float desiredAccelerationTimeGrounded) {
        isSliding = false;
        //If the player is fast enough, is on a surface and has pressed the crouch button while not already crouching; Slide.

        if (Mathf.Abs (vel.x) >= slideTrigger && IsGrounded && crouchInput.ReadValue<float> () == 1 && !isCrouching) {
            desiredAccelerationTimeGrounded = accelerationTimeGrounded + 1;
            // targetVelocityX -= slidingRate * Mathf.Sign (vel.x);
            Debug.Log (targetVelocityX -= slidingRate * Mathf.Sign (vel.x));
            Debug.Log (vel.x);
            isSliding = true;
        }

    }

    private void ChangeCollisionSize (bool smaller) {

        if (!smaller) {
            if (boxCollider2D.size == defaultSize || boxCollider2D.offset == defaultOffset) return;
            boxCollider2D.size = defaultSize;
            boxCollider2D.offset = defaultOffset;
            CalculateRaySpacing ();
            return;
        }
        boxCollider2D.size = new Vector2 (boxCollider2D.size.x, boxCollider2D.size.y / crouchHeight);
        boxCollider2D.offset = new Vector2 (boxCollider2D.offset.x, boxCollider2D.size.y / 2f);
        CalculateRaySpacing ();
    }

    protected override void HorizontalCollisionBehaivour (
        int i, RaycastHit2D hit, float curDirection, ref float rayLength, ref Vector2 velocity) {

        float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
        if (i == 0 && slopeAngle <= maxClimbAngle) {
            if (IsDecendingASlope) {
                IsDecendingASlope = false;
                velocity = OldVelocity;
            }
            float distanceToSlopeStart = 0;
            if (slopeAngle != PreviousSlopeAngle) {
                distanceToSlopeStart = hit.distance - skinWidth;
                velocity.x -= distanceToSlopeStart * curDirection;
            }
            ClimbSlope (ref velocity, slopeAngle);
            velocity.x += distanceToSlopeStart * curDirection;
        }

        if (IsClimbingASlope && CurrentSlopeAngle <= maxClimbAngle) return;

        velocity.x = Mathf.Min (Mathf.Abs (velocity.x), (hit.distance - skinWidth)) * curDirection;
        rayLength = Mathf.Min (Mathf.Abs (velocity.x) + skinWidth, hit.distance);
        if (IsClimbingASlope) {
            velocity.y = Mathf.Tan (CurrentSlopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x);
        }

    }

    protected override void VerticalCollisionBehaivour (
        int i, RaycastHit2D hit, float curDirection, ref float rayLength, ref Vector2 velocity) {
        base.VerticalCollisionBehaivour (i, hit, curDirection, ref rayLength, ref velocity);

        if (IsClimbingASlope) {
            velocity.x = velocity.y / Mathf.Tan (CurrentSlopeAngle * Mathf.Deg2Rad) * Mathf.Sign (velocity.x);
        }
        return;

    }

    protected override void VerticalCollisionBehaivour (
        float curDirection, ref float rayLength, ref Vector2 velocity) {
        if (IsClimbingASlope) {
            float directionX = Mathf.Sign (velocity.x);
            rayLength = Mathf.Abs (velocity.x) + skinWidth;

            Vector2 rayOrigin = ((directionX == -1) ? rayCollisions.collisionOrigin.bottomLeft : rayCollisions.collisionOrigin.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D[] hits = Physics2D.RaycastAll (rayOrigin, Vector2.right * directionX, rayLength);
            foreach (var newHit in hits) {
                if (!newHit || newHit.transform.gameObject == gameObject) continue;
                float slopeAngle = Vector2.Angle (newHit.normal, Vector2.up);
                if (slopeAngle != CurrentSlopeAngle) {
                    velocity.x = (newHit.distance - skinWidth) * directionX;
                    CurrentSlopeAngle = slopeAngle;
                }
                break;
            }
        }
    }

    private void ClimbSlope (ref Vector2 velocity, float slopeAngle) {
        float moveDistance = Mathf.Abs (velocity.x);
        float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelocityY) {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
            IsGrounded = true;
            IsClimbingASlope = true;
            CurrentSlopeAngle = slopeAngle;
        }

    }

    private void DecendSlope (ref Vector2 velocity) {
        float directionX = Mathf.Sign (velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? rayCollisions.collisionOrigin.bottomRight : rayCollisions.collisionOrigin.bottomLeft;

        RaycastHit2D[] hits = Physics2D.RaycastAll (rayOrigin, -Vector2.up, Mathf.Infinity);
        foreach (var hit in hits) {
            if (!hit || hit.transform.gameObject == gameObject) continue;
            float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

            if (slopeAngle != 0 && slopeAngle <= maxDecendAngle) {
                if (Mathf.Sign (hit.normal.x) == directionX && hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad * Mathf.Abs (velocity.x))) {
                    float moveDistance = Mathf.Abs (velocity.x);
                    float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
                    velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
                    velocity.y -= descendVelocityY;
                    CurrentSlopeAngle = slopeAngle;
                    IsDecendingASlope = true;
                    IsGrounded = true;
                }
            }
            break;
        }
    }

    [System.Serializable]
    public struct EventInformation {
        public string eventName;
        public UnityEvent Event;

    }

    [System.Serializable]
    public struct InputInformation {
        public string inputName;
        public InputAction inputAction;
    }

}