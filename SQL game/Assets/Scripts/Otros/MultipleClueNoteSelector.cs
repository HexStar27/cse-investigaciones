using UnityEngine;

public class MultipleClueNoteSelector : MonoBehaviour
{
    public Camera cPrincipal;
    public LayerMask notaDePista;

    [Header("Opciones")]
    public bool showSelectionRectangle = true;
    [SerializeField]
    private LineRenderer lineR;
    private Vector3[] posiciones = new Vector3[5];

    [Header("Límites de la selección")]
    public Transform objetivo;
    private Vector3 posicion;
    public float width;
    public float height;

    private Vector2 startPos;
    private Vector2 center;
    private Vector2 size;
    private Vector2 endPos;
    private Collider2D[] notas = new Collider2D[0];
    private RaycastHit2D clickedZone;

    //Indica si se está intentando seleccionar o no
    [SerializeField]
    private bool selectionMode = false;
    //Indica si ha empezado a arrastrar
    [SerializeField]
    private bool objectsFound = false; //GlobalDrag se pone a true un click después de selection mode

    private bool firstFrame = true;
    private const int leftClick = 0; //La id del botón izquierdo del ratón

    void OnEnable()
    {
        if (!cPrincipal) cPrincipal = Camera.main;
        if (!lineR) TryGetComponent(out lineR);

        GeneralPointDrawing(cPrincipal.transform.position);
        firstFrame = true;

        posicion = objetivo.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(leftClick))
        {
            startPos = MousePosDetector.MousePos();

            clickedZone = Physics2D.Raycast(startPos, Vector3.forward, 15, notaDePista);
            if(!firstFrame && DentroDeLimites()) selectionMode = !clickedZone.collider;

            //Agarra todas las notas si se arrastra al menos una de las notas que han sido seleccionadas.
            if (objectsFound && !selectionMode)
            {
                foreach (Collider2D col in notas)
                {
                    if(col == clickedZone.collider)
                    {
                        Arrastrable arr;
                        foreach (Collider2D nota in notas)
                        {

                            if (nota.TryGetComponent(out arr))
                            {
                                arr.ForceDrag();
                            }
                        }
                    }
                }
            }

            if (lineR && selectionMode) lineR.enabled = true;
            else firstFrame = false;
        }
        else if (Input.GetMouseButtonUp(leftClick))
        {
            if (objectsFound) //Modo de arrastrado global activado, se sueltan todas las notas
            {
                Arrastrable arr;
                foreach (Collider2D col in notas)
                {
                    if (col.TryGetComponent(out arr))
                    {
                        arr.ForceDrop();
                    }
                }
                
            }
            else if(notas.Length > 0) notas = new Collider2D[0];

            if (selectionMode) //Modo selección activado, se obtienen todas las notas dentro de la selección
            {
                //Fórmulas matemáticas: donde A es la posición inicial y B la final
                //'centro del cuadrado (como posición global)' C = B+(A-B)/2
                //'tamaño del cuadrado' D = |(A-B)/2|
                endPos = MousePosDetector.MousePos();
                size = startPos - endPos;
                center = endPos + size*0.5f;

                notas = Physics2D.OverlapBoxAll(new Vector2(center.x, center.y), new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y)), 0, notaDePista);

                objectsFound = notas.Length > 0;
                selectionMode = false;
            }
            if (lineR) lineR.enabled = false;
        }
        if (showSelectionRectangle && lineR.enabled && selectionMode) DrawRectangle();

    }

    void DrawRectangle()
    {
        Vector2 aux = MousePosDetector.MousePos();

        //Deslizamiento por los límites
        if (aux.x < posicion.x - width)
            aux.x = posicion.x - width;
        else if (aux.x > posicion.x + width)
            aux.x = posicion.x + width;
        
        if (aux.y < posicion.y - height)
            aux.y = posicion.y - height;
        else if (aux.y > posicion.y + height)
            aux.y = posicion.y + height;

        //Asignación de los vértices del rectángulo
        posiciones[0] = new Vector3(startPos.x, startPos.y);
        posiciones[1] = new Vector3(aux.x,startPos.y);
        posiciones[2] = aux;
        posiciones[3] = new Vector3(startPos.x, aux.y);
        posiciones[4] = new Vector3(startPos.x, startPos.y);

        lineR.positionCount = 5;
        lineR.SetPositions(posiciones);
    }

    void GeneralPointDrawing( Vector3 point)
    {
        Vector3[] posiciones = new Vector3[5];
        for (int i = 0; i < 5; i++)
        {
            posiciones[i] = point;
        }

        lineR.positionCount = 5;
        lineR.SetPositions(posiciones);
    }

    /// <summary>
    /// Comprueba si la posición del ratón se encuentra fuera del límite establecido
    /// </summary>
    bool DentroDeLimites()
    {
        Vector3 aux = MousePosDetector.MousePos();
        bool dentro = true;

        if (aux.x < posicion.x - width)
            dentro = false;
        else if (aux.x > posicion.x + width)
            dentro = false;

        if (aux.y < posicion.y - height)
            dentro = false;
        else if (aux.y > posicion.y + height)
            dentro = false;

        return dentro;
    }
}
