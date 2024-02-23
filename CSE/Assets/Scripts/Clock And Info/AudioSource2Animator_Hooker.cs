using System.Collections.Generic;
using UnityEngine;

public class AudioSource2Animator_Hooker : MonoBehaviour
{
    public AudioSource audioS;
    public List<AudioClip> clips = new();
    public float volume = 1.0f;

    public void PlayAudio(int i)
    {
        if (i < 0 || i >= clips.Count) return;
        audioS.PlayOneShot(clips[i],volume);
    }
}
