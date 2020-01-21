using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTest : MonoBehaviour
{
    [SerializeField] CharacterHealthStats healthStats;
    [SerializeField] Resistance.DamageType testType;
    [SerializeField] float testDamage;
    public void TestDamage()
    {
        healthStats.DealDamage(testDamage, testType);
    }

}
