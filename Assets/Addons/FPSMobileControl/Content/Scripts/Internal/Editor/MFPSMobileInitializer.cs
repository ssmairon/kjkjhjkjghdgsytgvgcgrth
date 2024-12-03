using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEditor;
using MFPSEditor;

public class MFPSMobileInitializer
{
    private const string DEFINE_KEY = "MFPSM";

#if !MFPSM
    [MenuItem("MFPS/Addons/MobileControl/Enable")]
    private static void Enable()
    {
        EditorUtils.SetEnabled(DEFINE_KEY, true);
    }
#endif

#if MFPSM
    [MenuItem("MFPS/Addons/MobileControl/Disable")]
    private static void Disable()
    {
        EditorUtils.SetEnabled(DEFINE_KEY, false);
    }
#endif

    [MenuItem("MFPS/Addons/MobileControl/Integrate")]
    public static void Instegrate()
    {
        bl_TouchHelper km = GameObject.FindObjectOfType<bl_TouchHelper>();
        if (km != null)
        {
            Debug.Log("The Mobile Control has been already integrated in this scene.");
            return;
        }

        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Addons/FPSMobileControl/Content/Prefab/MobileCanvas.prefab", typeof(GameObject)) as GameObject;
        if (prefab != null)
        {
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (bl_UIReferences.Instance != null)
            {
                instance.transform.SetParent(bl_UIReferences.Instance.transform, false);
                instance.transform.SetAsLastSibling();
            }
            Selection.activeGameObject = instance;
            EditorGUIUtility.PingObject(instance);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorUtility.SetDirty(instance);
            Debug.Log("<color=green>Mobile Control Integrated in this map.</color>");
        }
        else { Debug.LogWarning("Couldn't found the mobile control prefab!"); }
    }

    [MenuItem("MFPS/Addons/MobileControl/Integrate", true)]
    private static bool InstegrateValidate()
    {
        bl_TouchHelper km = GameObject.FindObjectOfType<bl_TouchHelper>();
        bl_GameManager gm = GameObject.FindObjectOfType<bl_GameManager>();
        return (km == null && gm != null);
    }
}