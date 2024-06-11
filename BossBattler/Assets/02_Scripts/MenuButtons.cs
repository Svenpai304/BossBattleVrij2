using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void LoadSceneIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
