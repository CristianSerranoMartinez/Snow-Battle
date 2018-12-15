using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ConfigurationData {
    public float musicVolume;
    public float effectsVolume;

    public ConfigurationData(float musicVolume, float effectsVolume)
    {
        this.musicVolume = musicVolume;

        this.effectsVolume = effectsVolume;
    }
}
