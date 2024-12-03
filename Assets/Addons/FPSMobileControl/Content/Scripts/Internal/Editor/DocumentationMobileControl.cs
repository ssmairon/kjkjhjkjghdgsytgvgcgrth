using UnityEngine;
using UnityEditor;
using MFPSEditor;

public class DocumentationMobileControl : TutorialWizard
{
    //required//////////////////////////////////////////////////////
    public string FolderPath = "mobile-control/editor/";
    public NetworkImages[] m_ServerImages = new NetworkImages[]
    {
        new NetworkImages{Name = "img-0.png", Image = null},
        new NetworkImages{Name = "img-1.png", Image = null},
        new NetworkImages{Name = "img-2.png", Image = null},
        new NetworkImages{Name = "img-3.png", Image = null},
        new NetworkImages{Name = "img-4.png", Image = null},
    };
    public Steps[] AllSteps = new Steps[] {
    new Steps { Name = "Integration", StepsLenght = 0 , DrawFunctionName = nameof(GetStartedDoc)},
    new Steps { Name = "Testing", StepsLenght = 0, DrawFunctionName = nameof(SecondSection) },
    new Steps { Name = "Settings", StepsLenght = 0, DrawFunctionName = nameof(SettingsDoc) },
    new Steps { Name = "Auto Fire", StepsLenght = 0, DrawFunctionName = nameof(AutoFireDoc) },
    };
    private readonly GifData[] AnimatedImages = new GifData[]
   {
        new GifData{ Path = "name.gif" },
   };

    public override void OnEnable()
    {
        base.OnEnable();
        base.Initizalized(m_ServerImages, AllSteps, FolderPath, AnimatedImages);
        Style.highlightColor = ("#c9f17c").ToUnityColor();
        allowTextSuggestions = true;
    }

    public override void WindowArea(int window)
    {
        AutoDrawWindows();
    }
    //final required////////////////////////////////////////////////

    void GetStartedDoc()
    {
        DrawText("<b><size=16>Integration</size></b>\n\n1. Enable the Mobile Control addon by clicking on the editor top navigation menu: <b>MFPS > Addons > MobileControl > Enable</b>, and then wait for the script compilation to finish.\n\n2. <b>In each map scene, run the auto-integration</b>: open the map scene in the editor > with the scene opened, click on the button below to run the auto-integration.");
        Space(10);
        using(new CenteredScope())
        {
            if (GUILayout.Button("Run Integration in this scene.", MFPSEditorStyles.EditorSkin.customStyles[11]))
            {
                MFPSMobileInitializer.Instegrate();
            }
        }
        DrawText("3. Done!");
        Space(50);
        DrawNote("If you experience an issue with the Mobile Control apparently being enabled, but the control doesn't respond, try clicking on the button below");
        Space(10);

        using (new CenteredScope())
        {
            if (GUILayout.Button("Force Enable", MFPSEditorStyles.EditorSkin.customStyles[11]))
            {
                EditorUtils.SetEnabled("MFPSM", true);
            }
        }

    }

    void SecondSection()
    {
        DrawSuperText("<b>In order to use the Mobile Control in the game, you have to play the game from a mobile device; for this, you have two options:</b>\n \n<b>1.</b> <b>Build the game</b> on the editor and install it on your mobile to open it on the mobile device.\n \n<b>2.</b> <b>Use Unity Remote</b>: Unity Remote is a tool that Unity provides for fast-testing mobile games; with this tool, you don't have to build the game each time you want to test something. Instead, you connect your mobile device to your computer via USB, and when you play the game in the editor, the game will also be playable on your connected mobile device.\n \nUnity Remote requires setting up a few things before being able to use, check the official documentation here:\n<?link=https://docs.unity3d.com/Manual/UnityRemote5.html>https://docs.unity3d.com/Manual/UnityRemote5.html</link>");
    }

    void SettingsDoc()
    {
        DrawText("You can find all the Mobile Control settings in the scriptable object located in: <i>Assets ➔ Addons ➔ FPSMobileControl ➔ Resources ➔ MobileControlSettings</i> , or by opening the MFPS Manager window <i>(Ctrl+M)</i> > <b>Mobile Control</b>.");
    }

    void AutoFireDoc()
    {
        DrawText("Mobile Control comes with an <b>Auto Fire</b> feature, which allows firing the weapon by just looking at the enemy without requiring to press the fire mobile button.\n \nThis feature is disabled by default and you can active it in <b>GameData > Auto Weapon Fire</b> toggle.\n\nYou can change the time delay before start auto firing after look at an enemy in the <b>MobileControlSettings > Auto Fire Delay</b>.");
    }

    [MenuItem("MFPS/Addons/MobileControl/Documentation")]
    [MenuItem("MFPS/Tutorials/Mobile Control")]
    static void Open()
    {
        GetWindow<DocumentationMobileControl>();
    }
}