using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum interactableTypeEnum
    {
        Note,
        Door,
        Key,
        Light,
        Kerosene
    }

    public string Name = "interactable";
    public interactableTypeEnum Type = interactableTypeEnum.Note;
    public int State = 0;
    public string NoteText = "Lorem ipsum dolor sit amet";

    public void Interact()
    {
        switch (Type)
        {
            case interactableTypeEnum.Door:
                if (State == 0) {
                    State = 1;
                } else
                {
                    State = 0;
                }
                break;
        }
    }
}
