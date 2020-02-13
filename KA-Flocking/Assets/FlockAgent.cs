using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    public float stability = 0.3f;
    // The speed at which the rotation of the agents stabilises
    public float stabilisationSpeed = 2.0f;

    Collider agentCollider;
    Rigidbody rb;
    public Collider AgentCollider { get {return agentCollider;} }

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 velocity) {
        transform.position += velocity * Time.deltaTime;
        transform.forward = velocity;
    }

    void FixedUpdate () {
        Vector3 predictedUp = Quaternion.AngleAxis(
            rb.angularVelocity.magnitude * Mathf.Rad2Deg * stability / stabilisationSpeed,
            rb.angularVelocity
        ) * transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        rb.AddTorque(torqueVector * stabilisationSpeed * stabilisationSpeed);
    }
}
