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
	public int State = 0; // 0 = closed, 1 = open
 	public bool Locked = false; // Does door require a key?
 	public int KeyID = 0; // For keys only
	public string NoteText = "Lorem ipsum dolor sit amet";

	public void Interact(GameObject Player)
	{
		switch (Type)
		{
			case InteractableTypeEnum.Door:
				if (State == 0)
				{
    				if (Locked)
					{
	 					bool keyFound = false;
						foreach (Interactable item in Player.GetComponent<PlayerInventory>().items.Keys) {
							if (item.Type == InteractableTypeEnum.Key && item.KeyID == KeyID) {
       							State = 1;
	       						Locked = false;
	       						keyFound = true;
	       						Player.GetComponent<PlayerInventory>().items.Remove(item);
	       						break;
							}
      					}
	    				if (!keyFound) {
							Player.GetComponent<Subtitles>().ShowMessage("Drzwi wymagają klucza", 1);
						}
  					}
       				else
					{
	    				State = 1;
					}
				}
    			else
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
