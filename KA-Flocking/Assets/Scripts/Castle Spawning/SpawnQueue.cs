using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpawnQueue : MonoBehaviour
{
    // origin for the spawned items in the GUI
    public Transform spawnPoint;
    // Default style of an item in the queue
    public GameObject defaultItem;
    // Lists to have a reference to the specific GUI gameobject
    [System.NonSerialized]
    public List<GameObject> currentSpawnedItems = new List<GameObject>();
    public RectTransform content;
    private Button addButton;
    // The current castle that the SpawnQueue is showing
    [System.NonSerialized]
    public Castle castle = null;
    public ErrorChat errorChat;
    public Flock flock;
    public Text money;
    // The fraction that needs to be paid when queueing (discount)
    [Range(0,1)]
    public float queueCostPercent = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
        // Set variables
        if (SceneManager.GetSceneByName("PlayerOneSetupScene").isLoaded) { 
            flock = GameObject.Find("Team 1 Flock").GetComponent<Flock>();
        } else if (SceneManager.GetSceneByName("PlayerTwoSetupScene").isLoaded) {
            flock = GameObject.Find("Team 2 Flock").GetComponent<Flock>();	
        }
        ToggleGroup troopToggles = GameObject.Find("TroopToggles").GetComponent<ToggleGroup>();
        Slider troopSlider = GameObject.Find("troopAmountSlider").GetComponent<Slider>();

        // Configure the add button
        addButton = GetComponentInChildren<Button>(spawnPoint);
        addButton.onClick.AddListener(delegate () {this.addButtonClicked(troopToggles, troopSlider);});
    }

    // Replaces the castle
    public void replaceCastle(Castle newCastle) {
        if (castle == newCastle) return;

        addButton.gameObject.SetActive(true);
        if (castle != null) ClearItems();
        castle = newCastle;
        UpdateUI();
    }

    private void ClearItems() {
        foreach (GameObject item in currentSpawnedItems)
        {
            Destroy(item);
        }
        currentSpawnedItems.Clear();
    }

    // Updates the UI for the spawnqueue
    public void UpdateUI() {
        // The amount of buttons in the content
        int i = 1;
        ClearItems();
        foreach ((int, FlockAgent, Unit, int,Sprite) item in castle.items)
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
            itemDetails.text.text = item.Item4.ToString();
            itemDetails.image.sprite = item.Item5;

            // Set button listener for delete
            Button button = spawnedItem.GetComponent<Button>();
            button.onClick.AddListener(delegate () {this.deleteFromQueue(spawnedItem); });
        }
        content.sizeDelta = new Vector2 (0,30*(i/4)-60);
    }

    // Deletes a specific item from the queue
    private void deleteFromQueue(GameObject spawnedItem) {
        int i = currentSpawnedItems.IndexOf(spawnedItem);

        // Merge troop amount
        if (i > 0 && i+1 < castle.items.Count 
                && castle.items[i-1].Item3 == castle.items[i+1].Item3) {

            // Update amount
            var v = castle.items[i-1];
            v.Item4 += castle.items[i+1].Item4;
            castle.items[i-1] = v;

            // Remove item
            castle.items.RemoveAt(i+1);
            Destroy (currentSpawnedItems[i+1]);
        }
        // Refund and remove item
        flock.moneyAmount += castle.items[i].Item1 * castle.items[i].Item4;
        money.text = "Money: " + flock.moneyAmount.ToString(); 
        castle.items.RemoveAt(i);
        Destroy (spawnedItem);

        // Update the SpawnQueue
        UpdateUI();
    }

    // Pays and adds units to queue
    private void addButtonClicked(ToggleGroup troopToggles, Slider troopSlider) {
        foreach (Toggle toggle in troopToggles.ActiveToggles())
        {
            TroopType troop = toggle.GetComponent<TroopType>();

            // add the discount
            int cost = (int) (troop.cost * queueCostPercent);

            if (troop.unitType is Castle) { errorChat.ShowError("Invalid unit type"); return;}

            // pay for units
            int maxTroopsAfforded = Mathf.Min((int) troopSlider.value, flock.moneyAmount/(cost == 0 ? 1:cost));
            flock.moneyAmount -= maxTroopsAfforded * cost;
            money.text = "Money: " + flock.moneyAmount.ToString();

            // add units and update queue
            addToQueue(troop, maxTroopsAfforded, toggle.GetComponentInChildren<Image>().sprite);
            UpdateUI();
        }
    }

    // Adds a unit to the queue
    private void addToQueue(TroopType type, int n, Sprite sprite) {
        int lastIndex = castle.items.Count - 1;

        // If the last thing in queue is the same type, update the number instead
        if (lastIndex >= 0 && castle.items[lastIndex].Item3 == type.unitType) {
            var v = castle.items[lastIndex];
            v.Item4 += n;
            castle.items[lastIndex] = v;

            // Updates the item text
            ItemDetails itemDetails = currentSpawnedItems[lastIndex].GetComponent<ItemDetails>();
            itemDetails.text.text = v.Item2.ToString();
        }
        else {
            // Required to extract TroopType since it's bound to a component
            (int, FlockAgent, Unit, int, Sprite) item = (type.cost, type.prefab, type.unitType, n, sprite);
            castle.items.Add(item);
        }
    }
}
