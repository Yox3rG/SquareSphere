using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class RewardListEditorWindow : EditorWindow
{
    public RewardList rewardList;

    private IList<int> indexedTiers;

    private Type selectedRewardType;
    private Reward.Type rewardType = Reward.Type.CURRENCY;
    private int tier = 1;
    private int viewIndex = 0;
    private bool returnRewardPackageIfExists = true;
    private bool returnRewardIfExists = true;

    private string str_nextRewardListName = "RewardList";

    // Looks
    GUIStyle horizontalLine;
    GUIStyle horizontalLineBigEdges;


    // File structure
    private readonly string path_defaultPackageFolder = "Assets/ScriptableObjects/Reward/Packages";
    private readonly string path_subFolderPrefixForPackages = "rwp_";
    private readonly string path_subFolderForRewards = "rwd";
    private string path_currentPackageFolder;

    [MenuItem("Window/RewardList Editor")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(RewardListEditorWindow));
    }

    void OnEnable()
    {
        horizontalLine = new GUIStyle();
        horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        horizontalLine.margin = new RectOffset(0, 0, 4, 4);
        horizontalLine.fixedHeight = 1;

        horizontalLineBigEdges = new GUIStyle(horizontalLine);
        horizontalLineBigEdges.margin = new RectOffset(40, 40, 10, 10);

        if (EditorPrefs.HasKey("ObjectPath"))
        {
            string objectPath = EditorPrefs.GetString("ObjectPath");
            rewardList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(RewardList)) as RewardList;
            if (rewardList != null)
            {
                if (rewardList.dictionary == null)
                    rewardList.Initialize();
                rewardList.name = GetNameFromPath(objectPath);
                indexedTiers = rewardList.dictionary.Keys;

                CreatePathAndFolderForPackages();
            }
        }
    }

    void OnGUI()
    {
        DrawRewardListManipulationHeader();

        DrawHorizontalLine(Color.grey, withSpacing: true);

        if (rewardList != null)
        {

            GUILayout.Label("Reward Package Navigation / Creation", EditorStyles.boldLabel);
            DrawRewardPackageCreationHeader();

            if (rewardList.dictionary == null)
                Debug.Log("RewardLists Dictionary is empty");
            if (rewardList.dictionary.Count > 0)
            {
                DrawRewardPackageNavigationHeader();

                if (rewardList.dictionary[indexedTiers[viewIndex]] != null)
                {
                    DrawHorizontalLine(Color.grey, withSpacing: true);
                    GUILayout.Label("Reward Creation", EditorStyles.boldLabel);
                    DrawRewardCreationHeader();
                }

                DrawHorizontalLine(Color.grey, withSpacing: true);
                DrawRewardPackageContent();
            }
            else
            {
                GUILayout.Label("This RewardList is Empty.");
            }
        }
        if (GUI.changed && rewardList)
        {
            EditorUtility.SetDirty(rewardList);
            if(indexedTiers.Count > 0 && rewardList.dictionary[indexedTiers[viewIndex]] != null)
            {
                EditorUtility.SetDirty(rewardList.dictionary[indexedTiers[viewIndex]]);
            }
        }
    }
    private void DrawRewardListManipulationHeader()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("RewardList Editor", EditorStyles.boldLabel);
        if (rewardList != null)
        {
            if (GUILayout.Button("Show"))
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = rewardList;
            }
        }
        if (GUILayout.Button("Open"))
        {
            OpenRewardList();
        }
        if (GUILayout.Button("New"))
        {
            CreateNewRewardList();
        }
        str_nextRewardListName = GUILayout.TextField(str_nextRewardListName, GUILayout.MaxWidth(150));
        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField("Current RewardList : ", rewardList.name);

    }

    private void DrawRewardPackageNavigationHeader()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("First", GUILayout.ExpandWidth(false)))
        {
            viewIndex = 0;
        }
        GUILayout.Space(5);
        if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
        {
            if (viewIndex > 0)
                viewIndex--;
        }
        GUILayout.Space(5);
        if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
        {
            if (viewIndex < rewardList.dictionary.Count - 1)
            {
                viewIndex++;
            }
        }
        GUILayout.Space(5);
        if (GUILayout.Button("Last", GUILayout.ExpandWidth(false)))
        {
            viewIndex = rewardList.dictionary.Count == 0 ? 0 : rewardList.dictionary.Count - 1;
        }

        GUILayout.Space(5);

        float labelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 75;
        viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Index", viewIndex, GUILayout.MaxWidth(125)), 0, rewardList.dictionary.Count - 1);
        EditorGUIUtility.labelWidth = 50;
        EditorGUILayout.LabelField("of  " + rewardList.dictionary.Count.ToString() + "  rewards", GUILayout.ExpandWidth(false));
        GUILayout.Space(5);
        EditorGUIUtility.labelWidth = 125;
        EditorGUILayout.LabelField("Current Tier:  " + indexedTiers[viewIndex].ToString(), GUILayout.ExpandWidth(false));
        EditorGUIUtility.labelWidth = labelWidth;
        GUILayout.EndHorizontal();
    }

    private void DrawRewardPackageCreationHeader()
    {
        GUILayout.BeginHorizontal();
        float labelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 60;
        tier = Mathf.Clamp(EditorGUILayout.IntField("New Tier", tier, new[] { GUILayout.ExpandWidth(true), GUILayout.MaxWidth(100) }), 1, Int32.MaxValue);
        EditorGUIUtility.labelWidth = labelWidth;

        GUILayout.Space(5);

        if (GUILayout.Button("Add Package", GUILayout.ExpandWidth(false)))
        {
            AddRewardPackage();
        }
        returnRewardPackageIfExists = GUILayout.Toggle(returnRewardPackageIfExists, GUIContent.none, GUILayout.ExpandWidth(false));
        if (GUILayout.Button("Remove Package", GUILayout.ExpandWidth(false)))
        {
            if (rewardList.dictionary.Count > 0)
            {
                RemoveRewardPackage(viewIndex);
            }
        }
        if (GUILayout.Button("Delete Package", GUILayout.ExpandWidth(false)))
        {
            if (rewardList.dictionary.Count > 0)
            {
                DeleteRewardPackage(viewIndex);
            }
        }
        GUILayout.EndHorizontal();
    }

    private void DrawRewardCreationHeader()
    {
        GUILayout.BeginHorizontal();



        GUILayout.Space(10);

        rewardType = (Reward.Type)EditorGUILayout.EnumPopup("", rewardType, GUILayout.MaxWidth(150));
        if (rewardType == Reward.Type.CURRENCY)
        {
            selectedRewardType = typeof(CurrencyReward);
        }
        else if (rewardType == Reward.Type.BALL)
        {
            selectedRewardType = typeof(BallReward);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Add Reward", GUILayout.ExpandWidth(false)))
        {
            AddReward();
        }
        if (GUILayout.Button("Add Empty", GUILayout.ExpandWidth(false)))
        {
            AddReward(isEmpty: true);
        }
        returnRewardIfExists = GUILayout.Toggle(returnRewardIfExists, GUIContent.none, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();
    }
    private void DrawRewardPackageContent()
    {
        GUILayout.Label("Reward Package", EditorStyles.boldLabel);

        RewardPackage package = rewardList.dictionary[indexedTiers.ElementAt(viewIndex)];
        if (package == null || package.Length == 0)
        {
            EditorGUILayout.LabelField("", "This package has no content or is null.", GUILayout.ExpandWidth(true));
            return;
        }

        GUILayout.BeginVertical();
        int i = 0;
        while (i < package.Length)
        {
            DrawHorizontalLine(Color.grey, bigEdges: true);
            GUILayout.BeginHorizontal();
            if (package[i] != null)
            {
                GUILayout.BeginVertical();
                EditorGUILayout.LabelField(package[i].GetTypeName(), "", GUILayout.ExpandWidth(false));

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Remove Reward", GUILayout.ExpandWidth(false)))
                {
                    RemoveReward(package[i]);
                    continue;
                }
                if (GUILayout.Button("Delete Reward", GUILayout.ExpandWidth(false)))
                {
                    DeleteReward(package[i]);
                    continue;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                SerializedObject so = new SerializedObject(package[i]);
                SerializedProperty sp = so.FindProperty("reward");

                EditorGUILayout.PropertyField(sp, GUIContent.none);
                so.ApplyModifiedProperties();
                so.Update();
            }
            else
            {
                GUILayout.BeginVertical();
                EditorGUILayout.LabelField("", "This reward is not assigned.", GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Remove Reward", GUILayout.ExpandWidth(false)))
                {
                    RemoveReward(package[i]);
                }
                GUILayout.EndVertical();
            }
            i++;
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

    }

    void CreateNewRewardList()
    {
        // There is no overwrite protection here!
        // There is No "Are you sure you want to overwrite your existing object?" if it exists.
        // This should probably get a string from the user to create a new name and pass it ...
        viewIndex = 0;
        
        RewardList list = RewardList.Create(str_nextRewardListName);
        if (list)
        {
            rewardList = list;
            rewardList.Initialize();
            string relPath = AssetDatabase.GetAssetPath(rewardList);
            EditorPrefs.SetString("ObjectPath", relPath);
            rewardList.name = GetNameFromPath(relPath);
            indexedTiers = rewardList.dictionary.Keys;

            CreatePathAndFolderForPackages();
        }
    }


    void OpenRewardList()
    {
        string absPath = EditorUtility.OpenFilePanel("Select RewardList", "", "");
        if (absPath.StartsWith(Application.dataPath))
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            rewardList = AssetDatabase.LoadAssetAtPath(relPath, typeof(RewardList)) as RewardList;
            if (rewardList.dictionary == null)
                rewardList.Initialize();
            if (rewardList)
            {
                EditorPrefs.SetString("ObjectPath", relPath);
                rewardList.name = GetNameFromPath(relPath);
                indexedTiers = rewardList.dictionary.Keys;

                CreatePathAndFolderForPackages();
            }
        }
    }

    void AddRewardPackage(bool isEmpty = false)
    {
        if (rewardList.dictionary.ContainsKey(tier))
        {
            Debug.Log("That tier already exists, no RewardPackage created. Navigating to existing one...");
            viewIndex = indexedTiers.ToList().IndexOf(tier);
        }
        else
        {
            RewardPackage newReward = null;
            if (!isEmpty)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(path_currentPackageFolder).
                    Append("/Tier").
                    Append(tier.ToString()).
                    Append(".asset");
                newReward = RewardPackage.Create(stringBuilder.ToString(), returnRewardPackageIfExists);
                    
                if (newReward == null)
                    return;
            }

            rewardList.dictionary.Add(tier, newReward);
            viewIndex = indexedTiers.ToList().IndexOf(tier);
            tier++;
        }
    }

    void RemoveRewardPackage(int index)
    {
        rewardList.dictionary.Remove(indexedTiers.ElementAt(index));
        AssetDatabase.SaveAssets();
    }

    void DeleteRewardPackage(int index)
    {
        RewardPackage package = rewardList.dictionary[indexedTiers[index]];
        rewardList.dictionary.Remove(indexedTiers[index]);
        RewardPackage.Delete(package, withChildren: true);
        AssetDatabase.SaveAssets();
    }

    void AddReward(bool isEmpty = false)
    {
        Reward newReward = null;
        if (!isEmpty)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(path_currentPackageFolder).
                Append("/").
                Append(path_subFolderForRewards).
                Append("/Rwd_T").
                Append(indexedTiers[viewIndex]).
                Append("_N").
                Append(rewardList.dictionary[indexedTiers[viewIndex]].Length).
                Append(".asset");

            newReward = Reward.Create(selectedRewardType, stringBuilder.ToString(), returnRewardIfExists);
            if (newReward == null)
                return;
        }
        rewardList.dictionary[indexedTiers[viewIndex]].Add(newReward);
    }

    void RemoveReward(Reward reward)
    {
        rewardList.dictionary[indexedTiers[viewIndex]].Remove(reward);
        //rewardList.dictionary.Remove(indexedTiers.ElementAt(index));
    }

    void DeleteReward(Reward reward)
    {
        rewardList.dictionary[indexedTiers[viewIndex]].Remove(reward);
        Reward.Delete(reward);
    }

    // New Stuff.
    private void DrawHorizontalLine(Color color, bool withSpacing = false, bool bigEdges = false)
    {
        if(withSpacing)
            GUILayout.Space(5);

        var c = GUI.color;
        GUI.color = color;
        GUILayout.Box(GUIContent.none, bigEdges ? horizontalLineBigEdges : horizontalLine);
        GUI.color = c;

        if(withSpacing)
            GUILayout.Space(5);
    }
    private string GetNameFromPath(string objectPath)
    {
        string[] slicedPath = objectPath.Split('/');
        
        return slicedPath[slicedPath.Length - 1].Split('.')[0];
    }

    private void CreatePathAndFolderForPackages()
    {
        path_currentPackageFolder = path_defaultPackageFolder + "/" + path_subFolderPrefixForPackages + rewardList.name;

        string folderName = path_subFolderPrefixForPackages + rewardList.name;
        if (!AssetDatabase.IsValidFolder(path_defaultPackageFolder + "/" + folderName))
        {
            AssetDatabase.CreateFolder(path_defaultPackageFolder, folderName);
        }
        if (!AssetDatabase.IsValidFolder(path_currentPackageFolder + "/" + path_subFolderForRewards))
        {
            AssetDatabase.CreateFolder(path_currentPackageFolder, path_subFolderForRewards);
        }
    }
}
