using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseCreator : MonoBehaviour
{
    public bool bonito = true;
    [TextArea(15,20)]
    public string json;
    public Caso caso;

    public void FixedUpdate()
    {
        json = JsonUtility.ToJson(caso, bonito);
    }

    [ContextMenu("Actualizar JSON")]
    public void Actualizar() => json = JsonUtility.ToJson(caso, bonito);
}
