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
    public Rigidbody rb;
    public Collider AgentCollider { get { return agentCollider; } }
    public Animator animator;
    bool attacking = false;
    Vector3 attackAlignment = Vector3.zero;
    float attackCountDown = 0f;
    public float dragCoefficient = 0.1f;
    int animationMode = 0;
    FightOrFlightBehaviour FOFH;


    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        FOFH = ScriptableObject.CreateInstance<FightOrFlightBehaviour>();
    }

    public void Move(Vector3 acceleration)
    {
        if (rb == null)
        {
            StabiliseY();
            return;
        }
        if (attacking)
        {
            attackCountDown -= Time.deltaTime;
            rb.velocity = Vector3.zero;
            if (attackCountDown <= 0) attacking = false;
            transform.forward = attackAlignment;
            StabiliseY();
            return;
        }

        acceleration.y = 0f;
        //Debug.Log(acceleration.ToString());
        rb.velocity += acceleration * Time.deltaTime * 10;
        //rb.velocity += HorizontalDrag(rb.velocity); for some reason this breaks everything

        if (rb.velocity != Vector3.zero)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = 0;
            transform.forward = velocity;
        }

        float sqrVelocity = rb.velocity.sqrMagnitude;
        float[] sqrAnimationSpeeds = { 0, 1.3f, 2.5f, 4.5f, 8, 14, 23, 35, 60, 90, };
        int newAnimationMode = animationMode;

        //looks to see if the velocity has changed enough to enter a new range. 
        //also ads a buffer of 0.2 and 0.3 respectilvy stopping a unity from jumping between two animations to frequently. 
        for (int i = sqrAnimationSpeeds.Length - 1; i >= 0; i--)
        {
            if (animationMode > i && sqrAnimationSpeeds[i + 1] * 0.8f > sqrVelocity)
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

        if (newAnimationMode == animationMode)
        {
            StabiliseY();
            return;
        }
        else
        {
            animationMode = newAnimationMode;
        }

        ResetAnimations();

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
                animator.SetTrigger("fastNarutoRun");
                break;
        }
        StabiliseY();
    }

    public void Initialize(Flock flock, Unit unitType)
    {
        AgentFlock = flock;
        unit = Instantiate(unitType);
    }

    void StabiliseY()
    {
        Vector3 predictedUp = Quaternion.AngleAxis(
            rb.angularVelocity.magnitude * Mathf.Rad2Deg * stability / stabilisationSpeed,
            rb.angularVelocity
        ) * transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        rb.AddTorque(torqueVector * stabilisationSpeed * stabilisationSpeed);
    }

    Vector3 HorizontalDrag(Vector3 speed)
    {
        Vector3 horizontalSpeed = speed;
        horizontalSpeed.y = 0f;
        return -horizontalSpeed.normalized * horizontalSpeed.sqrMagnitude * 0.1f;

    }

    public void Attack(List<Transform> targets, FlockAgent attacker, Flock flock)
    {
        if (FOFH != null)
        {
            FOFH.CalculateMove(attacker, targets, flock); //it´s ugly. But what is a coder to do when behaviour objects are involved....
            if (attacking == false && FOFH.isAttacking())
            {
                attacking = unit.Attack(targets, attacker, flock);
                if (attacking)
                {
                    attackCountDown = 1.4f;
                    ResetAnimations();
                    animator.SetTrigger("meleeAttack");

                    Vector3 velocity = rb.velocity;
                    velocity.y = 0;
                    attackAlignment = velocity;  //for making the character still stand up right while fighting. 
                }
            }
        }
    }

    void ResetAnimations()
    {
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
        animator.ResetTrigger("meleeAttack");
    }
}
