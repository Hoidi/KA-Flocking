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
    public Unit unit;
    Collider agentCollider;
    Rigidbody rb;
    int animationCounter = 100;
    public Collider AgentCollider { get { return agentCollider; } }
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    public void Move(Vector3 acceleration)
    {
        if (rb == null)
        {
            stabiliseY();
            return;
        }

        rb.velocity += acceleration * Time.deltaTime * 7;

        if (rb.velocity != Vector3.zero)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = 0;
            transform.forward = velocity;
        }

        if (animationCounter < 100)
        {
            animationCounter++;
            stabiliseY();
            return;
        }
        float sqrVelocity = rb.velocity.sqrMagnitude;
        animator.ResetTrigger("idle");
        animator.ResetTrigger("slowWalk");
        animator.ResetTrigger("walk");
        animator.ResetTrigger("fastWalk");
        animator.ResetTrigger("slowRun");
        animator.ResetTrigger("run");
        animator.ResetTrigger("fastRun");
        animator.ResetTrigger("slowNarutoRun");
        animator.ResetTrigger("narutoRun");
        animator.ResetTrigger("fastNarutoRun");
        if (sqrVelocity >= 100)
        {
            animator.SetTrigger("fastNarutoRun");
        }
        else if (sqrVelocity >= 70)
        {
            animator.SetTrigger("narutoRun");
        }
        else if (sqrVelocity >= 48)
        {
            animator.SetTrigger("slowNarutoRun");
        }
        else if (sqrVelocity >= 25)
        {
            animator.SetTrigger("fastRun");
            Debug.Log("fastRun");
        }
        else if (sqrVelocity >= 14)
        {
            animator.SetTrigger("run");

            Debug.Log("run");
        }
        else if (sqrVelocity >= 9)
        {
            animator.SetTrigger("slowRun");

            Debug.Log("slowRun");
        }
        else if (sqrVelocity >= 6)
        {
            animator.SetTrigger("fastWalk");
        }
        else if (sqrVelocity >= 3)
        {
            animator.SetTrigger("walk");
        }
        else if (sqrVelocity >= 1.5)
        {
            animator.SetTrigger("slowWalk");
        }
        else
        {
            animator.SetTrigger("idle");
        }
        animationCounter = 0;
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
