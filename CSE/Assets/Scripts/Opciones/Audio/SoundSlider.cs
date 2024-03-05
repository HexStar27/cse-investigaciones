using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour, ISliderValue
{
    public int mixerChannel = 0;
    [SerializeField] private Slider slider;

    public void Load()
    {
        float val = SoundConfiguration.Instance.volumeArray[mixerChannel];
        if (val != SoundConfiguration.mute)
            val = Mathf.InverseLerp(SoundConfiguration.lowerLimit, SoundConfiguration.upperLimit, val);
        slider.value = val;
    }

    public void Save()
    {
        SoundConfiguration.Instance.SetVolume(mixerChannel, slider.value);
    }

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
    }

    void OnEnable()
    {
        slider.onValueChanged.AddListener(Changed);
    }
    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(Changed);
    }

    private void Changed(float val)
    {
        SoundConfiguration.Instance.SetVolume(mixerChannel, val);
    }

    public Slider GetSlider()
    {
        return slider;
    }
}
