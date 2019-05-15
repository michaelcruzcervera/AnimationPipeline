using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Kinect = Windows.Kinect;

public class Character : MonoBehaviour
{
   /* //Object with data from kinect
    public GameObject BodySourceManager;
    public GameObject Avatar;
   
    //private GameObject temp;
    public List<GameObject> tempJoints;

    
    //Variables for joints


    [HeaderAttribute("Rigged model")]
    [SerializeField]
    ModelJoint[] Joints;
    //public bool Invert;
    public bool flip = true;


    // Create dictionary of joints


    Dictionary<Kinect.JointType, ModelJoint> jointsRigged = new 
        Dictionary<Kinect.JointType, ModelJoint>();


    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    public bool basic_rig { get; private set; }

    Dictionary<Kinect.JointType, ModelJoint> rig = new 
        Dictionary<Kinect.JointType, ModelJoint>();

    // Start is called before the first frame update
    void Start()
    {
        //rig.Clear();
        
    }

    // Update is called once per frame
    void Update()
    {
        //If body source is empty continue
        if (BodySourceManager == null)
        {
            return;
        }
        //no body is tracked end
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }
        //store kinect data
        Kinect.Body[] data = _BodyManager.GetData();
        //if data is null end
        if (data == null)
        {
            return;
        }
        //store tracked id's
        List<ulong> trackedIds = new List<ulong>();
        // fo each bone add body tracking id
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }
        //list of bones known id's
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
                rig.Clear();

            }
        }

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    //_Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
              
                RefreshBodyLocation(body, _Bodies[body.TrackingId]);
                RefreshJointRotation(body, _Bodies[body.TrackingId]);
            }
        }
    }
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject n = Instantiate(Avatar, new Vector3(), Quaternion.identity);
        n.name= "Body:" + id;
        
        for (int i = 0; i < Joints.Length; i++)
        {
            Joints[i].bone = GameObject.Find(Joints[i].jointType.ToString()).gameObject.transform;
        }


        for (int i = 0; i < Joints.Length; i++)
        {
            Joints[i].baseRotOffset = Joints[i].bone.localRotation;
            rig.Add(Joints[i].jointType, Joints[i]);
        }

        return n;
    }
    private void RefreshBodyLocation(Kinect.Body body, GameObject bodyObject)
    {
        Kinect.JointType Anchor = Kinect.JointType.SpineBase;

        Vector3 pos = Quaternion.Euler(0f, 180f, 0f) * (0.001f * GetVector3FromJoint(body.Joints[Anchor]));

        bodyObject.transform.position = pos;

    }
    private void RefreshJointRotation(Kinect.Body body, GameObject bodyObject)
    {


        foreach (var rigJoint in rig) {

            Kinect.Joint jt = body.Joints[rigJoint.Key];
            ModelJoint modelJoint = rigJoint.Value;

            //item.gameObject.transform.position = GetVector3FromJoint(a);

            //Quaternion n = GetRotationFromJoint(jt, body);
            //item.gameObject.transform.localRotation = n;
            //Quaternion rotation = GetRotationFromJoint(jt.JointType, body) * modelJoint.baseRotOffset;
            Quaternion rotation = GetRotationFromJointType(jt.JointType, body) * modelJoint.baseRotOffset;
            /*
            if (Quaternion.Euler(0,0,0).Equals(rotation) && modelJoint.bone.parent != null)
            {
                //Kinect.Transform.rotation;

            }
            

             modelJoint.bone.rotation = rotation;
        }
    }
 
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }

    private static Quaternion GetRotationFromJointType(Kinect.JointType jt, Kinect.Body body)
    {
        Quaternion rotation = new Quaternion
            (body.JointOrientations[jt].Orientation.X, 
            body.JointOrientations[jt].Orientation.Y,
            body.JointOrientations[jt].Orientation.Z,
            body.JointOrientations[jt].Orientation.W);
       
        //rotation = new Quaternion(rotation.x, -rotation.y, -rotation.z, -rotation.w);
        //Debug.Log(rotation);
        return rotation;
    }
/*
    static Quaternion ToQuaternionMirrored(this ModelJoint joint)
    {
        Vector3 jointUp = new Vector3(-joint.baseRotOffset.Matrix[1], joint.Orient.Matrix[4], -joint.Orient.Matrix[7]);   //Y(Up)
        Vector3 jointForward = new Vector3(joint.Orient.Matrix[2], -joint.Orient.Matrix[5], joint.Orient.Matrix[8]);   //Z(Forward)

        if (jointForward.magnitude < 0.01f)
            return Quaternion.identity; //should not happen

        return Quaternion.LookRotation(jointForward, jointUp);
    }
/*
    private static Quaternion GetRotationFromJoint(Kinect.Joint joint)
    {
       Vector4 vZ = mOrient.GetColumn(2);
		Vector4 vY = mOrient.GetColumn(1);

		if(!flip)
		{
			vZ.y = -vZ.y;
			vY.x = -vY.x;
			vY.z = -vY.z;
		}
		else
		{
			vZ.x = -vZ.x;
			vZ.y = -vZ.y;
			vY.z = -vY.z;
		}
		
		if(vZ.x != 0.0f || vZ.y != 0.0f || vZ.z != 0.0f)
			return Quaternion.LookRotation(vZ, vY);
		else
			return Quaternion.identity;
    }*/
}
