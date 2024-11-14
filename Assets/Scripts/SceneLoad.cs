using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class SceneLoad : MonoBehaviour
{
    [SerializeField]
    private InteractionController _playerCamera;

    [SerializeField]
    private PlayableDirector _timeline;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            _timeline.Play();
            if (_playerCamera.IsUnityNull())
                _playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InteractionController>();

            _playerCamera.IsReadingNote = true;

            // Saving player data
            PlayerPrefs.SetInt("KeroseneLevel", GameObject.FindGameObjectWithTag("PlayerLamp").GetComponent<KeroseneLamp>().KeroseseneLevel);
            string inventoryData = "";
            foreach (Interactable item in GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerInventory>().items)
            {
                inventoryData += JsonUtility.ToJson(item) + Environment.NewLine;
            }
            File.WriteAllText(Application.persistentDataPath + "/inventory.json", inventoryData);
        }
    }
}
