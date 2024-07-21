using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _interactableName;
    [SerializeField]
    private TextMeshProUGUI _interactableAction;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1.5f, Color.blue);
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1.5f))
        {
            if (hit.collider.tag == "Interactable")
            {
                Interactable focusedObject = hit.collider.GetComponent<Interactable>();
                if (focusedObject != null)
                {
                    _interactableName.text = focusedObject.Name;

                    string action = "";
                    switch (focusedObject.Type)
                    {
                        case Interactable.interactableTypeEnum.Note:
                            action = "Przeczytaj";
                            break;
                        case Interactable.interactableTypeEnum.Door:
                            if (focusedObject.State == 0)
                            {
                                action = "Otwórz";
                            } else
                            {
                                action = "Zamknij";
                            }
                            break;
                    }

                    _interactableAction.text = "[E] " + action;

                    if(Input.GetKeyDown(KeyCode.E))
                    {
                        focusedObject.Interact();
                    }
                }
            }
        } else
        {
            _interactableName.text = null;
            _interactableAction.text = null;
        }

    }
}
