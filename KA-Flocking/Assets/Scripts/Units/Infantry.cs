using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infantry
{
    [Range(1f,1000f)]
    public float health = 100f;
    // The amount of damage this unit deals in one Time unit
    [Range(0f,1000f)]
    public float damage = 2f;
    [Range(0f,100f)]
    public float range = 8f;

    public void TakeDamage(float amount, FlockAgent agent) {
        amount *= Time.deltaTime;
        if (amount < health) {
            Debug.Log(health);
            health -= amount;
        } else {
            health = 0;
            agent.tag = "Dead";
            agent.AgentFlock.agents.Remove(agent);
        }
    }

    public void Attack(List<Transform> targets, FlockAgent attacker) {
        if (targets.Count == 0) {
            return;
        }
        FlockAgent closest = targets[0].GetComponentInParent<FlockAgent>();
        float closestDistance = Vector3.SqrMagnitude(targets[0].position - attacker.transform.position);
        float sqrDistance;
        for (int i = 1; i < targets.Count; i++)
        {
            sqrDistance = Vector3.SqrMagnitude(targets[1].position - attacker.transform.position);
            if (sqrDistance < closestDistance) {
                closest = targets[1].GetComponentInParent<FlockAgent>();
                closestDistance = sqrDistance;
            }
        }
        closest.infantry.TakeDamage(damage, closest);
    }


}
