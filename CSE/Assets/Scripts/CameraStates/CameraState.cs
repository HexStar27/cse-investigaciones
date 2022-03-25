using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraState : MonoBehaviour
{
    [System.Serializable]
    public class OnFinished : UnityEvent { }

    [System.Serializable]
    private struct Estado
    {
        public Vector3 posicion;
        public Vector3 rotacion;
    }

    [SerializeField] private Transform cam;
    public float speed = 1.0f;
    [SerializeField] private List<Estado> estados = new List<Estado>();
    public OnFinished onFinished;

    private int objetivo;
    private Estado oldE;
    private bool finished = true;
    private float timer = 0.0f;

    void Awake()
    {
        if (cam == null) cam = transform;
        oldE = new Estado();
    }

    public void Transition(int estado)
    {
        if (estado >= estados.Count || estado < 0) return;
        oldE.posicion = cam.localPosition;
        oldE.rotacion = cam.localRotation.eulerAngles;
        objetivo = estado;
        timer = 0.0f;
        finished = false;
    }

    public int GetState()
    {
        return objetivo;
    }

    public int States()
    {
        return estados.Count;
    }

    private void Update()
    {
        if(!finished)
        {
            if ((cam.position - estados[objetivo].posicion).sqrMagnitude == 0) Finish();

            timer += Time.deltaTime * speed;

            cam.position = Vector3.Lerp(oldE.posicion, estados[objetivo].posicion, timer);
            cam.rotation = Quaternion.Lerp(Quaternion.Euler(oldE.rotacion), Quaternion.Euler(estados[objetivo].rotacion), timer);

            if (timer > 1) Finish();
        }
    }
    private void Finish()
    {
        finished = true;
        onFinished.Invoke();
    }
}