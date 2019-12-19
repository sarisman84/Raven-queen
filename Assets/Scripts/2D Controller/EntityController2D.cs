using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController2D : CustomEntity
{
    // Start is called before the first frame update

  
    protected float gravity = -20f;
    protected Vector2 _inputVelocity;
    public Vector2 velocity { get => _inputVelocity; set => _inputVelocity = value; }


    protected virtual void Update()
    {
        if (IsGrounded || IsCollidingAbove) _inputVelocity.y = 0;
        _inputVelocity.y += gravity * Time.deltaTime;
        Move(_inputVelocity * Time.deltaTime);
    }


   



}
