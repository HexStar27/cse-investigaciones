using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexstar
{
    public class BotonDialogo : MonoBehaviour
    {
        [SerializeField] DialogosCSE dialogo = null;
        [SerializeField] ElementoSeparador elemento = null;

        private void Start()
        {
            if (!dialogo) Debug.LogError("Este botón necesita tener una referencia al diálogo con el que se quiere usar.");
            if (!elemento) Debug.LogError("Este botón necesita tener una referencia a un elemento separador de la cinemática en la que se usará.");
        }

        public void Continuar()
        {
            if (dialogo.HaTerminado()) elemento.Escuchar();
            else dialogo.SaltarColocacionTexto();

        }
    }
}