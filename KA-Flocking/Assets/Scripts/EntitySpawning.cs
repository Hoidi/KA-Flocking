using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEditor;

public class EntitySpawning : MonoBehaviour
{

    public Flock flock;
    public FlockAgent[] troopPrefabs;
    public Unit[] defaultTroopObjects;
    public int[] troopCosts;
    public Camera cam;
    RaycastHit collisionWithPlane;
    public Toggle[] troopToggles;
    public Toggle CircularToggle;
    public Toggle RectangularToggle;
    public Toggle ArrowToggle;
    public Slider troopSlider;
    int troopLayer = 1 << 8;
    int planeLayer = 1 << 9;
    int obstacleLayer = 1 << 10;
    private int amountOfTroops = 1;
    public Text troopText;
    private bool inSpawningMethod = false;
    public Text money;
    public Text costOfSpawning;


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
            // Check which toggle is on
            for (int i = 0; i < troopToggles.Length; i++)
            {
                if (troopToggles[i].isOn) {
                    sum = amountOfTroops * troopCosts[i];
                    costOfSpawning.text = "Spawning cost: " + sum.ToString();
                }
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
        for (int i = 0; i < troopPrefabs.Length; i++)
        {
            if (troopToggles[i].isOn) {
                troopSpawning(troopPrefabs[i], defaultTroopObjects[i], troopCosts[i], "circle");
            }
        }
    }
    private void spawnRectangle(){
        for (int i = 0; i < troopPrefabs.Length; i++)
        {
            if (troopToggles[i].isOn) {
                troopSpawning(troopPrefabs[i], defaultTroopObjects[i], troopCosts[i], "rect");
            }
        }
    }

    private void spawnTriangle(){
        for (int i = 0; i < troopPrefabs.Length; i++)
        {
            if (troopToggles[i].isOn) {
                troopSpawning(troopPrefabs[i], defaultTroopObjects[i], troopCosts[i], "tri");
            }
        }
    }

    private bool validateLocation(Ray ray) {
        if (Input.GetMouseButton(0)){ //shortcut to place units, prone to change(?)
            if (Physics.Raycast(ray, out collisionWithPlane, 10000f, troopLayer | obstacleLayer) || EventSystem.current.IsPointerOverGameObject()){ //already a troop at location, or user clicked on UI, so dont spawn
                return false;
            }
            return true;
        }
        return false;
    }

    private void troopSpawning(FlockAgent agentPrefab, Unit unitType, int cost, string spawningType){
        Ray ray = cam.ScreenPointToRay(Input.mousePosition); //ray from camera towards mouse cursor 
        if (!validateLocation(ray)) return;
        Vector3 worldPos = new Vector3(0, 0, 0);
        if (Physics.Raycast(ray, out collisionWithPlane, 10000f, planeLayer)){ //raytracing to acquire spawn location
            worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
        }
        int switchSide = 1; //variable to make spawning on each "side" of the arrow shape possible..
        int arrowDirection = -1; //arrow formation should be in opposite direction for the two players
        if (SceneManager.GetActiveScene().name == "PlayerOneSetupScene") arrowDirection *= -1;
        Vector3 FinalWorldPos = new Vector3(0, 0, 0);
        Vector3 location;
        // Bound the amountOfTroops to the amount that can be afforded
        amountOfTroops = Mathf.Min(amountOfTroops, flock.moneyAmount/cost);
        for (int i = 0; i < amountOfTroops; i++){
            if(spawningType == "circle") {
                location = Random.insideUnitSphere * amountOfTroops * 0.4f;
                FinalWorldPos = worldPos + location;
            }
            else if(spawningType == "rect") {
                FinalWorldPos = new Vector3(worldPos.x + 1.5f * (i % Mathf.RoundToInt(Mathf.Sqrt(amountOfTroops))), worldPos.y, worldPos.z + 1.5f * Mathf.CeilToInt(i / Mathf.RoundToInt(Mathf.Sqrt(amountOfTroops))));
            }
            else{
                FinalWorldPos = new Vector3(worldPos.x + (i * switchSide), worldPos.y, worldPos.z - i * arrowDirection); //spawn location
            }
            //raycast to get the exact y coordinate
            if (Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer)){
                FinalWorldPos.y = hit.point.y; //location now has proper y coordinate
                flock.CreateUnit( //spawn troops in formation
                    agentPrefab,
                    FinalWorldPos,
                    Quaternion.Euler(Vector3.up),
                    unitType
                    );
                switchSide *= -1;
                flock.moneyAmount -= cost; //reduce money appropriately
                money.text = "Money: " + flock.moneyAmount.ToString();
            }
            else return; //spawned too close to a wall
        }
    }
}