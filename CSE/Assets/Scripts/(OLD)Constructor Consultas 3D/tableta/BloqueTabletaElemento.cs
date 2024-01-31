using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hexstar.CSE
{
    [RequireComponent(typeof(Image))] //Es necesario?
    public class BloqueTabletaElemento : MonoBehaviour, IPointerClickHandler
    {
        private static List<GameObject> bloquesEnMesa = new();

        public GameObject bloquePrefab;
        [HideInInspector] public Transform zonaSpawnBloques;
        [SerializeField] private Text tmesh;

        public static void DestruirBloquesEnMesa()
        {
            for (int i = 0; i < bloquesEnMesa.Count; i++)
            {
                var b = bloquesEnMesa[i];
                if(b != null) Destroy(b);
            }
            bloquesEnMesa.Clear();
        }

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
            var bloque = Instantiate(bloquePrefab, zonaSpawnBloques.position, Quaternion.identity);
            bloquesEnMesa.Add(bloque);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GenerarBloque();
        }
    }
}