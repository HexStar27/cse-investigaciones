using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hexstar
{

    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        public TabGroup tabGroup;
        public Image background;
        public UnityEvent onTabSelected;
        public UnityEvent onTabDeselected;
        private bool desbloqueado = true;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (desbloqueado) tabGroup.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (desbloqueado) tabGroup.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (desbloqueado) tabGroup.OnTabExit(this);
        }

        private void Start()
        {
            tabGroup.Suscribe(this);
        }

        public void Selected()
        {
            onTabSelected.Invoke();
        }

        public void Deselect()
        {
            onTabDeselected.Invoke();
        }

        public void Bloquear(bool value)
        {
            desbloqueado = !value;
        }

        public bool Desbloqueado()
        {
            return desbloqueado;
        }
    }
}