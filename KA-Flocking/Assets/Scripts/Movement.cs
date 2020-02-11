using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Rigidbody player;

    public float maxSpeed = 50f;
    Vector3 targetPos = new Vector3(0,0,0);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPosition = this.transform.position;
        if(player.velocity.magnitude > maxSpeed){
        
        }
        else{
            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
                player.AddForce(0, 0, 50);
            }
            else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
                player.AddForce(0, 0, -50);
            } 
            else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
                player.AddForce(-50, 0, 0); 
            }
            else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
                player.AddForce(50, 0, 0);
            }
            else{
                //Vector3 dir = currentPosition - targetPos;
                //player.velocity = new Vector3(0, player.velocity.y, 0);
                player.angularVelocity = Vector3.zero;
                //player.transform
                } 
            }
    }
}
