using UnityEditor;
using UnityEngine;

namespace Hexstar.Dialogue
{
	[CustomEditor(typeof(Dialogo))]
	public class DEditor : Editor
	{
		private Dialogo script;
		private void OnEnable()
		{
			script = (Dialogo)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Refresh"))
			{
				script.RefreshLooks();
				EditorUtility.SetDirty(script);
			}
			if(GUILayout.Button("Reload DB & Refresh"))
            {
				script.ddb.LoadFromAsset();
				script.RefreshLooks();
				EditorUtility.SetDirty(script);
			}
			if(GUILayout.Button("Use Range"))
            {
				script.RellenarSegunRango(true);
				script.RefreshLooks();
				EditorUtility.SetDirty(script);
			}
			if(GUILayout.Button("Add Range"))
            {
				script.RellenarSegunRango(false);
				script.RefreshLooks();
				EditorUtility.SetDirty(script);
			}
		}
	}
}