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
   
    float counter;

    private void OnEnable () {
        counter = 0;
        physics2D = physics2D ?? GetComponent<Rigidbody2D> ();
    }
    private void Update () {
        counter += Time.deltaTime;
        if (counter >= lifeTime) {
            physics2D.velocity = Vector2.zero;
            gameObject.SetActive (false);
        }
    }
    private void FixedUpdate () {
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

    public void OnAttack (Transform barrelPos, Vector3 dir) {
        physics2D.velocity = Vector2.zero;
        transform.position = barrelPos.position;
        fireDir = dir;
    }
}