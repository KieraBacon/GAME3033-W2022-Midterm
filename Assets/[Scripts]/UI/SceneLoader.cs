using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        TimeManager.StartNewLevel();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        TimeManager.StartNewLevel();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
