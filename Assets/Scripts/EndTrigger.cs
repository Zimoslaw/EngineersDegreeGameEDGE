using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EndTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject cameraObject;

    [SerializeField]
    private GameObject endTarget;

    [SerializeField]
    private Transform newCameraPosition;

    [SerializeField]
    private Transform demon;

    [SerializeField]
    private PlayableDirector timeline;

    [SerializeField]
    private bool cameraMoving = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !cameraMoving)
        {
            cameraObject.GetComponent<InteractionController>().IsReadingNote = true;

            newCameraPosition.position = cameraObject.transform.position + (endTarget.transform.position - cameraObject.transform.position) / 2;

            newCameraPosition.LookAt(demon);

            cameraMoving = true;

            timeline.Play();
        }
    }

    private void Update()
    {
        if (cameraMoving)
        {
            cameraObject.transform.position = new Vector3(Mathf.Lerp(cameraObject.transform.position.x, newCameraPosition.position.x, Time.deltaTime),
                Mathf.Lerp(cameraObject.transform.position.y, newCameraPosition.position.y, Time.deltaTime),
                Mathf.Lerp(cameraObject.transform.position.z, newCameraPosition.position.z, Time.deltaTime)
                );

            cameraObject.transform.rotation = Quaternion.Slerp(cameraObject.transform.rotation, newCameraPosition.rotation, Time.deltaTime);
        }
    }
}
