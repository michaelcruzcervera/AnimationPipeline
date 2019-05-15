using System.Collections.Generic;
using UnityEngine;

public class SimpleSkeletonAvatar3d : MonoBehaviour {

    public bool autoProcessing = true;
    [SerializeField] GameObject jointPrefab = null, connectionPrefab = null;

    nuitrack.JointType[] jointsInfo = new nuitrack.JointType[]
        {
        nuitrack.JointType.Head,
        nuitrack.JointType.Neck,
        nuitrack.JointType.LeftCollar,
        nuitrack.JointType.Torso,
        nuitrack.JointType.Waist,
        nuitrack.JointType.LeftShoulder,
        nuitrack.JointType.RightShoulder,
        nuitrack.JointType.LeftElbow,
        nuitrack.JointType.RightElbow,
        nuitrack.JointType.LeftWrist,
        nuitrack.JointType.RightWrist,
        nuitrack.JointType.LeftHand,
        nuitrack.JointType.RightHand,
        nuitrack.JointType.LeftHip,
        nuitrack.JointType.RightHip,
        nuitrack.JointType.LeftKnee,
        nuitrack.JointType.RightKnee,
        nuitrack.JointType.LeftAnkle,
        nuitrack.JointType.RightAnkle
        };

    nuitrack.JointType[,] connectionsInfo = new nuitrack.JointType[,]
    { //right and left collars are at same point at the moment, so we use only 1 collar here,
        //quite easy to add rightCollar if it ever changes
        {nuitrack.JointType.Neck,           nuitrack.JointType.Head},
        {nuitrack.JointType.LeftCollar,     nuitrack.JointType.Neck},
        {nuitrack.JointType.LeftCollar,     nuitrack.JointType.LeftShoulder},
        {nuitrack.JointType.LeftCollar,     nuitrack.JointType.RightShoulder},
        {nuitrack.JointType.LeftCollar,     nuitrack.JointType.Torso},
        {nuitrack.JointType.Waist,          nuitrack.JointType.Torso},
        {nuitrack.JointType.Waist,          nuitrack.JointType.LeftHip},
        {nuitrack.JointType.Waist,          nuitrack.JointType.RightHip},
        {nuitrack.JointType.LeftShoulder,   nuitrack.JointType.LeftElbow},
        {nuitrack.JointType.LeftElbow,      nuitrack.JointType.LeftWrist},
        {nuitrack.JointType.LeftWrist,      nuitrack.JointType.LeftHand},
        {nuitrack.JointType.RightShoulder,  nuitrack.JointType.RightElbow},
        {nuitrack.JointType.RightElbow,     nuitrack.JointType.RightWrist},
        {nuitrack.JointType.RightWrist,     nuitrack.JointType.RightHand},
        {nuitrack.JointType.LeftHip,        nuitrack.JointType.LeftKnee},
        {nuitrack.JointType.LeftKnee,       nuitrack.JointType.LeftAnkle},
        {nuitrack.JointType.RightHip,       nuitrack.JointType.RightKnee},
        {nuitrack.JointType.RightKnee,      nuitrack.JointType.RightAnkle}
    };

    GameObject[] connections;
    Dictionary<nuitrack.JointType, GameObject> joints;

    void Start()
    {
        CreateSkeletonParts();
    }

    void CreateSkeletonParts()
    {
        joints = new Dictionary<nuitrack.JointType, GameObject>();

        for (int i = 0; i < jointsInfo.Length; i++)
        {
            if (jointPrefab != null)
            {
                GameObject joint = Instantiate(jointPrefab, Vector3.zero, Quaternion.identity);
                joints.Add(jointsInfo[i], joint);
                joint.transform.parent = transform;
                joint.SetActive(false);
            }
        }

        connections = new GameObject[connectionsInfo.GetLength(0)];

        for (int i = 0; i < connections.Length; i++)
        {
            if (connectionPrefab != null)
            {
                GameObject conn = Instantiate(connectionPrefab, Vector3.zero, Quaternion.identity);
                connections[i] = conn;
                conn.transform.parent = transform;
                conn.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (CurrentUserTracker.CurrentSkeleton != null && autoProcessing) ProcessSkeleton(CurrentUserTracker.CurrentSkeleton);
    }

    public void ProcessSkeleton(nuitrack.Skeleton skeleton)
    {
        if (skeleton == null) return;

        if (!gameObject.activeSelf) gameObject.SetActive(true);

        for (int i = 0; i < jointsInfo.Length; i++)
        {
            nuitrack.Joint j = skeleton.GetJoint(jointsInfo[i]);
            if (j.Confidence > 0.5f)
            {
                if (!joints[jointsInfo[i]].activeSelf) joints[jointsInfo[i]].SetActive(true);

                joints[jointsInfo[i]].transform.position = 0.001f * j.ToVector3();
                joints[jointsInfo[i]].transform.rotation = j.ToQuaternionMirrored();
            }
            else
            {
                if (joints[jointsInfo[i]].activeSelf) joints[jointsInfo[i]].SetActive(false);
            }
        }

        for (int i = 0; i < connectionsInfo.GetLength(0); i++)
        {
            if (joints[connectionsInfo[i, 0]].activeSelf && joints[connectionsInfo[i, 1]].activeSelf)
            {
                if (!connections[i].activeSelf) connections[i].SetActive(true);

                Vector3 diff = joints[connectionsInfo[i, 1]].transform.position - joints[connectionsInfo[i, 0]].transform.position;

                connections[i].transform.position = joints[connectionsInfo[i, 0]].transform.position;
                connections[i].transform.rotation = Quaternion.LookRotation(diff);
                connections[i].transform.localScale = new Vector3(1f, 1f, diff.magnitude);
            }
            else
            {
                if (connections[i].activeSelf) connections[i].SetActive(false);
            }
        }
    }
}
