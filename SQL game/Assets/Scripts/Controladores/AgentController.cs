using TMPro;
using UnityEngine;

//Este debería ser MonoSingleton
public class AgentController : MonoBehaviour
{
    private int agentes;

    [SerializeField]
#pragma warning disable 0649
    private TextMeshProUGUI tm;
#pragma warning restore 0649

    public int GetAgentes()
    {
        return agentes;
    }

    public bool AgentesDisponibles()
    {
        return agentes > 0;
    }

    public void EnviarAgente()
    {
        agentes--;
        ActualizarUIAgentes();
    }

    public void ObtenerAgente()
    {
        agentes++;
        ActualizarUIAgentes();
    }

    public void FijarAgentes(int cantidad)
    {
        agentes = cantidad;
        ActualizarUIAgentes();
    }


    private void ActualizarUIAgentes()
    {
        if (tm)
        {
            tm.text = "Agentes: " + agentes;
        }
    }
}
