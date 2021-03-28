using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSmoothMove : MonoBehaviour
{
    public RectTransform defaultObjetive;
    public float defaultFactor;
    public Vector2[] posiciones;

    private bool isMoving = false;

    private void OnEnable()
    {
        isMoving = false;
    }

    /// <summary>
    /// Desplaza suavemente el defaultObjetive con un defaultFactor hasta la posición indicada
    /// </summary>
    /// <param name="indice">El índice de la posición</param>
    public void DespSuaveParaBoton(int indice)
    {
        if (isMoving) StopAllCoroutines();

        StartCoroutine(SmoothMove(defaultObjetive, posiciones[indice], defaultFactor));
    }

    public void DesplazamientoSuave(RectTransform objetivo, Vector2 nuevaPos, float factor)
    {
        if (isMoving) StopAllCoroutines();

        StartCoroutine(SmoothMove(objetivo, nuevaPos, factor));
    }

    private IEnumerator SmoothMove(RectTransform elemento, Vector2 nuevaPos, float factor)
    {
        WaitForFixedUpdate finFrame = new WaitForFixedUpdate();

        isMoving = true;
        while (Mathf.Abs(elemento.anchoredPosition.x - nuevaPos.x) > 1f ||
            Mathf.Abs(elemento.anchoredPosition.y - nuevaPos.y) > 1f)
        {
            elemento.anchoredPosition = Vector2.Lerp(elemento.anchoredPosition, nuevaPos, factor);
            yield return finFrame;

        }
        elemento.anchoredPosition = nuevaPos;
        isMoving = false;

        yield return null;
    }
}
