using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NuevaDificultad", menuName = "ScriptableObjects/Dificultad")]
public class ScriptableDificultad : ScriptableObject
{
    public List<int> intervalos;
    public List<string> descripciones;
}
