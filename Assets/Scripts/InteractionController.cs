using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
public class InteractionController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _interactableName;
    [SerializeField] private TextMeshProUGUI _interactableAction;
    [SerializeField] private GameObject _note;
    [SerializeField] private TextMeshProUGUI _noteText;

    public bool IsPaused = false;
    public bool IsReadingNote = false; // Is Player reading a note?
    public bool IsInventoryOpen = false;

    void Update()
    {
        if (IsPaused) return;

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 2f, Color.blue);

        if (IsReadingNote)
        {
            if (Input.GetButtonDown("Cancel"))
                HideNote();
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, 2f))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                if (hit.collider.TryGetComponent<Interactable>(out var focusedObject))
                {
                    _interactableName.text = focusedObject.Name;

                    if (focusedObject.Type != Interactable.InteractableTypeEnum.Info)
                    {
                        string action = "";
                        switch (focusedObject.Type)
                        {
                            case Interactable.InteractableTypeEnum.Note:
                                action = "Przeczytaj";
                                break;
                            case Interactable.InteractableTypeEnum.Kerosene:
                                action = "Zabierz";
                                break;
                            case Interactable.InteractableTypeEnum.Key:
                                action = "Zabierz";
                                break;
                            case Interactable.InteractableTypeEnum.Light:
                                if (focusedObject.State == 0)
                                {
                                    action = "Zapal";
                                }
                                else
                                {
                                    action = "Zgaś";
                                }
                                break;
                            case Interactable.InteractableTypeEnum.Door:
                                if (focusedObject.State == 0)
                                {
                                    action = "Otwórz";
                                }
                                else
                                {
                                    action = "Zamknij";
                                }
                                break;
                        }

                        _interactableAction.text = "[LPM] " + action;

                        if (Input.GetButtonDown("Interaction") && !IsInventoryOpen)
                        {
                            focusedObject.Interact(gameObject);
                        }
                    }
                }
            }
            else
            {
                _interactableName.text = null;
                _interactableAction.text = null;
            }
        }
        else
        {
            _interactableName.text = null;
            _interactableAction.text = null;
        }

        if (Input.GetButtonDown("Inventory"))
        {
            if (IsInventoryOpen)
            {
                gameObject.GetComponent<PlayerInventory>().CloseInventory();
                IsInventoryOpen = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                HideNote();
                gameObject.GetComponent<PlayerInventory>().OpenInventory();
                IsInventoryOpen = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            gameObject.GetComponent<Subtitles>().ShowMessage("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec semper et nunc a consectetur. Fusce sodales est diam, quis dapibus tortor eleifend sed. Ut ex metus, placerat ut dui pretium, gravida et. ", 1);
        }
    }

    public void ShowNote(string note)
    {
        _noteText.text = note;
        _note.SetActive(true);
        IsReadingNote = true;
    }

    public void HideNote()
    {
        _noteText.text = "";
        _note.SetActive(false);
        IsReadingNote = false;
    }
}
