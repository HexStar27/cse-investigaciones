using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hexstar.CSE
{

    public class Conector : MonoBehaviour
    {
        //Contiene distancias entre conectores desconectados
        public static Dictionary<Conector,float> distanciaEntradasTocando = new Dictionary<Conector, float>();
        
        private BlockMovAndConexion controlador;
        public enum ConexionType { TOP = 0, LEFT = 1, BOTTOM = 2, RIGHT = 3};
        [SerializeField] private ConexionType type;

        [Header("Debug")]
        [SerializeField] bool conectado = false;
        [SerializeField] Conector cConectado;
        [SerializeField] List<Conector> conectoresTocando = new List<Conector>(); // cConectado no debe estar en esta lista
        int indiceConectorMasCercano = 0;

        public ConexionType Tipo() { return type; }
        public bool EsEntrada() { return type < ConexionType.BOTTOM; } // <=> type == TOP || type == LEFT
        public bool EsHorizontal() { return ((int)type & 1) == 1; } // <=> type == LEFT || TYPE == RIGHT
        public bool EsOpuesto(ConexionType otro) { return Math.Abs(otro-type) == 2; }

        public bool EstaConectado() { return conectado; }
        public BlockMovAndConexion ElBloque() { return controlador; }
        public bool PerteneceA(BlockMovAndConexion b) { return controlador == b; }
        public Conector ConectorConectado() { return cConectado; }
        public Conector TocandoCercano() {
            if (indiceConectorMasCercano >= 0)
                return conectoresTocando[indiceConectorMasCercano];
            else
                return null;
        }
        public bool TieneCercanos() { return indiceConectorMasCercano >= 0; }
        
        public void ForzarConexion(Conector c)
        {
            if (c == null) return;
            if (this.EsEntrada())
            {
                if (EstaConectado()) cConectado.AplicarConexion(false);
                cConectado = c;
                cConectado.ForzarConexion(this);
                if (conectoresTocando.Contains(c)) conectoresTocando.Remove(c);
            }
            else cConectado = c;
            conectado = true;
        }
        public void AplicarConexion(bool v)
        {
            if (EsEntrada())
            {
                if (v)
                {
                    if (!TieneCercanos()) return;
                    //Conexión
                    if (EstaConectado()) cConectado.AplicarConexion(false);
                    cConectado = conectoresTocando[indiceConectorMasCercano];
                    cConectado.ForzarConexion(this);

                    //Control de la lista
                    conectoresTocando.RemoveAt(indiceConectorMasCercano);
                    CalculateClosestDistance();
                }
                else if (cConectado != null)
                {
                    cConectado.AplicarConexion(false);
                    cConectado = null;
                    //controlador.onBlockConectionChanged?.Invoke();
                }
            }
            else if (!v) { 
                cConectado = null;
                //controlador.onBlockConectionChanged?.Invoke();
            }
            conectado = v;
        }
        public Conector AplastarConexionConCercano()
        {
            if (!TieneCercanos() || !EsEntrada()) return null;
            Conector old = null;
            if(EstaConectado()) cConectado.AplicarConexion(false);
            old = cConectado;

            cConectado = conectoresTocando[indiceConectorMasCercano];
            conectoresTocando.Remove(cConectado);
            CalculateClosestDistance();

            cConectado.ForzarConexion(this);
            return old;
        }

        private float CalculateClosestDistance()
        {
            float closestDistance = float.PositiveInfinity;
            int iBest = 0;
            for (int i = 0; i < conectoresTocando.Count; i++)
            {
                float distance = Vector3.Distance(transform.position, conectoresTocando[i].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    iBest = i;
                }
            }
            indiceConectorMasCercano = iBest;
            return closestDistance;
        }

		private void Awake()
        {
            controlador = transform.parent.GetComponent<BlockMovAndConexion>();
        }

        //Sólo los conectores de tipo entrada (Top y Left) se encargan de comprobar colisiones

		private void OnTriggerEnter(Collider other)
        {
            if (!this.EsEntrada()) return;
            if (!other.TryGetComponent(out Conector entryConector)) return;
            if (entryConector.EstaConectado() || !EsOpuesto(entryConector.Tipo())) return;
            if (cConectado == entryConector) return;

            if (!conectoresTocando.Contains(entryConector))
            {
                conectoresTocando.Add(entryConector);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!this.EsEntrada()) return;
            if (!other.TryGetComponent(out Conector exitConector)) return;
            if (!exitConector.EsOpuesto(Tipo())) return;

            distanciaEntradasTocando[this] = CalculateClosestDistance();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!this.EsEntrada()) return;
            if (!other.TryGetComponent(out Conector exitConector)) return;

            int i = conectoresTocando.IndexOf(exitConector);
            if (i >= 0)
            {
                conectoresTocando.Remove(exitConector);
                if(i == indiceConectorMasCercano) distanciaEntradasTocando[this] = CalculateClosestDistance();
            }
            
            if (exitConector.EstaConectado() || !EsOpuesto(exitConector.Tipo())) return;

            if (exitConector == cConectado)
            {
                cConectado = null;
                conectado = false;
            }
            if(conectoresTocando.Count == 0) distanciaEntradasTocando.Remove(this);
        }
    }
}