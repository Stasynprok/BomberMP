using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonScript : MonoBehaviour
{
    public void ChangeScene() {
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    public void QuitApplication() {
        Application.Quit();
    }
}
