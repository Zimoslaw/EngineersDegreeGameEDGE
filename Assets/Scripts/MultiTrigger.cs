using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultiTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectsToActivate;

	[SerializeField]
	private GameObject[] objectsToDeactivate;

	[SerializeField]
    private Animator[] animatorsToPlay;

    [SerializeField]
    private AudioSource[] soundsToPlay;

    [SerializeField]
    private string subtitlesToShow;

    [SerializeField]
    private float subtitlesTime = 4;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Objects();
            Animations();
            Sounds();
            Subtitles();
            gameObject.SetActive(false);
        }
    }

    void Objects()
    {
        if (objectsToActivate.Length > 0)
        {
            foreach (GameObject obj in objectsToActivate)
            {
                obj.SetActive(true);
            }
        }

		if(objectsToDeactivate.Length > 0)
		{
			foreach(GameObject obj in objectsToDeactivate)
			{
				obj.SetActive(false);
			}
		}
	}
    void Animations()
    {
        if (animatorsToPlay.Length > 0) { 
            foreach (Animator anim in animatorsToPlay)
            {
                anim.Play("triggeranimation");
            }
        }
    }

    void Sounds()
    {
        if (soundsToPlay.Length > 0)
        {
            foreach (AudioSource sound in soundsToPlay)
            {
                if (!sound.isPlaying)
                {
                    sound.Play();
                }
            }
        }
    }

    void Subtitles()
    {
        if (subtitlesToShow.Length > 0)
        {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            if (!camera.IsUnityNull())
            {
                camera.GetComponent<Subtitles>().ShowMessage(subtitlesToShow, 0, subtitlesTime);
            }
        }
    }
}
