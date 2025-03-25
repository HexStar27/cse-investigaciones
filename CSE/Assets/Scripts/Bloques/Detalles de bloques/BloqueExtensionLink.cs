using UnityEngine;

namespace Hexstar.CSE {
    //Se encarga de colocar o quitar el bloque extension "paréntesis" o derivados
    public class BloqueExtensionLink : MonoBehaviour
    {
        [SerializeField] BlockMovAndConexion controladorLink;
        [SerializeField] BlockMovAndConexion controladorExtension;

        public void ReturnToLink()
        {
            if(!controladorLink.FormaParteDelChunk(controladorExtension))
            {
                ConectarseAlFinal();
            }
        }

        private void ConectarseAlFinal()
        {
            var b = controladorLink;
            var ultimo = b;
            int limit = 75;
            while (b != null && limit-- > 0)
            {
                ultimo = b;
                b = b.GetBloqueDerecho();
            }
            controladorExtension.DisconectEntryConectors();
            var cEntrada = controladorExtension.GetConector(Conector.ConexionType.LEFT);
            var cSalida = ultimo.GetConector(Conector.ConexionType.RIGHT);
            cEntrada.ForzarConexion(cSalida);
            BlockMovAndConexion.ColocarConexion(cEntrada, cSalida);
        }

        private void LateUpdate()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
                ReturnToLink();
        }
        /*
        private void OnEnable()
        {
            controladorLink.onBlockConectionChanged.AddListener(ReturnToLink);
            //controladorExtension.onBlockConectionChanged.AddListener(ReturnToLink);
        }
        private void OnDisable()
        {
            //controladorExtension.onBlockConectionChanged.RemoveListener(ReturnToLink);
            controladorLink.onBlockConectionChanged.RemoveListener(ReturnToLink);
        }
        */
    }
}