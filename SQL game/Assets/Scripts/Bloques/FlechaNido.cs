using UnityEngine;

 [RequireComponent(typeof(LineRenderer))]
public class FlechaNido : MonoBehaviour
{
    LineRenderer lr;
    Transform objetivo;
    bool seguir = false;
    Vector3[] posiciones = new Vector3[4];

    private static float factorDistancia = 1;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.numCornerVertices = 4;
        lr.numCapVertices = 1;
        lr.positionCount = 4;

        lr.renderingLayerMask = (uint)LayerMask.NameToLayer(Bloque.elemento);
        lr.useWorldSpace = false;
        lr.widthMultiplier = 0.23f;

        lr.SetPositions(posiciones);

        //Para evitar errores en tiempo de ejecución
        objetivo = transform;
    }

    private void FixedUpdate()
    {
        if(seguir)
        {
            transform.position = objetivo.position;
        }
    }

    ///Coloca la base de la flecha (la posición debería ser una posición cercana al bloque)
    public void PosicionarBase(Vector3 extremo)
    {
        //Colocar Vector 0
        posiciones[0] = extremo;
        //Calcular altura según diferencia de alturas entre v0 y v3
        float dif = extremo.y - posiciones[3].y;
        if (dif < 0) dif = -dif;
        //Colocar Vector 1 y 2
        posiciones[1] = extremo - new Vector3(dif/factorDistancia, 0, 0);
        posiciones[2] = posiciones[3] - new Vector3(dif/factorDistancia, 0, 0);
    }

    ///Coloca la punta de la flecha (la posición debería ser una posición cercana al bloque)
    public void PosicionarPunta(Vector3 extremo)
    {
        //Colocar Vector 3
        posiciones[3] = extremo;
        //Calcular altura según diferencia de alturas entre v0 y v3
        float dif = posiciones[0].y - extremo.y;
        if (dif < 0) dif = -dif;
        //Colocar Vector 1 y 2
        posiciones[2] = extremo - new Vector3(dif / factorDistancia, 0, 0);
        posiciones[1] = posiciones[0] - new Vector3(dif / factorDistancia, 0, 0);
    }

    ///Desactiva el objeto porque se va a dejar de usar, de forma que vuelva a estar disponible desde el pool 
    ///volviendo a su estado inicial.
    public void Reset()
    {
        gameObject.SetActive(false);
    }

    ///Actualiza la posición de la flecha (ambos extremos) según el transform del bloque en concreto cada frame de físicas.
    public void SeguirBloque(Transform bloque)
    {
        objetivo = bloque;
        seguir = true;
    }

    ///Deja de seguir cada frame de físicas al bloque que seguía anteriormente (opcional pero libera a la 
    ///CPU de trabajo inútil si no se va a mover la flecha)
    public void DejarDeSeguir()
    {
        seguir = false;
    }

    public static void SetFactorDistancia(float distanciaTope)
    {
        factorDistancia = distanciaTope;
        //Para evitar errores en tiempo real
        if (factorDistancia == 0) factorDistancia = 1;
    }
}
