using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoad : MonoBehaviour
{
    [SerializeField]
    private InteractionController playerCamera;

    [SerializeField]
    private string sceneToLoad;

    [SerializeField]
    private GameObject loadingImageObject;

    [SerializeField]
    private TextMeshProUGUI loadingTextObject;

    [SerializeField]
    private string loadingText = "Lorem ipsum";

    private bool preLoad = false;
    private bool load = false;
    private float timer = 0;

    void Update()
    {
        if (preLoad)
        {
            timer += Time.deltaTime;

            if (timer < 1)
            {
                Color imageColor = new(0, 0, 0, timer);
                loadingImageObject.GetComponent<Image>().color = imageColor;

            } else
            {
                timer = 0;
                Color imageColor = new (0, 0, 0, 1);
                loadingImageObject.GetComponent<Image>().color = imageColor;
                preLoad = false;
                load = true;
            }
        }

        if (load)
        {
            timer += Time.deltaTime;

            if (timer >= 1)
                loadingTextObject.text = loadingText;

            if (timer >= 10)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (playerCamera.IsUnityNull())
                playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InteractionController>();

            if (loadingImageObject.IsUnityNull())
                loadingImageObject = GameObject.FindGameObjectWithTag("LoadingImage");

            if (loadingTextObject.IsUnityNull())
                loadingTextObject = GameObject.FindGameObjectWithTag("LoadingText").GetComponent<TextMeshProUGUI>();

            playerCamera.IsReadingNote= true;
            loadingImageObject.SetActive(true);
            preLoad = true;
        }
    }
}
