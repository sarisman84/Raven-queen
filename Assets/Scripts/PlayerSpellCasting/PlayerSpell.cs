using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "TestSpell", menuName = "Spells/PlayerSpells/defaultSpell", order = 1)]
public class PlayerSpell : ScriptableObject
{
    [SerializeField]
    string name;
    [SerializeField]
    string description;

    public delegate void HoldSpell(SpellCaster caster);
    public delegate void CastSpell(SpellCaster caster);
    public delegate void ReleaseSpell(SpellCaster caster);

    public HoldSpell holdSpell;
    public CastSpell castSpell;
    public ReleaseSpell releaseSpell;

    public string Name { get => name; }
    public string Description { get => description; }

    public PlayerSpell(PlayerSpell s)
    {
        name = s.name;
        description = s.description;
        holdSpell = DefaultHold;
        castSpell = DefaultCast;
        releaseSpell = DefaultHold;

    }
    public  PlayerSpell()
    {
        name = "Unnamed";
        description = "";
        holdSpell = DefaultHold;
        castSpell = DefaultCast;
        releaseSpell = DefaultHold;
    }

    void DefaultCast(SpellCaster caster)
    {

    }
    void DefaultRelease(SpellCaster caster)
    {

    }
    void DefaultHold(SpellCaster caster)
    {

    }

}


