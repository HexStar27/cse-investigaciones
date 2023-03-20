using Hexstar.Bezier;
using UnityEngine;

namespace Hexstar.CSE
{

    public class Flecha3D : MonoBehaviour
    {
        public Vector3 offset;
        [SerializeField] LineRenderer lr;
        public GameObject flecha;
        public Vector3 arco = new Vector3(10, 0, 0);
        Vector3 puntoArco;
        bool actualizandose;
        Transform inicio, fin;
        float distanciaActual;
        Vector3[] posiciones;

        private void Awake()
        {
            if (lr == null) lr = GetComponent<LineRenderer>();
            posiciones = new Vector3[lr.positionCount];
            flecha.SetActive(false);
        }

        private void Update()
        {
            if(actualizandose)
            {
                Actualizar();
            }
        }

        public void Inicializar(Transform objetivoI, Transform objetivoF)
        {
            inicio = objetivoI; fin = objetivoF;
            distanciaActual = 0;
        }

        private void Actualizar()
        {
            float nuevaDist = fin.position.y - inicio.position.y;
            if (distanciaActual != Mathf.Abs(nuevaDist))
            {
                distanciaActual = nuevaDist;

                lr.GetPositions(posiciones);
                puntoArco = inicio.position + new Vector3(0, fin.position.y - inicio.position.y, fin.position.z - inicio.position.z) * 0.5f + arco + offset;
                Vector3 I = inicio.position + offset;
                Vector3 F = fin.position + offset;
                for (int i = 0; i < lr.positionCount; i++)
                {
                    posiciones[i] = BezierCurves.CuadraticBezierCurve(I, F, puntoArco, 
                        (float)(i) / (lr.positionCount-1));
                }
                lr.SetPositions(posiciones);

                BezierCurves.BezierData2D d = BezierCurves.FullDataCuadraticBezierCurve2D(I, F, puntoArco, 1);
                flecha.transform.rotation = Quaternion.LookRotation(Vector3.back, d.normal);
            }
            else
            {
                lr.GetPositions(posiciones);
                for (int i = 0; i < posiciones.Length; i++)
                {
                    posiciones[i] += new Vector3(0, nuevaDist - distanciaActual);
                }
                distanciaActual = nuevaDist;
            }
            flecha.transform.position = posiciones[posiciones.Length - 1];
        }

        public void Activar(bool value)
        {
            actualizandose = value;
            lr.enabled = value;
            flecha.SetActive(value);
        }
    }
}