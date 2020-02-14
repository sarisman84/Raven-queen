using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInformationViewer : MonoBehaviour {
    AttackHolder info;
    public InputAction action;

    public Animator animator;
    public PlayerController2D controller2D;

    private void OnEnable () {
        action.Enable ();
    }

    private void OnDisable () {
        action.Disable ();
    }
    private void Awake () {

        info = new AttackHolder ();
        info.AddAnAttack (
            "defaultAttack",
            new Attack (
                Attack.AttackType.Physical,
                gameObject, 5, 0.3f,
                AttackLibrary.DefaultAttack));
    }

    private void Update () {
        if (info.CurrentAttack.Trigger != null && action.triggered) {
            info.CurrentAttack.Trigger (animator, transform, controller2D.LookDirection, info.CurrentAttack);
        }
    }

}

public class AttackHolder {
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
        selectedAttack = att;
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

public static class AttackLibrary {
    public static AttackCollider DefaultAttack (Animator anim, Transform localCenter, Vector2 lookDirection, Attack info) {
        AttackCollider col = new AttackCollider (Vector2.left, Vector2.one);
        Debug.Log ("Currently Attacking!");
        if (anim != null) anim.SetTrigger ("Default_SwordAttack");
        Collider2D[] foundEntities = Physics2D.OverlapBoxAll (col.SetPositionTo (localCenter.position, lookDirection), col.localSize, 0);
        foreach (var entity in foundEntities) {
            if (entity.GetComponent<IEnemy> () != null) {
                entity.GetComponent<IEnemy> ().TakeDamage (info.damage, localCenter.gameObject);
            }
        }
        return col;
    }
}