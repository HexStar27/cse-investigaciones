using TMPro;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private float factorDeDistanciamiento = 1;
    private uint id;
    private string trozo = "";

    [Header("Opcinal")]
    [SerializeField] private TextMeshPro indice;

    private void OnEnable()
    {
        ActualizarIndice();
    }

    public void AsignarTrozoConsulta(string trozo)
    {
        this.trozo = trozo;
    }

    public string Trozo()
    {
        return trozo;
    }

    public uint Id() { return id; }

    public void FijarId(uint nuevaId)
    {
        id = nuevaId;
        ActualizarIndice();
        ActualizarPosicion();
    }

    public float Distanciamiento()
    {
        return factorDeDistanciamiento;
    }

    private void ActualizarPosicion()
    {
        transform.localPosition = new Vector3(0, -factorDeDistanciamiento * id, 0);
    }

    private void ActualizarIndice()
    {
        if (indice) indice.text = id.ToString();
    }
}
