using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerSpells : MonoBehaviour, SpellCaster
{

    [SerializeField] public InputAction castSpell;
    [SerializeField] public InputAction swapSpell;
    public List<PlayerSpell> playerSpells;
    public PlayerSpell currentSpell;

    bool spellHeld = false;
    [SerializeField] Transform spellOrigin;

    public Transform SpellOrigin()
    {
        return spellOrigin;
    }

    private void Start()
    {
        castSpell.Enable();

    }
    private void Update()
    {if (GetComponent<PlayerController2D>() != null)
            switch (GetComponent<PlayerController2D>().CurrentFacingDirection)
            {
                case FacingDirection.Left:
                    spellOrigin.eulerAngles = new Vector3(0, 180, 0);
                    break;
                case FacingDirection.Right:
                    spellOrigin.eulerAngles = new Vector3(0, 0, 0);
                    break;
                default:
                    break;
            }
        if (currentSpell != null)
            if (castSpell.ReadValue<float>() == 1)
            {
                if (!spellHeld)
                {
                    spellHeld = true;
                    currentSpell.castSpell(this);
                }
                else
                {
                    currentSpell.holdSpell(this);
                }
                Debug.Log("Cast Spell");
            }
            else if (spellHeld)
            {
                spellHeld = false;
                currentSpell.releaseSpell(this);
            }




    }

}

public interface SpellCaster
{
    Transform SpellOrigin();
}
