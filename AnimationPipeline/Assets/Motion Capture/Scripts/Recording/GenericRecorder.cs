#if UNITY_EDITOR

using UnityEngine;

using UnityEditor;
using System.Collections.Generic;

public class GenericRecorder : AnimRecorderInterface
{
    float time = 0.0f;

    List<ObjectAnimation> objectAnimations = new List<ObjectAnimation>();

    public GenericRecorder(Transform[] recordableTransform, Transform rootTransform)
    {
        foreach (Transform transform in recordableTransform)
        {
            string path = AnimationUtility.CalculateTransformPath(transform, rootTransform);
            objectAnimations.Add(new ObjectAnimation(path, transform));
        }
    }

    public void TakeSnapshot(float deltaTime)
    {
        time += deltaTime;
        //Debug.Log(time);
        foreach (ObjectAnimation animation in objectAnimations)
            animation.TakeSnapshot(time);
    }

    public AnimationClip GetClip
    {
        get
        {
            AnimationClip clip = new AnimationClip();
            //return new AnimationClip();
            foreach (ObjectAnimation animation in objectAnimations)
            {
                foreach (CurveContainer container in animation.Curves)
                {
                    if (container.Curve.keys.Length > 1)
                        clip.SetCurve(animation.Path, typeof(Transform), container.Property, container.Curve);
                }
            }

            return clip;
        }
    }
}

class ObjectAnimation
{
    Transform transform;

    public List<CurveContainer> Curves { get; private set; }

    public string Path { get; private set; }

    public ObjectAnimation(string hierarchyPath, Transform recordableTransform)
    {
        Path = hierarchyPath;
        transform = recordableTransform;

        Curves = new List<CurveContainer>
            {
                new CurveContainer("localPosition.x"),
                new CurveContainer("localPosition.y"),
                new CurveContainer("localPosition.z"),

                new CurveContainer("localRotation.x"),
                new CurveContainer("localRotation.y"),
                new CurveContainer("localRotation.z"),
                new CurveContainer("localRotation.w")
            };
    }

    public void TakeSnapshot(float time)
    {
        Curves[0].AddValue(time, transform.localPosition.x);
        Curves[1].AddValue(time, transform.localPosition.y);
        Curves[2].AddValue(time, transform.localPosition.z);

        Curves[3].AddValue(time, transform.localRotation.x);
        Curves[4].AddValue(time, transform.localRotation.y);
        Curves[5].AddValue(time, transform.localRotation.z);
        Curves[6].AddValue(time, transform.localRotation.w);
    }
}

class CurveContainer
{
    public string Property { get; private set; }
    public AnimationCurve Curve { get; private set; }

    float? lastValue = null;

    public CurveContainer(string _propertyName)
    {
        Curve = new AnimationCurve();
        Property = _propertyName;
    }

    public void AddValue(float time, float value)
    {
        if (lastValue == null || !Mathf.Approximately((float)lastValue, value))
        {
            Keyframe key = new Keyframe(time, value);
            Curve.AddKey(key);
            lastValue = value;
        }
    }
}
#endif

