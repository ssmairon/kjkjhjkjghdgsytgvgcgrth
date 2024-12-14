using System.Collections.Generic;
using UnityEngine;
using MFPSEditor;
using UnityEditor;
using MFPS.Others;
using UnityEditor.SceneManagement;

public class WeaponPack1Tutorial : TutorialWizard
{
    //required//////////////////////////////////////////////////////
    private const string ImagesFolder = "mfps2/editor/weapon-pack/";
    private NetworkImages[] m_ServerImages = new NetworkImages[]
    {
        new NetworkImages{Name = "img-0.jpg", Image = null},
        new NetworkImages{Name = "img-1.png", Image = null},
    };
    private Steps[] AllSteps = new Steps[] {
     new Steps { Name = "Resume", StepsLenght = 0 },
      new Steps { Name = "Integration", StepsLenght = 2 },
    };

    public override void WindowArea(int window)
    {
        if (window == 0) { Resume(); }
        if (window == 1) { DrawIntegration(); }
    }
    //final required////////////////////////////////////////////////

    bool infoAlready = false;
    bl_WeaponPackIntegration integrationScript;
    bl_PlayerNetwork playerPrefab;
    bool interationDone = false;

    /// <summary>
    /// 
    /// </summary>
    public override void OnEnable()
    {
        base.OnEnable();
        base.Initizalized(m_ServerImages, AllSteps, ImagesFolder);
        infoAlready = bl_GameData.Instance.AllWeapons.Exists(x => x.Name == "Rifle04") && bl_GameData.Instance.AllWeapons.Exists(x => x.Name == "Axe2");
        allowTextSuggestions = true;
    }

    /// <summary>
    /// 
    /// </summary>
    void Resume()
    {
        DrawText("This addon require <color=#328CD2>Animated Hands with Weapons Pack</color> asset from the Asset Store," +
            "so in order to use you have to import that package in your project.\n \nOnce you have you can continue with the integration");
        DownArrow();
        DrawTitleText("Setup Run Animations");
        DrawText("By default the Weapon Model package have all their animations clamped, and to work correctly some of them have to be looped, so you have to manually set up these,\n\nThe weapons that has to be modified are:\n\n•  Grenade\n•  Knife\n•  Axe\n•  Two Hands Axe\n\nYou only have to set the animation <b>Run</b> of each of these weapons to loop, for this go to the Hands_Weapons_Animations_Pack folder -> Modelas -> [Find the folder of one of the aboves weapon] -> Animations -> Select the Run Animation -> in the inspector window select the Animation tab -> select the animation in the Clip list -> Mark the toggle Loop Time -> Click on <b>Apply</b> button.\n\n");
        DrawServerImage(1);
    }

    public bool overrideSourcePrefab = true;
    void DrawIntegration()
    {
        if (subStep == 0)
        {
            DrawText("First, lets add the weapons info in GameData, this is only required to do one time:");
            Space(10);
            if (infoAlready)
            {
                DrawText("Seems like information has been already setup, continue with the next step");
            }
            else
            {
                if (DrawButton("Setup weapon info"))
                {
                    if(integrationScript == null)
                    {
                        GameObject igo = AssetDatabase.LoadAssetAtPath("Assets/Addons/WeaponPack[1]/Prefabs/Integration/Integration.prefab", typeof(GameObject)) as GameObject;
                        if (igo != null)
                        {
                            integrationScript = igo.GetComponent<bl_WeaponPackIntegration>();
                        }
                        else { Debug.LogWarning("Integration object can't be found."); }
                    }

                    if (integrationScript == null) return;
                    for (int i = 0; i < integrationScript.GunData.Count; i++)
                    {
                        if (!bl_GameData.Instance.AllWeapons.Exists(x => x.Name == integrationScript.GunData[i].Name))
                        {
                            bl_GameData.Instance.AllWeapons.Add(integrationScript.GunData[i]);
                        }
                    }
                    EditorUtility.SetDirty(bl_GameData.Instance);

                    // if the GunIDs doesn't match
                    if (integrationScript.VerifyExportsIds())
                    {
                        var pRefernce = Resources.Load("MPlayer [WP1]", typeof(GameObject)) as GameObject;
                        if (pRefernce != null)
                        {
                            var player = pRefernce.GetComponent<bl_PlayerReferences>();
                            integrationScript.VerifyPlayerWeaponsIds(player);
                        }
                        integrationScript.VerifyLoadouts();
                    }

                    NextStep();
                }
                using(new MFPSEditorStyles.CenteredScope())
                {
                    DrawText("<i><size=8>May take a few seconds the first time</size></i>");
                }
            }
        }else if(subStep == 1)
        {
            DrawSuperText("The addon package comes with a <?link=asset:Assets/Addons/WeaponPack[1]/Resources/MPlayer [WP1].prefab>Player prefab</link> that have all the weapons from the pack already integrated, you can use that prefab to test the weapons right away.\n \nBut if you want to add the weapons to another player prefab, you simply need to drag the player prefab in the field below and click on the <b>Integrate</b> button.");
            DrawNote("The Player Prefabs by default are located in the Resources folder of MFPS.");
            DownArrow();
            GUILayout.BeginHorizontal();
            playerPrefab = EditorGUILayout.ObjectField("Player Prefab", playerPrefab, typeof(bl_PlayerNetwork), true) as bl_PlayerNetwork;
            GUILayout.EndHorizontal();
            Space(10);
            GUI.enabled = playerPrefab != null;
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Apply changes to source player prefab?");
                var rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none);
                overrideSourcePrefab = MFPSEditorStyles.FeatureToogle(rect, overrideSourcePrefab, "");
                GUILayout.FlexibleSpace();
            }
            Space(10);
            if (DrawButton("Integrate"))
            {
                DoIntegration();
            }
            GUI.enabled = true;

            if (interationDone)
            {
                DownArrow();
                if (overrideSourcePrefab)
                {
                    DrawText("You all set!\nNow you will see an instance of the Player prefab in the hierarchy window, you can change the player classes loadouts in the <b>bl_GunManager</b> inspector ➔ Assault Class, etc...\n");
                }
                else
                {
                    DrawText("All done, now in your hierarchy will appear a unlink instance of your player prefab, simple apply the change to the source prefab <i>(by dragging the instance over the source prefab in the project view)</i> then you can set the weapon for each class in WeaponManager -> bl_GunManager -> Assault Class, etc...\n \nThat's you are ready to use the weapons in that player prefab.");
                }
                DrawServerImage(0);
            }
        }
    }

    void DoIntegration()
    {
        if (playerPrefab == null) return;
        if (integrationScript == null)
        {
            GameObject igo = AssetDatabase.LoadAssetAtPath("Assets/Addons/WeaponPack[1]/Prefabs/Integration/Integration.prefab", typeof(GameObject)) as GameObject;
            if (igo != null)
            {
                integrationScript = igo.GetComponent<bl_WeaponPackIntegration>();
            }
            else { Debug.LogWarning("Integration object can't be found."); }
        }

        if (integrationScript == null) return;

        GameObject playerInstance = playerPrefab.gameObject;
        if (playerPrefab.gameObject.scene.name == null)
        {
            playerInstance = PrefabUtility.InstantiatePrefab(playerPrefab.gameObject, EditorSceneManager.GetActiveScene()) as GameObject;
#if UNITY_2018_3_OR_NEWER
            PrefabUtility.UnpackPrefabInstance(playerInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
#endif
            playerInstance.name = playerPrefab.gameObject.name;
        }
        int i = 0;
        var gameManager = playerInstance.GetComponentInChildren<bl_GunManager>(true);
        foreach (var obj in integrationScript.weaponPack)
        {
            bl_WeaponExported we = obj;
            if (we == null) { Debug.Log(obj.name + " skipped due it is not a exported weapon."); continue; }
            if (obj.gameObject.scene.name == null)
            {
                GameObject weo = PrefabUtility.InstantiatePrefab(obj.gameObject, EditorSceneManager.GetActiveScene()) as GameObject;
#if UNITY_2018_3_OR_NEWER
                PrefabUtility.UnpackPrefabInstance(weo, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
#endif
                we = weo.GetComponent<bl_WeaponExported>();
            }
            if (we.WeaponInfo != null && !string.IsNullOrEmpty(we.WeaponInfo.Name))
            {
                if (!bl_GameData.Instance.AllWeapons.Exists(x => x.Name == we.WeaponInfo.Name))
                {
                    bl_GameData.Instance.AllWeapons.Add(we.WeaponInfo);
                    we.FPWeapon.GunID = bl_GameData.Instance.AllWeapons.Count - 1;
                    EditorUtility.SetDirty(bl_GameData.Instance);
                }
                else
                {
                    we.FPWeapon.GunID = bl_GameData.Instance.AllWeapons.FindIndex(x => x.Name == we.WeaponInfo.Name);
                    EditorUtility.SetDirty(bl_GameData.Instance);
                }
            }

            bl_GunManager gm = gameManager;
            if (gm != null)
            {
                we.FPWeapon.transform.parent = gm.transform;
                we.FPWeapon.transform.localPosition = we.FPWPosition;
                we.FPWeapon.transform.localRotation = we.FPWRotation;
                we.FPWeapon.name = we.FPWeapon.name.Replace("[FP]", "");
                gm.AllGuns.Add(we.FPWeapon);
                we.FPWeapon.gameObject.SetActive(i == 7);
            }

            if (we.TPWeapon != null)
            {
                we.TPWeapon.LocalGun = we.FPWeapon;
                we.TPWeapon.transform.parent = playerInstance.GetComponent<bl_PlayerNetwork>().NetworkGuns[0].transform.parent;
                we.TPWeapon.transform.localPosition = we.TPWPosition;
                we.TPWeapon.transform.localRotation = we.TPWRotation;
                we.TPWeapon.name = we.TPWeapon.name.Replace("[TP]", "");
                playerInstance.GetComponent<bl_PlayerNetwork>().NetworkGuns.Add(we.TPWeapon);
                we.TPWeapon.gameObject.SetActive(i == 7);
            }
            DestroyImmediate(we.gameObject);
            interationDone = true;
            i++;
        }

        EditorUtility.SetDirty(gameManager);
        EditorUtility.SetDirty(playerInstance);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (overrideSourcePrefab && playerPrefab != null)
        {
            string path = AssetDatabase.GetAssetPath(playerPrefab);
            PrefabUtility.SaveAsPrefabAssetAndConnect(playerInstance, path, InteractionMode.UserAction);
        }
    }

    [MenuItem("MFPS/Tutorials/Weapon Pack 1")]
    private static void ShowWindowMFPS()
    {
        EditorWindow.GetWindow(typeof(WeaponPack1Tutorial));
    }
}