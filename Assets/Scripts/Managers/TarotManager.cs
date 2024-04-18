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
        StartCoroutine(PlayAnimationAndLoadLevel());
    }

    IEnumerator PlayAnimationAndLoadLevel()
    {
        levelManager.SelectRandomCard();

        shuffleAnimation.Play("miAnimacion");

        while (shuffleAnimation.isPlaying)
        {
            yield return null;
        }
        
        levelManager.LoadLevel();
    }
}
