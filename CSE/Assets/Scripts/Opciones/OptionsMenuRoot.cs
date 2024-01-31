using UnityEngine;
public class OptionsMenuRoot : MonoBehaviour
{
    private static OptionsMenuRoot i;
    public static OptionsMenuRoot Instance() {
        if (i == null) Debug.LogError("No se ha encontrado una raiz del menú de opciones...");
        return i;
    }
    private void Awake()
    {
        i = this;
    }
}
