using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bloque : Arrastrable2
{
    [Header("Configuración del bloque")]
    public LayerMask targetLayer;
    public ScriptableBlock scriptableBlock;

    private SpriteRenderer sr;
    private BoxCollider2D bc;

    public static string elemento = "Elementos";
    public static string elementoColocado = "Elem_Colocados";
    protected static string bloqueLayer = "bloques";

    /// Funciones a implementar:
    ///
    /// Desconectar al agarrar el bloque
    /// Conectar al soltar el bloque
    /// Dibujar rectángulo semistransparente cuando encuentre un bloque al que conectarse (Update)
    /// 
    /// Actualizar las posiciones de los bloques adyacentes cada vez que se conecta y desconecta
    /// 
    /// Para detectar los otros bloques usará raycast
    /// 

    protected override void OnEnable()
    {
        if (accionAlInteractuar_.Count < 2) accionAlInteractuar_ = new List<UnityEvent>(2);

        base.OnEnable();

        if (sr == null) if (!TryGetComponent(out sr)) sr = gameObject.AddComponent<SpriteRenderer>();
        if (bc == null) if (!TryGetComponent(out bc)) bc = gameObject.AddComponent<BoxCollider2D>();

        if (this is IBloque)
        {
            //Pulsar = 0, soltar = 1
            accionAlInteractuar_[0].AddListener(Agarrar);
            accionAlInteractuar_[1].AddListener(Soltar);
        }
    }

    protected void OnDisable()
    {
        if (this is IBloque)
        {
            //Pulsar = 0, soltar = 1
            accionAlInteractuar_[0].RemoveListener(Agarrar);
            accionAlInteractuar_[1].RemoveListener(Soltar);
        }
    }

    /// <summary>
    /// Separa este bloque del anterior (pero se lleva consigo los que estén conectados por debajo suya)
    /// </summary>
    private void Agarrar()
    {
        //Actualización del sprite
        Vector3 pos = MousePosDetector.MousePos();
        transform.position = new Vector3(pos.x, pos.y, 0);
        sr.sprite = scriptableBlock.grabSprite;
        sr.sortingLayerName = elemento;
        bc.size = new Vector2(1, 1);

        IBloque este = (IBloque)this;
        IBloque otro = este.Anterior();

        este.ConectarAnterior(null);

        if (otro != null)
        {
            if (otro.Siguiente() == este) otro.ConectarSiguiente(null);
            else //Si no es el siguiente, entonces debe de ser uno de los bloques anidados
                for (uint i = 0; i < otro.NAnidados(); i++)
                    if (otro.Anidado(i) == este) otro.Anidar(null, i);
        }

        //Actualiza el cambio de capa de este y todos sus conectados
        TransmitirCambioDeCapa(este, false);

        if (BloqueController.Raiz() == este) BloqueController.AsignarBloque(null);
        if(BloqueController.UnidoA(este)) BloqueController.ActualizarPL();
    }

    private void Soltar()
    {
        sr.sprite = scriptableBlock.dropSprite; //Actualización del sprite
        bc.size = new Vector2(10, 1);

        gameObject.layer = LayerMask.NameToLayer("Default");//Se "autoconsidera" Default para no detectarse a si mismo
        
        Vector3 pos = MousePosDetector.MousePos();
        RaycastHit2D hit2D = Physics2D.Raycast(pos, Vector2.zero, 15, targetLayer);
        if (hit2D)
            ColocarBloque(hit2D, (IBloque)this, pos);
        else //No ha encontrado ningún bloque en el que soltarse
        {
            pos += new Vector3(0, BloqueController.alturaBloque * 0.25f, 0);
            hit2D = Physics2D.Raycast(pos, Vector2.zero, 15, targetLayer);

            if (hit2D) //Comprueba si esta vez SI ha encontrado un Bloque
                ColocarBloque(hit2D, (IBloque)this, pos);
            else
            {
                sr.sprite = scriptableBlock.grabSprite;
                bc.size = new Vector2(1, 1);
            }
        }
        
        gameObject.layer = LayerMask.NameToLayer(bloqueLayer);//Y se restablece la capa
        BloqueController.ActualizarPL();
    }

    private void ColocarBloque(RaycastHit2D hit2D, IBloque este, Vector3 pos)
    {
        if (hit2D.collider.TryGetComponent(out IBloque otro))
        {
            uint indiceAnidado = otro.ZonaEntradaBloque(pos.x, pos.y);

            IBloque ultimo = este;
            IBloque aux = este.Siguiente();
            while (aux != null) //Busca el último bloque que está conectado con este
            {
                ultimo = aux = aux.Siguiente();
            }

            if (indiceAnidado < otro.NAnidados()) //Como anidado 
            {
                este.ConectarAnterior(otro); //El agarrado pone como anterior el objetivo "otro"
                if (otro.Anidado(indiceAnidado) != null)
                {
                    otro.Anidado(indiceAnidado).ConectarAnterior(ultimo);
                    ultimo.ConectarSiguiente(otro.Anidado(indiceAnidado));
                }
                otro.Anidar(este, indiceAnidado);

                TransmitirCambioDeCapa(este, true);
            }
            else if (indiceAnidado == otro.NAnidados())//Como siguiente
            {
                este.ConectarAnterior(otro); //El agarrado pone como anterior el objetivo "otro"
                if (otro.Siguiente() != null) //Se acopla con el resto de los sucesores del objetivo
                {
                    otro.Siguiente().ConectarAnterior(ultimo);
                    ultimo.ConectarSiguiente(otro.Siguiente());
                }
                otro.ConectarSiguiente(este); //El objetivo toma como nuevo sucesor a este bloque

                TransmitirCambioDeCapa(este, true);
            }
            else //No se ha colocado en ningúna zona de ese bloque
            {
                sr.sprite = scriptableBlock.grabSprite;
                bc.size = new Vector2(1, 1);

            }
        }
        else if (hit2D.collider.TryGetComponent(out BloqueController o))
        {
            if (BloqueController.Raiz() == null)
            {
                BloqueController.AsignarBloque(este);
                transform.parent = o.transform;
                transform.localPosition = Vector3.zero;

                TransmitirCambioDeCapa(este, true);
            }
        }
    }


    /// <summary>
    /// Le envía un mensaje a cada bloque subyacente para que actualice su sortingLayer y se vea correctamente
    /// en la consola.
    /// </summary>
    public virtual void TransmitirCambioDeCapa(IBloque este, bool colocado)
    {
        if (colocado) sr.sortingLayerName = elementoColocado;
        else sr.sortingLayerName = elemento;

        Bloque b;
        if(este is Bloque)
        {
            if(este.Siguiente()!=null)
            {
                b = (Bloque)este.Siguiente();
                b.TransmitirCambioDeCapa(este.Siguiente(), colocado);
            }

            for(uint i = 0; i < este.NAnidados(); i++)
            {
                if(este.Anidado(i) != null)
                {
                    b = (Bloque)este.Anidado(i);
                    b.TransmitirCambioDeCapa(este.Anidado(i), colocado);
                }
            }
        }
    }

    public void CambiarSpriteLayer(string capa)
    {
        sr.sortingLayerName = capa;
    }
}
