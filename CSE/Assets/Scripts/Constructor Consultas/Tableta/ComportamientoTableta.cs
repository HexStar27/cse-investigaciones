using UnityEngine;
using System.Collections.Generic;

namespace Hexstar.CSE
{
    [RequireComponent(typeof(Animator))]
    public class ComportamientoTableta : MonoBehaviour
    {
        public RectTransform zonaBloquesLibres;
        public GameObject BloqueTabletaPrefab;
        public ContentScaler contenedor;
        private List<GameObject> elementos = new List<GameObject>();

        Animator anim;
        bool abriendo = false;
        void Start()
        {
            anim = GetComponent<Animator>();
            Rellenar();
        }

        public void Abrir()
        {
            anim.SetTrigger("Abrir");
        }

        public void Cerrar()
        {
            anim.SetTrigger("Cerrar");
        }

        public void Cambiar()
        {
            abriendo = !abriendo;
            if (abriendo) Abrir();
            else Cerrar();

        }

        public void Rellenar()
        {
            foreach (var elem in elementos)
            {
                Destroy(elem);
            }
            elementos.Clear();
            
            if (contenedor != null)
            {
                for(int i = 0; i < AlmacenDeBloques.instancia.datosBloques.Length; i++)
                {
                    BloqueTabletaElemento b = Instantiate(BloqueTabletaPrefab, contenedor.transform).GetComponent<BloqueTabletaElemento>();
                    b.Inicializar(AlmacenDeBloques.instancia.datosBloques[i], zonaBloquesLibres);
                    elementos.Add(b.gameObject);
                }
                contenedor.Actualizar();
            }
        }
    }
}