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
    public float speed = 20;
    public Unit unit;
    Collider agentCollider;
    Rigidbody rb;
    public Collider AgentCollider { get { return agentCollider; } }
    public Animator animator;
    int animationMode = 0;

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

        rb.velocity += acceleration * Time.deltaTime * 10;

        if (rb.velocity != Vector3.zero)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = 0;
            transform.forward = velocity;
        }

        float sqrVelocity = rb.velocity.sqrMagnitude;
        float[] sqrAnimationSpeeds = { 0, 1.5f, 3, 6, 9, 14, 25, 40, 60, 90,};
        int newAnimationMode = animationMode;

        //looks to see if the velocity has changed enough to enter a new range. 
        //also ads a buffer of 0.2 and 0.3 respectilvy stopping a unity from jumping between two animations to frequently. 
        for (int i = sqrAnimationSpeeds.Length-1; i >= 0; i--)
        {
            if (animationMode > i && sqrAnimationSpeeds[i+1] * 0.8f > sqrVelocity)
            {
                newAnimationMode = i;
                break;
            }
            else if (animationMode < i && sqrAnimationSpeeds[i] * 1.3f < sqrVelocity)
            {
                newAnimationMode = i;
                break;
            }
        }

        if(newAnimationMode == animationMode)
        {
            stabiliseY();
            return;
        }
        else
        {
            animationMode = newAnimationMode;
        }

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

        switch (animationMode)
        {
            case 0:
                animator.SetTrigger("idle");
                break;
            case 1:
                animator.SetTrigger("slowWalk");
                break;
            case 2:
                animator.SetTrigger("walk");
                break;
            case 3:
                animator.SetTrigger("fastWalk");
                break;
            case 4:
                animator.SetTrigger("slowRun");
                break;
            case 5:
                animator.SetTrigger("run");
                break;
            case 6:
                animator.SetTrigger("fastRun");
                break;
            case 7:
                animator.SetTrigger("slowNarutoRun");
                break;
            case 8:
                animator.SetTrigger("narutoRun");
                break;
            case 9:
                animator.SetTrigger("fastNarutorun");
                break;
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
