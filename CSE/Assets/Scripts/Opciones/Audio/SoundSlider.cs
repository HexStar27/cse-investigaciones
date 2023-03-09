using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    public string varName = "SfxVolume";
    [SerializeField] private AudioMixer mixer;
    private Slider slider;
    void OnEnable()
    {
        if (!slider) slider = GetComponent<Slider>();
        if (mixer.GetFloat(varName, out float val))
        {
            if(val != SoundConfiguration.mute)
                val = Mathf.InverseLerp(SoundConfiguration.lowerLimit, SoundConfiguration.upperLimit, val);
            
            slider.value = val;
        }
    }
}
