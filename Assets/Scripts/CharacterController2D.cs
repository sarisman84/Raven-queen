using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : Entity
{

    float velocityXSmoothing;


    public CharacterController2D(Collider2D _col, Transform _entity, EntityInfo _info) : base(_col, _entity, _info)
    {

        useGravity = true;

    }


    public void MoveCharacter(Vector2 input, CharacterInfo charInfo)
    {

        float targetVelocityX = input.x;
        if (collision.below || charInfo.airborneControl)
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, collision.below ? charInfo.accelerationTimeOnGround : charInfo.accelerationTimeWhileAirborne);

        MoveEntity(velocity, false);
    }



}

[System.Serializable]
public class CharacterInfo
{
    public float accelerationTimeOnGround;
    public float accelerationTimeWhileAirborne;
    public bool airborneControl = true;

}
