using UnityEngine;
using UnityEngine.EventSystems;

public class AppearZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Show(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Show(false);
    }

    private void Show(bool val)
    {
        for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(val);
    }
}
