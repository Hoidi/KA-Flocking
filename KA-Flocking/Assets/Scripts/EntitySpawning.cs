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
    bool isButtonSelected = false;
    IEnumerator<WaitForSeconds> newCoroutine(){
        yield return new WaitForSeconds(1);
    }
    public void spawnOne(){
        if(infantryToggle.isOn){
            spawnEntity(infantry);
        }
        else if(pikeToggle.isOn){
            //spawnEntity(pike);
        }
        else if(archerToggle.isOn){
            //spawnEntity(archer);
        }
    }

    public void spawnFive(){
        if(infantryToggle.isOn){
            spawnFiveEntites(infantry);
        }
        else if(pikeToggle.isOn){
            //spawnFiveEntites(pike);
        }
        else if(archerToggle.isOn){
            //spawnFiveEntites(archer);
        }
    }

    public void spawnTriangleShape(){
        if(infantryToggle.isOn){
            spawnTriangleShapedEntites(infantry);
        }
        else if(pikeToggle.isOn){
            //spawnTriangleShapedEntites(pike);
        }
        else if(archerToggle.isOn){
            //spawnTriangleShapedEntites(archer);
        }
    }
    public void spawnEntity(FlockAgent troop){
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

            FlockAgent spawnedAgent = Instantiate(troop, worldPos, transform.rotation); //spawn the troop at cursor location (=+ 1 for y coord)
            flock.agents.Add(spawnedAgent); //add the newly spawned troop into the list of flock members
            StartCoroutine(newCoroutine());
        
        }
    }

    public void spawnFiveEntites(FlockAgent troop){


    }

    public void spawnTriangleShapedEntites(FlockAgent troop){

        
    }
}
