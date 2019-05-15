using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;

public class AnimatorController : MonoBehaviour
{


    [SerializeField] Animator animator;
    [SerializeField] public AnimationClip anim;
    AnimatorOverrideController animatorOverrideController;
    [SerializeField] GameObject avatar;
    [SerializeField] public GameObject animChar;
    [SerializeField] public bool loop;
    [SerializeField] float speed;

    
    public void SetAnimation(string path)
    {
        GameObject CharacterManager = GameObject.Find("CharacterManager").gameObject;
        
        CharacterTrackingBehaviour characterManager = CharacterManager.GetComponent<CharacterTrackingBehaviour>();


        characterManager.Avatar.SetActive(false);
        
        if (animChar)
        {
            Destroy(animChar);
        }

       animChar = Instantiate(avatar, new Vector3(0, 0, 0), Quaternion.identity);
       animator = animChar.GetComponent<Animator>();
       anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
       UnityEditor.AssetDatabase.Refresh();
      
        
        if (anim)
        {

            

            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController();
            animatorOverrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
            
            animatorOverrideController["Default"] = anim;
            animator.runtimeAnimatorController = animatorOverrideController;

            loopAnimation(loop);
        }
        else
        { 
            Debug.LogError("No Animation Loaded");
        }



    }
    public void loopAnimation(bool loop)
    {
        SerializedProperty loopTime = new SerializedObject(anim).FindProperty("m_AnimationClipSettings.m_LoopTime");
        loopTime.boolValue = loop;
        loopTime.serializedObject.ApplyModifiedProperties();
    }
    public Animator getCurrentAnim()
    {
        if (this.anim)
        {
            return this.animator;
        }
        return null;
    }
}
