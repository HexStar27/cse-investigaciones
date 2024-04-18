using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CaseCreator))]
public class CreadorCasosEditor : Editor
{
	private CaseCreator script;

	private void OnEnable()
	{
		script = (CaseCreator)target;
	}

	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Actualizar JSON"))
		{
			script.Actualizar();
		}
        base.OnInspectorGUI();
    }
}
