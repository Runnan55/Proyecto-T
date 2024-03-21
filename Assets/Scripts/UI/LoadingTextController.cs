using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingTextController : MonoBehaviour
{
    [SerializeField] private TMP_Text loadingText;
    private void Start()
    {
        StartCoroutine(AnimateLoadingText());
        loadingText.text = "Loading...";
    }

    private IEnumerator AnimateLoadingText()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            loadingText.text = "Loading";
            yield return new WaitForSeconds(1);
            loadingText.text = "Loading.";
            yield return new WaitForSeconds(1);
            loadingText.text = "Loading..";
            yield return new WaitForSeconds(1);
            loadingText.text = "Loading...";
        }
    }
}
