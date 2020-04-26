using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEditor;

public class EntitySpawning : MonoBehaviour{

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
    GameObject circleAreaToSpawn;
    GameObject rectAreaToSpawn;
    GameObject triAreaToSpawn;
    GameObject[] formationAreaArray = new GameObject[3];
    public Material spawnAreaColor;
    private Mesh triangleMesh;
    private float triangleXOffset;
    [System.NonSerialized]
    public GameObject spawningWindow;
    private bool isFirstTurn;
    [Range(0f,100f)]
    public float requiredCastleRange = 15f;
    // 3 variables to help change the material when selecting a castle
    public Material previewMaterial;
    private Material previousMaterial;
    private FlockAgent previousCastle = null;
    public Material castleRangePreviewMaterial;

    void Start() {
        //if-statement is to get around a null pointer exception in flockscene (since the amount of money each player has isnt relevant the flocking scene)
        if (SceneManager.GetSceneByName("PlayerOneSetupScene").isLoaded || SceneManager.GetSceneByName("PlayerTwoSetupScene").isLoaded) {
            money.text = "Money: " + flock.moneyAmount.ToString();

            circleAreaToSpawn = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rectAreaToSpawn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            triAreaToSpawn = new GameObject();

            if (SceneManager.GetSceneByName("PlayerTwoSetupScene").isLoaded) triAreaToSpawn.transform.Rotate(0, 0, 180, 0); //rotate triangle properly depending on setup scene

            formationAreaArray[0] = circleAreaToSpawn;
            formationAreaArray[1] = rectAreaToSpawn;
            formationAreaArray[2] = triAreaToSpawn;

            initTriangleIndicator();
            setIndicatorColors(formationAreaArray);

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

            if (!isFirstTurn) {
                // Turn of deletion in consecutive turns
                gameObject.GetComponentInChildren<EntityDeletion>().enabled = false;
                // Generate circles around castles for spawning indication
                foreach (FlockAgent agent in flock.agents)
                {
                    if (agent.unit is Castle) {
                        generateCircle(agent.transform.position);
                    }
                }
            }
        } 
    }

    private void generateCircle(Vector3 position) {
        GameObject areaToDelete;
        areaToDelete = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        areaToDelete.transform.position = position + Vector3.up;
        areaToDelete.transform.localScale = new Vector3(requiredCastleRange * 2, 0, requiredCastleRange * 2);
        areaToDelete.GetComponent<Renderer>().material = castleRangePreviewMaterial;
        areaToDelete.GetComponent<Collider>().enabled = false;
        Color color = areaToDelete.GetComponent<Renderer>().material.color;
        color.a = 0.1f;
        areaToDelete.GetComponent<Renderer>().material.color = color;
    }

    void OnDestroy() {
        // Replace back the material of the last selected castle
        if (previousCastle != null) replaceMaterial(previousCastle, previousMaterial);
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
                sum = (!flock.spawnedFirstCastle && troop.unitType is Castle) ? 0 : (amountOfTroops * troop.cost);
                costOfSpawning.text = "Spawning cost: " + sum.ToString();
            }
        }
    }

    private void initTriangleIndicator(){
        triAreaToSpawn.AddComponent<MeshFilter>();
        triAreaToSpawn.AddComponent<MeshRenderer>();
        triangleMesh = triAreaToSpawn.GetComponent<MeshFilter>().mesh;
        triangleMesh.Clear();
        triangleMesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
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
            color.a = 0.3f;
            formation.GetComponent<Renderer>().material.color = color;
        }
    }

    private void spawnIndicator(char formationType) {
        foreach (GameObject formation in formationAreaArray){ //disable indicators by default
            formation.SetActive(false);
        }
        Vector3 Area = new Vector3(0, 0, 0);
        Mesh mesh = triAreaToSpawn.GetComponent<MeshFilter>().mesh;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //raycast to get the exact y coordinate
        if (Physics.Raycast(ray, out RaycastHit spawnIndicator, 10000f, planeLayer)){ //raytracing to acquire position for spawning indicator
            Area = spawnIndicator.point; //convert pixel coordinates to normal coordinates
            if (Physics.Raycast(new Vector3(Area.x, 100, Area.z), Vector3.down * 100f, out RaycastHit getYPos, Mathf.Infinity, planeLayer)){
                Area.y = getYPos.point.y; //location now has proper y coordinate
                foreach (GameObject formation in formationAreaArray){ //set proper coordinates
                    formation.transform.position = new Vector3(Area.x, Area.y+0.5f, Area.z);
                }
            }
            enableRelevantIndicator(Area, formationType); //enable and resize relevant indicator
        }
    }

    private void enableRelevantIndicator(Vector3 Area, char formationType){
        switch(formationType){ //enable and resize relevant indicator
            case 'c':
                float circleRadius = amountOfTroops * 0.8f;
                circleAreaToSpawn.transform.localScale = new Vector3(circleRadius, 0, circleRadius); 
                circleAreaToSpawn.SetActive(true); 
                break;
            case 'r':
                float offset = Area.x - (Area.x + 1.75f * ((Mathf.Pow(Mathf.RoundToInt(Mathf.Sqrt(amountOfTroops)), 2) - 1) % Mathf.RoundToInt(Mathf.Sqrt(amountOfTroops))));
                if (offset > -1) offset = -1; //required to show a small rectangle indicator for when amountOfTroops = 1
                rectAreaToSpawn.transform.localScale = new Vector3(offset, 0, offset); 
                rectAreaToSpawn.SetActive(true); 
                break;
            case 'a':
                triangleMesh.vertices = new Vector3[] { new Vector3(-amountOfTroops * 0.65f / 2, 0, 0), new Vector3(amountOfTroops * 0.65f, 0, -amountOfTroops * 0.65f), 
                new Vector3(amountOfTroops * 0.65f, 0, amountOfTroops * 0.65f) };
                triAreaToSpawn.SetActive(true);
                break;
        }
    }
    
    IEnumerator SpawnWithDelay(){
        if (!inSpawningMethod){
            inSpawningMethod = true;
            // There should only be one
            foreach (Toggle toggle in formationToggles.ActiveToggles()){
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
        foreach (Toggle toggle in troopToggles.ActiveToggles()){
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
        if (flockAgent.unit is Castle && flockAgent.AgentFlock == flock)
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
        foreach (Collider collider in colliders){
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
            if (!(unitType is Castle) && !isFirstTurn && !validateClosestCastle(FinalWorldPos)) {
                errorCastleRange = true;
                continue;
            }
            // Only spawn if the location is on the correct piece of land
            if (FinalWorldPos.x * arrowDirection < 0) {
                errorSpawnside = true;
                continue;
            }
            //raycast to get the exact y coordinate
            if (Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer)) { 

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
                if (!isFirstTurn && unitType is Castle) generateCircle(FinalWorldPos);
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
                FinalWorldPos = new Vector3(worldPos.x - (index * arrowDirection * -1), worldPos.y, worldPos.z + (index * switchSide) * 0.7f); //spawn location
                if (SceneManager.GetActiveScene().name == "PlayerOneSetupScene") FinalWorldPos.x -= (amountOfTroops * 0.65f / 2); //align spawnlocation to triangle outline
                else FinalWorldPos.x += (amountOfTroops * 0.65f / 2);
                break;
        }
        return FinalWorldPos;
    }
}