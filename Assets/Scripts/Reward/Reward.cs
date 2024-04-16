using System;
using System.Text;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Reward : ScriptableObject
{
    public enum Type
    {
        BALL,
        CURRENCY
    }

    public virtual Type GetRewardType() { return Type.BALL; }
    public virtual string GetTypeName() { return ""; }
    public virtual Sprite GetIcon() { return null; }

#if UNITY_EDITOR
    public static Reward Create(System.Type classType, string filePath, bool returnIfAlreadyExists = false)
    {
        if (classType.IsSubclassOf(typeof(Reward)))
        {
            Reward asset;
            if (asset = AssetDatabase.LoadAssetAtPath<Reward>(filePath))
            {
                if (!returnIfAlreadyExists)
                    Debug.Log("Can't create Reward. A file with this exact name already exists.");
                return returnIfAlreadyExists ? asset : null;
            }

            asset = ScriptableObject.CreateInstance(classType) as Reward;

            AssetDatabase.CreateAsset(asset, filePath);
            AssetDatabase.SaveAssets();

            return asset;
        }

        return null;
    }

    public static bool Delete(Reward reward)
    {
        return AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(reward));
    }
#endif
}
