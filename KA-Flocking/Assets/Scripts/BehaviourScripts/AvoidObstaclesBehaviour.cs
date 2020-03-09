using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/AvoidObstacles")]
public class AvoidObstaclesBehaviour : FilteredFlockBehaviour
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
            var collider = item.GetComponent<Collider>();
            var closestPoint = collider.ClosestPoint(agent.transform.position);
            Debug.DrawRay(agent.transform.position,closestPoint - agent.transform.position, Color.red, 0.1f);
            distanceSqr = Vector3.SqrMagnitude(closestPoint - agent.transform.position);
            if (distanceSqr < flock.SquareAvoidanceRadius) {
                avoidAmount ++;
                avoidanceMove += (agent.transform.position - closestPoint)/distanceSqr;
            }
        }
        if (avoidAmount > 0) {
            avoidanceMove /= avoidAmount;
        }
        
        return avoidanceMove;
    }
}
