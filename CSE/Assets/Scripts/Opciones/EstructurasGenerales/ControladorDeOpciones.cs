using System.Collections.Generic;
using UnityEngine;

public class ControladorDeOpciones : MonoBehaviour
{
    public List<OptionMB> thingsToSave = new List<OptionMB>();
    private void OnEnable()
    {
        foreach(var thing in thingsToSave)
        {
            thing.Load();
        }
    }
    private void OnDisable()
    {
        foreach (var thing in thingsToSave)
        {
            thing.Save();
        }
        PlayerPrefs.Save();
    }
}
