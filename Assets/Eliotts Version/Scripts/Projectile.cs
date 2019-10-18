using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class Projectile : MonoBehaviour {
    public float speed;
    public float lifeTime;
    public float distance;
    public int damage;
    public LayerMask whatisSolid;

    public GameObject destroyEffect;

    //Spyro's stuff
    Rigidbody2D physics2D;
    Vector2 fireDir;
    public Vector2 SetFireDirection {
        set {
            fireDir = value;
        }
    }
    float counter;

    private void OnEnable () {
        counter = 0;
        physics2D = physics2D ?? GetComponent<Rigidbody2D> ();
    }
    private void Update () {
        counter += Time.deltaTime;
        if (counter >= lifeTime) {
            gameObject.SetActive (false);
        }
    }
    private void FixedUpdate () {

        // RaycastHit2D hitinfo = Physics2D.Raycast(transform.position, transform.up, distance, whatisSolid);
        // if (hitinfo.collider != null)
        // {
        //     if (hitinfo.collider.CompareTag("Enemy"))
        //     {
        //         hitinfo.collider.GetComponent<Enemy>().TakeDamage(damage);
        //     }
        //     DestroyProjectile();
        // }
        // transform.Translate(transform.up * speed * Time.deltaTime);
        physics2D.velocity += fireDir.normalized * speed;

    }
    private void OnCollisionEnter2D (Collision2D other) {
        OnHit ();
        if(other.collider.GetComponent<Enemy>()){
            other.collider.GetComponent<Enemy>().TakeDamage(damage);
        }
    }

    void OnHit () {
        // Instantiate (destroyEffect, transform.position, Quaternion.identity);
        physics2D.velocity = Vector2.zero;
        gameObject.SetActive (false);

    }

    internal void OnAttack (Transform barrelPos) {

        transform.position = barrelPos.position;
    }
}