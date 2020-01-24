using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public delegate AttackInfo Attack ();
public struct AttackInfo {
    public GameObject owner;
    public float damage;
}

public class AttackModel : MonoBehaviour {

    public InputAction attackButton;
    Attack attackBehaivour;

    private void Awake () {
        attackBehaivour = AttackMethod;
        if (attackButton == null) throw new NullReferenceException ($"No button is set in the inspector -> {gameObject.name}");
        attackButton.Enable ();
    }

    private AttackInfo AttackMethod () {
        AttackInfo info = new AttackInfo ();
        info.damage = 10f;
        info.owner = gameObject;

        Debug.Log ($"{info.owner.name} has dealt {info.damage}. ");

        return info;
    }

    private void Update () {
        if (attackButton.triggered) attackBehaivour ();
    }
}