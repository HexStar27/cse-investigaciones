using UnityEngine;
using Hexstar.Graphics;
using TMPro;

public class GM2PC : MonoBehaviour
{
    GraphicMesh gm;

    [SerializeField] TextMeshProUGUI minT;
    [SerializeField] TextMeshProUGUI maxT;

    public void Setup(int[] valores)
    {
        float[] h = valores.Histograma(out int min, out int max);
        minT.text = min.ToString();
        maxT.text = max.ToString();
        float[] nuevo = new float[h.Length];
        for (int i = 0; i < nuevo.Length; i++) nuevo[i] = h[i];
        gm.datos = nuevo;
    }

    private void Awake()
    {
        gm = GetComponent<GraphicMesh>();
    }
}

public static class Stuff
{
    public static float[] Histograma(this int[] valores, out int min, out int max)
    {
        min = int.MaxValue;
        max = int.MinValue;
        for (int i = 0; i < valores.Length; i++)
        {
            int v = valores[i];
            if (v < min) min = v;
            if (v > max) max = v;
        }

        float[] h = new float[max - min + 1];
        for (int i = 0; i < valores.Length; i++)
            h[valores[i] - min]++;

        return h;
    }
}