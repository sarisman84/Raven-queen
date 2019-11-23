using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



using UnityEditor;

[CustomPropertyDrawer(typeof(CharacterInfo2DEditor))]
public class CharacterInfo2DEditor : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);

        EditorGUI.BeginProperty(position, label, property);




        List<GraphicElement> graphicElements = new List<GraphicElement>();

        // var amountRect = new Rect(position.x, position.y, 30, position.height);
        // var unitRect = new Rect(position.x + 35, position.y, 50, position.height);
        // var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

        // float offset = -5;

        Rect normalRect = position;
        Rect leftRect = position;
        Rect middleRect = position;
        Rect rightRect = position;

        //CharacterInfo
        graphicElements.Add(new GraphicElement(property, "gravity", middleRect, false));
        graphicElements.Add(new GraphicElement(property, "gravity", middleRect, false));
        graphicElements.Add(new GraphicElement(property, "gravity", middleRect, false));

        graphicElements.Add(new GraphicElement(property, "crouchSpeed", normalRect, true));

        graphicElements.Add(new GraphicElement(property, "crouchSpeed", normalRect, true));
        graphicElements.Add(new GraphicElement(property, "gravity", middleRect, false));

        graphicElements.Add(new GraphicElement(property, "crouchSpeed", normalRect, true));
        graphicElements.Add(new GraphicElement(property, "gravity", middleRect, false));

        graphicElements.Add(new GraphicElement(property, "crouchSpeed", normalRect, true));
        graphicElements.Add(new GraphicElement(property, "gravity", middleRect, false));



        //Entity info
        graphicElements.Add(new GraphicElement(property, "crouchSpeed", normalRect, true));
        graphicElements.Add(new GraphicElement(property, "gravity", middleRect, false));


        graphicElements.Add(new GraphicElement(property, "crouchSpeed", normalRect, true));
        graphicElements.Add(new GraphicElement(property, "gravity", middleRect, false));
        graphicElements.Add(new GraphicElement(property, "gravity", middleRect, false));

        graphicElements.Add(new GraphicElement(property, "crouchSpeed", normalRect, true));
        graphicElements.Add(new GraphicElement(property, "gravity", middleRect, false));

        DrawListOfSerialisedProperties(graphicElements, label);


        // // Set indent back to what it was


        EditorGUI.EndProperty();

    }

    void DrawListOfSerialisedProperties(List<GraphicElement> serializedProperties, GUIContent label)
    {
        EditorGUI.indentLevel++;
        Rect lastRow = new Rect();
        for (int i = 0; i < serializedProperties.Count; i++)
        {
            if (serializedProperties[i].moveElementDown)
            {
                serializedProperties[i].position.y *= i + 1;
                lastRow = serializedProperties[i].position;
                EditorGUI.PropertyField(serializedProperties[i].position, serializedProperties[i].property, label);
                continue;
            }
            serializedProperties[i].position.x = lastRow.position.x;
            serializedProperties[i].position.y = lastRow.position.y;
            EditorGUI.PropertyField(serializedProperties[i].position, serializedProperties[i].property, label);



        }

        EditorGUI.indentLevel--;
    }



    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 4 + 6;
    }

    class GraphicElement
    {
        public SerializedProperty property;
        public Rect position;

        public bool moveElementDown;

        public GraphicElement(SerializedProperty baseProperty, string propertyName, Rect _position, bool pushElementDown)
        {
            property = baseProperty.FindPropertyRelative(propertyName);
            position = _position;
            moveElementDown = pushElementDown;
        }
    }

}


