using Hexstar.Bezier;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierDemo : MonoBehaviour
{
    public Vector3 Inicio, Curva, Fin;
    float t;
    void FixedUpdate()
    {
        if (t > 1) t = 0;
        transform.position = BezierCurves.CuadraticBezierCurve(Inicio, Fin, Curva, t);
        t += Time.fixedDeltaTime;
    }
}
