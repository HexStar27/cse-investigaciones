using System.Collections;
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
    private bool playAnimation = true;
    public static bool globalStop = false;

    private void OnMouseEnter()
    {
        if (MenuPausa.Paused || globalStop) return;
        onEnter.Invoke();
        if (CanPlayAnim(enterAnim)) animator.Play(enterAnim);
    }

    private void OnMouseExit()
    {
        if (MenuPausa.Paused || globalStop) return;
        onExit.Invoke();
        if (CanPlayAnim(exitAnim)) animator.Play(exitAnim);
    }

    private void OnMouseUp()
    {
        if (MenuPausa.Paused || globalStop) return;
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
        if (clickable)
        {
            if(clickDelay > 0) StartCoroutine(Delay());
            onClick.Invoke();
            if (CanPlayAnim(clickAnim)) animator.Play(clickAnim);
        }
    }

    public void SetAnimationBlock(bool block) => playAnimation = !block;

    private bool CanPlayAnim(string stateName)
    {
        return animator != null && playAnimation && stateName != "" &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
}
