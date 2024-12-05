using MFPSEditor;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using MFPS.Addon;
using System.Collections.Generic;
using System;

public class WeaponPack2Documentation : TutorialWizard
{
    //required//////////////////////////////////////////////////////
    private const string ImagesFolder = "mfps2/editor/weapon-pack-2/";
    private NetworkImages[] m_ServerImages = new NetworkImages[]
    {
        new NetworkImages{Name = "img-0.jpg", Image = null},
        new NetworkImages{Name = "img-1.png", Image = null},
    };
    private Steps[] AllSteps = new Steps[] {
     new Steps { Name = "Resume", StepsLenght = 0 },
      new Steps { Name = "Integration", StepsLenght = 3 },
    };

    public override void WindowArea(int window)
    {
        if (window == 0) { Resume(); }
        if (window == 1) { DrawIntegration(); }
    }
    //final required////////////////////////////////////////////////

    bool infoAlready = false;
    public bl_WeaponPack2Integration integrationScript;
    private Vector2 playersScroll;
    private const string INTEGRATION_PATH = "Assets/Addons/WeaponPack[2]/Prefabs/Integration/Integration.prefab";
    private bool integrating = false;

    [Serializable]
    public class PlayerEntity
    {
        public GameObject Prefab;
        public bl_PlayerReferences References;
        public bool HasWeaponsIntegrated = false;
    }

    public List<PlayerEntity> allPlayers;

    /// <summary>
    /// 
    /// </summary>
    public override void OnEnable()
    {
        base.OnEnable();
        base.Initizalized(m_ServerImages, AllSteps, ImagesFolder);
        infoAlready = bl_GameData.Instance.AllWeapons.Exists(x => x.Name == "Rifle6") && bl_GameData.Instance.AllWeapons.Exists(x => x.Name == "LMG");
        allowTextSuggestions = true;
    }

    /// <summary>
    /// 
    /// </summary>
    void Resume()
    {
        DrawSuperText("<b>Get Started:</b>\n\nThis package includes all files including the weapon models, so after you import the addon package you will have all needed to use it, you only have to run the integration once and then integrate the weapons in your player prefabs.\n\nFor the integration see the next section <b>Integration.</b>\n\n<b>Credits:</b>\n\nThese weapon models, arms, and textures are created by <b>DJMaesen</b>\nunder CC BY 4.0 <?link=https://creativecommons.org/licenses/by/4.0/>https://creativecommons.org/licenses/by/4.0/</link>\n\n<b>You are allowed to use these models and/or textures for commercial or non-commercial project as long as you credited the author.\ncheck the License.txt file for complete license details and source link.</b>");
    }

    /// <summary>
    /// 
    /// </summary>
    void DrawIntegration()
    {
        if (subStep == 0)
        {
            DrawText("<size=16><b>Weapon Information Setup</b></size>\n\nFirst, you need to setup the new weapons information in GameData > All Weapons, for it click in the button below.");
            Space(10);
            if (infoAlready)
            {
                DrawText("<color=#75C52FFF><i>Seems like information has been already setup, continue with the next step.</i></color>");
            }
            else
            {
                if (DrawButton("Setup Weapon Information"))
                {
                    if (integrationScript == null)
                    {
                        GameObject igo = AssetDatabase.LoadAssetAtPath(INTEGRATION_PATH, typeof(GameObject)) as GameObject;
                        if (igo != null)
                        {
                            integrationScript = igo.GetComponent<bl_WeaponPack2Integration>();
                        }
                        else { Debug.LogWarning("Integration object can't be found."); }
                    }

                    if (integrationScript == null) return;

                    var allWeapons = bl_GameData.Instance.AllWeapons;
                    for (int i = 0; i < integrationScript.GunData.Count; i++)
                    {
                        if (!allWeapons.Exists(x => x.Name == integrationScript.GunData[i].Name))
                        {
                            bl_GameData.Instance.AllWeapons.Add(integrationScript.GunData[i]);
                        }
                    }
                    EditorUtility.SetDirty(bl_GameData.Instance);

                    // if the GunIDs doesn't match
                    if (integrationScript.VerifyExportsIds())
                    {
                        var pRefernce = Resources.Load("MPlayer [WP2]", typeof(GameObject)) as GameObject;
                        if (pRefernce != null)
                        {
                            var player = pRefernce.GetComponent<bl_PlayerReferences>();
                            integrationScript.VerifyPlayerWeaponsIds(player);
                        }
                        integrationScript.VerifyLoadouts();
                    }

                    NextStep();
                }
                using (new MFPSEditorStyles.CenteredScope())
                {
                    DrawNote("<i><size=8>May take a few seconds the first time</size></i>");
                }
            }
        }
        else if (subStep == 1)
        {
            DrawSuperText("The addon package comes with a <?link=asset:Assets/Addons/WeaponPack[2]/Resources/MPlayer [WP2].prefab>Player prefab</link> that have all the weapons from the pack already integrated, you can use that prefab to test the weapons right away by assigning that prefab in GameData > Player 1 or Player 2.\n \nIf you want to integrate the weapons in your own player prefabs, you can do it with the automated integration below.\n\n\n<size=16><b>Integrate Weapons In Player Prefabs</b></size>\n\nYou simply have to integrate the weapons once in each player prefab that you are using in your game, this integration is automated and you only have to decide in which player prefabs integrate.\n\nClick the button below to fetch all the player prefabs that you are currently using, after this you should be able to see the list of all the player prefabs that you are using > click in the respective button next to each player prefab to integrate the weapons.");
            Space(15);

            if (GUILayout.Button("Fetch Players", MFPSEditorStyles.EditorSkin.customStyles[11]))
            {
                FetchPlayers();
            }
            Space(15);
            if (allPlayers != null)
            {
                DownArrow();
                GUI.enabled = !integrating;
                playersScroll = GUILayout.BeginScrollView(playersScroll, "box", GUILayout.MinHeight(150), GUILayout.ExpandHeight(true));
                for (int i = 0; i < allPlayers.Count; i++)
                {
                    var p = allPlayers[i];
                    EditorGUILayout.BeginHorizontal("box");
                    GUILayout.Label($"<b>{p.Prefab.name}</b>", Style.TextStyle);
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Select", MFPSEditorStyles.EditorSkin.customStyles[11]))
                    {
                        Selection.activeGameObject = p.Prefab;
                        EditorGUIUtility.PingObject(p.Prefab);
                    }
                    Space(5);
                    if (p.HasWeaponsIntegrated)
                    {
                        GUILayout.Label("<color=#75C52FFF><i><size=11>Already Integrated!</size></i></color>", Style.TextStyle);
                    }
                    else
                    {
                        if (GUILayout.Button("Integrate", MFPSEditorStyles.EditorSkin.customStyles[11]))
                        {
                            DoIntegration(p);
                        }
                    }
                    Space(20);
                    EditorGUILayout.EndHorizontal();
                    Space(4);
                }
                Space(20);
                GUILayout.EndScrollView();
                GUI.enabled = true;
                Space(10);
                DrawText("<i>Once you finish integrating the weapons in all player prefabs continue with the next step.</i>");
            }
        }
        else if (subStep == 2)
        {
            DrawText("The last thing you have to do to completely finish the integration is to adjust the TPWeapons if you integrate them in a custom player model <i>(not the default MFPS player models)</i>.\n \n<b><size=16>Why this is not done automatically?</size></b>\n \nThis <i>(adjust the TPWeapon position)</i> can't be done automatically in custom player models since every model has a different axis direction, therefore a manual adjustment is required.");
            DrawText("<b><size=16>How to adjust the TPWeapon position?</size></b>\n \nThe process is actually easy to do:\n \n<b>1.</b> Instance the player prefab or open the player prefab in the editor.\n \n<b>2.</b> Select one of the just integrated TPWeapons inside the player prefab and active it in the hierarchy so you can see it in the scene view.\n \nThe TPWeapons are located on the right-hand bone inside of the player prefab > player model.\n \n<b>3.</b> Manually adjust the position and rotation of the weapon to simulate that the player is holding it and aiming forward.");
            DrawServerImage("img-0.png");
            DrawNote("With the TPWeapon selected > in the inspector window > bl_NetworkGun > click on the button <b>Edit Hand Position</b> to simulate the IK Movement and you can edit the left-hand position too.");
            DrawText("Do the same with the other integrated weapons and save the player prefabs changes if required.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void FetchPlayers()
    {

        int referenceGunId = bl_GameData.Instance.AllWeapons.FindIndex(x => x.Name == "Double Barrel");
        if (referenceGunId == -1)
        {
            Debug.LogWarning("Couldn't find the reference weapon in GameData, make sure to run the weapon information integration firsts.");
            return;
        }

        allPlayers = new List<PlayerEntity>();

        if (bl_GameData.Instance.Player1 != null) AddPlayer(bl_GameData.Instance.Player1.gameObject, referenceGunId);
        if (bl_GameData.Instance.Player2 != null) AddPlayer(bl_GameData.Instance.Player2.gameObject, referenceGunId);

#if PSELECTOR
        foreach (var p in bl_PlayerSelector.Data.AllPlayers)
        {
            if (p.Prefab == null) continue;
            AddPlayer(p.Prefab, referenceGunId);
        }
#endif

    }

    private void AddPlayer(GameObject player, int rGundId)
    {
        if (player == null) return;

        var entity = new PlayerEntity();
        entity.Prefab = player;
        entity.References = player.GetComponent<bl_PlayerReferences>();
        entity.HasWeaponsIntegrated = entity.References.gunManager.AllGuns.Exists(x =>
        {
            if (x == null) return false;
            if (x.GunID == rGundId) return true;
            return false;

        }) && entity.References.playerNetwork.NetworkGuns.Exists(x =>
        {
            if (x == null || x.LocalGun == null) return false;
            if (x.LocalGun.GunID == rGundId) return true;
            return false;
        });

        allPlayers.Add(entity);
    }

    /// <summary>
    /// 
    /// </summary>
    void DoIntegration(PlayerEntity playerEntity)
    {
        if (integrationScript == null)
        {
            GameObject igo = AssetDatabase.LoadAssetAtPath(INTEGRATION_PATH, typeof(GameObject)) as GameObject;
            if (igo != null)
            {
                integrationScript = igo.GetComponent<bl_WeaponPack2Integration>();
            }
            else { Debug.LogWarning("Integration object can't be found."); }
        }

        if (integrationScript == null) return;

        GameObject playerInstance = playerEntity.Prefab;
        if (playerInstance.scene.name == null)
        {
            playerInstance = PrefabUtility.InstantiatePrefab(playerInstance, EditorSceneManager.GetActiveScene()) as GameObject;
#if UNITY_2018_3_OR_NEWER
            PrefabUtility.UnpackPrefabInstance(playerInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
#endif
            playerInstance.name = playerEntity.Prefab.name;
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
            i++;
        }

        EditorUtility.SetDirty(gameManager);
        EditorUtility.SetDirty(playerInstance);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        string path = AssetDatabase.GetAssetPath(playerEntity.Prefab);
        PrefabUtility.SaveAsPrefabAssetAndConnect(playerInstance, path, InteractionMode.UserAction);

        DestroyImmediate(playerInstance);
        Debug.Log($"<color=#75C52FFF><i><size=11>Weapons Integrated in player <b>{playerEntity.Prefab.name}</b></size></i></color>");
        playerEntity.HasWeaponsIntegrated = true;
        Repaint();
        EditorUtility.DisplayDialog("Done!", $"The weapons has been integrated in the player prefab {playerEntity.Prefab.name}!", "Ok");
    }

    [MenuItem("MFPS/Tutorials/Weapon Pack 2")]
    private static void ShowWindowMFPS()
    {
        EditorWindow.GetWindow(typeof(WeaponPack2Documentation));
    }
}