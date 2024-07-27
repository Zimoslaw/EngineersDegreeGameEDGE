using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
	public enum InteractableTypeEnum
	{
		Note,
		Door,
		Key,
		Light,
		Kerosene
	}

	public string Name = "interactable";
	public InteractableTypeEnum Type = InteractableTypeEnum.Note;
	public int State = 0;
    public int KeyID = 0; // For keys only
    public string NoteText = "Lorem ipsum dolor sit amet";

	public void Interact(GameObject Player)
	{
		switch (Type)
		{
			case InteractableTypeEnum.Door:
				if (State == 0) {
					State = 1;
				} else
				{
					State = 0;
				}
				break;
			case InteractableTypeEnum.Light:
				if (State == 0)
				{
					State = 1;
				}
				else
				{
					State = 0;
				}
				break;
			case InteractableTypeEnum.Kerosene:
				Player.GetComponent<PlayerInventory>().PutItem(gameObject.GetComponent<Interactable>());
				gameObject.SetActive(false);
				break;
			case InteractableTypeEnum.Key:
                Player.GetComponent<PlayerInventory>().PutItem(gameObject.GetComponent<Interactable>());
                gameObject.SetActive(false);
                break;
			case InteractableTypeEnum.Note:
				Player.GetComponent<InteractionController>().ShowNote(NoteText);
				break;
		}
	}
}
