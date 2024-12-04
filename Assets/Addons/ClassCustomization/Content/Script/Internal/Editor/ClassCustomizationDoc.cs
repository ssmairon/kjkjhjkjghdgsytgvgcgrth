using System.Collections.Generic;
using UnityEngine;
using MFPSEditor;
using UnityEditor;
using MFPS.Addon.ClassCustomization;

public class ClassCustomizationDoc : TutorialWizard
{
    //required//////////////////////////////////////////////////////
    private const string ImagesFolder = "mfps2/editor/class-customizer/";
    private NetworkImages[] m_ServerImages = new NetworkImages[]
    {
        new NetworkImages{Name = "img-0.png", Image = null},
        new NetworkImages{Name = "img-1.png", Image = null},
    };
    private readonly GifData[] AnimatedImages = new GifData[]
    {
        new GifData{ Path = "none.gif" },

    };
    private Steps[] AllSteps = new Steps[] {
     new Steps { Name = "Get Started", StepsLenght = 0, DrawFunctionName = nameof(GetStartedDoc) },
     new Steps { Name = "Add Weapons", StepsLenght = 0, DrawFunctionName = nameof(AddWeaponDoc) },
     new Steps { Name = "Default Loadouts", StepsLenght = 0, DrawFunctionName = nameof(DefaultLoadoutsDoc) },
     new Steps { Name = "Soldier Model", StepsLenght = 0, DrawFunctionName = nameof(SoldierModelDoc) },
    };

    private bl_PlayerReferences tempPlayer;

    public override void WindowArea(int window)
    {
        AutoDrawWindows();
    }

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
    }
    //final required////////////////////////////////////////////////

    void GetStartedDoc()
    {
        DrawText("<b>Require:</b>\n\nMFPS 1.8++\nUnity 2018.4++\n\n■ To integrate with MFPS simply enable the addon in: <i><b>MFPS ➔ Addons ➔ ClassCustomizer ➔ Enable</b></i>\n\n■ Add the 'ClassCustomizer' scene in the Build Settings, the scene is located at: <i>Assets ➔ Addons ➔ ClassCustomization ➔ Content ➔ Scene ➔ *</i>");
    }

    void AddWeaponDoc()
    {
        DrawText("<b>Add New Weapons:</b>\n \n■  Open <b>ClassCustomizer</b> Scene.\n\n■  In the GameObject called <b>ClassManager</b> in the inspector of the script <b>bl_ClassCustomize</b> you have 4 list <i>(AssaultWeapons, EngineerWeapons, SupportWeapons and ReconWeapons)</i> one for each player class, in order to define which weapons will be available for each class simple click on the \"Plus\" or \"Minus\" button at the right side of each weapon field, darker highlighted weapons means that they are not available, which means that this weapon wouldn't appear in the list for this player class.\n \n  If you don't see some weapon in the lists, Click on the button \"<b>Update</b>\" (on the inspector of bl_ClassCustomize)");
        DrawServerImage(0);
    }

    void DefaultLoadoutsDoc()
    {
        DrawHyperlinkText("To change the default weapon load-outs: primary, secondary, Perk, and Letal weapon slot of each player class, you simply need to set them in the Loadouts in <link=asset:Assets/Addons/ClassCustomization/Resources/ClassManager.asset>ClassManager</link> ➔ Assault, Recon, etc...");
        DrawServerImage(1);
        DownArrow();
        DrawText("<b>NOTE:</b> Once you change the weapons through the customizer menu in-game, the default weapons will no longer be fetched from these default load-outs setups, instead they will be loaded from PlayerPrefs <i>(or Database if you're using ULogin Pro)</i>.\n \nIf you wanna delete the saved load-outs and fetch the default weapons again from these setups, click on the button below:");
        if(Buttons.FlowButton("Delete saved loadouts"))
        {
            bl_ClassManager.Instance.DeleteKeys();
        }
    }

    void SoldierModelDoc()
    {
        DrawText("Since version 1.7, the preview soldier <i>(the player model that appears in the class customization)</i> shows the last selected weapon in the player's hands.\n \nBut now is required that when you add a new weapon you also have to update this player model but don't worry, you don't have to do it manually, you can automatically updated it using the box below:\n \n<b>What do you need?</b>\n\n1. Have the <b>ClassCustomizer</b> scene opened.\n\n2. Drag the <b>MPlayer</b> prefab from where you wanna transfer the player model and their weapons.");
        DownArrow();
        Space(10);
        var soldierPreview = GameObject.FindObjectOfType<bl_SoldierPreview>();
        if (soldierPreview != null)
        {
            tempPlayer = EditorGUILayout.ObjectField("Player Prefab", tempPlayer, typeof(bl_PlayerReferences), false) as bl_PlayerReferences;
            Space(10);
            using (new MFPSEditorStyles.CenteredScope())
            {
                GUI.enabled = bl_ClassCustomize.Instance != null && tempPlayer != null;
                if (Buttons.FlowButton("Update Soldier Preview"))
                {
                    UpdateSoldier();
                }
                GUI.enabled = true;
            }
        }
        else
        {
            DrawText("Soldier preview can't be automatically update if there's not an Soldier Preview already in the ClassCustomizer scene.");
        }
    }

    void UpdateSoldier()
    {
        if (tempPlayer == null) return;

        var soldierPreview = GameObject.FindObjectOfType<bl_SoldierPreview>();
        if (soldierPreview == null) return;


        if (soldierPreview.soldierReference != null) DestroyImmediate(soldierPreview.soldierReference.gameObject);
        soldierPreview.weapons = new List<bl_SoldierPreviewWeapons>();

        var newPlayer = Instantiate(tempPlayer.gameObject) as GameObject;

        soldierPreview.soldierReference = newPlayer.transform;
        soldierPreview.soldierReference.parent = soldierPreview.transform;
        soldierPreview.soldierReference.localPosition = Vector3.zero;
        soldierPreview.soldierReference.localEulerAngles = Vector3.zero;

        soldierPreview.soldierReference = newPlayer.GetComponent<bl_PlayerSettings>().RemoteObjects.transform;
        soldierPreview.soldierReference.parent = soldierPreview.transform;
        soldierPreview.playerAnimator = soldierPreview.soldierReference.GetComponentInChildren<Animator>();
        var spa = soldierPreview.playerAnimator.gameObject.AddComponent<bl_SoldierPreviewAnimation>();
#if UNITY_EDITOR
        EditorUtility.SetDirty(spa);
        EditorUtility.SetDirty(soldierPreview.playerAnimator.gameObject);
#endif
        soldierPreview.FetchNetworkWeapons(soldierPreview.soldierReference);

        var cj = soldierPreview.soldierReference.GetComponentsInChildren<CharacterJoint>(true);
        for (int i = 0; i < cj.Length; i++)
            DestroyImmediate(cj[i]);

        var rig = soldierPreview.soldierReference.GetComponentsInChildren<Rigidbody>(true);
        for (int i = 0; i < rig.Length; i++)
            DestroyImmediate(rig[i]);

        var monos = soldierPreview.soldierReference.GetComponentsInChildren<MonoBehaviour>(true);
        for (int i = 0; i < monos.Length; i++)
        {
            if (monos[i] is bl_SoldierPreviewWeapons || monos[i] is bl_SoldierPreview || monos[i] is bl_SoldierPreviewAnimation) continue;
#if CUSTOMIZER
            if (monos[i] is bl_CustomizerWeapon) continue;
#endif
            
            DestroyImmediate(monos[i]);
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(soldierPreview.soldierReference);
#endif
        DestroyImmediate(newPlayer);
    }

    [MenuItem("MFPS/Tutorials/Class Customizer")]
    private static void Open()
    {
        EditorWindow.GetWindow(typeof(ClassCustomizationDoc));
    }

    [MenuItem("MFPS/Addons/ClassCustomizer/Documentation")]
    private static void Open2()
    {
        EditorWindow.GetWindow(typeof(ClassCustomizationDoc));
    }
}