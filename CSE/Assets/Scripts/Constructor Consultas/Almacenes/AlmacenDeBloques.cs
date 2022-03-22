using UnityEngine;

namespace Hexstar.CSE
{
    public class AlmacenDeBloques : MonoBehaviour
    {
        public DatosBloque[] datosBloques = new DatosBloque[0];

        public static AlmacenDeBloques instancia;

        private void Awake()
        {
            instancia = this;
        }

    }
}