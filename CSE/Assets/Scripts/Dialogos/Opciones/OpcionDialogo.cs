using System;
using UnityEngine;
using UnityEngine.UI;


namespace Hexstar.Dialogue {
    public class OpcionDialogo : MonoBehaviour
    {
        [SerializeField] Button boton;
        int idx = 0;
        int id = 0;
        [SerializeField] TMPro.TextMeshProUGUI textMesh;

        public void Inicializar(int id, int entrada, string texto)
        {
            this.id = id;
            idx = entrada;
            textMesh.text = texto;
        }

        public void Ejecutar() => ControladorOpcionesDialogo.OpcionElegida(idx, id);

        private void OnEnable()
        {
            boton.onClick.AddListener(Ejecutar);
        }
        private void OnDisable()
        {
            boton.onClick.RemoveListener(Ejecutar);
        }
    }
}