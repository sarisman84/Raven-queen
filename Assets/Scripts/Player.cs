using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    P_Input input;
    PlayerInput inputActions;
    CharacterController2D characterController2D;

    public float movementSpeed;

    public CharacterInfo2D characterInfo;

    private void Awake()
    {
        inputActions = new PlayerInput();
        input = new P_Input(inputActions);
        characterController2D = new CharacterController2D(GetComponent<BoxCollider2D>(), transform, characterInfo);


    }

    private void Update()
    {
        characterController2D.MoveCharacter(input.PlayerInput2D, input.JumpInput, input.SprintInput);
        Debug.Log(input.JumpInput);

    }

    #region Input
    private void OnEnable()
    {
        input.ChangeState(true, inputActions);
    }

    private void OnDisable()
    {
        input.ChangeState(false, inputActions);
    }
    #endregion

}

public class P_Input : PlayerInput.ICharacterActions
{

    float horizontalInput;
    public Vector2 PlayerInput2D => new Vector2(horizontalInput, 0).normalized;
    public bool AttackInput => isAttacking;
    public bool JumpInput => isJumping;
    public bool CrouchInput => isCrouching;
    public bool SprintInput => isSprinting;
    bool isAttacking, isJumping, isCrouching, isSprinting;


    public P_Input(PlayerInput input)
    {
        input.Character.SetCallbacks(this);
        input.Enable();



    }
    public void OnCrouch(InputAction.CallbackContext context)
    {
        isCrouching = context.performed;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        isAttacking = context.performed;
    }

    public void OnHorizontalMovement(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        isJumping = context.performed;
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.performed;
    }

    public void ChangeState(bool isActive, PlayerInput input)
    {
        if (isActive)
        {
            input.Enable();
            return;
        }
        input.Disable();
    }


}