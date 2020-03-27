using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEditor;




public class EntitySpawning : MonoBehaviour
{

    public Flock flock;
    public FlockAgent infantryPrefab;
    public FlockAgent pikemanPrefab;
    public FlockAgent archerPrefab;
    public FlockAgent scoutPrefab;
    public Unit defaultInfantryObject;
    public Unit defaultPikemanObject;
    public Unit defaultArcherObject;
    public Unit defaultScoutObject;
    public Camera cam;
    RaycastHit collisionWithPlane;
    public Toggle infantryToggle;
    public Toggle pikeToggle;
    public Toggle archerToggle;
    public Toggle scoutToggle;
    public Toggle CircularToggle;
    public Toggle RectangularToggle;
    public Toggle ArrowToggle;
    public Slider troopSlider;
    int troopLayer = 1 << 8;
    int planeLayer = 1 << 9;
    private int amountOfTroops = 1;
    public Text troopText;
    private bool inSpawningMethod = false;
    public Text money;
    public Text costOfSpawning;
    private int infantryCost = 100;
    private int archerCost = 300;
    private int pikeCost = 200;
    private int scoutCost = 1000;


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
            if (infantryToggle.isOn){
                sum = amountOfTroops * infantryCost;
                costOfSpawning.text = "Spawning cost: " + sum.ToString();
            }
            else if (pikeToggle.isOn) {
                sum = amountOfTroops * pikeCost;
                costOfSpawning.text = "Spawning cost: " + sum.ToString();
            }
            else if (archerToggle.isOn) { 
                sum = amountOfTroops * archerCost;
                costOfSpawning.text = "Spawning cost: " + sum.ToString();
            } else {
                sum = amountOfTroops * scoutCost;
                costOfSpawning.text = "Spawning cost: " + sum.ToString();
            }
        }
    }
    IEnumerator SpawnWithDelay(){
        if (!inSpawningMethod){
            inSpawningMethod = true;
            if (CircularToggle.isOn){
                spawnCircle();
            }
            else if (RectangularToggle.isOn){
                spawnRectangle();
            }
            else{
                spawnTriangle();
            }
        yield return new WaitForSeconds(0.1f);
            inSpawningMethod = false;
        }
    }
    private void setTroopAmount(){
        amountOfTroops = (int)troopSlider.value;
        troopText.text = amountOfTroops.ToString();

    }
    private void spawnCircle(){
        if (infantryToggle.isOn){
            spawnEntitiesCircular(infantryPrefab, defaultInfantryObject, infantryCost);
        }
        else if (pikeToggle.isOn){
            spawnEntitiesCircular(pikemanPrefab, defaultPikemanObject, pikeCost);
        }
        else if (archerToggle.isOn){
            spawnEntitiesCircular(archerPrefab, defaultArcherObject, archerCost);
        }
        else if (scoutToggle.isOn){
            spawnEntitiesCircular(scoutPrefab, defaultScoutObject, scoutCost);
        }
    }
    private void spawnRectangle(){
        if (infantryToggle.isOn){
            spawnEntitiesRectangular(infantryPrefab, defaultInfantryObject, infantryCost);
        }
        else if (pikeToggle.isOn){
            spawnEntitiesRectangular(pikemanPrefab, defaultPikemanObject, pikeCost);
        }
        else if (archerToggle.isOn){
            spawnEntitiesRectangular(archerPrefab, defaultArcherObject, archerCost);
        }
        else if (scoutToggle.isOn){
            spawnEntitiesCircular(scoutPrefab, defaultScoutObject, scoutCost);
        }
    }

    private void spawnTriangle(){
        if (infantryToggle.isOn){
            spawnEntitiesTriangular(infantryPrefab, defaultInfantryObject, infantryCost);
        }
        else if (pikeToggle.isOn){
            spawnEntitiesTriangular(pikemanPrefab, defaultPikemanObject, pikeCost);
        }
        else if (archerToggle.isOn){
            spawnEntitiesTriangular(archerPrefab, defaultArcherObject, archerCost);
        }
        else if (scoutToggle.isOn){
            spawnEntitiesCircular(scoutPrefab, defaultScoutObject, scoutCost);
        }
    }

    private void spawnEntitiesCircular(FlockAgent agentPrefab, Unit unitType, int cost){
        if (Input.GetMouseButton(0)){ //shortcut to place units, prone to change(?)
            Vector3 worldPos = new Vector3(0, 0, 0);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); //ray from camera towards mouse cursor 
            if (Physics.Raycast(ray, out collisionWithPlane, 10000f, troopLayer) || EventSystem.current.IsPointerOverGameObject()){ //already a troop at location, or user clicked on UI, so dont spawn
                return;
            }
            if (Physics.Raycast(ray, out collisionWithPlane, 10000f, planeLayer)){ //raytracing to acquire spawn location
                worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
            }
            Vector3 location;
            if (flock.moneyAmount - (amountOfTroops * cost) >= 0){ //can afford to spawn
                for (int i = 0; i < amountOfTroops; i++){
                    location = Random.insideUnitSphere * amountOfTroops * 0.4f;
                    Vector3 FinalWorldPos = worldPos + location;
                    //raycast to get the exact y coordinate
                    if (Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer)){
                        FinalWorldPos.y = hit.point.y; //location now has proper y coordinate
                    }
                    flock.CreateUnit( //spawn troops in formation
                        agentPrefab,
                        FinalWorldPos,
                        Quaternion.Euler(Vector3.up),
                        unitType
                    );
                }
                flock.moneyAmount -= (amountOfTroops * cost); //reduce money appropriately
                money.text = "Money: " + flock.moneyAmount.ToString(); //update money text
            }
            else{
                int amount = flock.moneyAmount;
                int x = 0;
                while (amount-cost >= 0){ //dirty solution to calculate how many troops player can afford to spawn 
                    amount -= cost;
                    x++;
                }
                if (flock.moneyAmount == 0){ //if player doesnt have any money left
                    return; 
                }
                else if (x == 0) { //if player doesnt have enough money to spawn a single troop of desired kind
                    //maybe add some indicator to the player here..
                }
                for (int y = 0; y < x; y++){ //spawn the amount of troops that player affords
                    location = Random.insideUnitSphere * x * 0.4f;
                    Vector3 FinalWorldPos = worldPos + location;
                    //raycast to get the exact y coordinate
                    if (Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer)){ 
                        FinalWorldPos.y = hit.point.y; //location now has proper y coordinate
                    }
                    flock.CreateUnit( //spawn troops in formation
                    agentPrefab,
                    FinalWorldPos,
                    Quaternion.Euler(Vector3.up),
                    unitType
                    );
                }
                flock.moneyAmount -= (x * cost); //reduce money appropriately
                money.text = "Money: " + flock.moneyAmount.ToString();
            }
        }
            
        }
    private void spawnEntitiesRectangular(FlockAgent agentPrefab, Unit unitType, int cost){
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        if (Input.GetMouseButton(0)){ //shortcut to place units, prone to change(?)
            Vector3 worldPos = new Vector3(0, 0, 0);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); //ray from camera towards mouse cursor 
            if (Physics.Raycast(ray, out collisionWithPlane, 10000f, troopLayer) || EventSystem.current.IsPointerOverGameObject()){ //already a troop at location, or user clicked on UI, so dont spawn
                return;
            }
            if (Physics.Raycast(ray, out collisionWithPlane, 10000f, planeLayer)){ //raytracing to acquire spawn location
                worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
            }
            if (flock.moneyAmount - (amountOfTroops * cost) >= 0){ //can afford to spawn
                for (int i = 0; i < amountOfTroops; i++){
                    Vector3 FinalWorldPos = new Vector3(worldPos.x + 1.5f * (i % Mathf.RoundToInt(Mathf.Sqrt(amountOfTroops))), worldPos.y, worldPos.z + 1.5f * Mathf.CeilToInt(i / Mathf.RoundToInt(Mathf.Sqrt(amountOfTroops))));
                    if (Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer)){ //raycast to get the exact y coordinate
                        FinalWorldPos.y = hit.point.y; //location now has proper y coordinate
                    }
                    if(flock.moneyAmount - cost >= 0) { //can afford to spawn
                        flock.CreateUnit( //spawn troops in formation
                            agentPrefab,
                            FinalWorldPos,
                            Quaternion.Euler(Vector3.up),
                            unitType
                        );
                    }
                }
                flock.moneyAmount -= (amountOfTroops * cost); //reduce money appropriately
                money.text = "Money: " + flock.moneyAmount.ToString(); //update money text
            }
            else{
                int amount = flock.moneyAmount;
                int x = 0;
                while (amount-cost >= 0){ //dirty solution to calculate how many troops player can afford to spawn 
                    amount -= cost;
                    x++;
                }
                if (flock.moneyAmount == 0) { //if player doesnt have any money left
                    return; 
                }
                else if (x == 0) { //if player doesnt have enough money to spawn a single troop of desired kind
                    //maybe add some indicator to the player here..
                }
                for (int y = 0; y < x; y++){ //spawn the amount of troops that player affords
                    Vector3 FinalWorldPos = new Vector3(worldPos.x + 1.5f * (y % Mathf.RoundToInt(Mathf.Sqrt(x))), worldPos.y, worldPos.z + 1.5f * Mathf.CeilToInt(y / Mathf.RoundToInt(Mathf.Sqrt(x))));
                    //raycast to get the exact y coordinate
                    if (Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer)){ 
                        FinalWorldPos.y = hit.point.y; //location now has proper y coordinate
                    }
                    flock.CreateUnit( //spawn troops in formation
                    agentPrefab,
                    FinalWorldPos,
                    Quaternion.Euler(Vector3.up),
                    unitType
                    );
                }
                flock.moneyAmount -= (x * cost); //reduce money appropriately
                money.text = "Money: " + flock.moneyAmount.ToString();
            }

        }

    }
    private void spawnEntitiesTriangular(FlockAgent agentPrefab, Unit unitType, int cost){
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        if (Input.GetMouseButton(0)){ //shortcut to place units, prone to change(?)
            Vector3 worldPos = new Vector3(0, 0, 0);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); //ray from camera towards mouse cursor 
            if (Physics.Raycast(ray, out collisionWithPlane, 10000f, troopLayer) || EventSystem.current.IsPointerOverGameObject()){ //already a troop at location, or user clicked on UI, so dont spawn
                return;
            }
            if (Physics.Raycast(ray, out collisionWithPlane, 10000f, planeLayer)){ //raytracing to acquire spawn location
                worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
            }
            int switchSide = 1; //variable to make spawning on each "side" of the arrow shape possible..
            int arrowDirection = -1; //arrow formation should be in opposite direction for the two players
            if (SceneManager.GetActiveScene().name == "PlayerOneSetupScene") arrowDirection *= -1;
            if (flock.moneyAmount - (amountOfTroops * cost) >= 0){ //can afford to spawn
                for (int i = 0; i < amountOfTroops; i++){
                    Vector3 FinalWorldPos = new Vector3(worldPos.x + (i * switchSide), worldPos.y, worldPos.z - i * arrowDirection); //spawn location
                    //raycast to get the exact y coordinate
                    if (Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer)){ 
                        FinalWorldPos.y = hit.point.y; //location now has proper y coordinate
                    }
                    if (flock.moneyAmount - cost >= 0){ //can afford to spawn
                        flock.CreateUnit( //spawn troops in formation
                            agentPrefab,
                            FinalWorldPos,
                            Quaternion.Euler(Vector3.up),
                            unitType
                        );
                        switchSide *= -1;
                    }
                }
                flock.moneyAmount -= (amountOfTroops * cost); //reduce money appropriately
                money.text = "Money: " + flock.moneyAmount.ToString(); //update money text
            }
            else{
                int amount = flock.moneyAmount;
                int x = 0;
                while (amount - cost >= 0){ //dirty solution to calculate how many troops player can afford to spawn 
                    amount -= cost;
                    x++;
                }
                if (flock.moneyAmount == 0) { //if player doesnt have any money left
                    return; 
                }
                else if(x == 0) { //if player doesnt have enough money to spawn a single troop of desired kind
                    //maybe add some indicator to the player here..
                }
                for (int y = 0; y < x; y++){ //spawn the amount of troops that player affords
                    Vector3 FinalWorldPos = new Vector3(worldPos.x + 1.5f * (y % Mathf.RoundToInt(Mathf.Sqrt(x))), worldPos.y, worldPos.z + 1.5f * Mathf.CeilToInt(y / Mathf.RoundToInt(Mathf.Sqrt(x))));
                    //raycast to get the exact y coordinate
                    if (Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer)){ 
                        FinalWorldPos.y = hit.point.y; //location now has proper y coordinate
                    }
                    flock.CreateUnit( //spawn troops in formation
                    agentPrefab,
                    FinalWorldPos,
                    Quaternion.Euler(Vector3.up),
                    unitType
                    );
                    switchSide *= -1;
                }
                flock.moneyAmount -= (x * cost); //reduce money appropriately
                money.text = "Money: " + flock.moneyAmount.ToString();
            }
        }
    }
}