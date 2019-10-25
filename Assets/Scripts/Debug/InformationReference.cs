using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InformationReference : MonoBehaviour
{
    public Player playerInformation;
    public TMP_Text horizontalVel, verticalVel, crouch, jump, grounded, slope, firing;


    private void Update()
    {
        horizontalVel.text = $"Horizontal Vel: {playerInformation.PlayerVelocity.x}";
        verticalVel.text = $"Vertical Vel: {playerInformation.PlayerVelocity.y}";
        crouch.text = $"Is Crouching: {((playerInformation.PlayerCrouchInput) ? "yes" : "no")}";
        jump.text = $"Has Jumped: {((playerInformation.PlayerJumpInput) ? "yes" : "no")}";
        grounded.text = $"Is grounded:{playerInformation.collisions.below} ";
        slope.text = $"Is on a slope: {((playerInformation.collisions.climbingSlope || playerInformation.collisions.decendingSlope))}";
        firing.text = $"Is firing: {playerInformation.PlayerFiringInput}";
    }


}
