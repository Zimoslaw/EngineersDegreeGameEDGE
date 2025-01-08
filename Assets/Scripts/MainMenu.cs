using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public Button PlayButton;
    public Toggle invertYAxisToggle;
    public Slider mouseSensitivitySlider;
    public Slider SFXVolumeSlider;
    public Slider MusicVolumeSlider;
    public AudioMixer MainAudioMixer;

    public PlayableDirector timeline;

    void Start()
    {
        // Setting defaults for player settings
        PlayerPrefs.SetInt("KeroseneLevel", 200);
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
            PlayerPrefs.SetInt("MouseSensitivity", 5);
        }
        else
        {
            mouseSensitivitySlider.value = PlayerPrefs.GetInt("MouseSensitivity");
        }

        if (!PlayerPrefs.HasKey("SoundsVolume"))
        {
            PlayerPrefs.SetFloat("SoundsVolume", 0.5f);
        }
        else
        {
            SFXVolumeSlider.value = PlayerPrefs.GetFloat("SoundsVolume");
        }

        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 0.5f);
        }
        else
        {
            MusicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }

        MainAudioMixer.SetFloat("SoundsVolume", Mathf.Log10(SFXVolumeSlider.value) * 20);
        MainAudioMixer.SetFloat("MusicVolume", Mathf.Log10(MusicVolumeSlider.value) * 20);

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
        invertYAxisToggle.Select();
    }

    public void SettingsCloseButton()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
        PlayButton.Select();
    }

    public void InvertYAxisCheckbox()
    {
        PlayerPrefs.SetInt("InvertYAxis", invertYAxisToggle.isOn? 1 : 0);
    }

    public void MouseSensitivitySlider()
    {
        PlayerPrefs.SetInt("MouseSensitivity", (int)mouseSensitivitySlider.value);
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

    public void ExitButton()
    {
        Application.Quit();
    }
}
