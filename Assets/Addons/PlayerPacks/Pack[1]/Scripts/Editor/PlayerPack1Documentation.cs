using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFPSEditor;
using UnityEditor;
using MFPS.Internal.Structures;
using MFPS.Addon.PlayerPack;

public class PlayerPack1Documentation : TutorialWizard
{
    //required//////////////////////////////////////////////////////
    private const string ImagesFolder = "mfps2/editor/player-pack/1/";
    private NetworkImages[] m_ServerImages = new NetworkImages[]
    {
        new NetworkImages{Name = "img-0.jpg", Image = null},
        new NetworkImages{Name = "img-1.jpg", Image = null},
        new NetworkImages{Name = "img-2.jpg", Image = null},
        new NetworkImages{Name = "img-3.png", Image = null},
    };
    private readonly GifData[] AnimatedImages = new GifData[]
    {
        new GifData{ Path = "createwindowobj.gif" },
        new GifData{ Path = "addnewwindowfield.gif" },
    };
    private Steps[] AllSteps = new Steps[] {
     new Steps { Name = "Get Started", StepsLenght = 0, DrawFunctionName = nameof(DocGetStarted) },
    new Steps { Name = "Integration", StepsLenght = 0, DrawFunctionName = nameof(DocIntegrate) },
    new Steps { Name = "Weapon Pack", StepsLenght = 0, DrawFunctionName = nameof(DocWeaponPack) },
    };

    public override void WindowArea(int window)
    {
        AutoDrawWindows();
    }
    //final required////////////////////////////////////////////////
    public bl_PlayerNetwork playerPrefab;


    public override void OnEnable()
    {
        base.OnEnable();
        base.Initizalized(m_ServerImages, AllSteps, ImagesFolder, AnimatedImages);
        GUISkin gs = Resources.Load<GUISkin>("content/MFPSEditorSkin") as GUISkin;
        if (gs != null)
        {
            base.SetTextStyle(gs.customStyles[2]);
            base.m_GUISkin = gs;
        }
        FetchWebTutorials("mfps2/tutorials/");
        //allowTextSuggestions = true;
    }

    void DocGetStarted()
    {
        DrawHyperlinkText("<b>Required:</b>\n\nMFPS 1.8++\nUnity 2018.4++\n\nThis Addon requires the <link=https://assetstore.unity.com/packages/3d/characters/humanoids/pbr-customized-soldier-111945?aid=1101lJFi&utm_source=aff>PBR Customized Soldiers</link> asset from the <b>Asset Store</b>, so in order to use this you have to import that package in your project before proceed.\n\nAfter import it you can continue with the integration.");
    }

    void DocIntegrate()
    {
        DrawHyperlinkText("There's no integration needed to use the player prefabs included in this package with <b>MFPS Core</b>, all you have to do is select which player prefab do you want to use for your teams.\n \nThe players are located in: <i>Assets -> Addons -> PlayerPacks -> Pack[1] -> Resources->*</i> <link=asset:Assets/Addons/PlayerPacks/Pack[1]/Resources/MPlayer [Soldier01].prefab>(Click here to open)</link>\n \nSimply select the player prefab that you want to use and then drag it in <link=asset:Assets/MFPS/Resources/GameData.asset>GameData</link> -> <b>Player 1</b> or <b>Player 2</b> field.\n \nThat's.");
        DrawLineFooter(20);
        DrawTitleText("Player Selector");
        Space(10);
        DrawText("If you are using the <b>Player Selector</b> addon, this package also includes an auto-integration for it, simply <b>click on the button below</b> and that's.");
        Space(10);
        using (new CenteredScope())
        {
            GUILayout.FlexibleSpace();
#if PSELECTOR
            if (Buttons.OutlineButton("Integrate with Player Selector"))
            {
                IntegrateWithPlayerSelector();
            }

#else
                DrawNote("Player Selector is not enabled.");
#endif
            GUILayout.FlexibleSpace();
        }
    }

    void DocWeaponPack()
    {
        DrawHyperlinkText("This package also includes integration with the <link=https://www.lovattostudio.com/en/shop/template/weapon-pack-1/>Weapon Pack #1</link> addon.\n \nAs is mentioned in the respective documentation, the Third Person Weapons appears in a wrong position/rotation when you change of player model in the player prefabs, what this integration does is <b>automatically set up all the TPWeapons</b> of the weapon pack in the Player Prefabs of this package, here is how you do it:\n \n1. <b>Integrate the weapons:</b>\nFollow the documentation of the Weapon Pack addon, there you will find the steps to automatically integrate the weapons in the player prefabs.\n \n2. Once the weapon integration is done and you applied and save the changes to the player prefab, drag the same player prefab in the field below then click on the <b>Set up</b> button.");

        Space(10);
        using (new CenteredScope())
        {
            GUILayout.FlexibleSpace();
            playerPrefab = EditorGUILayout.ObjectField("Player Prefab", playerPrefab, typeof(bl_PlayerNetwork), true, GUILayout.Width(400)) as bl_PlayerNetwork;
            Space(8);
            GUI.enabled = playerPrefab != null;
            if (Buttons.OutlineButton("Set up"))
            {
                SetupPlayerWeaponPack1();
            }
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
        }
        Space(10);
        DownArrow();
        DrawText("3. Don't forget to save the changes to the player prefab.");
    }

#if PSELECTOR
    static void IntegrateWithPlayerSelector()
    {
        var integrationObject = AssetDatabase.LoadAssetAtPath("Assets/Addons/PlayerPacks/Pack[1]/Prefabs/Integration/PlayersInfo.prefab", typeof(GameObject)) as GameObject;
        if (integrationObject == null)
        {
            Debug.LogWarning("The integration info couldn't be found!");
            return;
        }

        var integrationInfo = integrationObject.GetComponent<PlayerPrefabInfo>();
        var playerSelectorData = bl_PlayerSelector.Data;

        for (int i = 0; i < integrationInfo.players.Count; i++)
        {
            var info = integrationInfo.players[i];
            if (playerSelectorData.AllPlayers.Exists(x => x.Prefab == info.Prefab)) continue;

            var psinfo = new MFPS.Addon.PlayerSelector.bl_PlayerSelectorInfo();
            psinfo.Name = info.Name;
            psinfo.Prefab = info.Prefab;
            psinfo.Preview = info.Preview;
            psinfo.Unlockability = new MFPSItemUnlockability()
            {
                ItemType = MFPSItemUnlockability.ItemTypeEnum.PlayerSkin
            };

            playerSelectorData.AllPlayers.Add(psinfo);
            int id = playerSelectorData.AllPlayers.Count - 1;
            playerSelectorData.FFAPlayers.Add(id);
            if (info.PreferedTeam == Team.Team1) playerSelectorData.Team1Players.Add(id);
            else if (info.PreferedTeam == Team.Team2) playerSelectorData.Team2Players.Add(id);
        }
        Debug.Log("Integrated with Player Selector!");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(playerSelectorData);
#endif
    }
#endif

    void SetupPlayerWeaponPack1()
    {
        if (playerPrefab == null) return;

        var integrationObject = AssetDatabase.LoadAssetAtPath("Assets/Addons/PlayerPacks/Pack[1]/Prefabs/Integration/WP1_TP_Positions.prefab", typeof(GameObject)) as GameObject;
        if (integrationObject == null)
        {
            Debug.LogWarning("The integration info couldn't be found!");
            return;
        }

        var integrationInfo = integrationObject.GetComponent<TPPlayerWeaponPositions>();
        integrationInfo.ApplyModificationsToPlayer(playerPrefab);
        Debug.Log("Weapon Pack weapons modified!");
    }

    [MenuItem("MFPS/Addons/Player Pack/Pack 1/Documentation")]
    [MenuItem("MFPS/Tutorials/Player Pack/Pack 1/Documentation")]
    static void Open()
    {
        GetWindow<PlayerPack1Documentation>();
    }
}