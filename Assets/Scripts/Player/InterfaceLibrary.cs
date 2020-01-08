using UnityEngine;

public interface IDamageable {
    void TakeDamage (float attackDamage, GameObject owner);

}

public interface IEnemy : IDamageable {

    float CurrentHealth { get; }

}

public interface IDestructable {
    void Destroy ();
}