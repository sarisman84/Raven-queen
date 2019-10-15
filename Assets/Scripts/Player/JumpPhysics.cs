using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPhysics : MonoBehaviour
{
   

    Rigidbody physics;
    PlayerController player;
    public float fallMultiplier, lowJumpMultiplyer;

    private void Awake() {
        player = GetComponent<PlayerController>();
        physics = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
         if (physics.velocity.y < 0)
        {
            physics.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (physics.velocity.y > 0 && !player.HasJumped)
        {
            physics.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplyer - 1) * Time.fixedDeltaTime;
        }
    }
}
