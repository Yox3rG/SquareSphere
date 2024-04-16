using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    public bool isChallengeMode;

    public SettingsData()
    {
        masterVolume = 0;
        musicVolume = -40;
        sfxVolume = -40;

        isChallengeMode = true;
    }
}
