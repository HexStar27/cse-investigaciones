using Hexstar.Dialogue;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hexstar.UI
{
    public class DialogueRow4Helper : MonoBehaviour
    {
        private static int linesWitherrors = 0;
        [SerializeField] TMP_InputField dialogo;
        [SerializeField] TMP_InputField efectos;
        [SerializeField] TMP_InputField grupo;
        [SerializeField] TMP_InputField etiqueta;
        [SerializeField] TextMeshProUGUI indice;
        bool okdi = true, okef = true, okgr = true, oket = true;

        [SerializeField] Button delButton;
        [SerializeField] Button upButton;
        [SerializeField] Button downButton;

        int listIdx = 0;

        private Color c_error = new(1f, 0.5f, 0.5f), c_ok = new(0.1159131f, 0.1179948f, 0.1509434f);

        public void Setup(int idx)
        {
            listIdx = idx;
            indice.text = idx.ToString();
            var dict = DialogueEditorHelper.GetFila(idx);
            if (dict == null || dict.Count == 0) return;

            if (dict.TryGetValue("Dialogo", out object str))
                dialogo.SetTextWithoutNotify(str as string);
            else dialogo.SetTextWithoutNotify("");

            if (dict.TryGetValue("Efectos", out str))
                efectos.SetTextWithoutNotify(str as string);
            else efectos.SetTextWithoutNotify("");

            if (dict.TryGetValue("Grupo", out str))
                grupo.SetTextWithoutNotify(str as string);
            else grupo.SetTextWithoutNotify("");

            if (dict.TryGetValue("Etiqueta", out str))
                etiqueta.SetTextWithoutNotify(str as string);
            else etiqueta.SetTextWithoutNotify("");
        }
        public void Refresh(int newIdx)
        {
            listIdx = newIdx;
            indice.text = newIdx.ToString();
        }

        public static bool HasAnyErrors() => linesWitherrors > 0;

        private void ValidarDialogo(string newVal)
        {
            bool ok = true;
            if (newVal.Contains(';')) ok = false;
            if (newVal.Contains('\"')) ok = false;
            
            dialogo.GetComponent<Image>().color = ok ? c_ok : c_error;
            if (ok) DialogueEditorHelper.ActualizarFila(listIdx, "Dialogo", newVal);
            else DialogueEditorHelper.SetErrorMessage(listIdx, "Dialogo no puede contener ';' o '\"'");

            if (okdi != ok) linesWitherrors += ok ? -1 : 1;
            okdi = ok;
        }
        private void ValidarEfectos(string newVal)
        {
            bool ok = true;
            try
            { DialogueDataBase.LoadFXs(newVal); }
            catch (Exception e) 
            { DialogueEditorHelper.SetErrorMessage(listIdx, e.Message); ok = false; }
            
            efectos.GetComponent<Image>().color = ok ? c_ok : c_error;
            if (ok) DialogueEditorHelper.ActualizarFila(listIdx, "Efectos", newVal);

            if (okef != ok) linesWitherrors += ok ? -1 : 1;
            okef = ok;
        }
        private void ValidarGrupo(string newVal)
        {
            bool ok = true;
            if (newVal.Contains(' '))
            {
                DialogueEditorHelper.SetErrorMessage(listIdx, "Grupo no puede contener espacios"); ok = false;
            }
            if (newVal.Contains(';'))
            {
                DialogueEditorHelper.SetErrorMessage(listIdx, "Grupo no puede contener ';'"); ok = false;
            }

            grupo.GetComponent<Image>().color = ok ? c_ok : c_error;
            if (ok) DialogueEditorHelper.ActualizarFila(listIdx, "Grupo", newVal);

            if (okgr != ok) linesWitherrors += ok ? -1 : 1;
            okgr = ok;
        }
        private void ValidarEtiqueta(string newVal)
        {
            bool ok = true;
            if (newVal.Contains(' '))
            {
                DialogueEditorHelper.SetErrorMessage(listIdx, "Etiqueta no puede contener espacios"); ok = false;
            }
            if (newVal.Contains(';'))
            {
                DialogueEditorHelper.SetErrorMessage(listIdx, "Etiqueta no puede contener ';'"); ok = false;
            }

            etiqueta.GetComponent<Image>().color = ok ? c_ok : c_error;
            if (ok) DialogueEditorHelper.ActualizarFila(listIdx, "Etiqueta", newVal);

            if (oket != ok) linesWitherrors += ok ? -1 : 1;
            oket = ok;
        }

        private void Borrar() => DialogueEditorHelper.EliminarFila(listIdx);

        private void ShiftUp()
        {
            DialogueEditorHelper.DesplazarFila(listIdx, false);
        }
        private void ShiftDown()
        {
            DialogueEditorHelper.DesplazarFila(listIdx, false);
        }

        private void Awake()
        {
            delButton.onClick.AddListener(Borrar);
            upButton.onClick.AddListener(ShiftUp);
            downButton.onClick.AddListener(ShiftDown);
            dialogo.onEndEdit.AddListener(ValidarDialogo);
            efectos.onEndEdit.AddListener(ValidarEfectos);
            grupo.onEndEdit.AddListener(ValidarGrupo);
            etiqueta.onEndEdit.AddListener(ValidarEtiqueta);
        }
    }
}