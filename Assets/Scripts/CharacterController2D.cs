using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : Entity
{

    float velocityXSmoothing, jumpVelocity;
    bool crouchToggle;

    Vector2 defaultColSize, defaultColOffset;
    CharacterInfo2D charInfo;



    public CharacterController2D(Collider2D _col, Transform _entity, CharacterInfo2D _charInfo) : base(_col, _entity, _charInfo)
    {

        useGravity = true;
        charInfo = _charInfo;

        UpdateGravity(charInfo.jumpHeight, charInfo.timeToJumpApex);
        UpdateJumpVelocity(gravity, charInfo.timeToJumpApex);
        _charInfo.gravity = gravity;
        Debug.Log($"Calculated Gravity: {gravity}| Calculated Jump Velocity:{jumpVelocity} ");

        switch (col)
        {

            case BoxCollider2D box:
                defaultColSize = box.size;

                break;
        }

        defaultColOffset = col.offset;



    }

    public void UpdateJumpVelocity(float _gravity, float _timeToJumpApex)
    {
        jumpVelocity = Mathf.Abs(_gravity) * _timeToJumpApex;
    }

    public void UpdateGravity(float jumpHeight, float timeToJumpApex)
    {
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    }

    public void MoveCharacter(Vector2 input)
    {

        float targetVelocityX = input.x;
        if (collision.below || charInfo.airborneControl)
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, collision.below ? charInfo.accelerationTimeOnGround : charInfo.accelerationTimeWhileAirborne);

        MoveEntity(velocity, false);
    }

    public void MoveCharacter(Vector2 input, bool hasJumped)
    {
        if (useGravity)
        {

            if (collision.above || collision.below)
            {
                velocity.y = 0;
            }
            velocity.y += gravity * Time.deltaTime;

        }

        if (hasJumped && collision.below)
        {
            velocity.y = jumpVelocity;
            Debug.Log($"Jumped {velocity.y}");
        }

        MoveCharacter(input);
    }

    public void MoveCharacter(Vector2 input, bool hasJumped, bool isSprinting)
    {
        if (useGravity)
        {

            if (collision.above || collision.below)
            {
                velocity.y = 0;
            }
            velocity.y += gravity * Time.deltaTime;

        }

        if (hasJumped && collision.below)
        {
            velocity.y = jumpVelocity;
            Debug.Log($"Jumped {velocity.y}");
        }

        if (isSprinting)
        {
            input.x *= charInfo.sprintSpeed;
            MoveCharacter(input);
            return;
        }
        input.x *= charInfo.movementSpeed;
        MoveCharacter(input);
        return;


    }

    public void MoveCharacter(Vector2 input, bool hasJumped, bool isSprinting, bool isCrouching)
    {
        if (useGravity)
        {

            if (collision.above || collision.below)
            {
                velocity.y = 0;
            }
            velocity.y += gravity * Time.deltaTime;

        }

        if (hasJumped && collision.below)
        {
            velocity.y = jumpVelocity;
            Debug.Log($"Jumped {velocity.y}");
        }

        if (isSprinting && !isCrouching)
        {
            input.x *= charInfo.sprintSpeed;
            MoveCharacter(input);
            return;
        }

        crouchToggle = isCrouching ? !crouchToggle : crouchToggle;

        if (crouchToggle)
        {
            //Change Collision Size
            switch (col)
            {

                case BoxCollider2D box:

                    box.size = new Vector2(box.size.x, charInfo.crouchLevel);

                    break;
            }
            col.offset = new Vector2(0, -(charInfo.crouchLevel / 2));
            input.x *= charInfo.crouchSpeed;
            MoveCharacter(input);
            return;
        }
        if (col.offset != defaultColOffset)
        {
            switch (col)
            {

                case BoxCollider2D box:

                    box.size = defaultColSize;

                    break;
            }
            col.offset = defaultColOffset;
        }

        input.x *= charInfo.movementSpeed;
        MoveCharacter(input);
        return;


    }



}

[System.Serializable]
public class CharacterInfo2D : EntityInfo
{
    [Header("Base Movement Variables")]
    public bool airborneControl = true;
    public float movementSpeed = 4;
    public float sprintSpeed = 8;
    [Header("Crouch Variables")]
    public float crouchSpeed = 2;
    public float crouchLevel;



    [Header("Jumping")]
    [Space]
    public float jumpHeight = 4;
    public float timeToJumpApex = 0.4f;



    [Header("Movement Smoothing")]
    [Space]
    public float accelerationTimeOnGround;
    public float accelerationTimeWhileAirborne;




}
