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
        }
    }
}
