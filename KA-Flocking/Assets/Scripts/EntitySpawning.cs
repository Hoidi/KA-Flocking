using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class EntitySpawning : MonoBehaviour
{
    
    public Flock flock;
    public FlockAgent infantry;
    public Camera cam;
    RaycastHit collisionWithPlane;
    public Toggle infantryToggle;
    public Toggle pikeToggle;
    public Toggle archerToggle;
    public Slider troopSlider;
    private int amountOfTroops = 1;
    public Text troopText;
    public void setTroopAmount()
    {
        amountOfTroops = (int) troopSlider.value;
        troopText.text = amountOfTroops.ToString();

    }
    public void spawnCircle(){
        if(infantryToggle.isOn){
            spawnEntitiesCircular(infantry);
        }
        else if(pikeToggle.isOn){
            //spawnEntitiesCircular(pike);
        }
        else if(archerToggle.isOn){
            //spawnEntitiesCircular(archer);
        }
    }

    public void spawnRectangle(){
        if(infantryToggle.isOn){
            spawnEntitiesRectangular(infantry);
        }
        else if(pikeToggle.isOn){
            //spawnEntitiesRectangular(pike);
        }
        else if(archerToggle.isOn){
            //spawnEntitiesRectangular(archer);
        }
    }

    public void spawnTriangle(){
        if(infantryToggle.isOn){
            spawnEntitiesTriangular(infantry);
        }
        else if(pikeToggle.isOn){
            //spawnEntitiesTriangular(pike);
        }
        else if(archerToggle.isOn){
            //spawnEntitiesTriangular(archer);
        }
    }
    public void spawnEntitiesCircular(FlockAgent troop){
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        if (Input.GetKey("space")){ //shortcut to place units, prone to change(?)
            Vector3 worldPos;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); //raytracing towards mouse cursor from camera
            if (Physics.Raycast(ray, out collisionWithPlane, 100f)){ //raytracing collided with something
                worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
                worldPos.y += 1f; //make sure they dont fall through the ground
                }
            else{ //no collision
                worldPos =cam.ScreenToWorldPoint(mousePos);
                worldPos.y += 1f; //make sure they dont fall through the ground
                }

            for (int i = 0; i < amountOfTroops; i++)
            {
                Vector3 location = Random.insideUnitSphere * amountOfTroops*0.3f;
                location.y = 0;

                flock.agents.Add(Instantiate(troop, worldPos + location, Quaternion.Euler(Vector3.up)));
            }
                //FlockAgent spawnedAgent = Instantiate(troop, worldPos, transform.rotation); //spawn the troop at cursor location (=+ 1 for y coord)
                //flock.agents.Add(spawnedAgent); //add the newly spawned troop into the list of flock members
            }
    }

    public void spawnEntitiesRectangular(FlockAgent troop){
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        
        if (Input.GetKey("space")){ //shortcut to place units, prone to change(?)
            Vector3 worldPos;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); //raytracing towards mouse cursor from camera
            if (Physics.Raycast(ray, out collisionWithPlane, 100f))
            { //raytracing collided with something
                worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
                worldPos.y += 1f; //make sure they dont fall through the ground
            }
            else
            { //no collision
                worldPos = cam.ScreenToWorldPoint(mousePos);
                worldPos.y += 1f; //make sure they dont fall through the ground
            }
            flock.agents.Add(Instantiate(troop, new Vector3(worldPos.x, worldPos.y, worldPos.z), transform.rotation));
            for (int i = 1; i < amountOfTroops; i++){
                flock.agents.Add(Instantiate(troop, new Vector3(worldPos.x + 2.5f * (i % 5), worldPos.y, worldPos.z + 2.5f * Mathf.CeilToInt(i / 5)), transform.rotation)); //spawn troops in formation

            }

        }
    }

    public void spawnEntitiesTriangular(FlockAgent troop){
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);

        if (Input.GetKey("space"))
        { //shortcut to place units, prone to change(?)
            Vector3 worldPos;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); //raytracing towards mouse cursor from camera
            if (Physics.Raycast(ray, out collisionWithPlane, 100f))
            { //raytracing collided with something
                worldPos = collisionWithPlane.point; //convert pixel coordinates to normal coordinates
                worldPos.y += 1f; //make sure they dont fall through the ground
            }
            else
            { //no collision
                worldPos = cam.ScreenToWorldPoint(mousePos);
                worldPos.y += 1f; //make sure they dont fall through the ground
            }
            for (int i = 0; i < amountOfTroops; i++){
                flock.agents.Add(Instantiate(troop, new Vector3(worldPos.x + 5 * (i % 2), worldPos.y, worldPos.z - 5 * (i % 5)), transform.rotation)); //spawn troops in formation

            }  
        }

    }
}
