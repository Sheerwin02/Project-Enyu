using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Student1{ 
public class SwapAnimation : MonoBehaviour
{
    public Animator[] characters;
    public Text animationNameText;
    private int animationIndex = 0;

    // Start is called before the first frame update
    void Start()
    {

        //animators = new Animator[characters.length];
        //for (int i = 0; i < characters.length; i++)
        //{
        //    animators[i] = characters[i].getcomponent<animator>();
        //}

        animationNameText.text = characters[0].runtimeAnimatorController.animationClips[animationIndex].name;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PlayNextAnimation();
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PlayPreviousAnimation();
        } else if (Input.GetKeyDown(KeyCode.Space)){
            ReplayAnimation();
        }
    }

    public void PlayNextAnimation()
    {
        animationIndex++;
        if (animationIndex >= characters[0].runtimeAnimatorController.animationClips.Length)
        {
            animationIndex = 0;
        }
        for (int i = 0; i < characters.Length; i++)
        {
            string animationName = characters[i].runtimeAnimatorController.animationClips[animationIndex].name;
            characters[i].Play(animationName, 0, 0f);
        }
        animationNameText.text = characters[0].runtimeAnimatorController.animationClips[animationIndex].name;
    }
    public void PlayPreviousAnimation()
    {
        animationIndex--;
        if (animationIndex < 0)
        {
            animationIndex = characters[0].runtimeAnimatorController.animationClips.Length-1;
        }
        for (int i = 0; i < characters.Length; i++)
        {

            string animationName = characters[i].runtimeAnimatorController.animationClips[animationIndex].name;
            characters[i].Play(animationName, 0, 0f);
        }
        animationNameText.text = characters[0].runtimeAnimatorController.animationClips[animationIndex].name;
    }
    public void ReplayAnimation()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            string animationName = characters[i].runtimeAnimatorController.animationClips[animationIndex].name;
            characters[i].Play(animationName, 0, 0f);
        }
    }
}
}