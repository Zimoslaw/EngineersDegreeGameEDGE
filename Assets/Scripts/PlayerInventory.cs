using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Interactable;
using static UnityEditor.Progress;

public class PlayerInventory : MonoBehaviour
{

	public Dictionary<Interactable, int> items = new();

	[SerializeField] private GameObject _player;
    [SerializeField] private GameObject _inventoryBackground;
    [SerializeField] private TextMeshProUGUI _inventoryTest;

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

	public void UseItem()
	{

	}

	public void OpenInventory()
	{
		_inventoryTest.text = "";

        foreach (Interactable item in items.Keys)
		{
			if (items[item] > 1)
				_inventoryTest.text += item.Name + " (×" + items[item] + ")\n";
			else
                _inventoryTest.text += item.Name + "\n";
        }

        _inventoryBackground.SetActive(true);
    }

	public void CloseInventory()
	{
		_inventoryBackground.SetActive(false);
	}
}
