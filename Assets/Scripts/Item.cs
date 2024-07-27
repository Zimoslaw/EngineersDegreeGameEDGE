using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Interactable;

public class Item : MonoBehaviour
{

    public string Name = "item";
    public InteractableTypeEnum Type = InteractableTypeEnum.Kerosene;
    public int KeyID = 0; // For keys only

    public Item(string name, InteractableTypeEnum type)
    {
        Name = name;
        Type = type;
        KeyID = 0;
    }

    public Item(string name, int keyID)
    {
        Name = name;
        Type = InteractableTypeEnum.Key;
        KeyID = keyID;
    }
}
