using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Alignment")]
public class AlignmentBehaviour : FlockBehaviour
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock) {
        if (context.Count == 0)
        {
            return agent.transform.forward;
        }

        Vector3 aligntmentMove = Vector3.zero;
        float distanceSqr;
        foreach (Transform item in context)
        {
            distanceSqr = Vector3.SqrMagnitude(item.position - agent.transform.position);
            aligntmentMove += item.transform.forward/distanceSqr;
        }
        aligntmentMove /= context.Count;
        
        return aligntmentMove;
    }
}
