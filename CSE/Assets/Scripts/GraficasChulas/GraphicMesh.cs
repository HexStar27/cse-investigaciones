using UnityEngine;
using Hexstar.Bezier;

namespace Hexstar.Graphics
{
    public class GraphicMesh : MonoBehaviour
    {
        public Material material;
        public bool update;
        public bool useMinAsLow = false;
        [Range(0, 4)] public int suavizado;
        [Range(0.1f, 1)] public float factorSuave = 0.5f;
        public float[] datos;
        private float[] gaussConv;
        private int oldSizeChange;
        private float oldFactorSuave;

        CanvasRenderer cRenderer;
        RectTransform rect;

        [HideInInspector] public Vector3[] newVertices;
        [HideInInspector] public Vector2[] newUV;
        [HideInInspector] public int[] newTriangles;
        Mesh mesh;

        private void Awake()
        {
            oldSizeChange = 0;
            oldFactorSuave = factorSuave;

            mesh = new Mesh();
            if (!cRenderer) if (!TryGetComponent(out cRenderer)) cRenderer = gameObject.AddComponent<CanvasRenderer>();
            if (!rect) if (!TryGetComponent(out rect)) Debug.LogError("Este componente sólo funciona en un canvas.");
            cRenderer.materialCount = 1;
            if (material) cRenderer.SetMaterial(material, 0);
        }

        private void OnEnable()
        {
            if (datos != null)
            {
                DatosAGrafica(datos);
            }
            if (newVertices != null) GenerarMesh();
            if (update) mesh.MarkDynamic();
        }

        private void Update()
        {
            if (update)
            {
                DatosAGrafica(datos);
                GenerarMesh();
            }
        }

        public void GenerarMesh()
        {
            if(newVertices == mesh.vertices) return;

            mesh.Clear();
            mesh.SetVertices(newVertices);
            mesh.uv = newUV;
            mesh.triangles = newTriangles;

            cRenderer.SetMesh(mesh);
        }

        public void CambiarMaterial(Material mat)
        {
            cRenderer.SetMaterial(mat, 0);
        }

        public void DatosAGrafica(float[] datos)
        {   
            if(useMinAsLow)
            {
                float min = float.MaxValue;
                for (int i = 0; i < datos.Length; i++)
                {
                    float d = datos[i];
                    if (d < min) min = d;
                }
                for(int i = 0; i < datos.Length; i++) datos[i] -= min;
            }


            if (suavizado > 0)
            {
                int potencia = 1 << suavizado;
                float[] nuevosDatos = new float[(datos.Length << suavizado) - (potencia - 1)];

                // 1º Generar matriz (si es necesario)
                GenerarMatrizConvolucion();
                //gaussConv;

                // 2º Segmentar vector
                float interv = (float)1 / potencia;
                for (int i = 0; i < nuevosDatos.Length-1; i++)
                {
                    int ii = i >> suavizado;
                    float p0 = datos[ii];
                    float p1 = datos[ii + 1];
                    float f = (i % potencia) * interv;
                    nuevosDatos[i] = (p1 - p0) * f + p0;
                }
                nuevosDatos[nuevosDatos.Length - 1] = datos[datos.Length - 1];

                // 3º Aplicar convolución
                datos = new float[nuevosDatos.Length];
                for (int i = 0; i < datos.Length; i++)
                {
                    float convi = 0;
                    for (int j = 0; j < gaussConv.Length; j++)
                    {
                        int indiceAjustado = i + j - (gaussConv.Length >> 1);
                        if (indiceAjustado < 0 || indiceAjustado >= datos.Length) indiceAjustado = i;

                        convi += nuevosDatos[indiceAjustado] * gaussConv[j];
                    }
                    datos[i] = convi;
                }
            }

            float x = -rect.rect.width * 0.5f, y = -rect.rect.height * 0.5f, z = 0;
            int n = datos.Length;
            int nn = n - 1;
            if (nn < 1) nn = 1;
            float incrementoX = rect.rect.width / nn;
            float max = float.MinValue;
            for(int i = 0; i < n; i++)
            {
                float d = datos[i];
                if (d > max) max = d;   
            }

            newVertices = new Vector3[n<<1];
            newUV = new Vector2[n<<1];
            for(var i = 0; i < n; i++) // Asignación de vértices y UV
            {
                float tY = datos[i] / max;
                newVertices[i] = new Vector3(x+(incrementoX*i),Mathf.Lerp(y, y+rect.rect.height,tY),z);
                newVertices[i + n] = new Vector3(x + (incrementoX * i), y, z);

                float uvX = Mathf.Lerp(0, 1, (float)i / nn);
                newUV[i] = new Vector2(uvX, Mathf.Lerp(0, 1, tY));
                newUV[i + n] = new Vector2(uvX, 0);
            }

            newTriangles = new int[6 * nn];
            for(var i = 0; i < n-1; i++) // Asiganción de triángulos
            {
                int j = 6 * i;
                newTriangles[j    ] = i;
                newTriangles[j + 1] = i + n + 1;
                newTriangles[j + 2] = i + n;

                newTriangles[j + 3] = i;
                newTriangles[j + 4] = i + 1;
                newTriangles[j + 5] = i + n + 1;
            }
        }

        private void GenerarMatrizConvolucion()
        {
            if(datos.Length << suavizado != oldSizeChange || factorSuave != oldFactorSuave)
            {
                oldSizeChange = datos.Length << suavizado;
                oldFactorSuave = factorSuave;
                gaussConv = new float[datos.Length << suavizado];
                
                float sqrtPi = Mathf.Sqrt(Mathf.PI);
                float b = factorSuave;
                float b2 = b * b;
                float e = (float)System.Math.E;
                float desp = 2;
                for (int i = 0; i < gaussConv.Length; i++)
                {
                    float x = ((float)(i << 2) / (gaussConv.Length - 1)) - desp;
                    gaussConv[i] = Mathf.Pow(e, -(x * x) / b2) / (b * sqrtPi);
                }
            }
        }
    }
}