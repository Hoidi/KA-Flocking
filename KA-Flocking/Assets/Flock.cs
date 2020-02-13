using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

    public FlockAgent agentPrefab;
    const float agentDensity = 0.8f;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehaviour behaviour;

    [Range(10,1000)]
    public int startingAmount = 50;
    [Range(1f,100f)]
    public float driveFactor = 10f;
    [Range(1f,100f)]
    public float maxSpeed = 5f;
    [Range(1f,10f)]
    public float neighbourRadius = 1.5f;
    [Range(0f,1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed,squareNeighbourRadius,squareAvoidanceRadius;
    public float SquareAvoidanceRadius {get {return squareAvoidanceRadius;}}
    // Start is called before the first frame update
    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighbourRadius = neighbourRadius * neighbourRadius;
        squareAvoidanceRadius = avoidanceRadiusMultiplier * avoidanceRadiusMultiplier * squareNeighbourRadius;

        for (int i = 0; i < startingAmount; i++) {
            Vector3 location = Random.insideUnitSphere * startingAmount;
            location.y = 0;
        
            FlockAgent newAgent = Instantiate (
                agentPrefab,
                location,
                Quaternion.Euler((Vector3.up * Random.Range(0f,360f))),
                transform
            );
            newAgent.name = "Agent " + i;
            agents.Add(newAgent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (FlockAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);

            Vector3 move = behaviour.CalculateMove(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed) {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
        }
    }

    List<Transform> GetNearbyObjects(FlockAgent agent) {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighbourRadius);
        
        foreach (Collider c in contextColliders)
        {
            if (c != agent.AgentCollider && c.CompareTag("Player")) {
                context.Add(c.transform);
            }
        }
        return context;
    }
}
