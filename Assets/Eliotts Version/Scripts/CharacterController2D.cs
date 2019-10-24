using System;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D<Player> where Player : IPlayerInformation
{

    Rigidbody2D physics;
    bool crouch, jump;
    public CharacterController2D(Player entity, Animator anim)
    {
        player = entity;
        animator = anim;
        physics = entity.gameObject.GetComponent<Rigidbody2D>();
    }

    Player player;
    Animator animator;
    Vector3 ref_Velocity;

    public bool HasJumped
    {
        set
        {
            jump = value;
            if (jump == true && animator.GetBool("OnGround"))
            {
                animator.SetTrigger("Jump");
            }
        }

        get => jump;
    }

    public bool IsCrouching
    {
        set
        {
            crouch = value;
            animator.SetBool("Crouch", value);
            Debug.Log(crouch);
        }
    }

    public Vector3 GetFacingDirection(Transform trans)
    {
        if (m_FacingRight)
        {
            return trans.right;
        }
        if (!m_FacingRight)
        {
            return -trans.right;
        }
        return Vector3.zero;
    }

    bool m_FacingRight = true;
    float horizontalInput;
    public void Move(float horizontalMovement)
    {
        horizontalInput = horizontalMovement;
        // If crouching, check to see if the character can stand up
        if (physics == null) throw new NullReferenceException("Couldn't find Rigidbody2D Component on Player GameObject (Did you forget to add Rigidbody2D to your player?)");
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(player.CeilingCheck.position, player.CelingRadius, player.WhatIsGround))
            {
                crouch = true;
            }
        }

        //only control the player if grounded or airControl is turned on
        if (player.IsGrounded || player.AirControl)
        {
            // If crouching
            if (crouch)
            {
                // Reduce the speed by the crouchSpeed multiplier
                horizontalMovement *= player.CrouchSpeedMultiplier;
            }
            if (player.NormalCollider != null && player.CrouchCollider != null)
            {
                player.NormalCollider.enabled = !crouch;
                player.CrouchCollider.enabled = crouch;
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(horizontalMovement * 10f, physics.velocity.y);
            // And then smoothing it out and applying it to the character
            physics.velocity = Vector3.SmoothDamp(physics.velocity, targetVelocity, ref ref_Velocity, player.MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (horizontalMovement > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip(player.gameObject.transform);
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (horizontalMovement < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip(player.gameObject.transform);
            }
        }
        // If the player should jump...
        if (player.IsGrounded && jump)
        {
            // Add a vertical force to the player.

            player.IsGrounded = false;
            physics.AddForce(new Vector2(0f, player.JumpForce));
        }
    }

    private void Flip(Transform trans)
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = trans.localScale;
        theScale.x *= -1;
        trans.localScale = theScale;
    }

    public void UpdateGroundCheck()
    {
        bool wasGrounded = player.IsGrounded;
        player.IsGrounded = false;
        animator.SetBool("OnGround", false);
        

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.GroundCheck.position, player.GroundRadious, player.WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != player.gameObject)
            {
                player.IsGrounded = true;
                animator.SetBool("OnGround", true);
                break;

            }
        }
    }


    public void UpdateAnimations()
    {
        animator.SetFloat("Horizontal Vel", Mathf.Abs(horizontalInput * 10));
        animator.SetFloat("Vertical Vel", physics.velocity.normalized.y);
        //Debug.Log(physics.velocity.normalized.y);
    }
}
