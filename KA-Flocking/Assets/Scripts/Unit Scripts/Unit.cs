using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : ScriptableObject
{
    public abstract void Initialize();

    public ContextFilter attackFilter;
    [Range(1f, 1000f)]
    public float health = 100f;
    // The amount of damage this unit deals in one Time unit
    [Range(0f, 1000f)]
    public float damage = 10f;
    // The reach of the unit, still limited by the neighbourradius
    [Range(0f, 50f)]
    public float attackReach = 4f;

    public FlockBehaviour behaviour;
    protected string attackMode = "";
    protected float attackTime = 0f;
    public Vector3 attackDirection = Vector3.zero;
    public bool Attack(List<Transform> targets, FlockAgent attacker, Flock flock)
    {
        targets = (attackFilter == null) ? targets : attackFilter.Filter(attacker, targets);
        if (targets.Count == 0)
        {
            return false;
        }

        FlockAgent closest = null;
        float closestDistance = float.MaxValue;
        float sqrDistance;
        for (int i = 0; i < targets.Count; i++)
        {
            // Only relevant for units with large colliders such as castles
            Vector3 closestPoint = targets[i].GetComponent<Collider>().ClosestPoint(attacker.transform.position);
            Vector3 direction = closestPoint - attacker.transform.position;
            sqrDistance = Vector3.SqrMagnitude(direction);
            if (sqrDistance < closestDistance && sqrDistance < attackReach)
            {
                closest = targets[i].GetComponentInParent<FlockAgent>();
                closestDistance = sqrDistance;
                attackDirection = direction;
                attackDirection.y = 0;
            }
        }

        if (closest != null)
        {
            closest.unit.TakeDamage(damage, closest);
            return true;
        }
        else 
            return false;
    }

    public void TakeDamage(float amount, FlockAgent agent)
    {
        if (amount < health)
        {
            health -= amount;
            MakeSound(agent);
        }
        else
        {
            health = 0;
            agent.tag = "Dead";
            agent.AgentFlock.RemoveUnit(agent);
        }
    }

    internal void MakeSound(FlockAgent agent)
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        AudioSource audioSource = agent.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioManager.PlayRandomSFX(audioSource);
        }
    }


    public string GetAttackMode()
    {
        return attackMode;
    }

    public float GetAttackTime()
    {
        return attackTime;
    }
}
