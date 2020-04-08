﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnQueue : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject defaultItem;
    public RectTransform content;
    private List<(TroopType,int,Sprite)> items = new List<(TroopType,int,Sprite)>();
    private List<GameObject> currentSpawnedItems = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        UpdateQueue();
    }

    void UpdateQueue() {
        int i = 1;
        // Clear the list
        foreach (GameObject item in currentSpawnedItems)
        {
            Destroy(item);
        }
        currentSpawnedItems.Clear();
        foreach ((TroopType,int,Sprite) item in items)
        {
            float offsetY = (i/4) * 30;
            float offsetX = (i%4) * 30;
            i++;
            // New spawn position
            Vector3 pos = new Vector3(offsetX, -offsetY, spawnPoint.position.z);
            GameObject spawnedItem = Instantiate(defaultItem, pos, spawnPoint.rotation);
            spawnedItem.SetActive(true);
            spawnedItem.transform.SetParent(spawnPoint, false);
            currentSpawnedItems.Add(spawnedItem);
            // Set the item details
            ItemDetails itemDetails = spawnedItem.GetComponent<ItemDetails>();
            itemDetails.text.text = item.Item2.ToString();
            itemDetails.image.sprite = item.Item3;

            Button button = spawnedItem.GetComponent<Button>();
            button.onClick.AddListener(delegate () {this.deleteFromQueue(spawnedItem, item); });
        }
        content.sizeDelta = new Vector2 (0,30*(i/4)-60);
    }

    public void addToQueue(TroopType type, int n, Sprite sprite) {
        int lastIndex = items.Count-1;
        // If the last thing in queue is the same type, update the number instead
        if (lastIndex >= 0 && items[lastIndex].Item1 == type) {
            var v = items[lastIndex];
            v.Item2 += n;
            items[lastIndex] = v;
            // Updates the item text
            ItemDetails itemDetails = currentSpawnedItems[lastIndex].GetComponent<ItemDetails>();
            itemDetails.text.text = v.Item2.ToString();
        }
        else {
            (TroopType,int,Sprite) item = (type,n,sprite);
            items.Add(item);
            UpdateQueue();
        }
    }

    public void deleteFromQueue(GameObject spawnedItem, (TroopType,int,Sprite) item) {
        Debug.Log(item.Item2);
        int i = currentSpawnedItems.IndexOf(spawnedItem);
        items.RemoveAt(i);
        Destroy (spawnedItem);
    }

    public TroopType spawnTroop() {
        if (items.Count == 0) return null;
        var v = items[0];
        if (v.Item2 > 1) {
            v.Item2--;
            items[0] = v;
            return v.Item1;
        } else {
            items.RemoveAt(0);
            return v.Item1;
        }
    }
}
