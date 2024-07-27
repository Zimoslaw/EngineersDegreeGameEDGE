using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _interactableName;
	[SerializeField] private TextMeshProUGUI _interactableAction;
	[SerializeField] private GameObject _note;
	[SerializeField] private TextMeshProUGUI _noteText;

	public bool IsReadingNote = false; // Is Player reading a note?
    public bool IsInventoryOpen = false;

    void Update()
	{
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 2f, Color.blue);

        if (Input.GetKeyDown(KeyCode.E) && IsReadingNote)
        {
            HideNote();
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, 2f))
		{
			if (hit.collider.CompareTag("Interactable"))
			{
				Interactable focusedObject = hit.collider.GetComponent<Interactable>();
				if (focusedObject != null)
				{
					_interactableName.text = focusedObject.Name;

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
							} else
							{
								action = "Zamknij";
							}
							break;
					}

					_interactableAction.text = "[E] " + action;

					if(Input.GetKeyDown(KeyCode.E) && !IsInventoryOpen)
					{
						focusedObject.Interact(gameObject);
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

		if (Input.GetKeyDown(KeyCode.I))
		{
			if (IsInventoryOpen)
			{
                gameObject.GetComponent<PlayerInventory>().CloseInventory();
				IsInventoryOpen = false;
            }
			else
			{
				HideNote();
				gameObject.GetComponent<PlayerInventory>().OpenInventory();
				IsInventoryOpen = true;
			}
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
