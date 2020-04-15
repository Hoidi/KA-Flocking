using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Siege")]
public class SiegeBehaviour : FlockBehaviour
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock) {
        GameObject[] castleObjects = GameObject.FindGameObjectsWithTag("Castle");
        
        // Distance to closest enemy castle
        float closestSqrDistance = Mathf.Infinity;
        // Direction to closest enemy castle
        Vector3 closestDirection = Vector3.zero;
        foreach (GameObject castle in castleObjects)
        {
            if (castle.GetComponent<FlockAgent>() != flock) {
                Vector3 closestPoint = castle.GetComponent<Collider>().ClosestPoint(agent.transform.position);
                Vector3 direction = closestPoint - agent.transform.position;
                if (Vector3.SqrMagnitude(direction) < closestSqrDistance) {
                    closestSqrDistance = Vector3.SqrMagnitude(direction);
                    closestDirection = direction;
                }
            }
        }

        return closestDirection;
    }
}
