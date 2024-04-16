using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UnlockBall))]
public class UnlockBallPDrawer : PropertyDrawer
{
    Rect leftRect = new Rect();
    Rect rightRect = new Rect();
    readonly int lineCount = 3;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float height = position.height / lineCount;
        float objectWidth = position.width * .5f;
        float earlyStartPosition = position.x - 40;
        leftRect.Set(earlyStartPosition, position.y, objectWidth, height);
        rightRect.Set(earlyStartPosition + objectWidth + 5, position.y, objectWidth + 35, height);

        SerializedProperty prp_Generation = property.FindPropertyRelative("generation");
        SerializedProperty prp_isRandom = property.FindPropertyRelative("isRandom");
        SerializedProperty prp_Index = property.FindPropertyRelative("index");

        EditorGUI.LabelField(leftRect, "Generation");
        EditorGUI.PropertyField(rightRect, prp_Generation, GUIContent.none);
        NextRow(height);
        EditorGUI.LabelField(leftRect, "Is Random?");
        EditorGUI.PropertyField(rightRect, prp_isRandom, GUIContent.none);

        if (prp_isRandom.boolValue == false)
        {
            NextRow(height);
            EditorGUI.LabelField(leftRect, "Index");
            EditorGUI.PropertyField(rightRect, prp_Index, GUIContent.none);

        }
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * lineCount;
    }

    private void NextRow(float height)
    {
        leftRect.y += height;
        rightRect.y += height;
    }
}

/*
[CustomEditor(typeof(BallReward))]
[CanEditMultipleObjects]
public class BallRewardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BallReward ballReward = (BallReward)target;

        ballReward.isRandom = EditorGUILayout.Toggle("Random", ballReward.isRandom);

        if (!ballReward.isRandom)
        {
            ballReward.index = EditorGUILayout.IntField("ID", ballReward.index);
        }
    }
}
*/
