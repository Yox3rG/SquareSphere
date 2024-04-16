using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Currency))]
public class CurrencyPDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float objectWidth = position.width * .5f;
        float earlyStartPosition = position.x - 40;
        Rect leftRect = new Rect(earlyStartPosition, position.y, objectWidth, position.height);
        Rect rightRect = new Rect(earlyStartPosition + objectWidth + 5, position.y, objectWidth + 35, position.height);

        SerializedProperty prp_amount = property.FindPropertyRelative("_amount");
        SerializedProperty prp_type = property.FindPropertyRelative("_type");


        EditorGUI.PropertyField(leftRect, prp_amount, GUIContent.none);
        EditorGUI.PropertyField(rightRect, prp_type, GUIContent.none);

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
