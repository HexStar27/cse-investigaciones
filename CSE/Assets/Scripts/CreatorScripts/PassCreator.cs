using UnityEngine;

public class PassCreator : MonoBehaviour
{
    [TextArea(3,10)]
    public string texto;
    public string key, iv;
    [TextArea(3,10)]
    public string cifrado;
    private void Start()
    {
        Hexstar.SesionHandler.ciphKey = key;
        Hexstar.SesionHandler.ciphIv = iv;
    }
    void FixedUpdate()
    {
        cifrado = Hexstar.SesionHandler.Cifrar(texto);
    }
}
