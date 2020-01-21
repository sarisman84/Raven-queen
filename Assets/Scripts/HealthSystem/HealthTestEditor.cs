using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HealthTest))]
[CanEditMultipleObjects]
public class HealthTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HealthTest healthTest = (HealthTest)target;
        if (GUILayout.Button("Damage"))
        {
            healthTest.TestDamage();
        }
    }
}
