using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{

    private AsyncOperation m_operation; 
    private void OnEnable ()
    {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay ()
    {
        yield return new WaitForEndOfFrame();
        //SceneManager.LoadScene(AppScenes.GAME_SCENE);
       // m_operation = SceneManager.LoadSceneAsync(AppScenes.GAME_SCENE, LoadSceneMode.Single);
        m_operation.allowSceneActivation = false;

        while (!(m_operation.progress >= 0.9f))
        {
            Debug.Log(m_operation.progress.ToString("0.0000"));
            yield return null;
        }
        
        FinishLoading();

    }

    private void FinishLoading()
    {
        m_operation.allowSceneActivation = true;
    }

    /*private void FinishLoading (Scene scene, LoadSceneMode mode)
    {
        m_operation.allowSceneActivation = true;
        SceneManager.sceneLoaded -= FinishLoading;
        Destroy(this.gameObject);
    }*/
}
