using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProfileData
{
    public string name;

    public int level;
    public long experienceOnCurrentLevel;

    public long stars;
    public int gold;
    public int diamondApples;
    


    public List<byte> starsOnMaps;

    public ProfileData()
    {
        name = "";

        level = 0;
        experienceOnCurrentLevel = 0;
        stars = 0;
        gold = 0;
        diamondApples = 0;
        
        starsOnMaps = new List<byte>();
    }
}
