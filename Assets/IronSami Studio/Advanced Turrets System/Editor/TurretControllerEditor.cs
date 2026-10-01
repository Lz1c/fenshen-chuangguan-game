#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace IronSamiStudio.AdvancedTurretAI
{
    /// <summary>
    /// Custom inspector for TurretController that adds a Discord help prompt.
    /// </summary>
    [CustomEditor(typeof(TurretController))]
    public class TurretControllerEditor : Editor
    {
        SerializedProperty m_ScriptProp;

        void OnEnable()
        {
            // Cache the m_Script reference so we can exclude it in the drawer
            m_ScriptProp = serializedObject.FindProperty("m_Script");
        }

        public override void OnInspectorGUI()
        {
            // Add some breathing room
            EditorGUILayout.Space(10);

            serializedObject.Update();

            // Draw everything except the script reference
            DrawPropertiesExcluding(serializedObject, "m_Script");

            serializedObject.ApplyModifiedProperties();

            // Footer – Discord prompt
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Need Help?", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "For news, updates, setup tips, and community support — join our Discord!",
                MessageType.Info
            );

            if (GUILayout.Button("💬  Join the Discord"))
            {
                Application.OpenURL("https://discord.gg/MapKXwNeDz");
            }
        }
    }
}
#endif
