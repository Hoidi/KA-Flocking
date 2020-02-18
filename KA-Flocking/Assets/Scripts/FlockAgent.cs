using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    public Flock AgentFlock;
    public float stability = 0.3f;
    // The speed at which the rotation of the agents stabilises
    public float stabilisationSpeed = 2.0f;
    public Infantry infantry = new Infantry();

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
        if (velocity != Vector3.zero) {
            transform.forward = velocity;
        }
    }

    public void Initialize(Flock flock)
    {
        AgentFlock = flock;
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
