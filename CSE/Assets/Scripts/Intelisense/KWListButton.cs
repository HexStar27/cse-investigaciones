using UnityEngine;

public class KWListButton : MonoBehaviour
{
    [HideInInspector] public int idx;
    public void AutocompleteThis()
    {
        Intelisense.instance.Autocomplete(idx);
        Intelisense.instance.SelectText();
    }
}
