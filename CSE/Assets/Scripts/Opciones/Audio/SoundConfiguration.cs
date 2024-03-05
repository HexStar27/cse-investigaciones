using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class SoundConfiguration : OptionMB
{
    public static SoundConfiguration Instance { get; private set; }
    public AudioMixer mixer;
    private string[] mixerNames = { "SfxVolume", "MusicVolume", "MasterVolume" };
    [HideInInspector] public float[] volumeArray = new float[3];
    public UnityEvent onLoad = new UnityEvent();
    public UnityEvent onSave = new UnityEvent();

    private void Awake()
    {
        Instance = this;
    }

    public void SetVolume(int channel, float val)
    {
        if (val == 0) val = mute;
        else val = Mathf.Lerp(lowerLimit, upperLimit, val);
        volumeArray[channel] = val;
        mixer.SetFloat(mixerNames[channel], volumeArray[channel]);
    }

    public override void Save()
    {
        onSave?.Invoke();
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetFloat(mixerNames[i], volumeArray[i]);
        }
    }
    public override void Load()
    {
        if(Instance == null) Instance = this;
        for (int i = 0; i < 3; i++)
        {
            volumeArray[i] = PlayerPrefs.GetFloat(mixerNames[i], 0);
            mixer.SetFloat(mixerNames[i], volumeArray[i]);
        }
        onLoad?.Invoke();
    }

    public static readonly float lowerLimit = -40;
    public static readonly float mute = -80;
    public static readonly float upperLimit = 4;
}
