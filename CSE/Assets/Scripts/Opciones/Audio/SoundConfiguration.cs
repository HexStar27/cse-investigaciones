using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName ="SoundConfiguration",menuName ="Hexstar/Configuration/Sound")]
public class SoundConfiguration : ScriptableObject
{
    public AudioMixer mixer;
    public float sfxVolume = 0, bgmVolume = 0, generalVolume = 0;

    private void OnEnable()
    {
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 0);
        bgmVolume = PlayerPrefs.GetFloat("bgmVolume", 0);
        generalVolume = PlayerPrefs.GetFloat("generalVolume", 0);
        mixer.SetFloat("SfxVolume", sfxVolume);
        mixer.SetFloat("MusicVolume", bgmVolume);
        mixer.SetFloat("MasterVolume", generalVolume);
    }

    public void SetSfxVolume(float val)
    {
        if (val == 0) val = mute;
        else val = Mathf.Lerp(lowerLimit, upperLimit, val);
        sfxVolume = val;
        mixer.SetFloat("SfxVolume", sfxVolume);
    }
    public void SetBgmVolume(float val)
    {
        if (val == 0) val = mute;
        else val = Mathf.Lerp(lowerLimit, upperLimit, val);
        bgmVolume = val;
        mixer.SetFloat("MusicVolume", bgmVolume);
    }
    public void SetGeneralVolume(float val)
    {
        if (val == 0) val = mute;
        else val = Mathf.Lerp(lowerLimit, upperLimit, val);
        generalVolume = val;
        mixer.SetFloat("MasterVolume", generalVolume);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("SfxVolume",sfxVolume);
        PlayerPrefs.SetFloat("BgmVolume",bgmVolume);
        PlayerPrefs.SetFloat("GeneralVolume",generalVolume);
        PlayerPrefs.Save();
    }
    public static readonly float lowerLimit = -40;
    public static readonly float mute = -80;
    public static readonly float upperLimit = 4;
}
