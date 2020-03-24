using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/FightOrFlight")]
public class FightOrFlightBehaviour : FlockBehaviour
{
    Vector3 enemiesDirection = Vector3.zero;
    Vector3 friendsDirection = Vector3.zero; //is kept to enable more advanced behaviour in the future. 

    float enemiesStrength = 0f;
    float friendsStrength = 1f;
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0)
        {
            return Vector3.zero;
        }

        CalculateDirections(agent, context, flock);

        if (enemiesStrength > friendsStrength)
        {
            return enemiesDirection.normalized * -1;
        }
        else
        {
            return enemiesDirection.normalized;
        }
    }

    void CalculateDirections(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector3 cohesionMove = Vector3.zero;
        friendsDirection = Vector3.zero;
        enemiesDirection = Vector3.zero;
        friendsStrength = 1f;
        enemiesStrength = 0f;

        float distance;
        foreach (Transform item in context)
        {
            FlockAgent itemAgent = item.GetComponent<FlockAgent>();
            if (itemAgent != null)
            {
                distance = Vector3.Magnitude(item.position - agent.transform.position);
                if (itemAgent.AgentFlock == flock)
                {
                    friendsDirection += item.position - agent.transform.position;
                    friendsStrength += 1f / distance;
                }
                else
                {
                    enemiesDirection += item.position - agent.transform.position;
                    enemiesStrength += 1f / distance;
                }
            }
        }
    }
}
