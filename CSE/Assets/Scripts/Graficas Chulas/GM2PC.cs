using UnityEngine;
using Hexstar.Graphics;
using TMPro;

public class GM2PC : MonoBehaviour
{
    GraphicMesh gm;

    [SerializeField] TextMeshProUGUI minT;
    [SerializeField] TextMeshProUGUI maxT;
    [SerializeField] RectTransform rectGrafica; //RectTransform que pertenece a la gráfica
    [SerializeField] RectTransform marca; //Línea que indica en qué parte del rango de puntuación está el jugador

    public void Hide() { gm.HideMesh(); }

    public void Setup(int[] valores, float playerVal = 0f)
    {
        float[] h = valores.Histograma(out int min, out int max);
        minT.text = min.ToString();
        maxT.text = max.ToString();

        float posNormalizada = Mathf.InverseLerp(min, max, playerVal);
        marca.anchoredPosition = new Vector2(rectGrafica.rect.width * posNormalizada, marca.anchoredPosition.y);
        //Debug.Log("Valor = " + playerVal + ", normalizado = " + posNormalizada + ", posicion = " + (rectGrafica.rect.width * posNormalizada));

        gm.PrepararDatos(h,true);
        gm.GenerarMesh();
    }

    private void Awake()
    {
        gm = GetComponent<GraphicMesh>();
    }
}

public static class Stuff
{
    public static float[] Histograma(this int[] valores, out int min, out int max, int nBarrasTope = 10)
    {
        min = int.MaxValue;
        max = int.MinValue;
        for (int i = 0; i < valores.Length; i++)
        {
            int v = valores[i];
            if (v < min) min = v;
            if (v > max) max = v;
        }

        int nBarras = Mathf.Min(nBarrasTope, max-min);
        //Debug.Log("Nº barras a preparar: " + (nBarras));
        float[] h = new float[nBarras + 1];
        int rangoDeBarra = (max - min) / (nBarras);
        //Debug.Log("Rango de cada barra: " + rangoDeBarra);
        for (int i = 0; i < valores.Length; i++)
        {
            //Debug.Log("Valor a colocar: " + (valores[i] - min) + ", Indice en histograma: " + ((valores[i] - min) / rangoDeBarra) + " de "+nBarras);
            h[(valores[i] - min) / rangoDeBarra]++;
        }

        return h;
    }
}