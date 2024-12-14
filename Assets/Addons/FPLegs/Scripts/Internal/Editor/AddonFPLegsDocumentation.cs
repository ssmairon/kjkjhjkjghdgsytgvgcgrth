using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFPSEditor;
using UnityEditor;

public class AddonFPLegsDocumentation : TutorialWizard
{
    //required//////////////////////////////////////////////////////
    private const string FolderPath = "mfps2/editor/fplegs/";
    private NetworkImages[] m_ServerImages = new NetworkImages[]
    {
        new NetworkImages{Name = "img-1.jpg", Image = null},
    };
    private Steps[] AllSteps = new Steps[] {
     new Steps { Name = "Integration", StepsLenght = 0, DrawFunctionName = nameof(IntegrationDoc) },
     new Steps { Name = "Adjust Legs", StepsLenght = 0, DrawFunctionName = nameof(AdjustLegsDoc) },
     new Steps { Name = "FP Shadow", StepsLenght = 0, DrawFunctionName = nameof(FPLegsDoc) },
     //new Steps { Name = "Re-Integration", StepsLenght = 0, DrawFunctionName = nameof(ReIntegrationDoc) },     
    };
    private readonly GifData[] AnimatedImages = new GifData[]
    {
        new GifData{ Path = "addpt3.gif" },
        new GifData{ Path = "addnewwindowfield.gif" },
    };
    public override void OnEnable()
    {
        base.OnEnable();
        base.Initizalized(m_ServerImages, AllSteps, FolderPath, AnimatedImages);
        allowTextSuggestions = true;
    }
    public override void WindowArea(int window)
    {
        AutoDrawWindows();
    }
    //final required////////////////////////////////////////////////

    public bl_PlayerReferences playerPrefab;
    
    /// <summary>
    /// 
    /// </summary>
    void IntegrationDoc()
    {
        DrawText("This addon doesn't require to be enabled but you have to set up all your player prefabs, but don't worry, this process is automatic, all you have to do is <b>drag and drop the player prefabs in the field below</b> and then click on the <b>Integrate</b> button to run the auto-integration.");
        GUILayout.Space(12);
        DrawNote("Drag the Player prefab from the <b>Project</b> view folder, not the Hierarchy window.");
        GUILayout.Space(12);
        playerPrefab = EditorGUILayout.ObjectField("Player Prefab", playerPrefab, typeof(bl_PlayerReferences), false) as bl_PlayerReferences;
        GUILayout.Space(10);
        GUI.enabled = playerPrefab != null;
        using (new CenteredScope())
        {
            if (GUILayout.Button("Integrate", MFPSEditorStyles.EditorSkin.customStyles[11]))
            {
                RunIntegration(playerPrefab.gameObject);
            }
        }
        GUI.enabled = true;
        Space(18);
        using (new CenteredScope())
        {
            GUILayout.Label("<i><size=11><color=#76767694>If the integration succeeds, you should see a log in the console confirming it.</color></size></i>", Style.TextStyle);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void AdjustLegsDoc()
    {
        DrawText("<b><size=16>Position adjustment</size></b>\n \nThe auto-integration of the player's legs automatically positions the legs in a way that normally looks realistic from the player's camera POV, but it may not be perfect it could look wrong with some models, for these cases, a manual adjustment of the legs position or rotation is required.\n \nTo adjust the position or rotation of the legs all you have to do is open your player prefab or drag it in the hierarchy window > go to <i>(inside the player prefab)</i> > <b>References</b> > <b>Legs</b>, select this object, and move/rotate to the desire position, you can even change the size it if needed.");
        DrawServerImage("img-0.png");
    }

    void FPLegsDoc()
    {
        DrawSuperText("When you integrate the legs in a player prefab by default it also integrates a first-person shadow feature which allows the local player to see their own player shadow, although this is a pretty neat feature to have from the visual aspect it cost some extra performance and <b>is recommended to not use for low-end platforms like mobile devices</b>.\n \nTo deactivate or activate the FP Shadow feature, you simply have to turn off/on the toggle <b>Draw FP Shadow</b> in the\n<?link=asset:Assets/Addons/FPLegs/Resources/FPLegsSettings.asset>FPLegsSettings</link>.");
    }

    void ReIntegrationDoc()
    {
        

    }
    
    private void RunIntegration(GameObject prefab)
    {
        // instance the player prefab using the editor api
        var playerInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        // unlink the player prefab from the source
        PrefabUtility.UnpackPrefabInstance(playerInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        // get the player script references
        var pReferences = playerInstance.GetComponent<bl_PlayerReferences>();

        var pla = playerInstance.GetComponentInChildren<bl_PlayerLegsActivator>(true);
        if(pla == null)
        {
            pReferences.playerSettings.RemoteObjects.AddComponent<bl_PlayerLegsActivator>();
        }

        var pl = playerInstance.GetComponentInChildren<bl_PlayerLegs>(true);
        
        // if the legs root is already integrated
        if(pl != null)
        {
            if (!SetupLegsModel(pl, pReferences)) return;

            string path2 = AssetDatabase.GetAssetPath(prefab);
            PrefabUtility.SaveAsPrefabAssetAndConnect(playerInstance, path2, InteractionMode.UserAction);
            DestroyImmediate(playerInstance);
            Debug.Log($"Legs has been integrated in the <b>{prefab.name}</b> player prefab!");
            return;
        }

        var legsRoot = new GameObject("Legs").transform;
        legsRoot.parent = pReferences.playerSettings.FlagPosition.parent;
        legsRoot.localPosition = Vector3.zero;
        legsRoot.localRotation = Quaternion.identity;
        
        pl = legsRoot.gameObject.AddComponent<bl_PlayerLegs>();
        SetupLegsModel(pl, pReferences);

        string path = AssetDatabase.GetAssetPath(prefab);
        PrefabUtility.SaveAsPrefabAssetAndConnect(playerInstance, path, InteractionMode.UserAction);
        DestroyImmediate(playerInstance);
        Debug.Log($"Legs has been integrated in the <b>{prefab.name}</b> player prefab!");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="legs"></param>
    public static bool SetupLegsModel(bl_PlayerLegs legs, bl_PlayerReferences playerReferences)
    {
        // check if the legs do already have a child
        var oldLeg = legs.GetComponentInChildren<Animator>(true);
        if(oldLeg != null && !oldLeg.name.Contains("(Old)"))
        {
            if(!EditorUtility.DisplayDialog("Confirm Override", "This player prefab does already have the FP Legs integrated, do you want to integrate again and override the last integration?", "Yes", "Cancel"))
            {
                return false;
            }
            oldLeg.name += " (Old)";
            oldLeg.gameObject.SetActive(false);
        }

        var sourcePlayer = playerReferences.PlayerAnimator.gameObject;
        var remotePlayerCopy = GameObject.Instantiate(sourcePlayer) as GameObject;
        remotePlayerCopy.name = remotePlayerCopy.name.Replace("(Clone)", " (Legs)");
        remotePlayerCopy.transform.parent = legs.transform;
        remotePlayerCopy.transform.position = sourcePlayer.transform.position;
        remotePlayerCopy.transform.rotation = sourcePlayer.transform.rotation;
        legs.Animator = remotePlayerCopy.GetComponent<Animator>();
        legs.SetupLegs();
        EditorUtility.SetDirty(legs);
        return true;
    }

    [MenuItem("MFPS/Tutorials/FP Legs")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AddonFPLegsDocumentation));
    }
}
