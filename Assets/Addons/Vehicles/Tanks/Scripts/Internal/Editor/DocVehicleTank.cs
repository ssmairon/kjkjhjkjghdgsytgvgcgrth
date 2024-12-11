using System.Collections.Generic;
using UnityEngine;
using MFPSEditor;
using UnityEditor;
using MFPS.Runtime.Vehicles;

public class DocVehicleTank : TutorialWizard
{
    //required//////////////////////////////////////////////////////
    private const string ImagesFolder = "mfps2/editor/vehicle/tank/";
    private NetworkImages[] m_ServerImages = new NetworkImages[]
    {
        new NetworkImages{Name = "img-0.png", Image = null},
        new NetworkImages{Name = "img-1.png", Image = null},
        new NetworkImages{Name = "img-2.png", Image = null},
        new NetworkImages{Name = "img-3.png", Image = null},
        new NetworkImages{Name = "img-4.png", Image = null},
        new NetworkImages{Name = "img-5.png", Image = null},
        new NetworkImages{Name = "img-6.png", Image = null},
        new NetworkImages{Name = "img-7.png", Image = null},
    };
    private readonly GifData[] AnimatedImages = new GifData[]
    {
        new GifData{ Path = "vtttrp.gif" },

    };
    private Steps[] AllSteps = new Steps[] {
     new Steps { Name = "Get Started", StepsLenght = 0, DrawFunctionName = nameof(GetStartedDoc) },
     new Steps { Name = "Add Tank", StepsLenght = 5, DrawFunctionName = nameof(AddVehicleDoc), SubStepsNames = new string[]
     { "Change model", "Change Tracks", "Change Wheels", "Tank Colliders", "Adjust Properties" } },
     new Steps { Name = "Interior Camera", StepsLenght = 0, DrawFunctionName = nameof(InteriorCameraDoc) },
     new Steps { Name = "Colliders", StepsLenght = 0, DrawFunctionName = nameof(CollidersDoc) },
    };

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
        FetchWebTutorials("mfps2/tutorials/tank/");
    }
    //final required////////////////////////////////////////////////

    void GetStartedDoc()
    {
        DrawText("■ To integrate, follow the integration wizard <i>(click the button below)</i>"); Space(10);
        if (DrawButton("Open integration wizard"))
        {
            GetWindow<VehicleTankIntegration>();
        }
    }

    void AddVehicleDoc()
    {
        if (subStep == 0)
        {
            DrawText("You may want to add multiple tank models in your game, that of course is possible to do, basically, all that you have to do is <b>use the default tank prefab ➔ change the tank model ➔ adjusts any of the tank properties to your needs ➔ create a prefab of it ➔ done.</b>\n\nSo, here go, step 1:");
            DrawTitleText("Use the Tank prefab");
            DrawText("<i><color=#76767694>For make things more clear, open a new empty scene in the editor.</color></i>\n\nIf this is the first time adding a tank model you can use the default Tank prefab which is located in: <i>Assets ➔ Addons ➔ Vehicles ➔ Tanks ➔ Prefabs ➔ Tank ➔ Tank [M1 Abrahams]</i>");
            if (Buttons.FlowButton("Ping Tank prefab"))
            {
                var carPrefab = AssetDatabase.LoadAssetAtPath("Assets/Addons/Vehicles/Tanks/Prefabs/Tank/Tank [M1 Abrahams].prefab", typeof(GameObject)) as GameObject;
                Selection.activeGameObject = carPrefab;
                EditorGUIUtility.PingObject(carPrefab);
            }

            DrawHorizontalSeparator();
            DrawTitleText("Change Model");
            DrawText("■ Drag the Tank prefab and drop it in your scene hierarchy.\n\n■ Select the instanced tank prefab in the hierarchy window ➔ right mouse click over it ➔ (Click) <b>Unpack prefab completely</b>.\n\n■ Then, drag you new tank model into the tank prefab that you just instance in the hierarchy, put the tank model under the <b>Model</b> child object of the Tank prefab.\n\n■ Positioned your new model using the current tank model as reference of the position and rotation.");
            DrawServerImage(0);
            DrawText("■ Once you positioned the new model correctly you can <b>DISABLE</b> the old tank model for the moment.");
            Space(20);

            DrawSuperText("<size=18><b>SETUP TURRET PIVOTS</b></size>\n<?line=1></line>\n\nYou have to create an empty game object and place it inside the new tank model, then you have to position this object in the turret position where the turret axis should be <i>(in the bottom center of the turret)</i>.\n \nOnce you positioned it correctly, you have to drag inside this object the turret mesh of your tank model.");

            DrawServerImage(1);

            DrawText("Next, you have to create the cannon pivot, same process ➔ create an empty object ➔ this time place it inside of the turret mesh and positioned it on the cannon axis from where the tank cannon will spin up and down.\n \nThen put inside this object the cannon mesh of your tank model:");
            DrawServerImage(2);

            DrawText("Once more, create another empty game object <i>(Fire Point)</i> ➔ put it inside of the cannon mesh ➔ positioned it at the end of the cannon barrel, this point is where the tank projectiles will be instanced from.\n \nThe last one, create another empty game object <i>(Look At)</i> ➔ place it inside of the cannon mesh ➔ position it in the center, and then distanced it 60 meters approx from the cannon origin<b>in a straight line with respect to the tank cannon direction.</b>");
            DrawServerImage(3);
            DrawText("Now assign all these objects in the inspector of bl_TankTurret.cs ➔ Select the root of the tank prefab instance in the hierarchy window ➔ bl_TankTurret ➔ assigned the create objects like this:");
            DrawServerImage(4);

            DrawText("Finally, move the <b>Fire Effect</b> from the old tank model and put inside the new tank model cannon mesh and positioned it correctly at the end of the cannon barrel.");
            DrawServerImage(5);
        }
        else if (subStep == 1)
        {
            DrawText("The tank tracks are the chains normally on both sides of the tank that propulsion the vehicle.\n \nIn order to change the tank tracks, you have to make sure that your tank model meets these requirements:\n \n<b>1. Both tracks (left and right) have separated meshes:</b>\nEach track has to have a MeshRender with a separated UV and is separated from the main tank body mesh.\n \n<b>2. Both tracks are rigged.</b>\nThis is optional if you don't mind the tracks not reacting to the surface height, but if you do care, then the track has to be rigged by adding a bone in the position of the chain where a wheel will be placed at:");
            DrawAnimatedImage(0);
            DrawSuperText("The first one is imperative that is met, since if the track meshes are not separated then you won't even be able to hide them and use other track mesh, the second one if your tracks are not rigged, use the addon example track mesh, simply positioned them correctly in your tank model and you are good to go.\n \n<b><size=18>SETUP THE TRACKS</size></b>\n<?line=1></line>\n \nIn the tank prefab instance in the hierarchy window ➔ Tracks ➔ you will find the default tank tracks meshes already set up, if you are going to use these instead of your tank model tracks, then leave them as is and continue with the next step, otherwise, delete them from the hierarchy to start setting up your new track meshes.");
            DrawServerImage(6);
            DrawText("Put both of your rigged track meshes in the <b>Track</b> object ➔ in each track mesh add the script <b>bl_TankTrack</b>.\n \nIn the inspector of <b>bl_TankTrack</b> ➔ <b>Wheels</b> list ➔ Add as many fields as wheels your tank will have, each field will contain information and references about a wheel, but for now we are only interested in setting up the track bones, so in each list fields you will assign one of the track bones, for it, unfold a field in the list ➔ drag one of the track bones ➔ assign it in the <b>Track Bone</b> field, do the same with all the track bones.");
            DrawServerImage(7);
            DrawNote("Rename your bones to identify easily later when you have to assign the wheels in the same bone order, e.g you can rename them numeric ascend 1,2,3,4, etc... from the first bone to the last.");
            DrawText("In the Scene view window, you will see a yellow sphere gizmo indicating the position of each track bone, this is where you will have to place the wheels, more on that in the next step, now that you have finished setting up one of the tracks, next is set up the other track, here you have two ways to do it, one is just doing the same process you did with the first track, the second is just duplicating the track you already set up and positioning it instead of the other track mesh.\n \nOnce you have both track bones set up, select the tank prefab instance root in the hierarchy window ➔ <b>bl_TankController</b> inspector ➔ in the <b>Left Track</b> and <b>Right Track</b> fields assign the bl_TankTrack of each track that you just set up:");
            DrawServerImage("img-8.png");

            DrawSuperText("<size=18><b>MODIFY TRACK PROPERTIES</b></size>\n <?line=1></line>\n\nIdentify the right side tank track\nSelect the <b>bl_TankTrack</b> that corresponds to the right side of the tank ➔ in the inspector window, check the <b>Is Right Track</b> Toggle.\n \n<?background=#CCCCCCFF>Track Material</background>\n\nThe tank system scroll the track material UV to achieve the track movement effect, because that is imperative that the track mesh have their separate UV from the rest of the tank mesh.\n \n<?background=#CCCCCCFF>Assign the Track Material</background>\n \nSelect one of the <b>bl_TankTrack</b> scripts in the tank prefab instance ➔ in the inspector window, foldout the <b>Caterpillar Motion</b> ➔ In the <b>Track Mesh</b> field assign the <b>MeshRenderer</b> of your track, then do the same with the other track.\n \nFor the rest of the properties in <b>bl_TankTrack</b>, the best to adjust is by playing with them in real time <i>(while you are playing the editor)</i> until you get the desired result.");
            DrawServerImage("img-14.png");

        }
        else if (subStep == 2)
        {
            DrawText("Time to change the tank wheels, by default the tank example included in the package uses the <b>Unity WheelCollider</b> to handle the tank movement and physics, but there're some tanks that don't have wheels just the tracks, if this is the case of your new tank model this is also supported and you won't need to set up the wheels, so just make sure to hide/delete the old tank model wheels, disable or remove the <b>WheelColliders</b> located inside of the prefab instance <i>(* ➔ Colliders ➔ Wheels)</i> and skip this step.\n \nAnother case could be where your tank model doesn't have the wheel meshes separated, in that case, you have two options:\n \n1. No use the wheel models, the wheel colliders will still work but your wheel meshes won't move/rotate.\n \n2. Use another wheel model, e.g the ones of the default tank model.\n \nBut if your tank has the wheels separated then you can continue.");
            Space(10);
            DrawSuperText("<b><size=18>SETTING UP THE WHEELS</size></b>\n<?line=1></line>\n \nWe will start by positioning the <b>WheelsColliders</b> in the track bone positions, the default tank prefab does already have a set of wheel colliders for each track, they are located inside of the tank prefab <b>(* ➔ Colliders ➔ Wheels ➔ Left/Right ➔ *)</b>, you can use these colliders and you need more > duplicate one of them, if there are more of what you need for a track > delete the remain.");
            DrawServerImage("img-9.png");
            DrawText("Positioned the wheel colliders where the wheel meshes suppose to be on the tank track, use the yellow sphere gizmos as the reference of where the track bone is, and positioned the WheelCollider center aligned.\n \nThis is how it should look like after you set up all the WheelColliders:");
            DrawServerImage("img-10.png");

            DrawText("Now if your tank model has all the wheel meshes already placed, you can just reposition them manually in the same position where the WheelsColliders are, put <b>one wheel mesh for each WheelCollider</b>.\n \nIf you are using a different wheel mesh and doesn't have positioned them already, then you can use the below tool to try to automatically place them in the WheelColliders, all you need is one of the wheel meshes, so if you have more than one wheel meshes instanced already ➔ delete them and leave just one instance and assign the wheel instance in the field below:");
            Space(20);

            wheelMeshTemplate = EditorGUILayout.ObjectField("Wheel", wheelMeshTemplate, typeof(GameObject), true) as GameObject;
            tankInstance = EditorGUILayout.ObjectField("Tank instance", tankInstance, typeof(bl_TankController), true) as bl_TankController;
            Space(10);
            using (new CenteredScope())
            {
                if (wheelMeshTemplate != null && tankInstance != null)
                {
                    if (Buttons.GlowButton("<color=black>Place wheels automatically</color>", Style.whiteColor, GUILayout.Height(30)))
                    {
                        var allWColliders = tankInstance.GetComponentsInChildren<WheelCollider>();
                        Transform parent = null;
                        for (int i = 0; i < allWColliders.Length; i++)
                        {
                            var go = Instantiate(wheelMeshTemplate) as GameObject;

                            Vector3 pos;
                            Quaternion rot;
                            allWColliders[i].GetWorldPose(out pos, out rot);
                            go.transform.position = pos;
                            go.transform.rotation = rot;

                            if (parent == null)
                            {
                                var goParent = new GameObject("Wheel Meshes");
                                goParent.transform.parent = wheelMeshTemplate.transform.parent;
                                goParent.transform.localPosition = Vector3.zero;
                                goParent.transform.localRotation = Quaternion.identity;
                                parent = goParent.transform;
                            }

                            go.transform.parent = parent;
                            go.name = $"Wheel [{allWColliders[i].name}]";
                        }

                        wheelMeshTemplate.SetActive(false);
                    }
                }
            }

            DownArrow();

            DrawText("Once the wheel meshes are placed correctly it should look like this:");
            DrawServerImage("img-11.png");

            DrawText("Now you have to assign the wheel meshes and collides in the <b>bl_TankTracks ➔ Wheels</b> list, just like you did with the track bones, you have to assign these in each field of the list but this time you have to make sure that assign the wheel mesh and WheelCollider in the field where you assign the bone that is for that wheel position.\n \nYou can try to assign these automatically by assigning the tank instance below and clicking <b>Assign wheels</b> button, this method is not perfect so you will have to make sure that the wheels are assigned in the correct field.");

            Space(10);
            tankInstance = EditorGUILayout.ObjectField("Tank instance", tankInstance, typeof(bl_TankController), true) as bl_TankController;
            wheelMeshParent = EditorGUILayout.ObjectField("Wheel Meshes Parent", wheelMeshParent, typeof(Transform), true) as Transform;
            if (tankInstance != null && wheelMeshParent != null)
            {
                Space(10);
                using (new CenteredScope())
                {
                    if (Buttons.GlowButton("<color=black>Place wheels automatically</color>", Style.whiteColor, GUILayout.Height(30)))
                    {
                        AssignWheelsReferences();
                    }
                }
            }
            DownArrow();
            DrawText("After assigned the WheelColliders and WheelMeshes it should look like this for both <i>(left and right side)</i> bl_TankTrack inspectors");
            DrawServerImage("img-12.png");

            DrawText("Then you just have to make sure that all the wheel references <i>(Track Bone, Wheel Mesh, and Wheel Collider)</i> are assigned correctly, you can do this by checking in the scene view if there're yellow lines that cross a wheel to another, e.g:");
            DrawServerImage("img-13.png");

            DrawText("If there are problems, you have to manually fix them by reassigning the WheelColliders or Wheel Meshes that are not correctly assigned.\n \nIf everything looks correct then you have finished replacing the tank model you can now delete the default thank model from the prefab instance.");
        }
        else if (subStep == 3)
        {
            DrawSuperText("<b><size=18>SET UP THE TANK COLLIDERS</size></b>\n <?line=1></line>\n\nNow that you have replaced the tank model, is time to define the tank colliders.\n \nIdeally, the best way to define the tank collider is using a very low poly mesh of the tank in order to use <b>MeshCollider</b>, so if you have a low-poly version or you can make it then you should use that method, if you don't have and can't create a low-poly mesh, then you have another option which wraps the tank with primitive collider shapes <i>(Box, Capsules or Spheres Colliders).</i>\n \nThe target is to fit the colliders as accurate to the tank mesh as possible, you can add as many colliders as you need, you just need to make sure to cover the tank body, turret, and cannon, you don't have to add colliders for the track and wheels.\n \nHere are some examples of correct collider wraps:");
            DrawServerImage("img-15.png");

            DrawText("Once you have added all the required colliders, you have to set up some things <b>in each of them</b>:\n \n1. Add the <b>bl_VehicleHitBox.cs</b> script\nIn each collider add this script and in the inspector of the script setup the damage multiplier for that collider.\n \n2. Change the Tag to <b>Metal</b> and the Layer to <b>Vehicle</b>");
            DrawServerImage("img-16.png");
        }
        else if (subStep == 4)
        {
            DrawSuperText("<b><size=18>ADJUST THE TANK PROPERTIES</size></b>\n<?line=1></line>\n \nNow that you have finished setting up the tank model, you have to adjust the tank properties, this includes fixing possible unexpected movements or just tweaking values to reach the desired tank behavior.\n \n<?background=#CCCCCCFF>Tank Movement</background>\n \nYou can adjust all the tank movement-related properties in the <b>bl_TankController</b> script inspector, you should play with these values until you get the desired result, but there is one that you have to pay attention to, it is the <b>Center Of Mass</b> Vector3 field, you have to set this to where the tank center of mass should be, you can do this by checking the magenta sphere gizmo is located in the scene view, modify the <b>Center Of Mass</b> values until the magenta sphere gizmo is located where you want.");
            DrawServerImage("img-17.png");
            DrawText("In case your tank model doesn't use WheelColliders, in bl_TankController inspector ➔ turn off the <b>Real Wheels</b> toggle.");
            Space(15);
            DrawSuperText("<?background=#CCCCCCFF>Turret Properties</background>\n \nAll the Turret properties are exposed in <b>bl_TankTurret</b> inspector, most of the fields are self-explanatory in what they are for so you can adjust the values until you get the desired result.\n \nThere could be the case where your tank model has a different up axis vector direction which would cause the turret to rotate in the wrong axis, to fix this change the Up Axis value in bl_TankTurret, if it is set to <b>Z Is Up</b> > Change to <b>Y Is Up</b> or viseversa.\n \nIf the Turret is aiming backward instead of forwarding to the camera direction > change the <b>Invert Orientation</b> Toggle.");
            Space(15);
            DrawSuperText("<?background=#CCCCCCFF>Cannon Properties</background>\n \nAll the tank cannon properties are exposed in the script <b>bl_TankCannon</b>, pretty much all properties are self-explanatory and you can adjust as needed.");
            DrawPropertieInfo("Fire Target Mode", "enum", "This defines the ballistic of the tank projectiles, <b>Cannon Target</b> means that the projectile will follow the actual tank cannon direction, <b>Camera Target</b> means that the projectile will try to hit where the center of the camera is looking at.");
        }
    }

    public GameObject wheelMeshTemplate;
    public Transform wheelMeshParent;
    public bl_TankController tankInstance;
    private void AssignWheelsReferences()
    {
        var allWColliders = tankInstance.GetComponentsInChildren<WheelCollider>();
        var allWMeshes = wheelMeshParent.GetComponentsInChildren<MeshRenderer>();

        //Colliders
        for (int i = 0; i < allWColliders.Length; i++)
        {
            var wc = allWColliders[i];

            float d = float.MaxValue;
            int currentSelection = 0;
            bool selectionFromLeft = true;
            //Check Left tracks
            for (int e = 0; e < tankInstance.leftTrack.Wheels.Length; e++)
            {
                var trackInfo = tankInstance.leftTrack.Wheels[e];
                if (trackInfo.TrackBone == null) continue;

                float td = Vector3.Distance(wc.transform.position, trackInfo.TrackBone.position);
                if (td < d)
                {
                    d = td;
                    currentSelection = e;
                }
            }

            //Check Right tracks
            for (int e = 0; e < tankInstance.rightTrack.Wheels.Length; e++)
            {
                var trackInfo = tankInstance.rightTrack.Wheels[e];
                if (trackInfo.TrackBone == null) continue;

                float td = Vector3.Distance(wc.transform.position, trackInfo.TrackBone.position);
                if (td < d)
                {
                    d = td;
                    currentSelection = e;
                    selectionFromLeft = false;
                }
            }

            if (selectionFromLeft) tankInstance.leftTrack.Wheels[currentSelection].WheelCollider = wc;
            else tankInstance.rightTrack.Wheels[currentSelection].WheelModel = wc.transform;
        }

        // Meshes
        for (int i = 0; i < allWMeshes.Length; i++)
        {
            var wc = allWMeshes[i];

            float d = float.MaxValue;
            int currentSelection = 0;
            bool selectionFromLeft = true;
            //Check Left tracks
            for (int e = 0; e < tankInstance.leftTrack.Wheels.Length; e++)
            {
                var trackInfo = tankInstance.leftTrack.Wheels[e];
                if (trackInfo.TrackBone == null) continue;

                float td = Vector3.Distance(wc.transform.position, trackInfo.TrackBone.position);
                if (td < d)
                {
                    d = td;
                    currentSelection = e;
                }
            }

            //Check Right tracks
            for (int e = 0; e < tankInstance.rightTrack.Wheels.Length; e++)
            {
                var trackInfo = tankInstance.rightTrack.Wheels[e];
                if (trackInfo.TrackBone == null) continue;

                float td = Vector3.Distance(wc.transform.position, trackInfo.TrackBone.position);
                if (td < d)
                {
                    d = td;
                    currentSelection = e;
                    selectionFromLeft = false;
                }
            }

            if (selectionFromLeft) tankInstance.leftTrack.Wheels[currentSelection].WheelModel = wc.transform;
            else tankInstance.rightTrack.Wheels[currentSelection].WheelModel = wc.transform;
        }

        EditorUtility.SetDirty(tankInstance.leftTrack);
        EditorUtility.SetDirty(tankInstance.rightTrack);
        EditorUtility.SetDirty(tankInstance);
    }

    void InteriorCameraDoc()
    {
        DrawText("You can change the tank camera view in-game by pressing <i>(by Default)</i> the <b>P</b> key, which will switch between the Outside and Interior tank camera.\n \nFor your custom tanks, after you have added the model following the <b>Add Tank</b> tutorial, you have to set up the Interior Camera, which is quite simple, all you have to do is:\n \n1. Move the <b>InteriorCamera [Template]</b> object located inside the Tank Prefab ➔ Camera Rig ➔ <b>Interior Camera [Template]</b>, move it inside the tank model cannon.");
        DrawServerImage("img-18.png");
        DrawText("2. Once placed as a child of the tank model cannon mesh ➔ Active the object <i>(the interior camera object)</i> in the hierarchy so you can see how the camera looks and then proceed to reposition the camera to the desired position simulating that is the view from the tank interior.");
        DrawServerImage("img-19.png");
        DrawText("3. Assign the InteriorCamera [Template] in the tank prefab root ➔ bl_TankTurret ➔ <b>Interior Camera</b>.");
        DrawNote("Disable the interior camera object by default so it doesn't render on top of other camera in the Editor.");
    }

    void PassengerDoc()
    {
        DrawText("The <b>Passenger</b> seats are slots that can be used by any driver-team mate to enter/exit from the vehicle, you can set up as many passenger seats as you want.\n \nFor Add, Remove, or Modify any passenger seat you simply need to Add, Remove or Modify the seat info in the <b>Seats</b> list of <b>bl_VehiclePassenger.cs</b>, this script is attached in the root of the vehicle prefab instance.\n \nSo if you wanna add a new seat ➔ add a new field in the list, if you wanna remove one ➔ remove it from the list, if you wanna modify it ➔ fold out the seat info and make the pertinent changes.\n \nYou can preview the passenger seats in the Scene View with the <b>blue seated player gizmos</b>.");
        DrawServerImage(5);
    }

    void CollidersDoc()
    {
        DrawText("There are other colliders that may need to be set up, besides the colliders that you set up in the Add Tank guide, these other colliders have other purposes and you may require to modify the size or position of them in your custom tank models.\n \nThese colliders are located inside the tank prefabs ➔ <b>Colliders ➔ Triggers ➔ *</b>, here is their purposes of them");
        DrawText("<b><size=16>Detection Area</size></b>\n \nThis is a Trigger collider that detects the local player, when the local player enters this trigger the player camera will start detecting the tank colliders with the camera rays.\n \nYou have to make sure that this collider bound has enough space from the tank mesh.\n \n<b><size=16>Enter Trigger</size></b>\n \nThis collider detects when the player is near and looking at it and lets the player know if can enter the tank.\n \nYou have to make sure that this collider fits the tank mesh but watch out to not touch the tank WheelColliders with this collider since that could cause some weird movement in the tank when nobody is using it.\n \n<b><size=16>AI Collider</size></b>\n \nThis collider makes the bots detect the tank as an obstacle so they can calculate paths taking into account the tank even when it is moving.\n \nYou have to make sure this collider fits the tank mesh.");
    }

    [MenuItem("MFPS/Tutorials/Vehicles/Tank")]
    private static void Open()
    {
        EditorWindow.GetWindow(typeof(DocVehicleTank));
    }

    [MenuItem("MFPS/Addons/Vehicle/Tank/Documentation")]
    private static void Open2()
    {
        EditorWindow.GetWindow(typeof(DocVehicleTank));
    }
}