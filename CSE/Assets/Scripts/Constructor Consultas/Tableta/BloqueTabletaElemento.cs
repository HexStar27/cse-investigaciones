using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hexstar.CSE
{
    [RequireComponent(typeof(Image))]
    public class BloqueTabletaElemento : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        RectTransform zonaBloquesLibres;
        public GameObject bloquePrefab;
        [SerializeField] private Vector3 offsetInicial = new Vector3(-48, 4, 0);
#pragma warning disable IDE0052 // Quitar miembros privados no leídos
        private Image imagen;
#pragma warning restore IDE0052 // Quitar miembros privados no leídos
        private DatosBloque datos;
        BloqueConsulta ultimoBloque;

        private void Start()
        {
            imagen = GetComponent<Image>();
        }

        public void Inicializar(DatosBloque datos, RectTransform zonaBloquesLibres)
        {
            this.datos = datos;
            this.zonaBloquesLibres = zonaBloquesLibres;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            GenerarBloque();
            ultimoBloque.movimientoBloque.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            ultimoBloque.movimientoBloque.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ultimoBloque.movimientoBloque.OnEndDrag(eventData);
        }

        public void GenerarBloque()
        {
            Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mPos.z = 0;
            BloqueConsulta b = Instantiate(bloquePrefab, mPos + offsetInicial, Quaternion.identity, 
                zonaBloquesLibres.transform).GetComponent<BloqueConsulta>();
            b.Inicializar(datos);
            ultimoBloque = b;
        }
    }
}