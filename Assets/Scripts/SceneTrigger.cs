using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartTrigger : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad = "Start";

    void Start()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
