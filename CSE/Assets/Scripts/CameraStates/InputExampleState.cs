using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputExampleState : MonoBehaviour
{
    [SerializeField] private CameraState camS;

    private bool ready = true;

    private void Ready()
    {
        ready = true;
    }

    private void OnEnable()
    {
        camS.onFinished.AddListener(Ready);
    }
    private void OnDisable()
    {
        camS.onFinished.RemoveListener(Ready);
    }

    void Update()
    {
        if(ready)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                camS.Transition(0);
                ready = false;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                camS.Transition(1);
                ready = false;
            }

        }
    }
}
