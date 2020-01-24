using UnityEngine;
public delegate void CustomAttack (Attack info);
[System.Serializable]
public struct Attack {
    public enum AttackType { Physical, SpellCast }
    public AttackType type;
    public GameObject owner;
    public float damage;
    public CustomAttack attackBehaivour;
}