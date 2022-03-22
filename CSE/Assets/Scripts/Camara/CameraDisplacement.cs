using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDisplacement : MonoBehaviour
{
    public Transform defaultObjetive;
    public float defaultFactor;
    public Animator animador;
    public Transform[] posiciones;

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

    public void DesplazamientoDirecto(int indice)
    {
        if (isMoving) StopAllCoroutines();

        Vector3 offsetNuevaPos = posiciones[indice].position + (Vector3.forward * defaultObjetive.position.z);
        defaultObjetive.position = offsetNuevaPos;
    }

    public void DespDirectoAnimado(int indice)
    {
        if (isMoving) StopAllCoroutines();

        if(animador != null)
            StartCoroutine(DesplazamientoAnimado(defaultObjetive, posiciones[indice], animador));
        else
        {
            Vector3 offsetNuevaPos = posiciones[indice].position + (Vector3.forward * defaultObjetive.position.z);
            defaultObjetive.position = offsetNuevaPos;
        }
    }

    public void DesplazamientoSuave(Transform objetivo, Transform nuevaPos, float factor)
    {
        if (isMoving) StopAllCoroutines();

        StartCoroutine(SmoothMove(objetivo, nuevaPos, factor));
    }



    private IEnumerator DesplazamientoAnimado(Transform elemento, Transform nuevaPos, Animator animador)
    {
        //ESTO QUEDA POR HACER

        yield return null;
    }

    private IEnumerator SmoothMove(Transform elemento, Transform nuevaPos, float factor)
    {
        WaitForEndOfFrame finFrame = new WaitForEndOfFrame();

        Vector3 offsetNuevaPos = nuevaPos.position + (Vector3.forward * elemento.position.z);

        isMoving = true;
        while (Mathf.Abs(elemento.position.x - offsetNuevaPos.x) > 0.01f ||
            Mathf.Abs(elemento.position.y - offsetNuevaPos.y) > 0.01f)
        {
            elemento.position = Vector3.Lerp(elemento.position, offsetNuevaPos, factor);
            yield return finFrame;

        }
        elemento.position = offsetNuevaPos;
        isMoving = false;

        yield return null;
    }
}
