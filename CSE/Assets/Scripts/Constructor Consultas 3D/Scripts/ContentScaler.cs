///Reordena y coloca verticalmente todos sus hijos equidistantemente :v

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ContentScaler : MonoBehaviour
{
    public float childSize = 100;
    public Vector2 offset = new Vector2(4, 4);
    [HideInInspector] public RectTransform r;
    private List<RectTransform> hijos = new List<RectTransform>();
    private int numHijos;
    public int espaciosAdicionales;
    public float initialWidth = 550;

    private void OnEnable()
    {
        Actualizar();
    }

    private void Reordenar()
    {
        hijos.Clear();
        for (int i = 0; i < transform.childCount; i++)
            hijos.Add(transform.GetChild(i).GetComponent<RectTransform>());
        
        for (int i = 0; i < hijos.Count; i++)
        {
            Vector3 pos = new Vector3(offset.x, (-(childSize + offset.y) * i) - offset.y, 0);
            hijos[i].anchoredPosition = pos;
        }
    }

    private void FixedUpdate()
    {
        if (transform.childCount != numHijos) Actualizar();
        numHijos = transform.childCount;
    }

    public void Actualizar()
    {
        if (r == null) r = GetComponent<RectTransform>();
        float w = 0;
        if (initialWidth > 0) w = initialWidth;
        r.sizeDelta = new Vector2(w, (r.childCount + espaciosAdicionales) * (childSize + offset.y));
        Reordenar();
    }

    public int IndiceSegunPos(float altura)
    {
        return -(int)((altura - r.anchoredPosition.y) / (childSize + offset.y));
    }
}
