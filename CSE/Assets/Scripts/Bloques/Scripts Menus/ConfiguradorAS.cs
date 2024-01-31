using System.Collections.Generic;
using UnityEngine;

namespace Hexstar.CSE {
    [RequireComponent(typeof(BlockMovAndConexion))]
    public class ConfiguradorAS : MonoBehaviour
    {
        [SerializeField] BlockMovAndConexion controlador;
        [SerializeField] ConfiguradorBloqueValor cbv;
        [SerializeField] BlockType tipoColumna;
        [SerializeField] BlockType tipoTabla;
        private string valueInserted = null;
        private string elementRenamed = null;
        bool wasTable = false;

        private void OnEnable()
        {
            controlador.onBlockConectionChanged.AddListener(CheckFunctionForAS);
            cbv.onTextChanged.AddListener(CFfAS);
        }
        private void OnDisable()
        {
            controlador.onBlockConectionChanged.RemoveListener(CheckFunctionForAS);
            cbv.onTextChanged.RemoveListener(CFfAS);
        }

        private void CFfAS(string val)
        {
            CheckFunctionForAS();
        }

        private void CheckFunctionForAS()
        {
            QuitarAntiguos();
            
            //Obtener nuevo nombre del elemento
            string val = cbv.GetText();
            if (val == null) return;
            if (val.Contains("\"")) val = val.Trim('\"');
            
            //Comprobar que el conector Izq esté conectado.
            if (!controlador.AlMenosUnaEntradaConectada()) return;
            var entrada = controlador.GetConector(Conector.ConexionType.LEFT).ConectorConectado().ElBloque();
            elementRenamed = entrada.CQU.GetBlockTextContent();

            //Comprobamos si el AS modifica
            var tEntr = entrada.GetBlockData().tipoPropio;
            if(tEntr.Equals(tipoColumna)) wasTable = false;
            else if(tEntr.Equals(tipoTabla)) wasTable = true;
            
            InsertarValor(val);
        }

        private void QuitarAntiguos()
        {
            if (valueInserted == null || elementRenamed == null) return;
            if (wasTable) AlmacenDePalabras.aliasParaTablas[elementRenamed].Remove(valueInserted);
            else AlmacenDePalabras.aliasParaColumnas[elementRenamed].Remove(valueInserted);
            valueInserted = null;
            elementRenamed = null;
        }

        private void InsertarValor(string val)
        {
            if (val == null)
            {
                valueInserted = null;
                return;
            }
            if (wasTable) AlmacenDePalabras.AddAliasToTable(elementRenamed, val);
            else AlmacenDePalabras.AddAliasToColumn(elementRenamed, val);
            valueInserted = val;
        }

        private void OnDestroy()
        {
            QuitarAntiguos();
        }
    }
}