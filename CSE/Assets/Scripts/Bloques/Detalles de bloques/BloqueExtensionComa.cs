using System.Collections.Generic;
using UnityEngine;

namespace Hexstar.CSE {
    //Se encarga de colocar o quitar el bloque extension "coma"
    public class BloqueExtensionComa : MonoBehaviour
    {
        [SerializeField] BlockMovAndConexion controlador;
        [SerializeField] Collider coll;
        [SerializeField] SpriteRenderer sr;
        [SerializeField] BlockMovAndConexion controladorExtension;
        [SerializeField] List<BlockType> seccionesOK = new List<BlockType>();
        bool toogle = false;
        //Toogle para poner y quitar el bloque

        ///Comprueba si el bloque extension puede ponerse o no
        private void CheckValidity()
        {
            var seccion = controlador.GetSeccionPerteneciente();
            bool puede = (seccion == null || seccionesOK.Contains(seccion)) && 
                !ControladorSalidaConectadaAOtro();

            if (puede == false && toogle == true)
            {
                toogle = false;
                TOOGLE();
            }
            coll.enabled = puede;
            sr.enabled = puede;
        }

        private bool ControladorSalidaConectadaAOtro()
        {
            if(controlador.TieneSalidaConectada())
            {
                return controlador.GetBloqueDerecho() != controladorExtension;
            }
            return false;
        }

        private void TOOGLE()
        {
            //Poner o quitar bloque extensión
            var entrada = controladorExtension.GetConector(Conector.ConexionType.LEFT);
            if (toogle)
            {
                controladorExtension.gameObject.SetActive(true);
                var salida = controlador.GetConector(Conector.ConexionType.RIGHT);

                if (salida.EstaConectado())
                {
                    //Conectar con siguiente si bloque ya estaba conectado por ahí
                    var siguiente = salida.ConectorConectado();
                    siguiente.AplicarConexion(false);
                    siguiente.ForzarConexion(controladorExtension.GetConector(Conector.ConexionType.RIGHT));
                }
                entrada.ForzarConexion(salida);
            }
            else
            {
                var salida = controladorExtension.GetConector(Conector.ConexionType.RIGHT);
                if (salida.EstaConectado())
                {
                    salida.ConectorConectado().ElBloque().transform.parent = null;
                    salida.ConectorConectado().AplicarConexion(false);
                }
                entrada.AplicarConexion(false);
                controladorExtension.gameObject.SetActive(false);
            }
        }

        private void OnMouseUpAsButton()
        {
            toogle = !toogle;
            TOOGLE();
        }

        private void OnEnable()
        {
            controlador.onBlockConectionChanged.AddListener(CheckValidity);
        }
        private void OnDisable()
        {
            controlador.onBlockConectionChanged.RemoveListener(CheckValidity);
        }
    }
}