using UnityEngine;

public interface ISaveMe
{
    void Load();
    void Save();
}

[System.Serializable]
public class OptionMB : MonoBehaviour, ISaveMe
{
    public virtual void Load()
    {
    }

    public virtual void Save()
    {
    }
}