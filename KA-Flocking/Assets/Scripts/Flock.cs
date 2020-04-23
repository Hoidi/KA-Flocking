using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

    // The lists of agent. Should only be modified via AddUnit and RemoveUnit
    public List<FlockAgent> agents = new List<FlockAgent>();
    // Public Read Only reference to the agent list.
    public HashSet<FlockAgent> deadUnits = new HashSet<FlockAgent>();
    public List<FlockAgent> newUnits = new List<FlockAgent>();
    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 50f)]
    public float neighbourRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;
    // All layers that will be checked for collisions, currently Troop & Obstacle
    int colliderLayers = (1 << 8) | (1<<10) ;
    public bool spawnedFirstCastle = false;

    [System.NonSerialized]
    public int moneyAmount = 50000;

    float squareMaxSpeed, squareNeighbourRadius, squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindObjectsOfType(this.GetType()).Length > 2) {
            Destroy (this.gameObject);
            return;
        }
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighbourRadius = neighbourRadius * neighbourRadius;
        squareAvoidanceRadius = avoidanceRadiusMultiplier * avoidanceRadiusMultiplier * squareNeighbourRadius;
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (FlockAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);
            agent.Attack(context, agent, this);

            Vector3 move = agent.unit.behaviour.CalculateMove(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
        }
        // Clears all dead units from the flock
        RemoveUnitsFromFlock();
        // Adds all new units to the flock
        AddUnitsToFlock();
    }

    // Returns a list of all nearby collider's transforms with the tag "Player"
    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();

        Collider[] contextColliders;
        
        contextColliders = Physics.OverlapSphere(agent.transform.position, neighbourRadius, colliderLayers);
        foreach (Collider c in contextColliders)
        {
            if (c != agent.agentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }

    // The preferred threadsafe way to create an agent
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
        newUnits.Add(newAgent);
    }

    public void StartSpawning(Castle castle, FlockAgent flockAgent, Flock flock) {
        StartCoroutine(castle.SpawningRoutine(flockAgent, flock));
    }

    private void AddUnitsToFlock() {
        foreach (FlockAgent newUnit in newUnits){ 
            agents.Add(newUnit);
        }
        newUnits.Clear();
    }

    // Adds the agent to the dead units, this will disable them at the end of the update()
    // Threadsafe and is the only way that an agent should be removed
    public void RemoveUnit(FlockAgent agent)
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        AudioSource audiouSource = agent.GetComponent<AudioSource>();
        if (audioManager != null && audiouSource != null)
        {
            audioManager.PlayDeathSFX(audiouSource);
        }
        deadUnits.Add(agent);
    }

    // Removes all agents from the flock that are in the list deadUnits. Mainly for threadsafety
    // Note: This can be effectivised by clearing the deadUnits list after function call.
    private void RemoveUnitsFromFlock()
    {
        agents.RemoveAll(deadUnits.Contains);
        foreach (FlockAgent deadUnit in deadUnits){ //for testing purposes, until we know what to do with dead units
            deadUnit.gameObject.SetActive(false); //make dead troops invisible
        }
    }
}

