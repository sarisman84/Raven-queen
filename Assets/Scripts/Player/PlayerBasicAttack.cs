using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public delegate void AttackBehaivour (Collider2D foundEntity);
public class PlayerBasicAttack : MonoBehaviour {

    //What does this class do?
    /*
    -By using an input or trigger method, detect any enemies within an area while an animation plays.
        
    */
    #region Properties
    public bool AttackInput { private get; set; }
    public AttackBehaivour OnDamageable_CustomBehaviour { private get; set; }
    public AttackBehaivour OnDestructable_CustomBehaivour { private get; set; }
    #endregion

    public Animator anim;
    public Vector2 collisionPos, collisionSize = new Vector2 (1, 2);
    public float attackSpeed = 0.5f;
    public float attackDamage = 1f;
    float time;

    private void Update () {

    }
    private void FixedUpdate () {
        time += Time.deltaTime;
        time = Mathf.Clamp (time, 0, attackSpeed);

    }

    public void Attack () {
        if (time != attackSpeed) return;
        anim.SetTrigger ("IsAttacking");
        Debug.Log ("Is attacking!");
        Collider2D[] entitiesFound = Physics2D.OverlapBoxAll (RuntimePosition, collisionSize, 0);
        DamageEntities (entitiesFound);
        time = 0;
    }

    public void DamageEntities (Collider2D[] entitiesFound) {
        if (entitiesFound == null || entitiesFound.Length == 0) return;
        for (int e = 0; e < entitiesFound.Length; e++) {

            IDamageable entity = entitiesFound[e].GetComponent<IDamageable> ();
            IDestructable obj = entitiesFound[e].GetComponent<IDestructable> ();

            if (entity == null) continue;
            if (OnDamageable_CustomBehaviour != null)
                OnDamageable_CustomBehaviour (entitiesFound[e]);

            entity.TakeDamage (attackDamage, gameObject);

            if (obj == null) continue;
            if (OnDestructable_CustomBehaivour != null)
                OnDestructable_CustomBehaivour (entitiesFound[e]);
            obj.Destroy ();
            continue;

        }
    }

    private void OnDrawGizmosSelected () {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube (RuntimePosition, collisionSize);
    }

    Vector2 RuntimePosition {
        get {
            return new Vector2 (collisionPos.x * Mathf.Sign (transform.localScale.x), collisionPos.y) + new Vector2 (transform.position.x, transform.position.y);
        }
    }

}