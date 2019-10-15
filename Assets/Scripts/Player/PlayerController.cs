using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(JumpPhysics))]
public class PlayerController : MonoBehaviour, PlayerInput.ICharacterActions
{

    public float accelerationRate, jumpForce, movementSpeed, groundDetectionRange;
    float horizontalInput, jumpInput;

    bool hasJumped = false;
    Rigidbody physics;


    public bool HasJumped => hasJumped;

    private void Awake()
    {
        physics = GetComponent<Rigidbody>();
    }



    public void OnHorizontalMovement(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            hasJumped = true;

            return;
        }
        if (context.canceled)
        {
            hasJumped = false;
            counter = 0;
        }
    }

    PlayerInput controller;
    private void OnEnable()
    {
        EntityFollow.instance.SetCameraFollowTo = gameObject;
        controller = controller ?? new PlayerInput();
        controller.Character.SetCallbacks(this);
        controller.Enable();
    }

    private void OnDisable()
    {
        controller.Disable();
    }

    float counter;
    private void Update()
    {
        
        jumpInput = (hasJumped && IsTouchingTheGround(groundDetectionRange)) ? 1 : 0;
    }

    private bool Stopwatch(ref float counter, float value)
    {
        if (counter >= value)
        {
            counter = 0;
            return true;
        }
        counter += Time.deltaTime;
        return false;

    }

    private bool IsTouchingTheGround(float groundDetectionRange)
    {
        Color rayColor = Color.red;
        Vector3 offset = new Vector3(0, 0.9f);
        if (Physics.Raycast(transform.position - offset, -transform.up, groundDetectionRange))
        {
            rayColor = Color.green;
            Debug.DrawRay(transform.position - offset, -transform.up * groundDetectionRange, rayColor);
            return true;
        }
        Debug.DrawRay(transform.position - offset, -transform.up * groundDetectionRange, rayColor);
        return false;
    }

    private void FixedUpdate()
    {
        Vector3 resultedVelocity = physics.velocity + new Vector3(horizontalInput * accelerationRate, jumpInput * jumpForce);
        resultedVelocity.x = Mathf.Clamp(resultedVelocity.x, -movementSpeed, movementSpeed);
        physics.velocity = resultedVelocity;

    }


}
