using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Editor : EditorWindow
{
    
    Vector2 scrollPos;


    

    float turn;

    // public static CameraTurntable CT;
    //public Object turn;
    //DropDown shader
    int shaderSelected = 0;
    string[] shaderOptions = new string[]
    {
        "Option 1", "Option 2", "Option 3",
    };

    //Motion Capture Settings

    bool tposeRec = false;
    string savePath = "Assets/Motion Capture/Animations";
    string fileName = "Example";

    float yrot = 0.0f;
    float xrot = 0.0f;
    //GameObject CameraRotator = GameObject.Find("Camera Rotation");
    //resolution select

    int resWidth = 1920;
    int resHeight = 1080;
    int frameRate = 10;
    string spriteSavePath = "Assets/Motion Capture/Animations";
    string spritefileName = "Example";

    //Animation selection

    string animPath = "";
    string localAnimPath;
    bool loop;

    //shaders
    Object shaderScript;


    [MenuItem("Window/AnimationPipeline")]

    public static void ShowWindow()
    {
        GetWindow<Editor>("AnimationPipeline");
    }

    private void OnEnable()
    {
        // Link the SerializedProperty to the variable 
    }

    void OnGUI()
    {
        //Window code
     
        //scrolling
        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.ExpandHeight(true));

        /*Motion Capture
         * Character Model
         * Settings
         * -invert
         * -Smoothing
         * -angle slider
         * Button record
         * button stop
        */
        
        GUILayout.Label("Motion Capture", EditorStyles.boldLabel);

        GUILayout.Space(10);

        //Calling the currently running version of Character Manager
        GameObject CharacterManager = GameObject.Find("CharacterManager").gameObject;
        CharacterTrackingBehaviour characterManager = CharacterManager.GetComponent<CharacterTrackingBehaviour>();
        AnimatorController animatorController = CharacterManager.GetComponent<AnimatorController>();

        //Calling Camera turntable Script
        GameObject CameraRotation = GameObject.Find("CameraRotation").gameObject;
        CameraTurntable cameraTurntable = CameraRotation.GetComponent<CameraTurntable>();

        //Calling Frame Capture Manager
        GameObject FrameCaptureManager = GameObject.Find("FrameCaptureManager").gameObject;
        Capture framCaptureManager = FrameCaptureManager.GetComponent<Capture>();

        

        if (GUILayout.Button("Start Motion Capture"))
        {

            if (animatorController.animChar)
            {
                Destroy(animatorController.animChar);
            }

            characterManager.Avatar.SetActive(true);
           // NuitrackManager.SetActive(true);
        }

        if (GUILayout.Button("Stop Motion Capture"))
        {
            //Nuitrack manager
            GameObject NuitrackManager = GameObject.Find("NuitrackManager").gameObject;

            NuitrackManager.SetActive(false);
        }

        //Text field for Animation clip save path
        savePath = EditorGUILayout.TextField("Save Path", savePath);
        characterManager.savePath = savePath;

        //Text field for Animation clip name
        fileName = EditorGUILayout.TextField("File Name", fileName);
        characterManager.fileName= fileName;

        //Toggle for recording start/stop by t-pose calibration
        tposeRec = EditorGUILayout.Toggle("T-Pose (to start/stop Recording)", tposeRec);
        characterManager.tposeRec = tposeRec;

        //Sliders for Rotation of view
        yrot = EditorGUILayout.Slider("Y Rotation", yrot, 0, 360);
        xrot = EditorGUILayout.Slider("X Rotation", xrot, 0, 90);
        cameraTurntable.yrot = yrot;
        cameraTurntable.xrot = xrot;



        GUILayout.Space(20);

        if (GUILayout.Button("Record"))
        {
            characterManager.StartRec();
        }

        if (GUILayout.Button("Stop"))
        {
            characterManager.StopRec();
        }

        GUILayout.Space(30);

        /* Animation Replay
         * 
         * 
         */
        GUILayout.Label("Animation Replay", EditorStyles.boldLabel);

        GUILayout.Space(10);

        if (GUILayout.Button("Select Animation"))
        {
            string path = EditorUtility.OpenFilePanel("Choose Animation", "", "anim");

            if (path.Length == 0)
            {
                EditorUtility.DisplayDialog("Select Animation", "You must select an Animation first!", "OK");
                return;

            }
            else
            {
                animPath = path;
            }
        }

       if (animPath.StartsWith(Application.dataPath))
        {
            localAnimPath = "Assets" + animPath.Substring(Application.dataPath.Length);
       }
        Debug.Log(localAnimPath);

        localAnimPath = EditorGUILayout.TextField("Animation Path", localAnimPath);
        
        //Toggle for looping
        loop = EditorGUILayout.Toggle("Loop Animation", loop);
        animatorController.loop = loop;

        GUILayout.Space(20);

        if (GUILayout.Button("Set Animation"))
        {
            animatorController.SetAnimation(localAnimPath);
        }


        GUILayout.Space(30);


        /*Style
         * Shader dropdown
         * Create new shader
        
        GUILayout.Label("Style", EditorStyles.boldLabel);

        GUILayout.Space(10);

        shaderSelected = EditorGUILayout.Popup("Example Shaders", shaderSelected, shaderOptions);

        shaderScript = EditorGUILayout.ObjectField("Add Custom Shader", shaderScript, typeof(Object), true);

        GUILayout.Space(30);

        /*Generate
         * FrameRate
         * Resolution
         * Button Generate
        */
        
        GUILayout.Label("Frame Capturer", EditorStyles.boldLabel);

        GUILayout.Space(10);


        //Text field for sprite file name
        //spritefileName = EditorGUILayout.TextField("File Name", spritefileName);
     

        GUILayout.Space(20);

        if (GUILayout.Button("Generate"))
        {
            Debug.Log("Generated");
            framCaptureManager.record = true;
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
}
