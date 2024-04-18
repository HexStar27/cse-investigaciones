using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TABber : MonoBehaviour
{
    EventSystem system;
    void Start()
    {
        system = EventSystem.current;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (system.currentSelectedGameObject == null) return;
            if(system.currentSelectedGameObject.TryGetComponent<Selectable>(out var curr))
            {
                if(curr.TryGetComponent<TMP_InputField>(out var currif))
                {
                    if (currif.contentType == TMP_InputField.ContentType.Standard &&
                        currif.lineType != TMP_InputField.LineType.SingleLine) return;
                }

                var next = curr.FindSelectableOnRight();
                if (next == null)
                {
                    next = curr.FindSelectableOnDown();
                    if (next == null) return;
                }

                //print("NEXT FOUND: "+next.gameObject.name);
                if(next.TryGetComponent<TMP_InputField>(out var iField))
                {
                    iField.OnPointerClick(new PointerEventData(system));
                }

                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
        }
    }
}
