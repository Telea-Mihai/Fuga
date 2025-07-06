using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    public List<string> collectedItems = new List<string>();

    void Awake()
    {
        Instance = this;
    }

    public void AddItem(string item)
    {
        collectedItems.Add(item);
        Debug.Log("Item added: " + item);
    }

    public bool HasItem(string item)
    {
        return collectedItems.Contains(item);
    }
}