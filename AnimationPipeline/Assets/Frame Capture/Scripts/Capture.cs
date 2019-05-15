using System.Collections;
using UnityEngine;
using System.IO;


public class Capture : MonoBehaviour
{
    
    public Texture2D capturedFrame;
    public Texture2D capturedNorm;
    public int animId = 0;
    public int count = 0;
    public int frames = 0;
    [SerializeField] Animator animator;
    [SerializeField] Animation anim;
    public bool isRecording = false;
    public bool record = false;
    string animName;

    public IEnumerator captureSprite()
    {
        //Create folder

        Directory.CreateDirectory("Assets/Animations/Animation_" + animId + "/Sprites");

        while (isRecording)
        { 
        //capture whole screen
        Rect lRect = new Rect(0f, 0f, Screen.width, Screen.height);
        if (capturedFrame)
            Destroy(capturedFrame);

        yield return new WaitForEndOfFrame();

        capturedFrame = zzTransparencyCapture.capture(lRect);
        SaveToFile(capturedFrame, "Sprite");
        }

        animId++;
     
    }

    public IEnumerator captureNorm()
    {
        //Create folder

        Directory.CreateDirectory("Assets/Animations/Animation_" + animId + "/Normal");

        while (isRecording)
        {
            //capture whole screen
            Rect lRect = new Rect(0f, 0f, Screen.width, Screen.height);
            if (capturedNorm)
                Destroy(capturedNorm);

            yield return new WaitForEndOfFrame();

            capturedNorm = DepthCapture.capture(lRect);
            SaveToFile(capturedNorm, "Normal");
        }

        animId++;

    }

    public void Update()
    {
        frames++;

        GameObject CharacterManager = GameObject.Find("CharacterManager").gameObject;
        CharacterTrackingBehaviour characterManager = CharacterManager.GetComponent<CharacterTrackingBehaviour>();
        AnimatorController animatorController = CharacterManager.GetComponent<AnimatorController>();

        animator = animatorController.getCurrentAnim();


        if (animator != null && record == true)
        {
            animatorController.loopAnimation(false);
            animator.Play(animatorController.anim.name, -1, 10f);

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 && !animator.IsInTransition(0) && isRecording == false)
            {
                Debug.Log("true");
                isRecording = true;

                //Create file for Animation
                Directory.CreateDirectory("Assets/Animations/Animation_" + animId);

                StartCoroutine(captureSprite());

            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && isRecording == true)
            {
                Debug.Log("false");
                isRecording = false;
                record = false;
            }
        }
    }

    void SaveToFile(Texture2D frame, string type)
    {
        var temp = frame.EncodeToPNG();
        //Destroy(frame);
        File.WriteAllBytes(Application.dataPath + "/Animations" + "/Animation_" +animId+ "/" + type + "/Frame_" + count + ".PNG", temp);
        count++;
    }
        
}

