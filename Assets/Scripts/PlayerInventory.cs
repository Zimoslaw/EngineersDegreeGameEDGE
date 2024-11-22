using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Interactable;

[Serializable]
public class PlayerInventory : MonoBehaviour
{

    public List<Interactable> items = new();

    [SerializeField] private GameObject _player;
    [SerializeField] private KeroseneLamp _lamp;
    [SerializeField] private GameObject _inventoryBackground;
    [SerializeField] private GameObject _inventoryItem;
    [SerializeField] private AudioSource _audioSource;

    void Start()
    {
        // Add saved items into the inventory
        if (File.Exists(Application.persistentDataPath + "/inventory.json"))
        {
            foreach (string line in File.ReadAllLines(Application.persistentDataPath + "/inventory.json"))
            {
                if (line != "")
                {
                    // Temporary data for lading items into inventory
                    GameObject itemObject = new();
                    Interactable item = itemObject.AddComponent<Interactable>();

                    JsonUtility.FromJsonOverwrite(line, item);
                    PutItem(item);
                }
            }
        }
    }

    public void PutItem(Interactable item)
    {
        if (item.Type == InteractableTypeEnum.Kerosene)
        {
            foreach (Interactable listItem in items)
            {
                if (listItem.Type == InteractableTypeEnum.Kerosene)
                {
                    listItem.amount++;
                    return;
                }
            }
            items.Add(item);
        }
        else
        {
            items.Add(item);
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
                    foreach (Interactable listItem in items)
                    {
                        if(listItem.Type == InteractableTypeEnum.Kerosene)
                        {
                            listItem.amount--;
                            if (listItem.amount <= 0)
                            {
                                items.Remove(listItem);
                            }
                                break;
                        }
                    }
                    _audioSource.Play();
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
        foreach (Interactable item in items)
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

            if (item.amount > 1)
                newItem.GetComponentInChildren<TextMeshProUGUI>().text = "• " + item.Name + " (×" + item.amount + ")";
            else
                newItem.GetComponentInChildren<TextMeshProUGUI>().text = "• " + item.Name;
        }

        _inventoryBackground.SetActive(true);
    }

    public void CloseInventory()
    {
        _inventoryBackground.SetActive(false);
    }
}
