using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloqueDeConsulta : Arrastrable
{
    [Header("Configuración del bloque")]
    public ScriptableBlock contenidoBloque;
    [SerializeField] private LayerMask capaDeBloques;
    [SerializeField] private string tagSlot;
    [SerializeField] private Vector3 offset = Vector3.right;
    [Header("Ajustes del collider")]
    public Vector2 sizeWhileGrabbed;
    public Vector2 sizeWhileReleased;

    private Transform slotFijado;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D bc;

    private void OnEnable()
    {
        if (!spriteRenderer) TryGetComponent(out spriteRenderer);
        if (!bc) TryGetComponent(out bc);

        if (slotFijado)
        {
            spriteRenderer.sprite = contenidoBloque.dropSprite;
            bc.size = sizeWhileReleased;
        }
        else
        {
            spriteRenderer.sprite = contenidoBloque.grabSprite;
            bc.size = sizeWhileGrabbed;
        }
    }

    protected override void Update()
    {
        base.Update();

        if(slotFijado != null) transform.position = slotFijado.position + offset;
        
    }

    protected override void OnMouseUp()
    {
        base.OnMouseUp();
        Snap();
    }

    protected override void OnMouseDown()
    {
        if(slotFijado)
        {
            Vector3 pos = MousePosDetector.MousePos();
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }

        base.OnMouseDown();
        
        if(slotFijado != null)
        {
            if (slotFijado.TryGetComponent(out Slot slot))
            {
                slot.AsignarTrozoConsulta("");
            }
        }
        slotFijado = null;
        
        spriteRenderer.sprite = contenidoBloque.grabSprite;
        bc.size = sizeWhileGrabbed;
    }

    private void Snap()
    {
        Vector3 pos = MousePosDetector.MousePos();

        //1º Intento de detectar slot usando la posición del ratón
        BuscarSlot(Physics2D.RaycastAll(pos, Vector2.zero, 15, capaDeBloques));

        if (slotFijado == null) //2º Intento de detectar slot, esta vez con el centro del transform
            BuscarSlot(Physics2D.RaycastAll(transform.position, Vector2.zero, 15, capaDeBloques));
        

        if (slotFijado)
        {
            spriteRenderer.sprite = contenidoBloque.dropSprite;
            bc.size = sizeWhileReleased;
        }
    }

    private void BuscarSlot(RaycastHit2D[] rch)
    {
        for (int i = 0; i < rch.Length; i++)
        {
            if (rch[i].collider)
            {
                if (rch[i].collider.CompareTag(tagSlot))
                {
                    slotFijado = rch[i].transform;

                    if (slotFijado.TryGetComponent(out Slot slot))
                    {
                        if (slot.Trozo() == "") PasarTrozoConsulta(slot);
                        else slotFijado = null;
                    }
                    else Debug.LogError("Un objeto con el tag " + tagSlot + " debería tener siempre un tag. Corregir inmediatamente.");

                    i = rch.Length; //Para no malgastar ciclos de cpu
                }
            }
        }
    }

    private void PasarTrozoConsulta(Slot slot)
    {
        string trozoConsulta = contenidoBloque.cadena;

        //También debe unir el resto de parte del bloque según el tipo

        slot.AsignarTrozoConsulta(trozoConsulta);
    }
}