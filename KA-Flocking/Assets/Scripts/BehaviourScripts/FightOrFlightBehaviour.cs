using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/FightOrFlight")]
public class FightOrFlightBehaviour : FlockBehaviour
{
    Vector3 enemiesDirection = Vector3.zero;
    Vector3 friendsDirection = Vector3.zero;

    float enemiesStrength = 0f;
    float friendsStrength = 0f;
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0)
        {
            return Vector3.zero;
        }

        calculateDirections(agent, context, flock);

        if (enemiesStrength > friendsStrength)
        {
            return enemiesDirection.normalized * -1;
        }
        else
        {
            return enemiesDirection.normalized;
        }
    }

    public  void calculateDirections(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector3 cohesionMove = Vector3.zero;
        friendsDirection = Vector3.zero;
        enemiesDirection = Vector3.zero;
        friendsStrength = 0f;
        enemiesStrength = 0f;

        int enemies = 0, friends = 0;

        float distanceSqr;
        foreach (Transform item in context)
        {
            FlockAgent itemAgent = item.GetComponent<FlockAgent>();
            if (itemAgent != null)
            {
                distanceSqr = Vector3.SqrMagnitude(item.position - agent.transform.position);
                if (itemAgent.AgentFlock == agent.AgentFlock)
                {
                    friendsDirection += item.position / distanceSqr;
                    friendsStrength += 1f / distanceSqr;
                    friends++;
                }
                else
                {
                    enemiesDirection += item.position / distanceSqr;
                    enemiesStrength += 1f / distanceSqr;
                    enemies++;
                }
            }
        }
        if(friends != 0)
        {
            friendsDirection /= friends;
            friendsDirection -= agent.transform.position;
        }
        if (enemies != 0)
        {
            enemiesDirection /= enemies;
            enemiesDirection -= agent.transform.position;
        }
    }
}
