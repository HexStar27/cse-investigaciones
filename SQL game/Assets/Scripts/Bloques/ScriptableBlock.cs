using UnityEngine;

[CreateAssetMenu(fileName = "new Bloque", menuName = "Scriptable Objects/Bloque")]
public class ScriptableBlock : ScriptableObject
{
    public Vector2 siguienteBox;
    public Vector2[] anidadosBoxPos;
    public Vector2[] anidadosBoxSize;
    public Sprite dropSprite;
    public Sprite grabSprite;
    public string cadena;
}
