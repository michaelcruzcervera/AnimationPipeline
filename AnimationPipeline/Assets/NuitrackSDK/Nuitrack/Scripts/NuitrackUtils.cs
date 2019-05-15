using UnityEngine;
using System.Collections.Generic;

using JointType = nuitrack.JointType;

public static class NuitrackUtils
{
    #region Transform
    public static Vector3 ToVector3(this nuitrack.Vector3 v)
    {
        return new Vector3(v.X, v.Y, v.Z);
    }

    public static Vector3 ToVector3(this nuitrack.Joint joint)
    {
        return new Vector3(joint.Real.X, joint.Real.Y, joint.Real.Z);
    }

    public static Quaternion ToQuaternion(this nuitrack.Joint joint)
    {
        Vector3 jointUp = new Vector3(joint.Orient.Matrix[1], joint.Orient.Matrix[4], joint.Orient.Matrix[7]);   //Y(Up)
        Vector3 jointForward = new Vector3(joint.Orient.Matrix[2], joint.Orient.Matrix[5], joint.Orient.Matrix[8]);   //Z(Forward)

        return Quaternion.LookRotation(jointForward, jointUp);
    }

    public static Quaternion ToQuaternionMirrored(this nuitrack.Joint joint)
    {
        Vector3 jointUp = new Vector3(-joint.Orient.Matrix[1], joint.Orient.Matrix[4], -joint.Orient.Matrix[7]);   //Y(Up)
        Vector3 jointForward = new Vector3(joint.Orient.Matrix[2], -joint.Orient.Matrix[5], joint.Orient.Matrix[8]);   //Z(Forward)

        if (jointForward.magnitude < 0.01f)
            return Quaternion.identity; //should not happen

        return Quaternion.LookRotation(jointForward, jointUp);
    }

    #endregion


    #region ToTexture2D

    /// <summary>
    /// Get UnityEngine.Texture2D from nuitrack.ColorFrame (TextureFormat.RGB24)
    /// </summary>
    /// <param name="frame">Original nuitrack.ColorFrame</param>
    /// <returns>Unity Texture2D</returns>
    public static Texture2D ToTexture2D(this nuitrack.ColorFrame frame)
    {
        byte[] sourceData = frame.Data;

        for (int i = 0; i < sourceData.Length; i += 3)
        {
            byte temp = sourceData[i];
            sourceData[i] = sourceData[i + 2];
            sourceData[i + 2] = temp;
        }

        Texture2D rgbTexture = new Texture2D(frame.Cols, frame.Rows, TextureFormat.RGB24, false);
        rgbTexture.LoadRawTextureData(sourceData);
        rgbTexture.Apply();

        Resources.UnloadUnusedAssets();

        return rgbTexture;
    }

    static Color32 transparentColor = Color.clear;
    static Color32[] defaultColors = new Color32[]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.magenta,
        Color.yellow,
        Color.cyan,
        Color.grey
    };

    /// <summary>
    /// Get filled UnityEngine.Texture2D, where each user is colored in a different color. Background is transparent
    /// </summary>
    /// <param name="frame">Original nuitrack.UserFrame</param>
    /// <param name="customListColors">Optional colors for users</param>
    /// <returns>Unity Texture2D</returns>
    public static Texture2D ToTexture2D(this nuitrack.UserFrame frame, params Color32[] customListColors)
    {
        Color32[] currentColorList = customListColors ?? defaultColors;

        byte[] outSegment = new byte[frame.Cols * frame.Rows * 4];

        for (int i = 0; i < (frame.Cols * frame.Rows); i++)
        {
            Color32 currentColor = (frame[i] == 0) ? transparentColor : currentColorList[frame[i]];

            int ptr = i * 4;
            outSegment[ptr] = currentColor.a;
            outSegment[ptr + 1] = currentColor.r;
            outSegment[ptr + 2] = currentColor.g;
            outSegment[ptr + 3] = currentColor.b;
        }

        Texture2D segmentTexture = new Texture2D(frame.Cols, frame.Rows, TextureFormat.ARGB32, false);

        segmentTexture.LoadRawTextureData(outSegment);
        segmentTexture.Apply();

        Resources.UnloadUnusedAssets();

        return segmentTexture;
    }

    /// <summary>
    /// To black-and-white representation of the image from the sensor depth
    /// </summary>
    /// <param name="frame">Original nuitrack.DepthFrame</param>
    /// <param name="contrast">Contrast of the final image (0 - low contrast, 1 - high). Recommended range [0.8-0.95]</param>
    /// <returns>Unity Texture2D</returns>
    public static Texture2D ToTexture2D(this nuitrack.DepthFrame frame, float contrast = 0.9f)
    {
        byte[] outDepth = new byte[(frame.Data.Length / 2) * 3];
        int de = 1 + 255 - (int)(contrast * 255);

        for (int i = 0; i < frame.Data.Length / 2; i++)
        {
            byte depth = (byte)(frame[i] / de);

            Color32 currentColor = new Color32(depth, depth, depth, 255);

            int ptr = i * 3;

            outDepth[ptr] = currentColor.r;
            outDepth[ptr + 1] = currentColor.g;
            outDepth[ptr + 2] = currentColor.b;
        }

        Texture2D depthTexture = new Texture2D(frame.Cols, frame.Rows, TextureFormat.RGB24, false);
        depthTexture.LoadRawTextureData(outDepth);
        depthTexture.Apply();

        Resources.UnloadUnusedAssets();

        return depthTexture;
    }

    #endregion


    #region SkeletonUltils

    static readonly Dictionary<JointType, HumanBodyBones> nuitrackToUnity = new Dictionary<JointType, HumanBodyBones>()
    {
        {JointType.Head,                HumanBodyBones.Head},
        {JointType.Neck,                HumanBodyBones.Neck},
        {JointType.LeftCollar,          HumanBodyBones.LeftShoulder},
        {JointType.RightCollar,         HumanBodyBones.RightShoulder},
        {JointType.Torso,               HumanBodyBones.Hips},
        {JointType.Waist,               HumanBodyBones.Hips},   // temporarily


        {JointType.LeftFingertip,       HumanBodyBones.LeftMiddleDistal},
        {JointType.LeftHand,            HumanBodyBones.LeftMiddleProximal},
        {JointType.LeftWrist,           HumanBodyBones.LeftHand},
        {JointType.LeftElbow,           HumanBodyBones.LeftLowerArm},
        {JointType.LeftShoulder,        HumanBodyBones.LeftUpperArm},

        {JointType.RightFingertip,      HumanBodyBones.RightMiddleDistal},
        {JointType.RightHand,           HumanBodyBones.RightMiddleProximal},
        {JointType.RightWrist,          HumanBodyBones.RightHand},
        {JointType.RightElbow,          HumanBodyBones.RightLowerArm},
        {JointType.RightShoulder,       HumanBodyBones.RightUpperArm},


        {JointType.LeftFoot,            HumanBodyBones.LeftToes},
        {JointType.LeftAnkle,           HumanBodyBones.LeftFoot},
        {JointType.LeftKnee,            HumanBodyBones.LeftLowerLeg},
        {JointType.LeftHip,             HumanBodyBones.LeftUpperLeg},

        {JointType.RightFoot,           HumanBodyBones.RightToes},
        {JointType.RightAnkle,          HumanBodyBones.RightFoot},
        {JointType.RightKnee,           HumanBodyBones.RightLowerLeg},
        {JointType.RightHip,            HumanBodyBones.RightUpperLeg},
    };

    /// <summary>
    /// Returns the appropriate HumanBodyBones  for nuitrack.JointType
    /// </summary>
    /// <param name="nuitrackJoint">nuitrack.JointType</param>
    /// <returns>HumanBodyBones</returns>
    public static HumanBodyBones ToUnityBones(this JointType nuitrackJoint)
    {
        return nuitrackToUnity[nuitrackJoint];
    }

    #endregion
}