using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummy : MonoBehaviour, IDamageable {
    public void TakeDamage (float attackDamage, GameObject owner) {
        Debug.Log ($"{gameObject.name} has taken {attackDamage} damage by {owner.name}.");
    }

}