using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "new Bloque", menuName = "Scriptable Objects/Bloque")]
public class ScriptableBlock : ScriptableObject
{
    public Sprite releaseSprite;
    public Sprite grabSprite;
    public string cadena;
    public enum TipoBloque{SinCampo,CampoColumna,CampoTabla,CampoBloque};
    public TipoBloque tipoBloque;
}
