
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    float damage, velocity, decay;
    Vector2 facingDirection;
    Rigidbody2D physicsBody;

    public void UpdateBulletInfo(float bulletDamage, float bulletVelocity, float bulletDecay, Vector2 facingDir, Vector2 spawnPos)
    {
        damage = bulletDamage;
        velocity = bulletVelocity;
        decay += bulletDecay;
        physicsBody = physicsBody ?? GetComponent<Rigidbody2D>();
        physicsBody.gravityScale = 0;
        physicsBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        facingDirection = facingDir;
        transform.position = spawnPos;
    }
    private void Update()
    {
        if (Time.time > decay)
        {
            gameObject.SetActive(false);
            decay = Time.time;
        }
    }
    private void FixedUpdate()
    {
        physicsBody.velocity = facingDirection * velocity;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        Enemy enemy = other.collider.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage((int)damage);

        }
        if (other.collider.GetComponent<Player>() == null)
        {
            gameObject.SetActive(false);
            decay = Time.time;
        }
    }

    private void OnDisable()
    {
        if (physicsBody != null)
            physicsBody.velocity = Vector2.zero;
    }
}
