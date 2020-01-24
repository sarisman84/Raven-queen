using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] CharacterHealthStats healthStats;
}

[System.Serializable]
public class CharacterHealthStats
{
    [SerializeField] float maxHealth = 10;
    [SerializeField] float currentHealth = 10;

    //Different resistances to damage types. Damage dealt is damageAmount - damageAmount * typeResitance / (100 + typeResitance), 0, damageAmount.
    //Raw damage ignores resistances. Even if player has raw resistance. 
    //Mutiple resitances of same type stack in damage calculations.
    [SerializeField] List<Resistance> resistances = new List<Resistance>();
    
    [SerializeField] UnityEvent deathEvent;


    //Deals specified amount of damage as raw damage
    public float DealDamage(float damageAmount)
    {
        return DealDamage(damageAmount, Resistance.DamageType.pure);
    }


    public float DealDamage(float damageAmount, Resistance.DamageType type)
    {
        float typeResitance = 0.0f;
        if (type != Resistance.DamageType.pure)
            foreach (Resistance item in resistances)
            {
                if (item.type == type) typeResitance += item.resistance;
            }

        damageAmount = Mathf.Clamp(damageAmount - damageAmount * typeResitance / (100 + typeResitance), 0, damageAmount);// Mathf.Clamp(damageAmount * typeResitance / (100 + typeResitance), 0, damageAmount);
        Debug.Log(damageAmount);
        currentHealth -= damageAmount;

        if (currentHealth <= 0) Death();
        return damageAmount;

    }
    public void Death()
    {
        deathEvent.Invoke();
    }



}
//A resistance towards specified type of damage. 
[System.Serializable]
public struct Resistance
{
    public enum DamageType { pure, magic, physical }
    public float resistance;
    public DamageType type;



}
