using UnityEngine;
using UnityEngine.EventSystems;

namespace Hexstar.CSE
{
    public class BotonBloqueMov : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public BloqueConsulta bloque;
        Vector2 mousePosWorld;
        Vector2 mouseToBlockOffset;
        Camera mainC;
        bool vaADesplazar;

        private void Awake()
        {
            mainC = Camera.main;
        }

        public void OnBeginDrag(PointerEventData eventData) //Agarrar
        {
            if(bloque.ID() != -1)
            {
                if (ControladorBloques.instancia.Dentro(bloque.rTransform)) //Eliminar bloque de consulta
                {
                    ControladorBloques.instancia.QuitarBloque(bloque);

                    if (bloque.hijoActual != null && bloque.UsandoFlecha())
                    {
                        ControladorBloques.instancia.QuitarBloque(bloque.hijoActual);
                        bloque.hijoActual.gameObject.SetActive(false);
                    }
                    vaADesplazar = true;
                }
                else vaADesplazar = false;

                mousePosWorld = mainC.ScreenToWorldPoint(Input.mousePosition);
                mouseToBlockOffset = mousePosWorld - (Vector2)bloque.rTransform.position;

                PapeleraBloques.instancia.Activar(true);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (bloque.ID() != -1)
            {
                mousePosWorld = mainC.ScreenToWorldPoint(Input.mousePosition);

                bloque.rTransform.position = new Vector3( mousePosWorld.x - mouseToBlockOffset.x,
                    mousePosWorld.y - mouseToBlockOffset.y, bloque.rTransform.position.z);
                if (vaADesplazar) ControladorBloques.instancia.Desplazar(true, bloque.rTransform.anchoredPosition.y);
            }
        }

        public void OnEndDrag(PointerEventData eventData) //Soltar
        {
            if (bloque.ID() != -1)
            {
                //Comprobar si está tocando la papelera
                Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (PapeleraBloques.instancia.TocandoPapelera(mPos))
                {
                    Destroy(bloque.gameObject);
                }
                else if (ControladorBloques.instancia.Dentro(bloque.rTransform)) //Añadir bloque a consulta
                {
                    if (bloque.hijoActual != null && bloque.UsandoFlecha())
                        bloque.hijoActual.gameObject.SetActive(true);
                    
                    ControladorBloques.instancia.InsertarBloque(bloque, bloque.hijoActual);
                }
                ControladorBloques.instancia.Desplazar(false, bloque.rTransform.anchoredPosition.y);

                PapeleraBloques.instancia.Activar(false);
            }
        }
    }
}