using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuObject;
    public GameObject SettingsMenu;
    public GameObject ExitDialog;
    public Button ResumeButton;
    public Button ReturnButton;
    public Toggle InvertYAxisToggle;
    public Slider MouseSensitivitySlider;
    public Slider SFXVolumeSlider;
    public Slider MusicVolumeSlider;
    public AudioMixer MainAudioMixer;

    void Start()
    {
        // Getting saved settings
        InvertYAxisToggle.isOn = PlayerPrefs.GetInt("InvertYAxis") != 0;
        MouseSensitivitySlider.value = PlayerPrefs.GetInt("MouseSensitivity");
        SFXVolumeSlider.value = PlayerPrefs.GetFloat("SoundsVolume");
        MusicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
    }

    public void SettingsOpenButton()
    {
        SettingsMenu.SetActive(true);
        PauseMenuObject.SetActive(false);
        InvertYAxisToggle.Select();
    }

    public void SettingsCloseButton()
    {
        PauseMenuObject.SetActive(true);
        SettingsMenu.SetActive(false);
        ResumeButton.Select();
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

    public void SFXVolumeSlide()
    {
        PlayerPrefs.SetFloat("SoundsVolume", SFXVolumeSlider.value);
        MainAudioMixer.SetFloat("SoundsVolume", Mathf.Log10(SFXVolumeSlider.value) * 20);
    }

    public void MusicVolumeSlide()
    {
        PlayerPrefs.SetFloat("MusicVolume", MusicVolumeSlider.value);
        MainAudioMixer.SetFloat("MusicVolume", Mathf.Log10(MusicVolumeSlider.value) * 20);
    }

    public void ExitDialogOpenButton()
    {
        ExitDialog.SetActive(true);
        PauseMenuObject.SetActive(false);
        ReturnButton.Select();
    }

    public void ExitDialogCloseButton()
    {
        PauseMenuObject.SetActive(true);
        ExitDialog.SetActive(false);
        ResumeButton.Select();
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
