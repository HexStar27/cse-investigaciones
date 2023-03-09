using UnityEngine;

namespace Hexstar.Bezier
{
    public class BezierCurves
    {
        public struct BezierData
        {
            public Vector3 pos;
            public Vector3 tangent;
        }

        public struct BezierData2D
        {
            public Vector3 pos;
            public Vector3 tangent;
            public Vector3 normal;
        }

        public static Vector3 CuadraticBezierCurve(Vector3 puntoA, Vector3 puntoB, Vector3 pCurvatura, float t)
        {
            Vector3 pAC = Vector3.Lerp(puntoA, pCurvatura, t);
            Vector3 pCB = Vector3.Lerp(pCurvatura, puntoB, t);
            return Vector3.Lerp(pAC, pCB, t);
        }

        /// <summary>
        /// *Unclamped*
        /// </summary>
        /// <param name="p0">Punto inicial</param>
        /// <param name="p2">Punto final</param>
        /// <param name="p1">Punto intermedio</param>
        /// <param name="t">Valor de 0 a 1 </param>
        /// <returns> Punto en "t" de la curva, tangente de la curva en "t" y normal de la curva en "t"</returns>
        public static BezierData2D FullDataCuadraticBezierCurve2D(Vector3 p0, Vector3 p2, Vector3 p1, float t)
        {
            Vector3 tan = ((2 * t * p0) - (2 * p0) - (4 * t * p1) + (2 * p1) + (2 * t * p2)).normalized;
            Vector3 normal = new Vector3(-tan.y, tan.x); //Gira 90 grados
            BezierData2D data = new BezierData2D
            {
                pos = p0 - (2 * t * p0) + (t * t * p0) + (2 * t * p1) - (2 * t * t * p1) + (t * t * p2),
                tangent = tan,
                normal = normal
            };
            return data;
        }

        /// <summary>
        /// *Unclamped*
        /// </summary>
        /// <param name="p0">Punto inicial</param>
        /// <param name="p2">Punto final</param>
        /// <param name="p1">Punto intermedio</param>
        /// <param name="t">Valor de 0 a 1 </param>
        /// <returns> Punto en "t" de la curva, tangente de la curva en "t"</returns>
        public static BezierData FullDataCuadraticBezierCurve3D(Vector3 p0, Vector3 p2, Vector3 p1, float t)
        {
            Vector3 tan = ((2 * t * p0) - (2 * p0) - (4 * t * p1) + (2 * p1) + (2 * t * p2)).normalized;
            BezierData data = new BezierData
            {
                pos = p0 - (2 * t * p0) + (t * t * p0) + (2 * t * p1) - (2 * t * t * p1) + (t * t * p2),
                tangent = tan
            };
            return data;
        }

        public static Vector3 NPointBezierCurve( Vector3[] points, float t)
        {
            if(points.Length > 0)
            {
                for (int i = 0; i < points.Length - 1; i++)
                {
                    for (int j = 0; j < points.Length - i - 1; j++)
                    {
                        points[j] = Vector3.Lerp(points[j], points[j + 1], t);
                    }
                }
                return points[0];
            }
            return Vector3.zero;
        }
    }
}