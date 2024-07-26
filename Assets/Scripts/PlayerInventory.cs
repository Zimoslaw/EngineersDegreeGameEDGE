using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerInventory : MonoBehaviour
{

    public Dictionary<Interactable.InteractableTypeEnum, int> items = new Dictionary<Interactable.InteractableTypeEnum, int>();

    [SerializeField] private GameObject _player;

    private bool _isInventoryOpen = false;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("I"))
            OpenInventory();
    }

    public void PutItem(Interactable.InteractableTypeEnum item)
    {
        if (items.ContainsKey(item))
        {
            items[item]++;
        }
        else
        {
            items.Add(item, 1);
        }
    }

    public void UseItem()
    {

    }

    private void OpenInventory()
    {
        if (!_isInventoryOpen)
        {

        }
        else
        {

        }
    }
}
