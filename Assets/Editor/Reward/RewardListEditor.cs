using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(RewardList))]
public class RewardListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RewardList rewardList = (RewardList)target;
        EditorGUILayout.LabelField("", rewardList.name);

        var tierList = rewardList.dictionary.Keys;

        float labelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 30;
        for (int i = 0; i < rewardList.dictionary.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tier", tierList[i].ToString());
            rewardList.dictionary[tierList[i]] = (RewardPackage)EditorGUILayout.ObjectField(rewardList.dictionary[tierList[i]], typeof(RewardPackage), false, new[] { GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300) });
            EditorGUILayout.EndHorizontal();
        }
        EditorGUIUtility.labelWidth = labelWidth;

        //BallReward ballReward = (BallReward)target;

        //ballReward.isRandom = EditorGUILayout.Toggle("Random", ballReward.isRandom);

        //if (!ballReward.isRandom)
        //{
        //    ballReward.index = EditorGUILayout.IntField("ID", ballReward.index);
        //}
    }
}



/*
[CustomPropertyDrawer(typeof(RewardList))]
public class RewardListPDrawer : PropertyDrawer
{
    Rect leftRect = new Rect();
    Rect rightRect = new Rect();
    int lineCount = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        float height = position.height / lineCount;
        float objectWidth = position.width * .5f;
        float earlyStartPosition = position.x - 40;
        leftRect.Set(earlyStartPosition, position.y, objectWidth, height);
        rightRect.Set(earlyStartPosition + objectWidth + 5, position.y, objectWidth + 35, height);

        EditorGUI.LabelField(leftRect, "This is connected");

        SerializedProperty prp_dictionary = property.FindPropertyRelative("dictionary");
        int count = prp_dictionary.FindPropertyRelative("Count").intValue;
        for (int i = 0; i < count; i++)
        {
            NextRow(height);
            EditorGUI.PropertyField(leftRect, prp_dictionary.GetArrayElementAtIndex(i), GUIContent.none);
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
*/
