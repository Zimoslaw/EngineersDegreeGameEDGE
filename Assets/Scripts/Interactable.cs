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
        Kerosene,
        Info
    }

    public string Name = "interactable";
    public InteractableTypeEnum Type = InteractableTypeEnum.Note;
    public int State = 0; // 0 = closed, 1 = open
    public bool Locked = false; // Does door require a key?
    public int KeyID = 0; // For keys only
    public string NoteText = "Lorem ipsum dolor sit amet";
    public Animator Animator;
    public string OpeningAnimation = "open";
    public string ClosingAnimation = "close";
    public Collider Collider;
    public int amount = 1; // for inventory

    public void Interact(GameObject Player)
    {
        switch (Type)
        {
            case InteractableTypeEnum.Door:
                if(Animator.GetCurrentAnimatorStateInfo(0).length > Animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
                    break;

                if (State == 0)
                {
                    if (Locked)
                    {
                        bool keyFound = false;
                        foreach (Interactable item in Player.GetComponent<PlayerInventory>().items) {
                            if (item.Type == InteractableTypeEnum.Key && item.KeyID == KeyID) {
                                Animator.Play(OpeningAnimation);
                                Collider.isTrigger = true;
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
						Animator.Play(OpeningAnimation);
						Collider.isTrigger = true;
						State = 1;
                    }
                }
                else
                {
					Animator.Play(ClosingAnimation);
					Collider.isTrigger = false;
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
            case InteractableTypeEnum.Info:
                break;
        }
    }
}
