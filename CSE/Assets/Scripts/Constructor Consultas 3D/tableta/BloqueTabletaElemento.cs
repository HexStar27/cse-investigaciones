using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hexstar.CSE
{
    [RequireComponent(typeof(Image))]
    public class BloqueTabletaElemento : MonoBehaviour, IPointerClickHandler
    {
        public GameObject bloquePrefab;
        [SerializeField] private Vector3 offsetInicial = new Vector3(-48, 4, 0);
#pragma warning disable IDE0052 // Quitar miembros privados no leídos
        private Image imagen;
#pragma warning restore IDE0052 // Quitar miembros privados no leídos
        private DatosBloque datos;
        public Transform zonaSpawnBloques;
        [SerializeField] private Text tmesh;

        private void Start()
        {
            imagen = GetComponent<Image>();
            if (zonaSpawnBloques == null) zonaSpawnBloques = transform;
        }

        public void Inicializar(DatosBloque datos)
        {
            if(zonaSpawnBloques != null) transform.position = zonaSpawnBloques.position;
            this.datos = datos;
            tmesh.text = datos.prefijo;
        }

        public void GenerarBloque()
        {
            Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mPos.z = 0;
            BloqueInfo3D b = Instantiate(bloquePrefab, zonaSpawnBloques.position, 
                             Quaternion.identity).GetComponent<BloqueInfo3D>();
            b.Inicializar(datos);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GenerarBloque();
        }
    }
}