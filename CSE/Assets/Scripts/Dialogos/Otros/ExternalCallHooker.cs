using Hexstar;
using Hexstar.Dialogue;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ExternalCallHooker : MonoBehaviour
{
    public Dictionary<string, Action> calls;

    /// <summary>
    /// Indica al controlador de diálogos si el jugador está usando el modo de consulta manual.
    /// </summary>
    private void CheckManualQueryMode()
    {
        //print("Query Mode: "+QueryModeController.IsQueryModeOnManual());
        if(QueryModeController.IsQueryModeOnManual())
            ControladorDialogos.SetDialogueEvent("modo_consulta","preguntar_modo");
        else
            ControladorDialogos.SetDialogueEvent("modo_consulta", "modo_bloque");
    }

    private void ChangeQueryMode()
    {
        if (ControladorDialogos.GetDialogueEventValue("modo_consulta").Equals("modo_bloque"))
        {
            QueryModeController.ChangeQM2Manual(false);
            QueryModeController.ForceSave();
        }
    }

    /*private void HasSkippedTutorial()
    {
        if(ControladorDialogos.GetDialogueEventValue("tuto_skip").Equals("true"))
        {
            XAPI_Builder.CreateStatement_TutorialSkip();
        }
    }*/


    private void CheckCalls(string value) 
    {
        if (calls.TryGetValue(value, out var result)) result();
        else Debug.LogError("EXTERNAL CALL \""+value+"\""+" NOT FOUND");
    }

    private void Awake()
    {
        calls = new()
        {
            { "check_query_mode", CheckManualQueryMode },
            { "change_query_mode", ChangeQueryMode },
            //{ "check_tutorial_skip", HasSkippedTutorial },
        };
    }
    private void OnEnable()
    {
        InstruccionesReferenciaCinematicas.onExternalCall.AddListener(CheckCalls);
    }
    private void OnDisable()
    {
        InstruccionesReferenciaCinematicas.onExternalCall.RemoveListener(CheckCalls);
    }
}
