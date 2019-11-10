using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public float speed;
    private float dazedTime;
    public float startDazedTime;

    private Animator anim;

    public Color hitEffect;
    Color defaultMaterialColor, currentColor;
    float t;

    void Start()
    {
        defaultMaterialColor = GetComponent<MeshRenderer>().material.color;
        currentColor = defaultMaterialColor;

        //anim = GetComponent<Animator>();
        //anim.SetBool("isrunning", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (dazedTime <= 0)
        {
            GetComponent<PlatformController>().speed = 1;
        }
        else
        {
            GetComponent<PlatformController>().speed = 0;
            dazedTime -= Time.deltaTime;
        }
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        
        currentColor = Color.Lerp(defaultMaterialColor, hitEffect, t);
        GetComponent<MeshRenderer>().material.color = currentColor;
        t -= Time.deltaTime;
        t = Mathf.Clamp01(t);
    }

    public void TakeDamage(int damage)
    {
        dazedTime = startDazedTime;
        health -= damage;
        Debug.Log("damage TAKEN");
        t = 1;
    }
}
