using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLimiter : MonoBehaviour
{
    public float bottom = 0, top = 0;

    private void LateUpdate()
    {
        if (transform.localPosition.y < bottom) transform.localPosition += Vector3.up * (bottom - transform.localPosition.y);
        else if (transform.localPosition.y > top) transform.localPosition += Vector3.up * (top - transform.localPosition.y);
    }
}
