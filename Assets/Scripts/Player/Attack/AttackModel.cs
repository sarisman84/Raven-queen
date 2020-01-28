using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackModel : MonoBehaviour {

    public InputAction attackButton;
    public Attack info;

    // private void Awake () {
    //     info.Trigger = AttackMethod;
    //     if (attackButton == null) throw new NullReferenceException ($"No button is set in the inspector -> {gameObject.name}");
    //     attackButton.Enable ();
    // }

    private void AttackMethod (Attack info) {

        Debug.Log ($"{info.owner.name} has dealt {info.damage} {(info.type == Attack.AttackType.Physical ? "Physical" : "Spell")} damage. ");

    }

    // private void Update () {
    //     if (attackButton.triggered) info.Trigger (info);
    // }
}