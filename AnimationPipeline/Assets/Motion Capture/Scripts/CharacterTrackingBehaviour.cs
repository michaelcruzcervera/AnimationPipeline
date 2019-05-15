using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterTrackingBehaviour : MonoBehaviour
{
    //Variables

    [Header("Save")]
    [SerializeField] public string savePath = "Assets/NuitrackSDK/Tutorials/Motion Capture/Animations";
    [SerializeField] public string fileName = "Example";


    [Header("Avatar to Animate")]
    public GameObject Avatar;

    public List<GameObject> tempJoints;

    [HeaderAttribute("Rigged model")]
    [SerializeField]
    ModelJoint[] Joints;
    //public bool Invert;
    public bool tposeRec = false;

    [Header("Recording Controls")]
    [SerializeField] TPoseCalibration poseCalibration;
    [SerializeField] GameObject recordIcon;

    bool isRecording = false;
    
    [Header("Generic Animation")]
    [SerializeField] Transform root;
    [SerializeField] Transform[] transforms;
    
AnimRecorderInterface recordable = null;

    //Joint Dictionary
    Dictionary<nuitrack.JointType, ModelJoint> jointsRigged = new Dictionary<nuitrack.JointType, ModelJoint>();

    // Start is called before the first frame update
    void Start()
    {
      PopulateCharacterJoints();

      poseCalibration.onSuccess += PoseCalibration_onSuccess;

      recordable = new GenericRecorder(transforms, root);

      if (recordIcon.activeInHierarchy)
        {
            recordIcon.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentUserTracker.CurrentSkeleton != null)
        {
            RefreshBodyLocation(CurrentUserTracker.CurrentSkeleton, Avatar);
            RefreshJointRotation(CurrentUserTracker.CurrentSkeleton);
        }
        if (isRecording)
        {
            recordable.TakeSnapshot(Time.deltaTime);
        }
    }

    private void PopulateCharacterJoints()
    {
        //populate joints
        for (int i = 0; i < Joints.Length; i++)
        {
            Joints[i].bone = GameObject.Find(Joints[i].jointType.ToString()).gameObject.transform;
            Joints[i].baseRotOffset = Joints[i].bone.rotation;
            jointsRigged.Add(Joints[i].jointType, Joints[i]);
        }
        //populate transforms for animation
        root = Avatar.transform;
        for (int i = 0; i < transforms.Length; i++)
        {
           
            transforms[i] =Joints[i].bone;
            
        }
    }

    private void RefreshBodyLocation(nuitrack.Skeleton skeleton, GameObject body)
    {
        nuitrack.JointType Anchor = nuitrack.JointType.Torso;

        Vector3 pos = Quaternion.Euler(0f, 180f, 0f) * (0.001f * skeleton.GetJoint(Anchor).ToVector3());

        body.transform.position = pos;

    }

    private void RefreshJointRotation(nuitrack.Skeleton skeleton)
    {


        foreach (var riggedJoint in jointsRigged)
        {

            nuitrack.Joint joint = skeleton.GetJoint(riggedJoint.Key);
            ModelJoint modelJoint = riggedJoint.Value;
            Quaternion jointOrient = Quaternion.Inverse(CalibrationInfo.SensorOrientation) * (joint.ToQuaternion()) * modelJoint.baseRotOffset;
            modelJoint.bone.rotation = jointOrient;

        }

    }
    private void PoseCalibration_onSuccess(Quaternion rotation)
    {
        if (tposeRec == true)
        {
            if (!isRecording)
            {
                StartRec();
            }
            else
            {
                StopRec();
            }
        }
    }

    public void StartRec()
    {
        Debug.Log("Start recording");
        isRecording = true;
        recordIcon.SetActive(true);
    }

    public void StopRec()
    {
        Debug.Log("Stop recording");
        isRecording = false;
        SaveToFile(recordable.GetClip);
        recordIcon.SetActive(false);
    }

    void SaveToFile(AnimationClip clip)
    {
        string path = savePath + "/" + fileName + ".anim";
        clip.name = fileName;

        AssetDatabase.CreateAsset(clip, path);
        Debug.Log("Save to: " + path);
    }

    private void OnDestroy()
    {
        poseCalibration.onSuccess -= PoseCalibration_onSuccess;
    }
}
