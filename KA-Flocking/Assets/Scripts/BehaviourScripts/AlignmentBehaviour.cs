using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Alignment")]
public class AlignmentBehaviour : FilteredFlockBehaviour
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock) {
        if (context.Count == 0)
        {
            return agent.transform.forward;
        }

        Vector3 aligntmentMove = Vector3.zero;
        float distanceSqr;
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        foreach (Transform item in filteredContext)
        {
            FlockAgent a = item.GetComponentInParent<FlockAgent>();
            distanceSqr = Vector3.SqrMagnitude(item.position - agent.transform.position);
            if (a.GetUnit().GetType().ToString().Equals("Scout") ) {

                if (distanceSqr != 0) aligntmentMove += 20*item.transform.forward/distanceSqr;

            } 
            else {
                if (distanceSqr != 0) aligntmentMove += item.transform.forward/distanceSqr;
            }
        }
        aligntmentMove /= context.Count;
        return aligntmentMove;
    }
}
