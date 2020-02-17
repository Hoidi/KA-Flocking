using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class EntitySpawning : MonoBehaviour
{
    
    public Flock flock;
    public FlockAgent infantry;
    public Camera cam;
    RaycastHit collisionWithPlane;

    public void spawnEntity(){
        
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        if (Input.GetKeyDown("space")) //shortcut to place units, prone to change(?)
        {
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

            FlockAgent spawnedAgent = Instantiate(infantry, worldPos, transform.rotation); //spawn the troop at cursor location (=+ 1 for y coord)
            flock.agents.Add(spawnedAgent); //add the newly spawned troop into the list of flock members
        
        }
        
    }
}
