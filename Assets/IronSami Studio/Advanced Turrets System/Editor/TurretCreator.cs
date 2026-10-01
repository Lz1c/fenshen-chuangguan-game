#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace IronSamiStudio.AdvancedTurretAI
{
    /// <summary>
    /// Adds two context-menu commands that simply instantiate the stock prefabs
    /// without touching or overriding any of their settings.
    /// Place this script in an Editor folder.
    /// </summary>
    internal static class TurretPrefabSpawner
    {
        private const string SINGLE_PREFAB =
            "Assets/IronSami Studio/Advanced Turrets System/Prefabs/Turrets/Turret_Single_Barrel_Default.prefab";

        private const string DOUBLE_PREFAB =
            "Assets/IronSami Studio/Advanced Turrets System/Prefabs/Turrets/Turret_Double_Barrel_Default.prefab";

        [MenuItem("GameObject/Turret/Spawn Default Single Barrel Turret", false, 10)]
        private static void SpawnSingle() => Spawn(SINGLE_PREFAB, "Turret (Single Barrel)");

        [MenuItem("GameObject/Turret/Spawn Default Double Barrel Turret", false, 11)]
        private static void SpawnDouble() => Spawn(DOUBLE_PREFAB, "Turret (Double Barrel)");

        // ──────────────────────────────────────────────────────────────────────────
        private static void Spawn(string prefabPath, string undoName)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"Prefab not found at:\n{prefabPath}");
                return;
            }

            // Instantiates exactly what’s stored in the prefab
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            Undo.RegisterCreatedObjectUndo(instance, undoName);
            Selection.activeGameObject = instance;
        }
    }
}
#endif
