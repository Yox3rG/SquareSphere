
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "rwdList", menuName = "ScriptableObjects/Reward/List", order = 121)]
public class RewardList : ScriptableObject, ISerializationCallbackReceiver
{
    public SortedList<int, RewardPackage> dictionary;

    [SerializeField, HideInInspector] int[] tierList;
    [SerializeField, HideInInspector] RewardPackage[] rewardList;
    //[SerializeField] List<int> tierList;
    //[SerializeField] List<Reward> rewardList;

    public void Initialize()
    {
        dictionary = new SortedList<int, RewardPackage>();
    }

    public bool ContainsKey(int key)
    {
        return dictionary.ContainsKey(key);
    }

#if UNITY_EDITOR
    public static RewardList Create(string fileName)
    {
        RewardList asset = ScriptableObject.CreateInstance<RewardList>() as RewardList;
        string filePath = "Assets/ScriptableObjects/Reward/" + fileName + ".asset";
        if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(filePath))
        {
            Debug.Log("Can't create RewardList. A file with the default name already exists.");
            return null;
        }
        else
        {
            AssetDatabase.CreateAsset(asset, filePath);
            AssetDatabase.SaveAssets();
        }

        return asset;
    }
#endif

    public void OnBeforeSerialize()
    {
        if (dictionary == null)
            return;

        if (dictionary.Count > 0)
        {
            tierList = new int[dictionary.Count];
            rewardList = new RewardPackage[dictionary.Count];

            dictionary.Keys.CopyTo(tierList, 0);
            dictionary.Values.CopyTo(rewardList, 0);
        }
    }

    public void OnAfterDeserialize()
    {
        if (dictionary == null)
            Initialize();

        if (tierList.Length == rewardList.Length)
        {
            for(int i = 0; i < tierList.Length; i++)
            {
                dictionary[tierList[i]] = rewardList[i];
            }
        }
    }
}
