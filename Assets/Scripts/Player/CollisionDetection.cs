using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CircleCollider2D))]
public class CollisionDetection : MonoBehaviour {
    [SerializeField] OnAttack addReference;
    private void OnCollisionEnter2D (Collision2D other) {
        if (addReference == null) return;
        addReference.OnOtherCollisionEnter2D (other);
    }
}