using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarotManager : MonoBehaviour
{
    public LevelManager levelManager;
    public Animation shuffleAnimation;

    void OnTriggerEnter(Collider other)
    {
        StartTarot();
    }

    void StartTarot()
    {
        PlayAnimationAndLoadLevel();
    }

    public void PlayAnimationAndLoadLevel()
    {
        levelManager.SelectRandomCard();

        //shuffleAnimation.Play("miAnimacion");

/*         while (shuffleAnimation.isPlaying)
        {
            yield return null;
        } */
        
        levelManager.LoadLevel();
    }
}
