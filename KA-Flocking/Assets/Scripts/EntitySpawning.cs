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
        if (Input.GetKeyDown("space"))
        {
            Vector3 worldPos;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out collisionWithPlane, 100f)){
                worldPos = collisionWithPlane.point;
                worldPos.y += 1f; //make sure they dont fall through the ground
            }
            else{
                worldPos =cam.ScreenToWorldPoint(mousePos);
                worldPos.y += 1f; //make sure they dont fall through the ground
            }

            FlockAgent spawnedAgent = Instantiate(infantry, worldPos, transform.rotation);
            flock.agents.Add(spawnedAgent);
        
        }
        
    }
}
