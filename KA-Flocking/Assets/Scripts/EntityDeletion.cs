using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EntityDeletion : MonoBehaviour{

    public Flock flock;
    public TroopType[] troopTypes;
    public Camera cam;
    RaycastHit collisionWithTroop;
    int troopLayer = 1 << 8;
    int planeLayer = 1 << 9;
    public Text money;
    private int radius = 5;
    GameObject areaToDelete;
    public Material areaColor;

    // Start is called before the first frame update
    private void Start(){
        if (SceneManager.GetSceneByName("PlayerOneSetupScene").isLoaded) { 
            flock = GameObject.Find("Team 1 Flock").GetComponent<Flock>();
        } else if (SceneManager.GetSceneByName("PlayerTwoSetupScene").isLoaded) {
            flock = GameObject.Find("Team 2 Flock").GetComponent<Flock>();	
        }
        areaToDelete = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        areaToDelete.transform.localScale = new Vector3(radius, 0, radius);
        areaToDelete.GetComponent<Renderer>().material = areaColor;
        areaToDelete.GetComponent<Collider>().enabled = false;
        Color color = areaToDelete.GetComponent<Renderer>().material.color;
        color.a = 0.1f;
        areaToDelete.GetComponent<Renderer>().material.color = color;
    }

    // Update is called once per frame
    private void Update(){
        if (Input.GetAxis("Mouse ScrollWheel") > 0f){ // forward
            if (radius < 25){
                radius++;
                areaToDelete.transform.localScale = new Vector3(radius, 0, radius);
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f){ // backwards
            if (radius > 5){
                radius--;
                areaToDelete.transform.localScale = new Vector3(radius, 0, radius);
            }
        }
        areaToDelete.SetActive(false);
        if (Input.GetMouseButton(1)){
            areaToDelete.SetActive(true);
            if (SceneManager.GetSceneByName("PlayerOneSetupScene").isLoaded || SceneManager.GetSceneByName("PlayerTwoSetupScene").isLoaded){
                Ray ray = cam.ScreenPointToRay(Input.mousePosition); //ray from camera towards mouse cursor 
                if (Physics.SphereCast(ray.origin, radius, ray.direction, out collisionWithTroop, 10000f, planeLayer)){ //if there is a troop in the area
                    RaycastHit[] hits = null;
                    GameObject[] troopArray;
                    Vector3 Area = new Vector3(0, 0, 0);
                    //raycast to get the exact y coordinate
                    if (Physics.Raycast(ray, out RaycastHit deletionIndicator, 10000f, planeLayer)){ //raytracing to acquire position for deletion indicator
                        Area = deletionIndicator.point; //convert pixel coordinates to normal coordinates
                        if (Physics.Raycast(new Vector3(Area.x, 100, Area.z), Vector3.down * 100f, out RaycastHit getYPos, Mathf.Infinity, planeLayer)){
                            hits = Physics.SphereCastAll(new Vector3(Area.x, 100, Area.z), radius * 0.5f, Vector3.down * 1000f, 10000f, troopLayer); //all troops to be deleted
                            Area.y = getYPos.point.y; //location now has proper y coordinate
                            areaToDelete.transform.position = new Vector3(Area.x, Area.y + 1, Area.z);
                        }
                    }
                    if (hits.Length > 0){
                        troopArray = new GameObject[hits.Length];
                        for (int i = 0; i < hits.Length; i++){ //get colliders of all troops
                            troopArray[i] = hits[i].collider.gameObject;
                        }
                        removeTroops(troopArray); //delete troops inside area
                    }
                }
            }
        }
    }
    private void destroyTroop(GameObject troop){
        troop.SetActive(false);
        flock.agents.Remove(troop.GetComponent<FlockAgent>());
    }

    private void removeTroops(GameObject[] troops){
        foreach (GameObject troop in troops) {
            //make sure you can only delete troops from your own team
            if (troop.transform.parent.gameObject.name == flock.name) {
                foreach (TroopType type in troopTypes)
                {
                    if (troop.name.StartsWith(type.unitType.name)) {
                        flock.moneyAmount += type.cost;
                        money.text = "Money: " + flock.moneyAmount.ToString();
                        destroyTroop(troop); //removes the troop
                    }
                }
            }
        }
    }
}
