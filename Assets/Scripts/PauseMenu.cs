using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject SettingsMenu;
    public GameObject ExitDialog;
    public Toggle InvertYAxisToggle;
    public Slider MouseSensitivitySlider;

    void Start()
    {
        // Getting saved settings
        InvertYAxisToggle.isOn = PlayerPrefs.GetInt("InvertYAxis") != 0;
        MouseSensitivitySlider.value = PlayerPrefs.GetInt("MouseSensitivity");
    }

    public void SettingsOpenButton()
    {
        SettingsMenu.SetActive(true);
    }

    public void SettingsCloseButton()
    {
        SettingsMenu.SetActive(false);
    }

    public void InvertYAxisCheckbox()
    {
        PlayerPrefs.SetInt("InvertYAxis", InvertYAxisToggle.isOn ? 1 : 0);
        gameObject.GetComponent<PlayerControl>().InvertYAxis = InvertYAxisToggle.isOn;
    }

    public void MouseSensitivitySlide()
    {
        PlayerPrefs.SetInt("MouseSensitivity", (int)MouseSensitivitySlider.value);
        gameObject.GetComponent<PlayerControl>().RotationSpeed = MouseSensitivitySlider.value;
    }

    public void ExitDialogOpenButton()
    {
        ExitDialog.SetActive(true);
    }

    public void ExitDialogCloseButton()
    {
        ExitDialog.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
