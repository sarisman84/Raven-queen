using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    float horizontalMove = 0f;
    public float runSpeed = 40f;
    bool jump = false;
    bool crouch = false;
    

    // Update is called once per frame
    void Update()
    {
       horizontalMove =  Input.GetAxisRaw("Horizontal")* runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump") & jump != true)
        {

            animator.SetBool("jump", true);
            jump = true;
        }

        if (Input.GetButtonDown("Crouch"))
        {
            animator.SetBool("crouching", true);
            crouch = true;

            
        }else if (Input.GetButtonUp("Crouch"))
            {
            animator.SetBool("crouching", false);
                crouch = false;
            }
       
    }
    public void OnLanding()
        {
            animator.SetBool("jump", false);
        }
    private void FixedUpdate()
    {
        controller.Move(horizontalMove*Time.fixedDeltaTime,crouch,jump);
        jump = false;
    }
}
