using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Interactable;

public class InventoryItem : MonoBehaviour
{
    public InteractableTypeEnum Type = InteractableTypeEnum.Kerosene;
    public int KeyID = 0; // For keys only

    public InventoryItem(InteractableTypeEnum type, int keyID)
    {
        Type = type;
        KeyID = keyID;
    }

    public void Init(InteractableTypeEnum type, int keyID)
    {
        Type = type;
        KeyID = keyID;
    }
}
