using System;
using System.Collections.Generic;
using UnityEngine;

public class TableCreator
{
    public int maxWidth = 40;

    public List<int> widthPerColumn = new List<int>();
    public int currentWidth = 0;
    public List<int> heightsPerRow = new List<int>();
    public bool intentarExpandirAntesDeAumentarAltura = true;

    public void AddColumn(string value, int extraPadding = 0)
    {
        int s = value.Length + extraPadding;
        if (currentWidth + s <= maxWidth)
            widthPerColumn.Add(s);
        currentWidth = Sum(widthPerColumn);
    }

    public int AddRow(List<string> values, int extraPadding = 0)
    {
        int totalHeightForRow = 1;
        int n = Math.Min(values.Count,widthPerColumn.Count);
        for(int i = 0; i < n; i++)
        {
            //Comprobar la altura necesaria para cada columna
            int l = values[i].Length + extraPadding;
            int heightForRow = Mathf.CeilToInt((float)l / widthPerColumn[i]);

            //Expandimos anchura de columna si se puede para intentar no aumentar altura
            if (intentarExpandirAntesDeAumentarAltura && heightForRow > 1 && currentWidth < maxWidth)
            {
                int ancho = widthPerColumn[i];
                int falta = l - ancho;
                int nuevaAnchTotal = Math.Min(currentWidth + falta,maxWidth);
                int incremento = nuevaAnchTotal - currentWidth;
                currentWidth = nuevaAnchTotal;
                widthPerColumn[i] += incremento;
            }

            // Recalculamos, y si sigue siendo más grande no hay más remedio que aumentar altura
            heightForRow = Mathf.CeilToInt((float)l / widthPerColumn[i]);
            if (heightForRow > totalHeightForRow) totalHeightForRow = heightForRow;
        }
        heightsPerRow.Add(totalHeightForRow);
        return totalHeightForRow;
    }

    static int Sum(List<int> values)
    {
        int t = 0;
        for (int i = 0; i < values.Count; i++)
            t += values[i];
        return t;
    }
}
