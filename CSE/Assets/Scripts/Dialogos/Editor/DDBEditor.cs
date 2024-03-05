using UnityEditor;
using UnityEngine;

namespace Hexstar.Dialogue
{
    [CustomEditor(typeof(DialogueDataBase))]
    public class DDBEditor : Editor
    {
        private DialogueDataBase script;
        private void OnEnable()
        {
            script = (DialogueDataBase)target;
        }

		public override void OnInspectorGUI()
		{
			// Draw default inspector before button...
			base.OnInspectorGUI();

			if (GUILayout.Button("PreLoad"))
			{
				script.LoadFromAsset();
				EditorUtility.SetDirty(script);
			}
		}
	}
}