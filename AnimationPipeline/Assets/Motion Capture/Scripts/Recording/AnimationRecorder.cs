using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimationRecorder : MonoBehaviour
{
    /*
    [Header("Save")]
    [SerializeField] string savePath = "Assets/NuitrackSDK/Tutorials/Motion Capture/Animations";
    [SerializeField] string fileName = "Example";

    [Header("Control")]
    [SerializeField] TPoseCalibration poseCalibration;
    [SerializeField] GameObject recordIcon;

    [SerializeField] bool isRecording = false;
    AnimRecorderInterface recordable = null;

    [Header("Generic Animations")]
    [SerializeField] Transform root;
    [SerializeField] Transform[] transforms;

    void Start()
    {
        poseCalibration.onSuccess += PoseCalibration_onSuccess;

        recordable = new GenericRecorder(transforms, root);
        
    }

    private void OnDestroy()
    {
        poseCalibration.onSuccess -= PoseCalibration_onSuccess;
    }

    private void PoseCalibration_onSuccess(Quaternion rotation)
    {
        if (!isRecording)
        {
            Debug.Log("Start recording");
            isRecording = true;
        }
        else
        {
            Debug.Log("Stop recording");
            isRecording = false;
            SaveToFile(recordable.GetClip);
        }

        recordIcon.SetActive(isRecording);
    }

    void Update()
    {
        if (isRecording) 
            recordable.TakeSnapshot(Time.deltaTime);
    }

    void SaveToFile(AnimationClip clip)
    {
        string path = savePath + "/" + fileName + ".anim";
        clip.name = fileName;

        AssetDatabase.CreateAsset(clip, path);
        Debug.Log("Save to: " + path);
    }
    */
}
