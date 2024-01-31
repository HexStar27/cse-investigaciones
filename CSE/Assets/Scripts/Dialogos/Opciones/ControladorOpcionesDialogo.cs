using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hexstar.Dialogue {
    public class ControladorOpcionesDialogo : MonoBehaviour
    {
        private static ControladorOpcionesDialogo Instance;
        static List<GameObject> opcionesCreadas = new();
        [SerializeField] RectTransform contenedor;
        [SerializeField] GameObject opcionPrefab;

        [Header("Debug")]
        public bool showDebug = false;

        public static UnityEvent<int,int> onOptionSelected = new();

        public void CrearOpciones(List<string> opciones, List<int> indicesEntradas)
        {
            LimpiarContenedor();
            ActivarContenedor(true);
            int n = Mathf.Min(opciones.Count,indicesEntradas.Count);
            if (showDebug) print("Creando "+n+" opciones...");
            for (int i = 0; i < n; i++)
            {
                GameObject go = Instantiate(opcionPrefab, contenedor);
                go.GetComponent<OpcionDialogo>().Inicializar(i, indicesEntradas[i], opciones[i]);
                opcionesCreadas.Add(go);
            }
        }

        internal static void OpcionElegida(int entrada, int opcionId)
        {
            if (Instance.showDebug) print("Opción elegida= "+opcionId+", equivale al nodo "+ entrada+ " de la cinématica");
            onOptionSelected?.Invoke(entrada, opcionId);
            Instance.ActivarContenedor(false);
        }

        private void LimpiarContenedor()
        {
            foreach (var go in opcionesCreadas) Destroy(go);
            opcionesCreadas.Clear();
        }

        private void ActivarContenedor(bool enable) => contenedor.gameObject.SetActive(enable);
        

        private void Awake()
        {
            Instance = this;
            onOptionSelected.RemoveAllListeners();
        }
    }
}