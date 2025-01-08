using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public Button TryAgainButton;
    public void Start()
    {
        TryAgainButton.Select();
    }

    public void TryAgain()
    {
        SceneManager.LoadScene("Labyrinth");
    }
}
