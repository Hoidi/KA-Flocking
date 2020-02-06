using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Avoidance")]
public class AvoidanceBehaviour : FlockBehaviour
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock) {
        if (context.Count == 0)
        {
            return Vector3.zero;   
        }

        Vector3 avoidanceMove = Vector3.zero;
        int avoidAmount = 0;
        foreach (Transform item in context)
        {
            if (Vector3.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius) {
                avoidAmount ++;
                avoidanceMove += agent.transform.position - item.position;
            }
        }
        if (avoidAmount > 0) {
            avoidanceMove /= avoidAmount;
        }
        
        return avoidanceMove;
    }
}
