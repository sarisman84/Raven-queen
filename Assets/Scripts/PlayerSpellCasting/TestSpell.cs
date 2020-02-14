using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spells
{
    [System.Serializable][CreateAssetMenu(fileName = "TestSpell", menuName = "Spells/PlayerSpells/TestBananaSpell", order = 1)]
    public class TestSpell : PlayerSpell
    {
        [SerializeField]  Rigidbody2D banana;
        float timeHeld = 0;
        [SerializeField] float chargeRate = 1;
        
        public TestSpell()
        {
            holdSpell = ChargeBanana;
            releaseSpell = ReleaseBanana;
        }


        void ChargeBanana(SpellCaster spellCaster)
        {
            timeHeld += chargeRate * Time.deltaTime;
        }
        void ReleaseBanana(SpellCaster spellCaster)
        {
            for (int i = 0; i < timeHeld; i++)
            {
                Rigidbody2D newBanana = Object.Instantiate(banana, spellCaster.SpellOrigin().position, spellCaster.SpellOrigin().rotation, null);
                newBanana.AddForce(new Vector2(100,0));
            }
            timeHeld = 0;
        }


    }
}