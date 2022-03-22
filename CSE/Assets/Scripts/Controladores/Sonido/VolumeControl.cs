using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] string _volumeParameter = "MasterVolume";
    public AudioMixer _mixer;
    public Slider _slider;
    [SerializeField] float _multiplier = 30f;

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(_volumeParameter, _slider.value);
    }

    private void Start()
    {
        _slider.value = PlayerPrefs.GetFloat(_volumeParameter, _slider.value);
    }

    private void Awake()
    {
        _slider.onValueChanged.AddListener(HandleSliderValueChanged);
    }

    private void HandleSliderValueChanged(float arg0)
    {
        if (arg0 != 0) _mixer.SetFloat(_volumeParameter, Mathf.Log10(arg0) * _multiplier);
        else _mixer.SetFloat(_volumeParameter, -80);
    }
}
