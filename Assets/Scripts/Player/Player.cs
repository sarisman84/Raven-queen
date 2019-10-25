using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : CharacterController2D, PlayerInput.ICharacterActions
{


    Vector3 velocity;
    float horizontalInput, gravity, jumpForce;



    PlayerInput inputActions;


    [Header("Animation")]
    public Animator animator;
    [Space]
    [Header("Movement")]
    public float moveSpeed = 10;
    public float jumpHeight = 4, jumpSpeed = .4f;
    [Space]
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    [Space]
    public bool movementSmoothing = true;
    public bool airborneControl = true;


    [Header("Attack Information")]
    public bool usingRangedWeapon = true;
    [Header("Ranged Weapon:")]

    public Transform barrelTransform;
    public float fireRate;
    public GameObject bulletPrefab;
    public float bulletDamage;
    public float bulletVelocity;
    [Header("Melee Weapon: ")]
    public BoxCollider2D attackArea;
    public float attackRate;
    public float attackDamage;


    bool hasJumped = false, isCrouching = false, isFiring = false;
    private float velocityXSmoothing;


    public Vector3 PlayerVelocity => velocity;
    public bool PlayerCrouchInput => isCrouching;
    public bool PlayerJumpInput => hasJumped;

    public object PlayerFiringInput => isFiring;

    private void OnEnable()
    {
        inputActions = inputActions ?? new PlayerInput();
        inputActions.Character.SetCallbacks(this);
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }



    private void Awake()
    {
        ObjectPooler.PoolObject(bulletPrefab, 15, new GameObject($"{bulletPrefab.name}'s list.").transform);
        CalculateJumpVelocityAndGravity();
    }

    private void CalculateJumpVelocityAndGravity()
    {
        gravity = -(2 * jumpHeight) / Mathf.Pow(jumpSpeed, 2);
        jumpForce = Mathf.Abs(gravity) * jumpSpeed;
        print($"Gravity {gravity}, Jump Force: {jumpForce}");
    }

    private void Update()
    {
        UpdateCollisionBox();
        UpdateAnimatorController(velocity);
        UpdatePhysicsBehaivour();
        FlipAnimation(velocity, animator.transform);

        if (Time.time > delay)
        {
            isFiring = false;
        }
        Move(velocity * Time.deltaTime);
    }

    private void UpdateCollisionBox()
    {
        UpdatingCollider(isCrouching);
    }

    private void UpdateAnimatorController(Vector3 velocity)
    {
        if (collisions.below && animator.GetFloat("Vertical Vel") != 0)
        {
            animator.SetFloat("Vertical Vel", 0);
        }
        animator.SetFloat("Horizontal Vel", Mathf.Abs(horizontalInput * 2));
        if (!collisions.below)
            animator.SetFloat("Vertical Vel", velocity.y);
        if (hasJumped) animator.SetTrigger("Jump");

        animator.SetBool("OnGround", collisions.below);
        animator.SetBool("Crouch", isCrouching);





    }



    private void FlipAnimation(Vector3 velocity, Transform animationBody)
    {
        Vector3 newSize = animationBody.localScale;
        newSize.z = (velocity.normalized.x < 0) ? -1 : 1;
        animationBody.localScale = newSize;
    }

    private void UpdatePhysicsBehaivour()
    {
        //Reset gravity if have landed on ground or have collided above you.
        if (collisions.above || collisions.below)
        {
            velocity.y = 0;
        }



        //Jump when you have collided with something bellow you.
        if (hasJumped && collisions.below)
        {
            hasJumped = false;
            velocity.y = jumpForce;
        }

        float target = horizontalInput * moveSpeed;
        //If the player is on the ground OR if the airborneControl boolean is true, update the horizontal velocity
        if (collisions.below || airborneControl)
            //If movementSmoothing is true, apply a smooth damp on the x velocity, otherwise add the initial input directly to the horizontal velocity.
            velocity.x = (movementSmoothing) ? Mathf.SmoothDamp(velocity.x, target, ref velocityXSmoothing, (collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne) : target;

        //Apply gravity.
        velocity.y += gravity * Time.deltaTime;
    }



    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouching = true;
        }

        if (context.canceled)
        {
            isCrouching = false;
        }
    }

    float delay;
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed && !usingRangedWeapon)
        { Attack(); return; }
        if (context.performed) FireWeapon();

        if (context.performed)
        {
            isFiring = true;
            delay = Time.time + 0.5f;
        }

    }

    private void FireWeapon()
    {
        Bullet bullet = ObjectPooler.GetPooledObject<Bullet>();
        bullet.UpdateBulletInfo(bulletDamage, bulletVelocity, 5f, ((velocity.normalized.x < 0) ? -Vector2.right : Vector2.right), barrelTransform.position);
        bullet.gameObject.SetActive(true);


    }

    private void Attack()
    {
        Debug.Log("Melee Attack. Hyaa!");
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
        }
    }

}
