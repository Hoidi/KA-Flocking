using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEditor;

public class EntitySpawning : MonoBehaviour
{

    public Flock flock;
    public Camera cam;
    RaycastHit collisionWithPlane;
    public ToggleGroup troopToggles;
    public ToggleGroup formationToggles;
    public Slider troopSlider;
    int troopLayer = 1 << 8;
    int planeLayer = 1 << 9;
    int obstacleLayer = 1 << 10;
    private int amountOfTroops = 1;
    public Text troopText;
    private bool inSpawningMethod = false;
    public Text money;
    public Text costOfSpawning;
    private bool spawnedFirstCastle = false;


    void Start(){
        //if-statement is to get around a null pointer exception in flockscene (since the amount of money each player has isnt relevant the flocking scene)
        if (SceneManager.GetSceneByName("PlayerOneSetupScene").isLoaded || SceneManager.GetSceneByName("PlayerTwoSetupScene").isLoaded) { 
            money.text = "Money: " + flock.moneyAmount.ToString(); 
        }
    }

    private void Update(){
        if (Input.GetMouseButton(0)){
            StartCoroutine(SpawnWithDelay());
        }
        int sum;
        //if-statement is to get around a null pointer exception in flockscene (since the cost of spawning troops isnt relevant in the flocking scene)
        if (SceneManager.GetSceneByName("PlayerOneSetupScene").isLoaded || SceneManager.GetSceneByName("PlayerTwoSetupScene").isLoaded){
            // Check which toggle is on, there should only be one
            foreach (Toggle toggle in troopToggles.ActiveToggles())
            {
                TroopType troop = toggle.GetComponent<TroopType>();
                // Sum depends on if you spawned your first castle
                sum = (!spawnedFirstCastle && troop.unitType.name.StartsWith("Castle")) ? 0 : amountOfTroops * troop.cost;
                costOfSpawning.text = "Spawning cost: " + sum.ToString();
            }
        }
    }
    IEnumerator SpawnWithDelay(){
        if (!inSpawningMethod){
            inSpawningMethod = true;
            // There should only be one
            foreach (Toggle toggle in formationToggles.ActiveToggles())
            {
                // sends the first letter of the toggle to spawnTroop
                spawnTroop(char.ToLower(toggle.ToString()[0]));
            }
        yield return new WaitForSecondsRealtime(0.1f);
            inSpawningMethod = false;
        }
    }
    private void setTroopAmount(){
        amountOfTroops = (int)troopSlider.value;
        troopText.text = amountOfTroops.ToString();

    }
    public void limitTroopAmount(Toggle toggle){
        troopSlider.maxValue = (toggle.isOn) ? 1 : 100;
    }
    private void spawnTroop(char formation){
        // There should only be one
        foreach (Toggle toggle in troopToggles.ActiveToggles())
        {
            TroopType troop = toggle.GetComponent<TroopType>();
            if (!spawnedFirstCastle && troop.unitType.name.StartsWith("Castle")) {
                troopSpawning(troop.prefab, troop.unitType, 0, formation);
            } else {
                troopSpawning(troop.prefab, troop.unitType, troop.cost, formation);
            }
        }
    }

    private bool validateLayers(Ray ray) {
        if (Physics.Raycast(ray, out collisionWithPlane, 10000f, troopLayer | obstacleLayer) || EventSystem.current.IsPointerOverGameObject()){ //already a troop at location, or user clicked on UI, so dont spawn
            return false;
        }
        return true;
    }
    // Validates if there are any troop colliders within a small radius (dependent on if the unit is a Castle) of the position
    private bool validateColliders(Vector3 worldPos, Unit unitType) {
        Collider[] colliders = Physics.OverlapSphere(worldPos, 10, troopLayer);
        foreach (Collider collider in colliders)
        {
            // If there is a troop collider within a range of 10(^2) (if collider is castle) or 1 meters, don't spawn
            if ((collider.ClosestPoint(worldPos)-worldPos).sqrMagnitude < (unitType.name.StartsWith("Castle") ? 100:0.5)) {
                return false;
            }
        }
        return true;
    }

    private void troopSpawning(FlockAgent agentPrefab, Unit unitType, int cost, char formationType){
        Ray ray = cam.ScreenPointToRay(Input.mousePosition); //ray from camera towards mouse cursor 
        Vector3 worldPos = new Vector3(0, 0, 0);
        if (Physics.Raycast(ray, out collisionWithPlane, 10000f, planeLayer)){ //raytracing to acquire spawn location
            worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
        }
        if (!validateLayers(ray) || (!validateColliders(worldPos, unitType))) return;
        int switchSide = 1; //variable to make spawning on each "side" of the arrow shape possible..
        int arrowDirection = -1; //arrow formation should be in opposite direction for the two players
        if (SceneManager.GetActiveScene().name == "PlayerOneSetupScene") arrowDirection *= -1;
        // Only spawn if the location is on the correct piece of land. TODO: Error noise if true
        if (worldPos.x * arrowDirection < 0) return;
        Vector3 FinalWorldPos = new Vector3(0, 0, 0);
        Vector3 location;
        // Bound the amountOfTroops to the amount that can be afforded
        amountOfTroops = Mathf.Min(amountOfTroops, flock.moneyAmount/(cost == 0 ? 1:cost));
        for (int i = 0; i < amountOfTroops; i++){
            if(formationType == 'c') {
                location = Random.insideUnitSphere * amountOfTroops * 0.4f;
                FinalWorldPos = worldPos + location;
            }
            else if(formationType == 'r') {
                FinalWorldPos = new Vector3(worldPos.x + 1.5f * (i % Mathf.RoundToInt(Mathf.Sqrt(amountOfTroops))), worldPos.y, worldPos.z + 1.5f * Mathf.CeilToInt(i / Mathf.RoundToInt(Mathf.Sqrt(amountOfTroops))));
            }
            else if(formationType == 'a') {
                FinalWorldPos = new Vector3(worldPos.x + (i * switchSide), worldPos.y, worldPos.z - i * arrowDirection); //spawn location
            }
            //raycast to get the exact y coordinate
            if (!Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer)) return;
            if (!validateColliders(FinalWorldPos, unitType)) return;
            FinalWorldPos.y = hit.point.y; //location now has proper y coordinate
            flock.CreateUnit( //spawn troops in formation
                agentPrefab,
                FinalWorldPos,
                Quaternion.Euler(0, cam.transform.eulerAngles.y, 0),
                unitType
                );
            switchSide *= -1;
            flock.moneyAmount -= cost; //reduce money appropriately
            money.text = "Money: " + flock.moneyAmount.ToString();
            if (!spawnedFirstCastle && unitType.name.StartsWith("Castle")) spawnedFirstCastle = true;
        }
    }
}