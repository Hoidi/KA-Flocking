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
    public ErrorChat errorChat;
    [System.NonSerialized]
    public GameObject spawningWindow;
    private bool isFirstTurn;
    [Range(0f,100f)]
    public float requiredCastleRange = 15f;
    // 3 variables to help change the material when selecting a castle
    public Material previewMaterial;
    private Material previousMaterial;
    private FlockAgent previousCastle = null;

    void Start(){
        //if-statement is to get around a null pointer exception in flockscene (since the amount of money each player has isnt relevant the flocking scene)
        if (SceneManager.GetSceneByName("PlayerOneSetupScene").isLoaded) { 
            flock = GameObject.Find("Team 1 Flock").GetComponent<Flock>();
        } else if (SceneManager.GetSceneByName("PlayerTwoSetupScene").isLoaded) {
            flock = GameObject.Find("Team 2 Flock").GetComponent<Flock>();	
        }
        money.text = "Money: " + flock.moneyAmount.ToString(); 

        // Find and disable the spawn queue
        spawningWindow = GameObject.Find("CastleSelection");
        spawningWindow.SetActive(false);

        // Check if this is the first turn
        isFirstTurn = !flock.spawnedFirstCastle;
    }

    void OnDestroy() {
        // Replace back the material of the last selected castle
        if (previousCastle != null) replaceMaterial(previousCastle, previousMaterial);
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
                sum = (!flock.spawnedFirstCastle && troop.unitType is Castle) ? 0 : (amountOfTroops * troop.cost);
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
    // limits the troop amount slider if the castle toggle is active
    public void limitTroopAmount(Toggle toggle){
        troopSlider.maxValue = (toggle.isOn) ? 1 : 30;
    }
    private void spawnTroop(char formation){
        // There should only be one
        foreach (Toggle toggle in troopToggles.ActiveToggles())
        {
            TroopType troop = toggle.GetComponent<TroopType>();
            if (!flock.spawnedFirstCastle && troop.unitType is Castle) {
                troopSpawning(troop.prefab, troop.unitType, 0, formation);
            } else {
                troopSpawning(troop.prefab, troop.unitType, troop.cost, formation);
            }
        }
    }

    private bool selectCastle() {
        FlockAgent flockAgent = collisionWithPlane.transform.gameObject.GetComponent<FlockAgent>();
        // If you already selected the castle, don't do anything
        if (flockAgent == previousCastle) return true;
        if (flockAgent.unit is Castle)
        { 
            // Enable the spawning window
            spawningWindow.SetActive(true);
            Castle selectedCastle = (Castle) flockAgent.unit;
            SpawnQueue currentQueue = spawningWindow.GetComponentInChildren<SpawnQueue>();
            
            // Change back the material of the previous castle
            if (currentQueue.castle != null) {
                replaceMaterial(previousCastle, previousMaterial);
            }

            // Replace the castle
            currentQueue.replaceCastle(selectedCastle);
            replaceMaterial(flockAgent, previewMaterial);
            previousCastle = flockAgent;
            
            // Start spawning routine if not active (if you don't select a castle you can't queue units, so there's no problem that it's not guaranteed to be called upon)
            if (selectedCastle.spawning == false) flock.StartSpawning(selectedCastle, flockAgent, flock);
            return true;
        }
        return false;
    }

    // Replaces the material of all MeshRenderers in the agent's children
    private void replaceMaterial(FlockAgent agent, Material material) {
        MeshRenderer[] meshes = agent.GetComponentsInChildren<MeshRenderer>();
        previousMaterial = meshes[0].material;
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material = material;
        }
    }

    private bool validateLayers(Ray ray) {
        // user clicked on UI, so dont spawn
        if (EventSystem.current.IsPointerOverGameObject()) return false;
        // already a troop at location
        if (Physics.Raycast(ray, out collisionWithPlane, 10000f, troopLayer)){ 
            if (!selectCastle()) errorChat.ShowError("Already a unit at location");
            if (Physics.Raycast(ray, out collisionWithPlane, 10000f, obstacleLayer)) errorChat.ShowError("An obstacle obstructs the location");
            return false;
        } else if (Physics.Raycast(ray, out collisionWithPlane, 10000f, planeLayer)) {
            return true;
        }
        return false;
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

    private bool validateHeight(Vector3 worldPos) {
        if (worldPos.y > 10) {
            errorChat.ShowError("Cannot spawn on mountains");
            return false;
        }
        return true;
    }

    private int validateAmount(int amountOfTroops, int cost) {
        // Bound the amountOfTroops to the amount that can be afforded
        int maxTroopsAfforded;
        if (cost == 0) {
            maxTroopsAfforded = amountOfTroops;
        } else {
            maxTroopsAfforded = Mathf.Min(amountOfTroops, flock.moneyAmount/cost);
        }
        if (maxTroopsAfforded != amountOfTroops) errorChat.ShowError("All units could not be afforded");
        return maxTroopsAfforded;
    }

    private bool validateClosestCastle(Vector3 FinalWorldPos) {
        foreach (FlockAgent agent in flock.agents)
        {
            if (agent.unit is Castle && (agent.transform.position - FinalWorldPos).sqrMagnitude < requiredCastleRange * requiredCastleRange) {
                return true;
            }
        }
        return false;
    }

    private void troopSpawning(FlockAgent agentPrefab, Unit unitType, int cost, char formationType){
        Ray ray = cam.ScreenPointToRay(Input.mousePosition); //ray from camera towards mouse cursor 
        Vector3 worldPos = new Vector3(0, 0, 0);
        if (Physics.Raycast(ray, out collisionWithPlane, 10000f, planeLayer)){ //raytracing to acquire spawn location
            worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
        }
        if (!validateLayers(ray) || (!validateColliders(worldPos, unitType))) return;
        
        // arrow formation should be in opposite direction for the two players
        int arrowDirection = -1; 
        if (SceneManager.GetActiveScene().name == "PlayerOneSetupScene") arrowDirection *= -1;

        int maxTroopsAfforded = validateAmount(amountOfTroops, cost);
        
        // Three bools to keep track of if errors should be shown.
        bool errorSpawnside, errorUnitOverlap, errorCastleRange;
        errorSpawnside = errorUnitOverlap = errorCastleRange = false;
        for (int i = 0; i < maxTroopsAfforded; i++){
            Vector3 FinalWorldPos = troopSpawningFormation(i, formationType, worldPos, maxTroopsAfforded, arrowDirection);
            if (!isFirstTurn && !validateClosestCastle(FinalWorldPos)) {
                errorCastleRange = true;
                continue;
            }
            // Only spawn if the location is on the correct piece of land
            if (FinalWorldPos.x * arrowDirection < 0) {
                errorSpawnside = true;
                continue;
            }
            //raycast to get the exact y coordinate
            if(Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer)){
            if (!validateColliders(FinalWorldPos, unitType)) {
                errorUnitOverlap = true;
                continue;
            }
            FinalWorldPos.y = hit.point.y; //location now has proper y coordinate
            if (!validateColliders(FinalWorldPos, unitType) || !validateHeight(FinalWorldPos)) continue;
            flock.CreateUnit( //spawn troops in formation
                agentPrefab,
                FinalWorldPos,
                Quaternion.Euler(0, cam.transform.eulerAngles.y, 0),
                unitType
                );
            flock.moneyAmount -= cost; //reduce money appropriately
            money.text = "Money: " + flock.moneyAmount.ToString();
            if (!flock.spawnedFirstCastle && unitType is Castle) flock.spawnedFirstCastle = true;
            }
        }
        if (errorSpawnside) errorChat.ShowError("Invalid Spawnside");
        if (errorUnitOverlap) errorChat.ShowError("Overlap of unit(s)' position");
        if (errorCastleRange) errorChat.ShowError("Units can only be spawned close to castles on consecutive turns");
    }

    private Vector3 troopSpawningFormation(int index, char formationType, Vector3 worldPos, int totalTroopAmount, int arrowDirection)
    {
        Vector3 FinalWorldPos = new Vector3(0, 0, 0);
        Vector3 location;
        switch (formationType) {
            case 'c':
                location = Random.insideUnitSphere * totalTroopAmount * 0.4f;
                FinalWorldPos = worldPos + location;
                break;
            case 'r':
                // x and z pos of last troop of largest square
                float xoffset = worldPos.x + 1.5f * ((Mathf.Pow(Mathf.RoundToInt(Mathf.Sqrt(totalTroopAmount)), 2) - 1) % Mathf.RoundToInt(Mathf.Sqrt(totalTroopAmount)));
                float zoffset = worldPos.z + 1.5f * Mathf.CeilToInt((Mathf.Pow(Mathf.RoundToInt(Mathf.Sqrt(totalTroopAmount)), 2) - 1) / Mathf.RoundToInt(Mathf.Sqrt(totalTroopAmount)));

                FinalWorldPos = new Vector3(
                    worldPos.x + 1.5f * (index % Mathf.RoundToInt(Mathf.Sqrt(totalTroopAmount))) + (worldPos.x - xoffset) / 2, 
                    worldPos.y,
                    worldPos.z + 1.5f * Mathf.CeilToInt(index / Mathf.RoundToInt(Mathf.Sqrt(totalTroopAmount))) + (worldPos.z - zoffset) / 2.4f);
                break;
            case 'a':
                //variable to make spawning on each "side" of the arrow shape possible..
                int switchSide = (index % 2 == 1) ? 1 : (-1); 

                FinalWorldPos = new Vector3(worldPos.x + (index * switchSide), worldPos.y, worldPos.z - index * arrowDirection); //spawn location
                break;
        }
        return FinalWorldPos;
    }
}