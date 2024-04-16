using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager main;

    public Slider sliderMusic;
    public Slider sliderSfx;
    public AudioMixer mixer;

    void Awake()
    {
        if(main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SetVolumeFloatsFromData();
    }

    public void SetVolumeFloatsFromData()
    {
        Dictionary<string, float> volumes = ProfileDataBase.main.GetMusicVolumes();

        foreach (KeyValuePair<string, float> entry in volumes)
        {
            SetVolumeFloat(entry.Key, entry.Value);
        }

        sliderMusic.value = volumes["musicVol"];
        sliderSfx.value = volumes["sfxVol"];
    }

    private void SetVolumeFloat(string name, float value)
    {
        mixer.SetFloat(name, value);
    }

    public void SetMasterVolume(float masterVolume)
    {
        mixer.SetFloat("masterVol", masterVolume);
        ProfileDataBase.main.ChangeDefaultMasterVolume(masterVolume);
    }

    public void SetMusicVolume(float musicVolume)
    {
        mixer.SetFloat("musicVol", musicVolume);
        ProfileDataBase.main.ChangeDefaultMusicVolume(musicVolume);
    }

    public void SetSFXVolume(float sfxVolume)
    {
        mixer.SetFloat("sfxVol", sfxVolume);
        ProfileDataBase.main.ChangeDefaultSFXVolume(sfxVolume);
    }
}
