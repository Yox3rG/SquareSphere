using System.Collections.Generic;
using System;
using System.Text;
using UnityEditor;
using UnityEngine;


[Serializable]
[CreateAssetMenu(fileName = "rwp_New", menuName = "ScriptableObjects/Reward/Packages", order = 120)]
public class RewardPackage : ScriptableObject
{
    [SerializeField] public List<Reward> rewards = new List<Reward>();

#if UNITY_EDITOR
    public static RewardPackage Create(string pathwithID, bool returnIfAlreadyExists = false)
    {
        RewardPackage asset;

        if (asset = AssetDatabase.LoadAssetAtPath<RewardPackage>(pathwithID))
        {
            if(!returnIfAlreadyExists)
                Debug.Log("Can't create RewardPackage. A file with this exact name already exists.\nAdding this file.");
            
            return returnIfAlreadyExists ? asset : null;
        }
        
        asset = ScriptableObject.CreateInstance<RewardPackage>();

        AssetDatabase.CreateAsset(asset, pathwithID);
        AssetDatabase.SaveAssets();

        return asset;
    }

    public static bool Delete(RewardPackage package, bool withChildren = false)
    {
        if (withChildren && package != null)
        {
            foreach (Reward r in package.rewards)
            {
                Reward.Delete(r);
            }
        }
        return AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(package));
    }
#endif

    public Reward this[int index]
    {
        get
        {
            return rewards[index];
        }
        set
        {
            rewards[index] = value;
        }
    }

    public void Add(Reward reward)
    {
        rewards.Add(reward);
    }

    public bool Remove(Reward reward)
    {
        return rewards.Remove(reward);
    }

    public int Length { get
        {
            return rewards.Count;
        } 
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder("RewardPackage: ");
        char separator = ';';
        foreach(Reward r in rewards)
        {
            stringBuilder.Append(r.ToString()).Append(separator);
        }
        return stringBuilder.ToString();
    }
}