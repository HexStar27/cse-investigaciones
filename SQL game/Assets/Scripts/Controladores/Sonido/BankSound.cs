using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class BankSound : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public Sound(string s, AudioSource a) {
            name = s; 
            source = a;
        }
        public string name;
        public AudioSource source;
        public int bpm;
    }

    [SerializeField]
    private Sound[] sounds = new Sound[0];

    public static BankSound instancia;
    public static int PCMrate = 44000;

    private void Awake()
    {
        PCMrate = AudioSettings.outputSampleRate;
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void Play(string nombre)
    {
        Sound s = Array.Find(sounds, sound => sound.name == nombre);
        if(s != null) s.source.Play();
    }

    public void Play(uint indice)
    {
        if (indice < sounds.Length) sounds[indice].source.Play();
    }

    public void Pause(string nombre)
    {
        Sound s = Array.Find(sounds, sound => sound.name == nombre);
        if (s != null) s.source.Pause();
    }

    public void Pause(uint indice)
    {
        if (indice < sounds.Length) sounds[indice].source.Pause();
    }

    public void Stop(string nombre)
    {
        Sound s = Array.Find(sounds, sound => sound.name == nombre);
        if (s != null) s.source.Stop();
    }

    public void Stop(uint indice)
    {
        if (indice < sounds.Length) sounds[indice].source.Stop();
    }

    public void Continue(string nombre)
    {
        Sound s = Array.Find(sounds, sound => sound.name == nombre);
        if (s != null) s.source.UnPause();
    }

    public void Continue(uint indice)
    {
        if (indice < sounds.Length) sounds[indice].source.UnPause();
    }

    public uint Indice(string nombre)
    {
        uint i = 0;
        bool found = false;
        for (; i < sounds.Length && !found; i++)
            if (sounds[i].name.CompareTo(nombre) == 0) found = true;

        return i;
    }


    public void Loop(string nombre, bool mantener)
    {
        Sound s = Array.Find(sounds, sound => sound.name == nombre);
        if (s != null) s.source.loop = mantener;
    }

    public void Loop(uint indice, bool mantener)
    {
        if (indice < sounds.Length) sounds[indice].source.loop = mantener;
    }

    public float Tiempo(string nombre)
    {
        Sound s = Array.Find(sounds, sound => sound.name == nombre);
        if (s != null) return s.source.time;
        return 0;
    }

    public float Tiempo(uint indice)
    {
        if (indice < sounds.Length) return sounds[indice].source.time;
        return 0;
    }

    public void PlayFrom(string nombre, int pcm)
    {
        Sound s = Array.Find(sounds, sound => sound.name == nombre);
        if (s != null)
        {
            s.source.Play();
            s.source.timeSamples = pcm;
        }
    }

    public void PlayFrom(uint indice, int pcm)
    {
        if (indice < sounds.Length)
        {
            sounds[indice].source.Play();
            sounds[indice].source.timeSamples = pcm;
        }
    }

    public int BeatToPCM(int BPM, int beat)
    {
        return 1+(int)(PCMrate / ((double)BPM / 60)) * beat;
    }

    public int PCMtoBeat(int BPM, int PCM)
    {
        return (int)(PCM / (PCMrate / ((double)BPM / 60)));
    }

    public void Mute(string nombre)
    {
        Sound s = Array.Find(sounds, sound => sound.name == nombre);
        if (s != null) s.source.mute = true;
    }

    public void Mute(uint indice)
    {
        if (indice < sounds.Length) sounds[indice].source.mute = true;
    }

    public bool IsPlaying(string nombre)
    {
        Sound s = Array.Find(sounds, sound => sound.name == nombre);
        if (s != null) return s.source.isPlaying;
        return false;
    }

    public bool IsPlaying(uint indice)
    {
        if (indice < sounds.Length) return sounds[indice].source.isPlaying;
        return false;
    }

    public Sound Audio(uint indice) { return sounds[indice]; }

    public void AddAudio(Sound s)
    {
        sounds.ToList().Add(s);
    }

    public void RemoveAudio(uint indice)
    {
        sounds.ToList().Remove(sounds[indice]);
    }
}
