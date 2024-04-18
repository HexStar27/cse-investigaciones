using SimpleJSON;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace Hexstar.UI
{
    public class DialogueEditorHelper : MonoBehaviour
    {
        // Se debe poder hacer lo siguiente:
        //  -   Cargar DDB en lista
        //  -   Creación automatica de filas (hay un boton que se ajusta al final de la lista,
        //      creará la fila y la pondrá arriba suya)
        //  -   Eliminación de fila por índice
        //  -   Comprobador de validez de filas (la fila se pone en rojo si está mal y habrá una zona de output de errores)
        //  -   Guardar Cambios (No se podrán guardar cambios mientras hayan errores en filas)
        //
        //  La validación se va a hacer en el componente que tenga cada fila.

        private static DialogueEditorHelper instance;
        private readonly static List<GameObject> lista = new();
        private static List<Dictionary<string, object>> ddbContent = new();

        public Transform listHolder;
        public GameObject rowPrefab;
        public GameObject lastRowPrefab;
        [Space]
        public TMP_InputField errorField;

        private static string cachedDDB = "";

        public static void CargarTextoDDB(string ddb)
        {
            if (cachedDDB.Equals(ddb) && !cachedDDB.Equals("")) return;
            //csvContent = CSVReader.ReadString(ddb);
            var json = JSON.Parse(ddb);
            if (json != null && json.IsArray)
            {
                ddbContent.Clear();
                ddbContent.Capacity = json.Count;
                for (int i = 0; i < json.Count; i++)
                {
                    var row = json[i];
                    Dictionary<string, object> entry = new(4) {
                        { "Dialogo", row["Dialogo"].Value },
                        { "Efectos", row.HasKey("Efectos") ? row["Efectos"].Value : "" },
                        { "Grupo", row.HasKey("Grupo") ? row["Grupo"].Value : "" },
                        { "Etiqueta", row.HasKey("Etiqueta") ? row["Etiqueta"].Value : "" }
                    };
                    ddbContent.Add(entry);
                }
            }
            PopulateListWithCSVContent();
        }
        public static string GuardarTextoDDB()
        {
            JSONArray arr = new();
            int n = ddbContent.Count;
            for (int i = 0; i < n; i++)
            {
                JSONObject entry = new();
                Dictionary<string, object> row = ddbContent[i];
                if (row.TryGetValue("Dialogo", out object val)) entry.Add("Dialogo", (string)val);
                if (row.TryGetValue("Efectos", out val)) entry.Add("Efectos", (string)val);
                if (row.TryGetValue("Grupo", out val)) entry.Add("Grupo", (string)val);
                if (row.TryGetValue("Etiqueta", out val)) entry.Add("Etiqueta", (string)val);
                arr.Add(entry);
            }
            return cachedDDB = arr.ToString();
        }
        

        public static void AddNewEmptyRow()
        {
            GameObject boton = lista[^1];
            GameObject row = Instantiate(instance.rowPrefab, instance.listHolder);
            lista[^1] = row;
            lista.Add(boton);
            Dictionary<string, object> d = new() {
                { "Dialogo", "" },
                { "Efectos", "" },
                { "Grupo", "" },
                { "Etiqueta", "" }
            };
            ddbContent.Add(d);
            row.GetComponent<DialogueRow4Helper>().Setup(ddbContent.Count - 1);
            boton.transform.SetAsLastSibling();
        }
        private static void PopulateListWithCSVContent()
        {
            Limpiar();
            lista.Capacity = ddbContent.Count + 1;
            for (int i = 0; i < ddbContent.Count; i++)
            {
                GameObject row = Instantiate(instance.rowPrefab, instance.listHolder);
                lista.Add(row);
                row.GetComponent<DialogueRow4Helper>().Setup(i);
            };
            lista.Add(Instantiate(instance.lastRowPrefab, instance.listHolder));
        }

        public static void EliminarFila(int idx)
        {
            Destroy(lista[idx]);
            lista.RemoveAt(idx);
            ddbContent.RemoveAt(idx);
            for (int i = idx; i < lista.Count - 1; i++)
            {
                lista[i].GetComponent<DialogueRow4Helper>().Setup(i);
            }
        }

        public static bool DesplazarFila(int idx, bool abajo)
        {
            if (idx < 0 || idx >= lista.Count) return false;
            if (idx == 0 && !abajo) return false;
            if (idx == (lista.Count - 1) && abajo) return false;


            int other = idx + (abajo ? 1 : -1);
            lista[idx].transform.SetSiblingIndex(other);
            (lista[idx], lista[other]) = (lista[other], lista[idx]);
            (ddbContent[idx], ddbContent[other]) = (ddbContent[other], ddbContent[idx]);

            lista[idx].GetComponent<DialogueRow4Helper>().Refresh(idx);
            lista[other].GetComponent<DialogueRow4Helper>().Refresh(other);
            return true;
        }

        /// <summary>
        /// Se presupone que el valor ya ha sido validado por la fila
        /// </summary>
        public static void ActualizarFila(int idx, string column, string newValue)
        {
            ddbContent[idx][column] = newValue;
            SetErrorMessage(-1,"");
        }
        public static Dictionary<string, object> GetFila(int idx) => ddbContent[idx];
        public static void SetErrorMessage(int idx, string msg)
        {
            if (msg.Equals("") || idx < 0) instance.errorField.text = "";
            else instance.errorField.text = "Error en la línea " + idx + ": " + msg;
        }
        

        private void Awake()
        {
            instance = this;
            Limpiar();
        }

        private static void Limpiar()
        {
            foreach (var ob in lista) Destroy(ob);
            lista.Clear();
        }
    }
}