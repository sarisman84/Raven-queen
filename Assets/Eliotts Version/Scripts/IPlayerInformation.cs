using UnityEngine;
public interface IPlayerInformation
{
    Transform CeilingCheck { get; }
    float CelingRadius { get; }



    Transform GroundCheck { get; }
    float GroundRadious { get; }
    LayerMask WhatIsGround { get; }

    bool IsGrounded { get; set; }
    bool AirControl { get; }

    float CrouchSpeedMultiplier { get; }
    
    float MovementSmoothing { get; }
    float JumpForce { get; }

    Collider2D CrouchCollider { get; }
    Collider2D NormalCollider { get; }

    GameObject gameObject { get; }

}
