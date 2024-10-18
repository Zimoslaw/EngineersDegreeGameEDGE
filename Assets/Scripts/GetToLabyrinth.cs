using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GetToLabyrinth : MonoBehaviour
{
    [SerializeField]
    private Transform[] objectsToMoveIntoLabyrinth;
    [SerializeField]
    private GameObject labyrinth;
    [SerializeField]
    private GameObject loadingImageObject;
    [SerializeField]
    private DemonAi demon;

    private bool fadingIn = false;
    private bool fadingOut = false;
    private float timer = 0;

    private (int z, int x) transforTo;

    void Update()
    {
        if (fadingIn)
        {
            timer += Time.deltaTime;

            if (timer < 1)
            {
                Color imageColor = new(0, 0, 0, timer);
                loadingImageObject.GetComponent<Image>().color = imageColor;

            }
            
            if (timer >= 1)
            {
                Color imageColor = new(0, 0, 0, 1);
                loadingImageObject.GetComponent<Image>().color = imageColor;

                foreach (Transform obj in objectsToMoveIntoLabyrinth)
                {
                    Vector3 newPosition = new(transforTo.x * 4, obj.position.y, transforTo.z * 4);
                    obj.position = newPosition;
                }

                demon = GameObject.FindWithTag("Demon").GetComponent<DemonAi>();
                if (!demon.IsUnityNull())
                {
                    demon.ChangeDestination(demon.CurrentDestination);
                }
            }

            if (timer >= 2)
            {
                timer = 1;
                fadingIn = false;
                fadingOut = true;
            }
        }

        if (fadingOut)
        {
            timer -= Time.deltaTime;

            if (timer > 0)
            {
                Color imageColor = new(0, 0, 0, timer);
                loadingImageObject.GetComponent<Image>().color = imageColor;

            }
            else
            {
                Color imageColor = new(0, 0, 0, 0);
                loadingImageObject.GetComponent<Image>().color = imageColor;
                fadingOut = false;
                loadingImageObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transforTo = labyrinth.GetComponent<LabyrinthGenerator>().startCell;
            fadingIn = true;
            loadingImageObject.SetActive(true);
        }
    }
}
