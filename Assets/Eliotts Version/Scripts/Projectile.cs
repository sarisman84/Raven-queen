using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float distance;
    public int damage;
    public LayerMask whatisSolid;

    public GameObject destroyEffect;

    public void Start()
    {
        Invoke("DestroyProjectile", lifeTime);
    }
    private void Update()
    {
        RaycastHit2D hitinfo = Physics2D.Raycast(transform.position, transform.up, distance, whatisSolid);
        if (hitinfo.collider != null)
        {
            if (hitinfo.collider.CompareTag("Enemy"))
            {
                hitinfo.collider.GetComponent<Enemy>().TakeDamage(damage);
            }
            DestroyProjectile();
        }
        transform.Translate(transform.up * speed * Time.deltaTime);
    }

    void DestroyProjectile()
    {
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
