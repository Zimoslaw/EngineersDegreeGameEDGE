using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public Toggle invertYAxisToggle;
    public Slider mouseSensitivitySlider;

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

        if (!PlayerPrefs.HasKey("InvertYAxis"))
        {
            PlayerPrefs.SetInt("InvertYAxis", 0);
        }
        else
        {
            invertYAxisToggle.isOn = PlayerPrefs.GetInt("InvertYAxis") != 0; 
        }

        if (!PlayerPrefs.HasKey("MouseSensitivity"))
        {
            PlayerPrefs.SetInt("MouseSensitivity", 4);
        }
        else
        {
            mouseSensitivitySlider.value = PlayerPrefs.GetInt("MouseSensitivity");
        }

		// Reseting cursor
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
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

    public void InvertYAxisCheckbox()
    {
        PlayerPrefs.SetInt("InvertYAxis", invertYAxisToggle.isOn? 1 : 0);
    }

    public void MouseSensitivitySlider()
    {
        PlayerPrefs.SetInt("MouseSensitivity", (int)mouseSensitivitySlider.value);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
