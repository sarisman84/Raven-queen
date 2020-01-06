using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class OnAttack : MonoBehaviour {

    public Transform sword;
    public UnityEvent OnHit;
    bool isAttacking = false;
    public bool AttackInput { set => isAttacking = value; }

    float rotationZVal = 0;
    float result;

    public void OnOtherCollisionEnter2D (Collision2D other) {
        //if(other.collider.GetComponent<Enemy>() != null) Fill in with the proper type

        OnHit.Invoke ();
    }

    private void Update () {

        //What i need to do;
        /*
                    -Everytime i start an attack, rotate the object around its z axis.
                    -Once it finishes its rotation at a set rotation value, reset its rotation back to the original rotation.
                    -Once the object finishes its rotation back to its original state, allow the player to attack again.

                    TODO  Rotate an object towards a rotation point.
                    TODO  Create a toggle condition (making the attack available after a rotation is done)


        
                */

        Debug.Log ($"{rotationZVal} <- {Keyboard.current.spaceKey.isPressed}");
        if (!Keyboard.current.spaceKey.isPressed) {
            rotationZVal = 0;
            return;
        }

        rotationZVal += 10f;
        rotationZVal = Mathf.Clamp (rotationZVal, 0, 100f);

        transform.rotation = Quaternion.LookRotation (Vector3.forward, );

    }

}