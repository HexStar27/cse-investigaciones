using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hexstar.CSE
{
    public class ControladorBloques : MonoBehaviour
    {
        public static ControladorBloques instancia;
        public ContentScaler contentScaler;
        private List<BloqueConsulta> bloques = new List<BloqueConsulta>();
        public RectTransform circulacionBloques;
        public ScrollRect scroll;
        private RectTransform r;
        [SerializeField] float offset = 0;
        [SerializeField] float velocidadArrastre = 1;
        private bool desplazando = false;
        private float bloqueAlturaAux = 0;

        private void Awake()
        {
            instancia = this;
            r = GetComponent<RectTransform>();
        }

        private void FixedUpdate()
        {
            if(desplazando)
            {
                float diferencia = 0;
                //Sobresale por arriba
                if (bloqueAlturaAux > 0) diferencia = bloqueAlturaAux * velocidadArrastre;
                else if (-bloqueAlturaAux > r.rect.height) diferencia = (bloqueAlturaAux + r.rect.height) * velocidadArrastre;

                scroll.velocity = new Vector2(scroll.velocity.x, -diferencia);
            }
        }

        public void InsertarBloque(BloqueConsulta bloque, BloqueConsulta hijo = null)
        {
            float altura = bloque.rTransform.anchoredPosition.y + offset - bloque.rTransform.rect.height;
            int indice = contentScaler.IndiceSegunPos(altura);
            if (indice < 0) indice = 0;
            bloque.transform.SetParent(contentScaler.transform);
            bloque.transform.SetSiblingIndex(indice);

            if (indice >= bloques.Count) bloques.Add(bloque);
            else bloques.Insert(indice, bloque);

            if(hijo != null)
            {
                indice++;
                hijo.transform.SetParent(contentScaler.transform);
                hijo.transform.SetSiblingIndex(indice);
                contentScaler.Actualizar();

                if (indice >= bloques.Count) bloques.Add(hijo);
                else bloques.Insert(indice, hijo);

                bloque.flechaActual.Activar(true);
            }

            contentScaler.Actualizar();
        }

        public void QuitarBloque(BloqueConsulta bloque)
        {
            bloque.transform.SetParent(circulacionBloques);
            bloques.Remove(bloque);

            if (bloque.UsandoFlecha()) bloque.flechaActual.Activar(false);           
        }

        public bool Dentro(RectTransform otro)
        {
            //Dentro de eje X
            if (otro.localPosition.x + otro.rect.width > 0 && otro.localPosition.x - circulacionBloques.rect.width < 0)
            {
                float posY = otro.localPosition.y;
                if (otro.parent == contentScaler.r) posY += contentScaler.r.anchoredPosition.y;
                //Dentro de eje y
                if (posY > -circulacionBloques.rect.height &&
                posY - otro.rect.height < 0) return true;
            }
            return false;
        }

        public void Desplazar(bool value, float bloqueAltura)
        {
            desplazando = value;
            bloqueAlturaAux = bloqueAltura;
        }

        public string ConsultaFinal()
        {
            string c = "";
            for(int i = 0; i < bloques.Count; i++)
            {
                if (i != 0) c += " ";
                c += bloques[i].ConsultaParcial();
            }
            return c;
        }

        public void ConsultaDebug()
        {
            Debug.Log(ConsultaFinal());
        }
    }
}