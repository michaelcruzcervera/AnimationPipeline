using UnityEngine;

public interface AnimRecorderInterface
{
    void TakeSnapshot(float deltaTime);

    AnimationClip GetClip { get; }
}
