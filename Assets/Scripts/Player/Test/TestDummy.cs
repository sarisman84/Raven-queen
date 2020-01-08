using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummy : MonoBehaviour, IDamageable {
    public void TakeDamage (float attackDamage, GameObject owner) {
        Debug.Log ($"Taken {attackDamage} damage by {owner.name}.");
    }

}