using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoadHandlerJSON<T>
{
    private static readonly string path = Application.persistentDataPath;



    public static bool Save(T data, string fileName)
    {
        string tempPath = Path.Combine(path, fileName);

        try
        {
            using (StreamWriter streamWriter = File.CreateText(tempPath))
            {
                string jsonString = JsonUtility.ToJson(data);
                streamWriter.Write(jsonString);
#if UNITY_EDITOR
                Debug.Log("Saved succesfully");
#endif
                return true;
            }
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.Log("File save error\n" + e.Message);
#endif
        }
        return false;
    }
    
    public static bool Load(string fileName, out T data)
    {
        string tempPath = Path.Combine(path, fileName);
        //BinaryFormatter binaryFormatter = new BinaryFormatter();
        //FileStream fileStream = File.Open(fileName, FileMode.Open);

        try
        {
            using (StreamReader streamReader = File.OpenText(tempPath))
            {
                string jsonString = streamReader.ReadToEnd();
#if UNITY_EDITOR
                Debug.Log("Loaded succesfully");
#endif
                data = JsonUtility.FromJson<T>(jsonString);
                return true;
            }
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.Log("File load error\n" + e.Message);
#endif
        }
        data = default;
        return false;
    }

    // This is used for statistical logging purposes.
    public static bool AppendLine(T data, string fileName)
    {
        string tempPath = Path.Combine(path, fileName);

        try
        {
            using (StreamWriter streamWriter = File.AppendText(tempPath))
            {
                string dataString = data.ToString();
                streamWriter.WriteLine(dataString);
#if UNITY_EDITOR
                Debug.Log("Appended succesfully");
#endif
                return true;
            }
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.Log("File append error\n" + e.Message);
#endif
        }
        return false;
    }

    public static bool FileExists(string fileName)
    {
        string tempPath = Path.Combine(path, fileName);

        return File.Exists(tempPath);
    }
}
