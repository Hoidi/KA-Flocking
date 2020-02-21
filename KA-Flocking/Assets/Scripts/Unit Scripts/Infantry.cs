using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Units/Infantry")]
public class Infantry : Unit
{

    public ContextFilter filter;
    [Range(1f,1000f)]
    public float health = 100f;
    // The amount of damage this unit deals in one Time unit
    [Range(0f,1000f)]
    public float damage = 2f;
    // The reach of the unit depending on the neighbourradius
    [Range(0f,1f)]
    public float reachMultiplier = 1f;

    public override void TakeDamage(float amount, FlockAgent agent) {
        amount *= Time.deltaTime;
        if (amount < health) {
            health -= amount;
        } else {
            health = 0;
            // Debug.Log(agent.name + " Died");
            agent.tag = "Dead";
            agent.AgentFlock.RemoveUnit(agent);
        }
    }

    // Deal damage to the closest target from another flock within the unit's reach
    public override void Attack(List<Transform> targets, FlockAgent attacker, Flock flock, float sqrReach) {
        targets = (filter == null) ? targets : filter.Filter(attacker, targets);
        if (targets.Count == 0) {
            return;
        }

        FlockAgent closest = targets[0].GetComponentInParent<FlockAgent>();
        float closestDistance = Vector3.SqrMagnitude(targets[0].position - attacker.transform.position);
        float sqrDistance;
        for (int i = 1; i < targets.Count; i++)
        {
            sqrDistance = Vector3.SqrMagnitude(targets[1].position - attacker.transform.position);
            if (sqrDistance < closestDistance && sqrDistance < sqrReach * reachMultiplier) {
                closest = targets[1].GetComponentInParent<FlockAgent>();
                closestDistance = sqrDistance;
            }
        }
        closest.unit.TakeDamage(damage, closest);
    }


}
