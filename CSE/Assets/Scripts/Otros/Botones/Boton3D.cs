using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boton3D : MonoBehaviour
{
    [System.Serializable]
    public class Evento : UnityEvent {}
    public float clickDelay = 0f;
    public Evento onClick;
    public Evento onEnter;
    public Evento onExit;
    [Header("Opcional")]
    public Animator animator;
    public string clickAnim;
    public string enterAnim;
    public string exitAnim;

    private bool clickable = true;

    private void OnMouseEnter()
    {
        if (MenuPausa.Paused) return;
        onEnter.Invoke();
        if (animator != null && enterAnim != "") animator.Play(enterAnim);
    }

    private void OnMouseExit()
    {
        if (MenuPausa.Paused) return;
        onExit.Invoke();
        if (animator != null && exitAnim != "") animator.Play(exitAnim);
    }

    private void OnMouseUp()
    {
        if (MenuPausa.Paused) return;
        SendClick();
    }

    private IEnumerator Delay()
    {
        clickable = false;
        yield return new WaitForSeconds(clickDelay);
        clickable = true;
    }

    public void SendClick()
    {
        if (clickable && !MenuPausa.Paused)
        {
            if(clickDelay > 0) StartCoroutine(Delay());
            onClick.Invoke();
            if (animator != null && clickAnim != "") animator.Play(clickAnim);
        }
    }
}
