using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EntityDeletion : MonoBehaviour{

    public Flock flock;
    public FlockAgent infantryPrefab;
    public FlockAgent pikemanPrefab;
    public FlockAgent archerPrefab;
    public FlockAgent scoutPrefab;
    public Camera cam;
    RaycastHit collisionWithTroop;
    int troopLayer = 1 << 8;
    int planeLayer = 1 << 9;
    private int infantryCost = 100;
    private int archerCost = 300;
    private int pikeCost = 200;
    private int scoutCost = 1000;
    public Text money;
    public int radius;

    // Start is called before the first frame update
    private void Start(){
        
    }

    // Update is called once per frame
    private void Update(){

        if (Input.GetAxis("Mouse ScrollWheel") > 0f){ // forward
            radius++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f){ // backwards
            radius--;
        }

        if (Input.GetMouseButton(1)){
            if (SceneManager.GetSceneByName("PlayerOneSetupScene").isLoaded || SceneManager.GetSceneByName("PlayerTwoSetupScene").isLoaded){
                Ray ray = cam.ScreenPointToRay(Input.mousePosition); //ray from camera towards mouse cursor 
                if (Physics.SphereCast(ray.origin, radius, ray.direction, out collisionWithTroop, 10000f, planeLayer)){ //if there is a troop in the area
                    RaycastHit []hits = Physics.SphereCastAll(ray.origin, 10, ray.direction, 10000f, troopLayer); //all troops to be deleted
                    GameObject[] troops;
                    if (hits.Length > 0){
                        troops = new GameObject[hits.Length];
                        for (int i = 0; i < hits.Length; i++){ //get colliders of all troops
                            troops[i] = hits[i].collider.gameObject;
                        }
                        if (SceneManager.GetSceneByName("PlayerOneSetupScene").isLoaded){ //player 1 setup scene -> make sure only flock 1 troops can be deleted
                            foreach (GameObject troop in troops){
                                if(troop.transform.parent.gameObject.name == "Team 1 Flock"){ //make sure you can only delete troops from your own team
                                    if(troop.name.StartsWith("Infantry")){ 
                                        flock.moneyAmount += infantryCost;
                                        money.text = "Money: " + flock.moneyAmount.ToString();
                                    }
                                    else if (troop.name.StartsWith("Pikemen")){
                                        flock.moneyAmount += pikeCost;
                                        money.text = "Money: " + flock.moneyAmount.ToString();
                                    }
                                    else if (troop.name.StartsWith("Archer")){
                                        flock.moneyAmount += archerCost;
                                        money.text = "Money: " + flock.moneyAmount.ToString();
                                    }
                                    else{
                                        flock.moneyAmount += scoutCost;
                                        money.text = "Money: " + flock.moneyAmount.ToString();
                                    }
                                    destroyTroop(troop); //removes the troop
                                }
                            }
                        }
                        else { //player 2 setup scene -> make sure only flock 2 troops can be deleted
                            foreach (GameObject troop in troops){
                                if(troop.transform.parent.gameObject.name == "Team 2 Flock"){ //make sure you can only delete troops from your own team
                                    if(troop.name.StartsWith("Infantry")){ 
                                        flock.moneyAmount += infantryCost;
                                        money.text = "Money: " + flock.moneyAmount.ToString();
                                    }
                                    else if (troop.name.StartsWith("Pikemen")){
                                        flock.moneyAmount += pikeCost;
                                        money.text = "Money: " + flock.moneyAmount.ToString();
                                    }
                                    else if (troop.name.StartsWith("Archer")){
                                        flock.moneyAmount += archerCost;
                                        money.text = "Money: " + flock.moneyAmount.ToString();
                                    }
                                    else{
                                        flock.moneyAmount += scoutCost;
                                        money.text = "Money: " + flock.moneyAmount.ToString();
                                    }
                                    destroyTroop(troop); //removes the troop
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void destroyTroop(GameObject troop) {
        troop.SetActive(false);
        flock.agents.Remove(troop.GetComponent<FlockAgent>());
    }
}
