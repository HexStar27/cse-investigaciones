using System.Collections.Generic;
using UnityEngine;

public class MenuTablesFilter : MonoBehaviour
{
    public static List<MenuTablesFilter> activados = new List<MenuTablesFilter>();
    private ConfiguradorBloqueValor cbv;
    [HideInInspector] public string currentTable;

    private void UpdateList(string text)
    {
        currentTable = text;
    }


    private void Awake()
    {
        cbv = GetComponent<ConfiguradorBloqueValor>();
        activados.Add(this);
    }
    private void OnDestroy()
    {
        activados.Remove(this);
    }

    private void OnEnable()
    {
        cbv.onTextChanged.AddListener(UpdateList);
    }
    private void OnDisable()
    {
        cbv.onTextChanged.RemoveListener(UpdateList);
    }
}
