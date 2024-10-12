using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;

    public PlayableDirector timeline;

    void Start()
    {
        // Reseting all saved data
        PlayerPrefs.SetInt("KeroseneLevel", 100);
        PlayerPrefs.SetInt("LabyrinthSeed", -1);
        if (File.Exists(Application.persistentDataPath + "/inventory.json"))
        {
            File.Delete(Application.persistentDataPath + "/inventory.json");
        }
    }

    public void PlayGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        timeline.Play();
    }

    public void SettingsOpenButton()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void SettingsCloseButton()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
