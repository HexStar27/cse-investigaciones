using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosDetector : MonoBehaviour
{
    public Camera cPrincipal;
    private static Vector3 mousePos;

    public static Vector3 MousePos()
    {
        return mousePos;
    }

    private void Update()
    {
        mousePos = cPrincipal.ScreenToWorldPoint(Input.mousePosition);
    }
}
