using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

    public FlockAgent agentPrefab;
    const float agentDensity = 0.8f;
    // The lists of agent. Should only be modified via AddUnit and RemoveUnit
    private List<FlockAgent> _agents = new List<FlockAgent>();
    // Public Read Only reference to the agent list. 
    public IList<FlockAgent> agents { get { return _agents.AsReadOnly(); } }
    public Unit defaultInfantryObject;
    private HashSet<FlockAgent> deadUnits = new HashSet<FlockAgent>();
    public FlockBehaviour behaviour;
    [Range(10, 1000)]
    public int startingAmount = 50;
    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 50f)]
    public float neighbourRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed, squareNeighbourRadius, squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    public int CountDeadUnits{ get { return deadUnits.Count; } }
    // Start is called before the first frame update
    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighbourRadius = neighbourRadius * neighbourRadius;
        squareAvoidanceRadius = avoidanceRadiusMultiplier * avoidanceRadiusMultiplier * squareNeighbourRadius;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (FlockAgent agent in _agents)
        {
            List<Transform> context = GetNearbyObjects(agent);
            agent.unit.Attack(context, agent, this);

            Vector3 move = behaviour.CalculateMove(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
        }
        // Clears all dead units from the flock
        RemoveUnitsFromFlock();
    }

    // Returns a list of all nearby collider's transforms with the tag "Player"
    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighbourRadius);

        foreach (Collider c in contextColliders)
        {
            if ((c != agent.AgentCollider && c.CompareTag("Player")) || c.CompareTag("Obstacle"))
            {
                context.Add(c.transform);
            }
        }
        return context;
    }

    // The preferred way to create an agent
    public void CreateUnit(FlockAgent prefab, Vector3 location, Quaternion rotation, Unit unitType) {
        FlockAgent newAgent = Instantiate(
            prefab,   // The prefab of the new agent, should correspond to the unitType
            location, // The location of the new agent
            rotation, // The rotation of the new agent
            transform // Decides the hierarchy in the scene (Flock > Agents)
        );
        // Sets the name in the scene
        newAgent.name = unitType + " " + (agents.Count + deadUnits.Count + 1);
        // Calls the agent's initialize function
        newAgent.Initialize(this, unitType);
        // Adds the agent to the list of agents
        _agents.Add(newAgent);
    }

    // Adds the agent to the dead units, this will disable them at the end of the update()
    // Threadsafe and is the only way that an agent should be removed
    public void RemoveUnit(FlockAgent agent)
    {
        deadUnits.Add(agent);
    }

    // Removes all agents from the flock that are in the list deadUnits. Mainly for threadsafety
    // Note: This can be effectivised by clearing the deadUnits list after function call.
    private void RemoveUnitsFromFlock()
    {
        _agents.RemoveAll(deadUnits.Contains);
        foreach (FlockAgent deadUnit in deadUnits){ //for testing purposes, until we know what to do with dead units
            deadUnit.gameObject.SetActive(false); //make dead troops invisible
        }
        
    }
}

