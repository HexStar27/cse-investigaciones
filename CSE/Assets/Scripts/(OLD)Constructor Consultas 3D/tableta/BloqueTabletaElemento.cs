using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hexstar.CSE
{
    [RequireComponent(typeof(Image))] //Es necesario?
    public class BloqueTabletaElemento : MonoBehaviour, IPointerClickHandler
    {
        public GameObject bloquePrefab;
        [HideInInspector] public Transform zonaSpawnBloques;
        [SerializeField] private Text tmesh;

        private void Start()
        {
            if (zonaSpawnBloques == null) zonaSpawnBloques = transform;
        }

        public void Inicializar(AlmacenDeBloquesSimple.Bloque bloque)
        {
            if(zonaSpawnBloques != null) transform.position = zonaSpawnBloques.position;
            bloquePrefab = bloque.prefab;
            tmesh.text = bloque.titulo;
        }

        public void GenerarBloque()
        {
            Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mPos.z = 0;
            Instantiate(bloquePrefab, zonaSpawnBloques.position, Quaternion.identity);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GenerarBloque();
        }
    }
}