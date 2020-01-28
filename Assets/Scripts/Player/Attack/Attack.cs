using UnityEngine;
public delegate AttackCollider CustomAttack
    (Animator anim, Transform localCenter, Vector2 lookDirection, Attack info);

[System.Serializable] public struct Attack {
    public enum AttackType { Physical, SpellCast }
    public AttackType type;
    public GameObject owner;
    public float damage, attackSpeed;
    public CustomAttack Trigger;

    public Attack (AttackType _type, GameObject _owner, float _damage, float _attSpeed, CustomAttack _att) {
        type = _type;
        owner = _owner;
        damage = _damage;
        attackSpeed = _attSpeed;
        Trigger = _att;
    }
}

public struct AttackCollider {
    public Vector2 offset;
    public Vector2 localSize;

    public Vector2 SetPositionTo (Vector2 pos, Vector2 dir) {
        return new Vector2 (pos.x + offset.x * dir.x, pos.y + offset.y) * new Vector2 (1 * (localSize.x + 1), 1 * (localSize.y + 1));

    }

    public AttackCollider (Vector2 _offset, Vector2 _localSize) {
        offset = _offset;
        localSize = _localSize;
    }
}