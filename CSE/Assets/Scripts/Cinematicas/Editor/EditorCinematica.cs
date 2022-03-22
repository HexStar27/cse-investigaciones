using UnityEditor;
using UnityEngine;

namespace Hexstar
{
    [CustomEditor(typeof(ElementoSeparador))]
    public class EditorCinematica : Editor
    {
        SerializedProperty m_SeparadorA, m_SeparadorB;

        private void OnEnable()
        {
            m_SeparadorA = serializedObject.FindProperty("separadorA");
            m_SeparadorB = serializedObject.FindProperty("separadorB");
        }

        public override void OnInspectorGUI()
        {
            // If we call base the default inspector will get drawn too.
            // Remove this line if you don't want that to happen.
            base.OnInspectorGUI();

            ElementoSeparador separador = target as ElementoSeparador;

            switch(separador.tipo)
            {
                case ElementoSeparador.Tipo.EsperarEvento:
                    EditorGUILayout.HelpBox("Buena elección ¬u¬", MessageType.Info);
                    EditorGUILayout.HelpBox("Para usarlo, suscríbelo a un evento con la función 'Escuchar()'", MessageType.Info);
                    break;
                case ElementoSeparador.Tipo.EsperarSegundos:
                    separador.segundos = EditorGUILayout.DelayedFloatField("Segundos", separador.segundos);
                    break;
                case ElementoSeparador.Tipo.EsperarContador:
                    separador.contador = separador.contadorMax = EditorGUILayout.DelayedIntField("Contador", separador.contadorMax);
                    break;
                case ElementoSeparador.Tipo.OperarAND:
                    EditorGUILayout.PropertyField(m_SeparadorA, new GUIContent("Separador A"));
                    EditorGUILayout.PropertyField(m_SeparadorB, new GUIContent("Separador B"));
                    break;
                case ElementoSeparador.Tipo.OperarOR:
                    EditorGUILayout.PropertyField(m_SeparadorA, new GUIContent("Separador A"));
                    EditorGUILayout.PropertyField(m_SeparadorB, new GUIContent("Separador B"));
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}