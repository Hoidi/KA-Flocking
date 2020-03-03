using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/FightOrFlight")]
public class FightOrFlightBehaviour : FlockBehaviour
{
    //SteeredCohesionBehaviour friendlyCohesionBehaviour = CreateInstance<SteeredCohesionBehaviour>();
    //SteeredCohesionBehaviour enemyCohesionBehaviour = CreateInstance<SteeredCohesionBehaviour>();
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //enemyCohesionBehaviour.filter= CreateInstance<OtherFlockFilter>();
        //friendlyCohesionBehaviour.filter = CreateInstance<SameFlockFilter>();
        if (context.Count == 0)
        {
            return Vector3.zero;
        }
        //Vector3 enemyVector = enemyCohesionBehaviour.CalculateMove(agent, context, flock);
        //Vector3 friendlyVector = friendlyCohesionBehaviour.CalculateMove(agent, context, flock);
        Vector3 friendlyVector = calculateDirection(agent, context, flock, false);
        Vector3 enemyVector = calculateDirection(agent, context, flock, true);

        if (enemyVector.magnitude > friendlyVector.magnitude)
        {
            Debug.Log("Im running away");
            return enemyVector * -10;
        }
        else
        {
            Debug.Log("Im attacking!!!");
            return enemyVector;
        }
    }

    //stolen from cohension class
    public  Vector3 calculateDirection(FlockAgent agent, List<Transform> context, Flock flock, bool friend)
    {
        if (context.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 cohesionMove = Vector3.zero;

        float distanceSqr;
        List<Transform> filteredContext = filter(agent, context, friend);  
        foreach (Transform item in filteredContext)
        {
            distanceSqr = Vector3.SqrMagnitude(item.position - agent.transform.position);
            cohesionMove += item.position / distanceSqr;
        }
        cohesionMove /= context.Count;
        cohesionMove -= agent.transform.position;
        return cohesionMove;
    }

    //stolen from filter class 
    public List<Transform> filter(FlockAgent agent, List<Transform> original,bool friends)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (var item in original)
        {
            FlockAgent itemAgent = item.GetComponent<FlockAgent>();
            if (itemAgent != null && ((itemAgent.AgentFlock != agent.AgentFlock)^friends))
            {
                filtered.Add(itemAgent.transform);
            }
        }
        return filtered;
    }
}
