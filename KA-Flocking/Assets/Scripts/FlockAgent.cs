﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    public Flock AgentFlock;
    public float stability = 0.3f;
    // The speed at which the rotation of the agents stabilises
    public float stabilisationSpeed = 2.0f;
    public Unit unit;
    Collider agentCollider;
    Rigidbody rb;
    public Collider AgentCollider { get { return agentCollider; } }

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 acceleration)
    {
        if(rb != null)
        {
            rb.velocity += acceleration * Time.deltaTime * 3;

            if (rb.velocity != Vector3.zero)
            {
                Vector3 velocity = rb.velocity;
                velocity.y = 0;
                transform.forward = velocity;
            }
        }
        stabiliseY();
    }

    public void Initialize(Flock flock, Unit unitType)
    {
        AgentFlock = flock;
        unit = Instantiate(unitType);
    }

    void stabiliseY()
    {
        Vector3 predictedUp = Quaternion.AngleAxis(
            rb.angularVelocity.magnitude * Mathf.Rad2Deg * stability / stabilisationSpeed,
            rb.angularVelocity
        ) * transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        rb.AddTorque(torqueVector * stabilisationSpeed * stabilisationSpeed);
    }
}
