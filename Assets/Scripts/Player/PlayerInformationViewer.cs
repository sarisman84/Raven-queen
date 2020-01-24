using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class PlayerInformationViewer : MonoBehaviour {

}

public class PlayerInformation {
    Dictionary<string, Attack> knownAttacks = new Dictionary<string, Attack> ();
    int attackAmm = 1;

    #region Properties and Methods
    public int AmountOfKnownAttacks { set => attackAmm = value; }
    public Attack CurrentAttack => selectedAttack;
    public void SelectAnAttack (string attack) {
        selectedAttack = knownAttacks[attack];
    }
    public void AddAnAttack (string name, Attack att) {
        if (knownAttacks.Count >= attackAmm) return;
        knownAttacks.Add (name, att);
    }
    public void RemoveAnAttack () {
        knownAttacks.Remove (knownAttacks.Last ().Key);
    }
    public void RemoveAnAttack (string name) {
        knownAttacks.Remove (name);
    }
    #endregion

    Attack selectedAttack;

}