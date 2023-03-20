using TMPro;
using UnityEngine;

namespace Hexstar.CSE {
    //Esta clase se va a encargar de hacer que los bloques de tipo Seccion (SELECT, FROM, WHERE, etc) sean bonitos
    public class SectionBlockDecorator : MonoBehaviour
    {
        private BlockMovAndConexion bloque;
        [SerializeField] private TextMeshPro label;
        private void Awake()
        {
            bloque = GetComponent<BlockMovAndConexion>();
        }
        private void OnEnable()
        {
            RefreshDecoration();
        }

        public void RefreshDecoration()
        {
            //Mostrar el nombre del tipo en la parte superior del bloque.
            label.text = bloque.GetBlockData().name;
        }
    }
}