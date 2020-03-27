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
    RaycastHit collisionWithPlane;
    int troopLayer = 1 << 8;
    int planeLayer = 1 << 9;
    private int infantryCost = 100;
    private int archerCost = 300;
    private int pikeCost = 200;
    private int scoutCost = 1000;
    public Text money;

    // Start is called before the first frame update
    private void Start(){
        
    }

    // Update is called once per frame
    private void Update(){
        if (Input.GetMouseButtonDown(1)){
            if (SceneManager.GetSceneByName("PlayerOneSetupScene").isLoaded || SceneManager.GetSceneByName("PlayerTwoSetupScene").isLoaded){
                Ray ray = cam.ScreenPointToRay(Input.mousePosition); //ray from camera towards mouse cursor 
                if (Physics.Raycast(ray, out collisionWithPlane, 10000f, troopLayer)){ //raycast to find troop to delete
                    GameObject troop = collisionWithPlane.collider.gameObject;
                    if(troop.name.StartsWith("Infantry")) {
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
    private void destroyTroop(GameObject troop) {
        troop.SetActive(false);
        flock.agents.Remove(troop.GetComponent<FlockAgent>());
    }
}
