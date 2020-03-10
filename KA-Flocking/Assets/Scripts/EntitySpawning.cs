using UnityEngine;
using UnityEngine.UI;



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
    public Slider troopSlider;
    int planeLayer = 1 << 9;
    private int amountOfTroops = 1;
    public Text troopText;
    public void setTroopAmount()
    {
        amountOfTroops = (int)troopSlider.value;
        troopText.text = amountOfTroops.ToString();

    }
    public void spawnCircle(){
        if(infantryToggle.isOn){
            spawnEntitiesCircular(infantryPrefab, defaultInfantryObject);
        }
        else if (pikeToggle.isOn)
        {
            //spawnEntitiesCircular(pike);
        }
        else if (archerToggle.isOn)
        {
            //spawnEntitiesCircular(archer);
        }
    }
    public void spawnRectangle(){
        if(infantryToggle.isOn){
            spawnEntitiesRectangular(infantryPrefab, defaultInfantryObject);
        }
        else if (pikeToggle.isOn)
        {
            //spawnEntitiesRectangular(pike);
        }
        else if (archerToggle.isOn)
        {
            //spawnEntitiesRectangular(archer);
        }
    }

    public void spawnTriangle(){
        if(infantryToggle.isOn){
            spawnEntitiesTriangular(infantryPrefab, defaultInfantryObject);
        }
        else if (pikeToggle.isOn)
        {
            //spawnEntitiesTriangular(pike);
        }
        else if (archerToggle.isOn)
        {
            //spawnEntitiesTriangular(archer);
        }
    }

    public void spawnEntitiesCircular(FlockAgent agentPrefab, Unit unitType){
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        if (Input.GetKey("space"))
        { //shortcut to place units, prone to change(?)
            Vector3 worldPos = new Vector3(0,0,0);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); //ray from camera towards mouse cursor 
            if (Physics.Raycast(ray, out collisionWithPlane, 100f)){ //raytracing to acquire spawn location
                worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
            }
            Vector3 location;
            for (int i = 0; i < amountOfTroops; i++){
                location = Random.insideUnitSphere * amountOfTroops * 0.8f;
                Vector3 FinalWorldPos = worldPos + location;
                if (Physics.Raycast(new Vector3(FinalWorldPos.x, 100, FinalWorldPos.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer)) { //raycast to get the exact y coordinate
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
            if (Physics.Raycast(ray, out collisionWithPlane, 100f)){ //raytracing to acquire spawn location
                worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
            }
            for (int i = 0; i < amountOfTroops; i++) {
                Vector3 FinalWorldPos = new Vector3(worldPos.x + 3 * (i % 5), worldPos.y, worldPos.z + 3 * Mathf.CeilToInt(i / 5));
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
    public void spawnEntitiesTriangular(FlockAgent agentPrefab, Unit unitType){
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        if (Input.GetKey("space")){ //shortcut to place units, prone to change(?)
            Vector3 worldPos = new Vector3(0, 0, 0);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); //ray from camera towards mouse cursor 
            if (Physics.Raycast(ray, out collisionWithPlane, 100f)){ //raytracing to acquire spawn location
                worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
            }
            int switchSide = 1; //variable to make spawning on each "side" of the arrow shape possible..
            for (int i = 0; i < amountOfTroops; i++){
                Vector3 FinalWorldPos = new Vector3(worldPos.x + (2 * i * switchSide), worldPos.y, worldPos.z - (2 * i)); //spawn location 
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