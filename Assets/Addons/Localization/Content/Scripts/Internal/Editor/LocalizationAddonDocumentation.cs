using UnityEngine;
using UnityEditor;
using MFPSEditor;

public class LocalizationAddonDocumentation : TutorialWizard
{
    //required//////////////////////////////////////////////////////
    public string FolderPath = "mfps2/editor/localization/";
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
    new Steps { Name = "Usage", StepsLenght = 0, DrawFunctionName = nameof(SecondSection) },
    new Steps { Name = "Add new localized text", StepsLenght = 3, DrawFunctionName = nameof(AddLocalizedTextDoc)
    , SubStepsNames = new string[] { "Add new text", "Add static text", "Add dynamic text" } },
    new Steps { Name = "Define Translation", StepsLenght = 0, DrawFunctionName = nameof(DefineTranslationDoc) },
    new Steps { Name = "Export/Import from CSV", StepsLenght = 3, DrawFunctionName = nameof(ImportFromCSVDoc),
    SubStepsNames = new string[] { "CSV File" , "Export to CSV", "Import CSV" } },
    };
    private readonly GifData[] AnimatedImages = new GifData[]
   {
        new GifData{ Path = "name.gif" },
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

    void GetStartedDoc()
    {
        DrawText("To integrate this addon, follow the integration wizard.\nOpen the integration wizard with the button below and follow the indications in the new window.");
        Space(10);
        if(GUILayout.Button("Integration Wizard"))
        {
            GetWindow<LocalizationAddonIntegration>();
        }
    }

    void SecondSection()
    {
        DrawText("After you integrate this addon in your MFPS project, you will be able to change the language in-game through the game settings menu in the lobby in the room menu under the <b>General</b> tab, there you should see a language dropdown with which you can change the current game language.");
    }

    void AddLocalizedTextDoc()
    {
        if (subStep == 0)
        {
            DrawText("There are two different text in the game that you can localized, the static text and dynamic text:\n \n<b>The Static text</b> is the text that is defined in a game UI text component directly and that text never change during the game.\n \n<b>The Dynamic text</b> is all text that changed during the game or is defined in a script parameter.\n \nLocalize those two text have a different solution which you can see in the following sections");
        }
        else if (subStep == 1)
        {
            DrawText("<b><size=16>Static Text</size></b>\n\nTo localize a new static text you have to attach the script <b>bl_LocalizationUI</b> in the same object where the Text or TextMeshProUGUI component is attached > then you set up the locale text properties in the inspector of this script\n \n<b>1.</b> Click in the Add New Text button.\n\n<b>2.</b> In the Key input field, set a unique short key to represent this text, this can be anything and don't need to be something that you have to remember, it just have to be unique.\n\n<b>3.</b> Set the Default Text which is the same text that is defined in the Text / TextMeshProUGUI component inspector.\n\n<b>4.</b> Add click in the Add Text Button.\n \n- That's for now, check the <i>Define Translation</i> section to see how you assign the localized text for this text in each language you are going to support.");
            DrawServerImage("img-0.png");
            DrawText("- If you are adding a text which is already localized but the is in a different text component, simply search the localized text by it's key by clicking the <b>Find</b> button in the inspector of <b>bl_LocalizationUI</b>");
        }
        else if (subStep == 2)
        {
            DrawText("<b><size=16>Dynamic Text</size></b>\n\nThe first thing you have to do for localize a dynamic text is to add the text to the localize text data if you haven't localized this text before, for this, open the <b>Localization</b> editor window in the editor top navigation menu > <b>MFPS > Addons > Localization > Editor</b>\n\n- In the opened window, you will see a list with the localized text, what you have to do is add your new text, for that click the button <b>ADD NEW</b> located in the left side below the last field of the list.\n\nAfter you click that button a new field will be added with empty input fields, at the moment simply assign a Key which is the first input field, the Key doesn't have to be something that you have to remember but it have to be unique; then just add the default language text and click in the check button (✔) to save the text and with that you have the first part.");
            DrawServerImage("img-1.png");
            DrawText("Since dynamic text is changed by code, you have to add a few line of codes to localize the text.\n \nI'm going to use a pratical and common case where we change the text in a UI text component by script:");
            Space(10);
            DrawCodeText("public class ExampleClass : MonoBehaviour\n{\n    public TextMeshProUGUI myGameText;\n \n    void ChangeText()\n    {\n        myGameText.text = \"Hello World\";\n    }\n}|");
            DrawText("On this simple example we assign the text to UI Text component whenever the ChangeText() function is called, to localized this text with our localized text we simply add add extension function to the text: <b>.Localized(\"MY_TEXT_KEY\");</b>\nwhere <i>'MY_TEXT_KEY'</i> is the key you assign for your localize text that you added before, in this example, the key I assign was <b>'helloworld'</b> so I'll use that, but you have to use the one you assigned or correspond to the target text, this is how the code from this example will look like:");
            DrawCodeText("public class ExampleClass : MonoBehaviour\n{\n    public TextMeshProUGUI myGameText;\n \n    void ChangeText()\n    {\n        myGameText.text = \"Hello World\".Localized(\"helloworld\");\n    }\n}");
            DrawText("And that is, now our text will be localized, now you simply have to add the translations for the added text, for that check the next section.");
        }
    }

    void DefineTranslationDoc()
    {
        DrawText("After you added a new text to localize, you did added the default language only, now you have to add the translation of that text to the languages you are supporting in your game.\n \nHow you translate this text depends on you, you can use a simple translation tool like <i>Google Translate</i> or if you are more sirious, you can contract a native of the language you want to translate to do it for you.\n \nOnce you got the translation, all you have to do is assigned in the text input field for the corresponding language data:\n \n1. Open the Localization Editor window in the editor top navbigation menu > <b>MFPS > Addons > Localization > Editor</b>.\n \n2. Find the original text in the list.\n \n3.Assign the translated text in corresponding input field in the same row of the original text");
        DrawServerImage("img-2.png");
        DrawText("4. Do the same with the rest of the languages and that's");
    }

    void ImportFromCSVDoc()
    {
        if (subStep == 0)
        {
            DrawSuperText("<b><size=16>Export or Import to CSV</size></b>\n\nThis functionality enables you to import or export language translations using a CSV document. This feature becomes particularly handy when you need to delegate the translation task to someone else. Here is how it works:\n \n<?list=•>Export the text that needs to be translated into a CSV document. This universally compatible format ensures that the recipient can open and edit the document on virtually any device.\n \nSend the CSV document to the individual responsible for the translation. They can then add their translations directly into the document.\n \nOnce the translation is complete, they can return the document to you.\n \nNow, you can import the translated text from the CSV document back into your game project.</list>\n \nBy using this workflow, you can easily coordinate language translation efforts, making the process seamless and efficient regardless of device compatibility.");
            DrawText("The exported CSV file will comprise all the text utilized within the game, neatly arranged in pairs for easy reference. On the left column, you'll find the game's text in the default language. Correspondingly, the translated version of this text should be populated in the right column. This layout facilitates a clear comparison between the original and translated text");
            DrawServerImage("img-3.png");
        }
        else if (subStep == 1)
        {
            DrawText("To export a language to CSV all you have to do is open the Localization Editor Window on the editor top navigation menu > <b>MFPS > Addons > Localization > Editor.</b>\n \nIn the editor window > select the language that you want to export and next to the header name of language you will see the button <b>CSV</b>, click it and a new window should appear, in the new window click on the '<b>Export Language To CSV File</b>' > select the folder where you want to save the .csv file and that's.");
            DrawServerImage("img-4.png");
        }
        else if(subStep == 2)
        {
            DrawText("To import a language from a .csv file > open the Localization editor window on the editor top navigation menu > <b>MFPS > Addons > Localization > Editor</b>, in the editor window > select the language for which you want to import and next to the header name you will see the <b>CSV</b> button, click it and in the new window > click in the button '<b>Import CSV file to Language</b>' > a dialog will open to select the .csv file, once selected > another dialog will open to select the folder to save the language file, you have to select a folder inside your Unity project and that's.");
            DrawSuperText("Once saved, you will find a new scriptableobject in the folder that you selected, that scriptableobject is the translated text from your .csv file, if you want to use this language, simply assign it in the <?link=asset:Assets/Addons/Localization/Resources/Localization.asset>Localization</link> data > Language list, if you already have that language in that list > replace it.");
        }
    }

    [MenuItem("MFPS/Addons/Localization/Documentation")]
    [MenuItem("MFPS/Tutorials/Localization")]
    static void Open()
    {
        GetWindow<LocalizationAddonDocumentation>();
    }
}