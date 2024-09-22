using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Interactable;
using static UnityEditor.Progress;

public class PlayerInventory : MonoBehaviour
{

    public Dictionary<Interactable, int> items = new();

    [SerializeField] private GameObject _player;
    [SerializeField] private KeroseneLamp _lamp;
    [SerializeField] private GameObject _inventoryBackground;
    [SerializeField] private GameObject _inventoryItem;

    public void PutItem(Interactable item)
    {
        if (item.Type == InteractableTypeEnum.Kerosene)
        {
            foreach (Interactable key in items.Keys)
            {
                if (key.Type == InteractableTypeEnum.Kerosene)
                {
                    items[key]++;
                    return;
                }
            }
            items.Add(item, 1);
        }
        else
        {
            items.Add(item, 1);
        }
    }

    public void UseItem(InventoryItem item)
    {
        switch(item.Type)
        {
            case InteractableTypeEnum.Kerosene:
                if(_lamp.KeroseseneLevel < 100)
                {
                    _lamp.FillUp();
                    foreach (Interactable key in items.Keys)
                    {
                        if(key.Type == InteractableTypeEnum.Kerosene)
                        {
                            items[key]--;
                            if (items[key] <= 0)
                            {
                                items.Remove(key);
                            }
                                break;
                        }
                    }
                }
                    else
                {
                    gameObject.GetComponent<Subtitles>().ShowMessage("Lampa jest już napełniona", 0);
                }
                break;
        }

            OpenInventory(); // Refreshing inventory
    }

    public void OpenInventory()
    {
        int yPos = 380; // Y position of next item

        // Deleting previous items
        foreach(Transform child in _inventoryBackground.transform)
        {
            if(!child.gameObject.CompareTag("Player"))
                Destroy(child.gameObject);
        }

        // Displaying items
        foreach (Interactable item in items.Keys)
        {
            GameObject newItem = Instantiate(_inventoryItem);
            // In case of key, button is disabled
            if (item.Type == InteractableTypeEnum.Key)
                newItem.GetComponent<Button>().interactable = false;

            newItem.transform.SetParent(_inventoryBackground.transform, false);
            newItem.GetComponent<RectTransform>().localPosition = new Vector3(-420, yPos-=48, 0);

            InventoryItem inventoryItem = newItem.AddComponent<InventoryItem>();

            switch (item.Type)
            {
                case InteractableTypeEnum.Kerosene:
                    
                    inventoryItem.Init(item.Type, 0);
                    break;
                default:
                    inventoryItem.Init(item.Type, item.KeyID);
                    break;
            }

            newItem.GetComponent<Button>().onClick.AddListener(() => UseItem(inventoryItem));

            if (items[item] > 1)
                newItem.GetComponentInChildren<TextMeshProUGUI>().text = item.Name + " (×" + items[item] + ")";
            else
                newItem.GetComponentInChildren<TextMeshProUGUI>().text = item.Name;
        }

        _inventoryBackground.SetActive(true);
    }

    public void CloseInventory()
    {
        _inventoryBackground.SetActive(false);
    }
}
