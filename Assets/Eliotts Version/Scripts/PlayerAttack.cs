using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float timeBtwAttack;
    public float StartTimeBtwAttack;

    public Transform attackpos;
    public float attackRange;
    public LayerMask whatisEnemies; //en layermask som markerar allt inom radius som fiender
    public int damage;
    public Animator camAnim;

    private void Update()
    {
        if (timeBtwAttack <= 0)
        {
            if (Input.GetKey(KeyCode.P))
            {
                camAnim.SetTrigger("shake");
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackpos.position, attackRange,whatisEnemies);
                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);
                }
            }
            //då kan du attackera
            timeBtwAttack = StartTimeBtwAttack;
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(attackpos.position, attackRange);
    }
}
