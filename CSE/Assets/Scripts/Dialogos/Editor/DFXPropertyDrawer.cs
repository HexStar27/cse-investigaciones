using UnityEditor;
using UnityEngine;

namespace Hexstar.Dialogue
{
    [CustomPropertyDrawer(typeof(DialogueFX))]
    public class DFXPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            float w = position.width/2;
            w = Mathf.Min(w,150);
            var tipoRect = new Rect(position.x, position.y, w, position.height);
            var valueRect = new Rect(position.x + w + 5, position.y, position.width - (w + 10), position.height);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(tipoRect, property.FindPropertyRelative("tipo"), GUIContent.none);
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}