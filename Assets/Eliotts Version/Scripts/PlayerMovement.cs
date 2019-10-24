using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour, PlayerInput.ICharacterActions, IPlayerInformation
{
    [System.Serializable]
    private class ControllerInfo
    {
        [SerializeField] public float m_JumpForce = 400f;							// Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] public float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
        [Range(0, .3f)] [SerializeField] public float m_MovementSmoothing = .05f;	// How much to smooth out the movement
        [SerializeField] public bool m_AirControl = false;							// Whether or not a player can steer while jumping;
        [SerializeField] public LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
        [SerializeField] public Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
        [SerializeField] public Transform m_CeilingCheck;							// A position marking where to check for ceilings
        [SerializeField] public Collider2D m_CrouchDisableCollider;         // A collider that will be disabled when crouching
        public const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        public bool m_Grounded;            // Whether or not the player is grounded.
        public const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
        public float runSpeed = 40f;
    }

    [SerializeField] ControllerInfo movementAndCollisionInfo; //Reference to the class above.

    //Eliotts stuff
    CharacterController2D<PlayerMovement> controller; //Reference to the CharacterController2D module.
    [Space] public Animator animator;
    float horizontalMove = 0f;

    // Spyro´s stuff.
    PlayerInput playerInput; //Reference to the InputSystem
    float counter; //Counter variable used in a time logic.
    public float fireRate = 0.4f;

    [Header("Weapon Variables")]
    public Transform barrelPos;
    public GameObject projectilePrefab;

    #region Property List (will change this later)
    public Transform CeilingCheck => movementAndCollisionInfo.m_CeilingCheck;

    public float CelingRadius => ControllerInfo.k_CeilingRadius;

    public LayerMask WhatIsGround => movementAndCollisionInfo.m_WhatIsGround;

    public bool IsGrounded { get => movementAndCollisionInfo.m_Grounded; set => movementAndCollisionInfo.m_Grounded = value; }

    public bool AirControl => movementAndCollisionInfo.m_AirControl;

    public float CrouchSpeedMultiplier => movementAndCollisionInfo.m_CrouchSpeed;

    public float MovementSmoothing => movementAndCollisionInfo.m_MovementSmoothing;

    public float JumpForce => movementAndCollisionInfo.m_JumpForce;

    public Collider2D CrouchCollider => movementAndCollisionInfo.m_CrouchDisableCollider;

    public Collider2D NormalCollider => GetComponent<Collider2D>();

    public Transform GroundCheck => movementAndCollisionInfo.m_GroundCheck;

    public float GroundRadious => ControllerInfo.k_GroundedRadius;
    #endregion

    private void Awake()
    {

        Transform bulletParent = new GameObject("Bullet list").transform;
        ObjectPooler.PoolObject(projectilePrefab, 10, bulletParent);
    }
    private void OnEnable()
    {
        controller = controller ?? new CharacterController2D<PlayerMovement>(this, animator);
        playerInput = playerInput ?? new PlayerInput();
        playerInput.Character.SetCallbacks(this);
        playerInput.Enable();
        counter = 0;

    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

   
    void Update()
    {

        
        counter = (counter <= fireRate) ? counter + Time.deltaTime : fireRate;
        controller.UpdateAnimations();
        Debug.DrawRay(transform.position, controller.GetFacingDirection(transform) * 10, Color.red);
    }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime);
        controller.UpdateGroundCheck();
        controller.HasJumped = false;

    }

    public void OnHorizontalMovement(InputAction.CallbackContext context)
    {
        horizontalMove = context.ReadValue<float>() * movementAndCollisionInfo.runSpeed;
        


    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log($"Can you jump? {controller.HasJumped}");
        if (context.performed && controller.HasJumped != true)
        {

            
            controller.HasJumped = true;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Is Crouching");
            controller.IsCrouching = true;

        }
        else if (context.canceled)
        {
            Debug.Log("Is not crouching");
            controller.IsCrouching = false;
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Attack();

        }
    }

    private void Attack()
    {
        if (counter != fireRate) return;

        Projectile newProjectile = ObjectPooler.GetPooledObject<Projectile>();
        newProjectile.gameObject.SetActive(true);
        newProjectile.OnAttack(barrelPos, controller.GetFacingDirection(transform));
        counter = 0;
    }
}