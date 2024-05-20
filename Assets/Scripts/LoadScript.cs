using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScript : MonoBehaviour
{
    [SerializeField] GameObject LoadingScreen;
    public void ChangeScene(int sceneIndex)
    {
        GameObject LoadScreen = Instantiate(LoadingScreen);
        LoadGetInfo slider = LoadScreen.GetComponent<LoadGetInfo>();
        StartCoroutine(AsyncLoadScnene(sceneIndex, slider.sliderGet)) ;
    }
    public void ChangeScene()
    {
        Debug.Log("Loading");
        GameObject LoadScreen = Instantiate(LoadingScreen);
        LoadGetInfo slider = LoadScreen.GetComponent<LoadGetInfo>();
        StartCoroutine(AsyncLoadScnene(SceneManager.GetActiveScene().buildIndex, slider.sliderGet));
    }
    IEnumerator AsyncLoadScnene(int sceneIndex, Slider loadSlider)
    {
        AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(sceneIndex);
        loadSceneOperation.allowSceneActivation = false;

        while (!loadSceneOperation.isDone)
        {
            loadSlider.value = loadSceneOperation.progress;
            if (loadSceneOperation.progress >= 0.9f)
            {
                loadSceneOperation.allowSceneActivation = true;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
