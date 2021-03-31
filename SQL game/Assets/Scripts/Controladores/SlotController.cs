using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotController : MonoBehaviour
{
    [SerializeField] private ObjectPool op;
    [SerializeField] private uint cantidadInicial = 10;
    [SerializeField] private List<GameObject> enPosesion = new List<GameObject>();

    private float top, bottom;

    private void OnEnable()
    {
        ResetController();
    }

    private void LateUpdate()
    {
        if (transform.position.y < bottom) transform.position += Vector3.up * (bottom - transform.position.y);
        else if (transform.position.y > top) transform.position += Vector3.up * (top - transform.position.y);
    }

    private void OnDisable()
    {
        foreach (GameObject slotBloque in enPosesion)
        {
            slotBloque.SetActive(false);
        }

        enPosesion.Clear();
    }

    /// <summary>
    /// Vuelve a almacenar y a recolocar los solts, 
    /// además de re-ajustar los límites de su desplazamiento
    /// </summary>
    public void ResetController()
    {
        float distancia = 0;
        for (uint i = 0; i < cantidadInicial; i++)
        {
            GameObject slotBloque = op.RequestObj();
            if (slotBloque.TryGetComponent(out Slot slot))
            {
                slot.FijarId(i);
                enPosesion.Add(slotBloque);

                distancia += slot.Distanciamiento();
            }
            else
            {
                Debug.LogError("Esta clase \"SlotController\" debería tener una referencia a una \"Object Pool\" " +
                    "cuyos objetos sean prefabs de los slots. \nComo no lo cambies no va ha funcionar.");

                i = cantidadInicial;
            }
        }
        top = transform.position.y + distancia;
        bottom = transform.position.y;
    }

    public string Harvest()
    {
        string consultaFinal = "";
        for(int i = 0; i < enPosesion.Count; i++)
        {
            if (enPosesion[i].TryGetComponent(out Slot slot))
            {
                consultaFinal += slot.Trozo();
            }
        }

        return consultaFinal;
    }

    /// <summary>
    /// Esta función es sólo para comprobar
    /// el buen funcionamiento del recolector.
    /// Una vez funcione, debe ser eliminada por completo.
    /// </summary>
    public void ImprimirConsulta()
    {
        Debug.Log(Harvest());
    }
}
