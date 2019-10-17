using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, PlayerInput.ICharacterActions {
    public CharacterController2D controller;
    public Animator animator;
    float horizontalMove = 0f;
    public float runSpeed = 40f;
    bool jump = false;
    bool crouch = false;

    PlayerInput playerInput;

    private void OnEnable () {
        playerInput = playerInput ?? new PlayerInput ();
        playerInput.Character.SetCallbacks (this);
        playerInput.Enable ();

    }

    private void OnDisable () {
        playerInput.Disable ();
    }

    // Update is called once per frame
    void Update () {
        //    horizontalMove =  Input.GetAxisRaw("Horizontal")* runSpeed;
        animator.SetFloat ("Speed", Mathf.Abs (horizontalMove));

        // if (Input.GetButtonDown ("Jump") & jump != true) {

        //     animator.SetBool ("jump", true);
        //     jump = true;
        // }

        // if (Input.GetButtonDown ("Crouch")) {
        //     animator.SetBool ("crouching", true);
        //     crouch = true;

        // } else if (Input.GetButtonUp ("Crouch")) {
        //     animator.SetBool ("crouching", false);
        //     crouch = false;
        // }

    }
    public void OnLanding () {
        animator.SetBool ("jump", false);
    }
    private void FixedUpdate () {
        controller.Move (horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }

    public void OnHorizontalMovement (InputAction.CallbackContext context) {
        horizontalMove =  context.ReadValue<float>() * runSpeed;
    }

    public void OnJump (InputAction.CallbackContext context) {
          if (context.performed && jump != true) {

            animator.SetBool ("jump", true);
            jump = true;
        }
    }

    public void OnCrouch (InputAction.CallbackContext context) {
        if (context.performed) {
            animator.SetBool ("crouching", true);
            crouch = true;

        } else if (context.canceled) {
            animator.SetBool ("crouching", false);
            crouch = false;
        }
    }
}