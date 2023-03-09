using UnityEngine;
using UnityEngine.Events;

public class GenericCompleteButon3D : MonoBehaviour
{
    [System.Serializable] public class OnEvent : UnityEvent { };
    public OnEvent onMouseDown      = new OnEvent();
    public OnEvent onMouseDrag      = new OnEvent();
    public OnEvent onMouseEnter     = new OnEvent();
    public OnEvent onMouseExit      = new OnEvent();
    public OnEvent onMouseOver      = new OnEvent();
    public OnEvent onMouseUp        = new OnEvent();
    public OnEvent onMouseUpAsButton = new OnEvent();

    private void OnMouseDown()
    {
        onMouseDown.Invoke();
    }
    private void OnMouseDrag()
    {
        onMouseDrag.Invoke();
    }
    private void OnMouseEnter()
    {
        onMouseEnter.Invoke();
    }
    private void OnMouseExit()
    {
        onMouseExit.Invoke();
    }
    private void OnMouseOver()
    {
        onMouseOver.Invoke();
    }
    private void OnMouseUp()
    {
        onMouseUp.Invoke();
    }
    private void OnMouseUpAsButton()
    {
        onMouseUpAsButton.Invoke();
    }
}
