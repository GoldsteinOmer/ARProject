using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ScanWorld()
    {
        SceneManager.LoadSceneAsync(1);
    }

        public void takeATest()
    {
        SceneManager.LoadSceneAsync(2);
    }

            public void knowledge()
    {
        SceneManager.LoadSceneAsync(3);
    }
}
