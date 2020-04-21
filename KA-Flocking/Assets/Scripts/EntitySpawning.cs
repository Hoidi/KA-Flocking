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
    public ErrorChat errorChat;
    GameObject circleAreaToSpawn;
    GameObject rectAreaToSpawn;
    GameObject triAreaToSpawn;
    GameObject[] formationAreaArray = new GameObject[3];
    public Material spawnAreaColor;
    private Mesh triangleMesh;
    void Start(){
        //if-statement is to get around a null pointer exception in flockscene (since the amount of money each player has isnt relevant the flocking scene)
        if (SceneManager.GetSceneByName("PlayerOneSetupScene").isLoaded || SceneManager.GetSceneByName("PlayerTwoSetupScene").isLoaded) { 
            money.text = "Money: " + flock.moneyAmount.ToString();
            circleAreaToSpawn = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rectAreaToSpawn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            triAreaToSpawn = new GameObject();
            if(SceneManager.GetSceneByName("PlayerOneSetupScene").isLoaded) triAreaToSpawn.transform.Rotate(0, 0, 180, 0); //rotate triangle properly depending on setup scene
            triAreaToSpawn.transform.Rotate(90, -45, 0, 0);
            formationAreaArray[0] = circleAreaToSpawn;
            formationAreaArray[1] = rectAreaToSpawn;
            rectAreaToSpawn.transform.localScale = new Vector3(5, 0, 5); //otherwise its invisible when amountToSpawn is 1
            formationAreaArray[2] = triAreaToSpawn;
            initTriangleIndicator();
            setIndicatorColors(formationAreaArray);
        }
    }
    
    private void Update(){
        // There should only be one
        foreach (Toggle formationType in formationToggles.ActiveToggles()){
            spawnIndicator(char.ToLower(formationType.ToString()[0]));
        }
        if (Input.GetMouseButton(0)){
            StartCoroutine(SpawnWithDelay());
        }
        if (Input.GetMouseButton(1)){ //remove spawning indicator while deleting
            foreach(GameObject formation in formationAreaArray){
                formation.SetActive(false);
            }
        }
        int sum;
        //if-statement is to get around a null pointer exception in flockscene (since the cost of spawning troops isnt relevant in the flocking scene)
        if (SceneManager.GetSceneByName("PlayerOneSetupScene").isLoaded || SceneManager.GetSceneByName("PlayerTwoSetupScene").isLoaded){
            // Check which toggle is on, there should only be one
            foreach (Toggle toggle in troopToggles.ActiveToggles()){
                TroopType troop = toggle.GetComponent<TroopType>();
                // Sum depends on if you spawned your first castle
                sum = (!spawnedFirstCastle && troop.unitType.name.StartsWith("Castle")) ? 0 : amountOfTroops * troop.cost;
                costOfSpawning.text = "Spawning cost: " + sum.ToString();
            }
        }
    }
    private void initTriangleIndicator(){
        triAreaToSpawn.AddComponent<MeshFilter>();
        triAreaToSpawn.AddComponent<MeshRenderer>();
        triangleMesh = triAreaToSpawn.GetComponent<MeshFilter>().mesh;
        triangleMesh.Clear();
        triangleMesh.vertices = new Vector3[] { new Vector3(-2, 0, 2), new Vector3(-2, 0, -2), new Vector3(2, 0, -2) };
        triangleMesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
        triangleMesh.triangles = new int[] { 0, 1, 2 };
    }
    private void setIndicatorColors(GameObject [] formationAreaArray) {
        foreach (GameObject formation in formationAreaArray) {
            formation.GetComponent<Renderer>().material = spawnAreaColor;
            if(formation.GetComponent<Collider>() != null) { // triangle doesnt have collider so this is needed
            formation.GetComponent<Collider>().enabled = false;
            }
            Color color = formation.GetComponent<Renderer>().material.color;
            color.a = 0.1f;
            formation.GetComponent<Renderer>().material.color = color;
        }
    }
    private void spawnIndicator(char formationType) {
        foreach (GameObject formation in formationAreaArray){ //disable indicators by default
            formation.SetActive(false);
        }
        Mesh mesh = triAreaToSpawn.GetComponent<MeshFilter>().mesh;
        Vector3 Area = new Vector3(0, 0, 0);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //raycast to get the exact y coordinate
        if (Physics.Raycast(ray, out RaycastHit spawnIndicator, 10000f, planeLayer)){ //raytracing to acquire position for spawning indicator
            Area = spawnIndicator.point; //convert pixel coordinates to normal coordinates
            float offset = Area.x - (Area.x + 1.75f * ((Mathf.Pow(Mathf.RoundToInt(Mathf.Sqrt(amountOfTroops)), 2) - 1) % Mathf.RoundToInt(Mathf.Sqrt(amountOfTroops))));
            if (Physics.Raycast(new Vector3(Area.x, 100, Area.z), Vector3.down * 100f, out RaycastHit getYPos, Mathf.Infinity, planeLayer)){
                Area.y = getYPos.point.y; //location now has proper y coordinate
                foreach (GameObject formation in formationAreaArray){ //set proper coordinates
                    formation.transform.position = new Vector3(Area.x, Area.y+0.5f, Area.z);
                }
                float circleRadius = amountOfTroops * 0.8f;
                circleAreaToSpawn.transform.localScale = new Vector3(circleRadius, 0, circleRadius);
                rectAreaToSpawn.transform.localScale = new Vector3(offset, 0, offset);
                triangleMesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, amountOfTroops, 0), new Vector3(-amountOfTroops, 0, 0) };
            }   
        }
        //enable relevant indicator
        if (formationType == 'c'){
            circleAreaToSpawn.SetActive(true);
        }
        else if (formationType == 'r') {
            rectAreaToSpawn.SetActive(true);
        }
        else if(formationType == 'a') {
            triAreaToSpawn.SetActive(true);
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
        // user clicked on UI, so dont spawn
        if (EventSystem.current.IsPointerOverGameObject()) return false;
        // already a troop at location
        if (Physics.Raycast(ray, out collisionWithPlane, 10000f, troopLayer | obstacleLayer) || EventSystem.current.IsPointerOverGameObject()){ 
            errorChat.ShowError("Already a unit or obstacle at location");
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
        Vector3 FinalWorldPos = new Vector3(0, 0, 0);
        Vector3 location;
        // Bound the amountOfTroops to the amount that can be afforded
        int maxTroopsAfforded;
        if (cost == 0) {
            maxTroopsAfforded = amountOfTroops;
        } else {
            maxTroopsAfforded = Mathf.Min(amountOfTroops, flock.moneyAmount/cost);
        }
        if (maxTroopsAfforded != amountOfTroops) errorChat.ShowError("All units could not be afforded");
        // Two bools to keep track of if errors should be shown.
        bool errorSpawnside = false;
        bool errorUnitOverlap = false;
        for (int i = 0; i < maxTroopsAfforded; i++){
            if(formationType == 'c') {
                location = Random.insideUnitSphere * maxTroopsAfforded * 0.4f;
                FinalWorldPos = worldPos + location;
            }
            else if(formationType == 'r') {
                float xoffset = worldPos.x + 1.5f * ((Mathf.Pow(Mathf.RoundToInt(Mathf.Sqrt(maxTroopsAfforded)), 2) - 1) % Mathf.RoundToInt(Mathf.Sqrt(maxTroopsAfforded))); //x pos of last troop of largest square
                float zoffset = worldPos.z + 1.5f * Mathf.CeilToInt((Mathf.Pow(Mathf.RoundToInt(Mathf.Sqrt(maxTroopsAfforded)), 2) - 1) / Mathf.RoundToInt(Mathf.Sqrt(maxTroopsAfforded)));//z pos of last troop of largest square
                FinalWorldPos = new Vector3(
                    worldPos.x + 1.5f * (i % Mathf.RoundToInt(Mathf.Sqrt(maxTroopsAfforded))) + (worldPos.x - xoffset) / 2, 
                    worldPos.y,
                    worldPos.z + 1.5f * Mathf.CeilToInt(i / Mathf.RoundToInt(Mathf.Sqrt(maxTroopsAfforded))) + (worldPos.z - zoffset) / 2.4f);
            }
            else if(formationType == 'a') {
                FinalWorldPos = new Vector3(worldPos.x - (i * arrowDirection*-1) * 0.7f, worldPos.y, worldPos.z + (i * switchSide) * 0.7f); //spawn location
            }
            // Only spawn if the location is on the correct piece of land
            if (FinalWorldPos.x * arrowDirection < 0) {
                errorSpawnside = true;
                continue;
            }
            //raycast to get the exact y coordinate
            Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer);
            if (!validateColliders(FinalWorldPos, unitType)) {
                errorUnitOverlap = true;
                continue;
            }
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
        if (errorSpawnside) errorChat.ShowError("Invalid Spawnside");
        if (errorUnitOverlap) errorChat.ShowError("Overlap of unit(s)' position");
    }
}