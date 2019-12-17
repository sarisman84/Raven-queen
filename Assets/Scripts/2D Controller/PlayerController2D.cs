﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

enum InputControl { Active, Disable }
public class PlayerController2D : EntityController2D
{
    [Header("Horizontal Movement")]
    public float moveSpeed;
    public float sprintSpeed;

    public float accelerationTimeAirborne = 0.2f, accelerationTimeGrounded = 0.1f;
    public bool airborneControl;


    [Header("Jump")]
    public float jumpHeight;
    public float timeToJumpApex;


    [Header("Crouch Movement")]
    public float crouchHeight;
    public float crouchSpeedMultiplier;

    [Header("Sliding")]
    public float slideTrigger = 5f;
    public float slidingRate = 3f;


    [Header("Slopes")]
    public float maxClimbAngle = 80f;
    public float maxDecendAngle = 70f;


    public InputAction horizontalInput, jumpInput, crouchInput;


    protected Vector2 vel;

    float jumpVelocity = 8;
    private float velocityXSmoothing;
    private bool isSliding, isCrouching;
    private Vector2 defaultSize;
    private Vector2 defaultOffset;

    protected override void Awake()
    {
        base.Awake();
        UpdateJumpVariables();
        Debug.Log($"Gravity: {gravity}|| Jump Velocity: {jumpVelocity}");

        defaultSize = boxCollider2D.size;
        defaultOffset = boxCollider2D.offset;
    }

    private void OnValidate()
    {
        UpdateJumpVariables();
    }

    private void UpdateJumpVariables()
    {
        gravity = CalculateGravity;
        jumpVelocity = CalculateJumpVelocity;
    }

    #region Input Setup
    private void OnEnable()
    {
        SetInputTo(InputControl.Active);
    }

    private void OnDisable()
    {
        SetInputTo(InputControl.Disable);
    }

    private void SetInputTo(InputControl inputControl)
    {
        switch (inputControl)
        {

            case InputControl.Active:
                horizontalInput.Enable();
                jumpInput.Enable();
                crouchInput.Enable();
                break;

            case InputControl.Disable:
                horizontalInput.Disable();
                jumpInput.Disable();
                crouchInput.Disable();
                break;
        }

    }

    #endregion


    protected override void Update()
    {
        float targetVelocityX = ((IsGrounded || airborneControl) && !isSliding) ? horizontalInput.ReadValue<float>() * moveSpeed : vel.x;
        float desiredAccelerationTimeGrounded = accelerationTimeGrounded;
        CrouchAndSlidingDefinitions(ref targetVelocityX, ref desiredAccelerationTimeGrounded);

        vel.x = Mathf.SmoothDamp(vel.x, targetVelocityX, ref velocityXSmoothing, (IsGrounded) ? desiredAccelerationTimeGrounded : accelerationTimeAirborne);
        if (IsGrounded || IsCollidingAbove) vel.y = 0;

        if (jumpInput.ReadValue<float>() == 1 && IsGrounded) vel.y = jumpVelocity;
        vel.y += gravity * Time.deltaTime;
        Move(vel * Time.deltaTime);
    }

    private void CrouchAndSlidingDefinitions(ref float targetVelocityX, ref float desiredAccelerationTimeGrounded)
    {
        Slide(ref targetVelocityX, ref desiredAccelerationTimeGrounded);
        ChangeCollisionSize(false);
        if (crouchInput.ReadValue<float>() == 1 || isSliding)
        {
            ChangeCollisionSize(true);
        }
        Crouch(ref targetVelocityX, ref desiredAccelerationTimeGrounded);
    }

    private void Crouch(ref float targetVelocityX, ref float desiredAccelerationTimeGrounded)
    {
        isCrouching = false;
        if (Mathf.Abs(vel.x) < slideTrigger && IsGrounded && crouchInput.ReadValue<float>() == 1)
        {
            targetVelocityX = (IsGrounded || airborneControl) ? horizontalInput.ReadValue<float>() * moveSpeed * crouchSpeedMultiplier : vel.x;
            desiredAccelerationTimeGrounded = accelerationTimeGrounded / 2f;
            isCrouching = true;
        }
    }

    private void Slide(ref float targetVelocityX, ref float desiredAccelerationTimeGrounded)
    {
        isSliding = false;
        //If the player is fast enough, is on a surface and has pressed the crouch button while not already crouching; Slide.
        if (Mathf.Abs(vel.x) >= slideTrigger && IsGrounded && crouchInput.ReadValue<float>() == 1 && !isCrouching)
        {
            desiredAccelerationTimeGrounded = accelerationTimeGrounded + 1;
            targetVelocityX -= slidingRate * Mathf.Sign(vel.x);
            isSliding = true;
        }
    }

    private void ChangeCollisionSize(bool smaller)
    {

        if (!smaller)
        {
            boxCollider2D.size = defaultSize;
            boxCollider2D.offset = defaultOffset;
            CalculateRaySpacing();
            return;
        }
        boxCollider2D.size = new Vector2(boxCollider2D.size.x, crouchHeight);
        boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, -((GetComponent<MeshRenderer>().bounds.size.y / 2) - (boxCollider2D.size.y / 2)));
        CalculateRaySpacing();
    }


    protected override void HorizontalCollisionBehaivour(int i,
                                                         RaycastHit2D hit,
                                                         float curDirection,
                                                         ref float rayLength,
                                                         ref Vector2 velocity)
    {

        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        if (i == 0 && slopeAngle <= maxClimbAngle)
        {
            float distanceToSlopeStart = 0;
            if (slopeAngle != PreviousSlopeAngle)
            {
                distanceToSlopeStart = hit.distance - skinWidth;
                velocity.x -= distanceToSlopeStart * curDirection;
            }
            ClimbSlope(ref velocity, slopeAngle);
            velocity.x += distanceToSlopeStart * curDirection;
        }

        if (IsClimbingASlope && CurrentSlopeAngle <= maxClimbAngle) return;


        velocity.x = Mathf.Min(Mathf.Abs(velocity.x), (hit.distance - skinWidth)) * curDirection;
        rayLength = Mathf.Min(Mathf.Abs(velocity.x) + skinWidth, hit.distance);
        if (IsClimbingASlope)
        {
            velocity.y = Mathf.Tan(CurrentSlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
        }


    }

    protected override void VerticalCollisionBehaivour(int i,
                                                       RaycastHit2D hit,
                                                       float curDirection,
                                                       ref float rayLength,
                                                       ref Vector2 velocity)
    {
        base.VerticalCollisionBehaivour(i, hit, curDirection, ref rayLength, ref velocity);
        if (IsClimbingASlope)
        {
            velocity.x = velocity.y / Mathf.Tan(CurrentSlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
        }

    }

    private void ClimbSlope(ref Vector2 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            IsGrounded = true;
            IsClimbingASlope = true;
            CurrentSlopeAngle = slopeAngle;
        }

    }


    float CalculateGravity => -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);


    float CalculateJumpVelocity => Mathf.Abs(gravity) * timeToJumpApex;
}
