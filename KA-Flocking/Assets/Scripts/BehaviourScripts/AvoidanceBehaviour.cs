using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Avoidance")]
public class AvoidanceBehaviour : FilteredFlockBehaviour
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock) {
        if (context.Count == 0)
        {
            return Vector3.zero;   
        }

        Vector3 avoidanceMove = Vector3.zero;
        int avoidAmount = 0;
        float distanceSqr;
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        foreach (Transform item in filteredContext)
        {
            distanceSqr = Vector3.SqrMagnitude(item.position - agent.transform.position);
            if (distanceSqr < flock.SquareAvoidanceRadius) {
                avoidAmount ++;
                avoidanceMove += 10 * (agent.transform.position - item.position).normalized/distanceSqr;
            }
        }
        if (avoidAmount > 0) {
            avoidanceMove /= avoidAmount;
        }
        
        return avoidanceMove;
    }
}
