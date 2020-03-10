using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;



public class EntitySpawning : MonoBehaviour
{

    public Flock flock;
    public FlockAgent infantryPrefab;
    public Infantry defaultInfantryObject;
    public Camera cam;
    RaycastHit collisionWithPlane;
    public Toggle infantryToggle;
    public Toggle pikeToggle;
    public Toggle archerToggle;
    public Toggle CircularToggle;
    public Toggle RectangularToggle;
    public Toggle ArrowToggle;
    public Slider troopSlider;
    int troopLayer = 1 << 8;
    int planeLayer = 1 << 9;
    private int amountOfTroops = 1;
    public Text troopText;
    private bool inSpawningMethod = false;

    private void Update(){
        if (Input.GetKey(KeyCode.Space)){
            StartCoroutine(wait());
        }
    }
    IEnumerator wait(){
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
    public void setTroopAmount(){
        amountOfTroops = (int)troopSlider.value;
        troopText.text = amountOfTroops.ToString();

    }
    public void spawnCircle(){
        if (infantryToggle.isOn){
            spawnEntitiesCircular(infantryPrefab, defaultInfantryObject);
        }
        else if (pikeToggle.isOn){
            //spawnEntitiesCircular(pike);
        }
        else if (archerToggle.isOn){
            //spawnEntitiesCircular(archer);
        }
    }
    public void spawnRectangle(){
        if (infantryToggle.isOn){
            spawnEntitiesRectangular(infantryPrefab, defaultInfantryObject);
        }
        else if (pikeToggle.isOn){
            //spawnEntitiesRectangular(pike);
        }
        else if (archerToggle.isOn){
            //spawnEntitiesRectangular(archer);
        }
    }

    public void spawnTriangle(){
        if (infantryToggle.isOn){
            spawnEntitiesTriangular(infantryPrefab, defaultInfantryObject);
        }
        else if (pikeToggle.isOn){
            //spawnEntitiesTriangular(pike);
        }
        else if (archerToggle.isOn){
            //spawnEntitiesTriangular(archer);
        }
    }
    
    public void spawnEntitiesCircular(FlockAgent agentPrefab, Unit unitType){
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        if (Input.GetKey("space")){ //shortcut to place units, prone to change(?)
            Vector3 worldPos = new Vector3(0, 0, 0);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); //ray from camera towards mouse cursor 
            if (Physics.Raycast(ray, out collisionWithPlane, 10000f, troopLayer)){ //already a troop at location, so dont spawn
                return;
            }
            if (Physics.Raycast(ray, out collisionWithPlane, 10000f, planeLayer)){ //raytracing to acquire spawn location
                
                worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
            }
            Vector3 location;
            for (int i = 0; i < amountOfTroops; i++){
                location = Random.insideUnitSphere * amountOfTroops * 0.4f;
                Vector3 FinalWorldPos = worldPos + location;
                if (Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer)){ //raycast to get the exact y coordinate
                    FinalWorldPos.y = hit.point.y; //location now has proper y coordinate
                }
                flock.CreateUnit( //spawn troops in formation
                    agentPrefab,
                    FinalWorldPos,
                    Quaternion.Euler(Vector3.up),
                    unitType
                );
            }
        }
    }
    public void spawnEntitiesRectangular(FlockAgent agentPrefab, Unit unitType){
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        if (Input.GetKey("space")){ //shortcut to place units, prone to change(?)
            Vector3 worldPos = new Vector3(0, 0, 0);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); //ray from camera towards mouse cursor 
            if (Physics.Raycast(ray, out collisionWithPlane, 10000f, troopLayer)){ //already a troop at location, so dont spawn
                return;
            }
            if (Physics.Raycast(ray, out collisionWithPlane, 10000f, planeLayer)){ //raytracing to acquire spawn location
                worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
            }
            for (int i = 0; i < amountOfTroops; i++){
                Vector3 FinalWorldPos = new Vector3(worldPos.x + 1.5f * (i % Mathf.RoundToInt(Mathf.Sqrt(amountOfTroops))), worldPos.y, worldPos.z + 1.5f * Mathf.CeilToInt(i / Mathf.RoundToInt(Mathf.Sqrt(amountOfTroops))));
                if (Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer)){ //raycast to get the exact y coordinate
                    FinalWorldPos.y = hit.point.y; //location now has proper y coordinate
                }
                flock.CreateUnit( //spawn troops in formation
                    agentPrefab,
                    FinalWorldPos,
                    Quaternion.Euler(Vector3.up),
                    unitType
                );
            }

        }

    }
    public void spawnEntitiesTriangular(FlockAgent agentPrefab, Unit unitType)
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        if (Input.GetKey("space")){ //shortcut to place units, prone to change(?)
            Vector3 worldPos = new Vector3(0, 0, 0);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); //ray from camera towards mouse cursor 
            if (Physics.Raycast(ray, out collisionWithPlane, 10000f, troopLayer)){ //already a troop at location, so dont spawn
                return;
            }
            if (Physics.Raycast(ray, out collisionWithPlane, 10000f, planeLayer)){ //raytracing to acquire spawn location
                worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
            }
            int switchSide = 1; //variable to make spawning on each "side" of the arrow shape possible..
            int arrowDirection = -1; //arrow formation should be in opposite direction for the two players
            if (SceneManager.GetActiveScene().name == "PlayerOneSetupScene") arrowDirection *= -1;
            for (int i = 0; i < amountOfTroops; i++){
                Vector3 FinalWorldPos = new Vector3(worldPos.x + (i * switchSide), worldPos.y, worldPos.z - i * arrowDirection); //spawn location 
                if (Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer)){ //raycast to get the exact y coordinate
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

        }
    }
}