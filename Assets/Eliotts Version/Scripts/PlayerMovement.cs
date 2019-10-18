using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, PlayerInput.ICharacterActions {
    //Eliotts stuff
    public CharacterController2D controller;
    public Animator animator;
    float horizontalMove = 0f;
    public float runSpeed = 40f;
    bool jump = false;
    bool crouch = false;

    // Spyro´s stuff.
    PlayerInput playerInput;
    float counter;
    public float fireRate = 0.4f;

    [Header ("Weapon Variables")]
    public Transform barrelPos;
    public GameObject projectilePrefab;

    private void Awake () {
        Transform bulletParent = new GameObject ("Bullet list").transform;
        bulletParent.parent = transform;
        ObjectPooler.PoolObject (projectilePrefab, 10, bulletParent);
    }
    private void OnEnable () {
        playerInput = playerInput ?? new PlayerInput ();
        playerInput.Character.SetCallbacks (this);
        playerInput.Enable ();
        counter = 0;

    }

    private void OnDisable () {
        playerInput.Disable ();
    }

    // Update is called once per frame
    void Update () {

        animator.SetFloat ("Speed", Mathf.Abs (horizontalMove));
        counter = (counter <= fireRate) ? counter + Time.deltaTime : fireRate;
        Debug.DrawRay (transform.position, controller.FacingDirection * 10, Color.red);

    }
    public void OnLanding () {
        animator.SetBool ("jump", false);
        jump = false;
    }
    private void FixedUpdate () {
        controller.Move (horizontalMove * Time.fixedDeltaTime, crouch, jump);
        
    }

    public void OnHorizontalMovement (InputAction.CallbackContext context) {
        horizontalMove = context.ReadValue<float> () * runSpeed;
    }

    public void OnJump (InputAction.CallbackContext context) {
        Debug.Log($"Can you jump? {jump}");
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

    public void OnFire (InputAction.CallbackContext context) {
        if (context.performed) {
            Attack ();
            Debug.Log ("Just attacked");
        }
    }

    private void Attack () {
        if (counter <= fireRate) return;
        Debug.Log("Fired!");
        Projectile newProjectile = ObjectPooler.GetPooledObject<Projectile> ();
        newProjectile.gameObject.SetActive (true);
       // Debug.Log(newProjectile.gameObject.activeInHierarchy);
        newProjectile.SetFireDirection = controller.FacingDirection;

        newProjectile.OnAttack (barrelPos);
        counter = 0;

        //Attack

    }
}