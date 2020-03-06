using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/AttackBehaviour")]
public class AttackBehaviour : FilteredFlockBehaviour
{
    Vector3 currentVelocity;
    public float agentSmoothTime = 0.5f;
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 attackMove = Vector3.zero;
        float distanceSqr;
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        foreach (Transform item in filteredContext)
        {
            distanceSqr = Vector3.SqrMagnitude(item.position - agent.transform.position);
            attackMove += item.position / distanceSqr;
        }
        attackMove /= context.Count;
        attackMove -= agent.transform.position;
        attackMove = Vector3.SmoothDamp(agent.transform.forward, attackMove, ref currentVelocity, agentSmoothTime, flock.maxSpeed);
        return attackMove;
    }
}
