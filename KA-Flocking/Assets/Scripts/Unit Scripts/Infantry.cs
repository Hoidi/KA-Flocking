using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Units/Infantry")]
public class Infantry : Unit
{

    public ContextFilter attackFilter;
    [Range(1f,1000f)]
    private float health = 100f;
    // The amount of damage this unit deals in one Time unit
    [Range(0f,1000f)]
    private readonly float damage = 10f;
    // The reach of the unit, still limited by the neighbourradius
    [Range(0f,50f)]
    private readonly float attackReach = 4f;

    public override void TakeDamage(float amount, FlockAgent agent) {
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
    public override bool Attack(List<Transform> targets, FlockAgent attacker, Flock flock) {
        targets = (attackFilter == null) ? targets : attackFilter.Filter(attacker, targets);
        if (targets.Count == 0) {
            return false;
        }

        FlockAgent closest = null;
        float closestDistance = float.MaxValue;
        float sqrDistance;
        for (int i = 0; i < targets.Count; i++)
        {
            // Only relevant for units with large colliders such as castles
            Vector3 closestPoint = targets[i].GetComponent<Collider>().ClosestPoint(attacker.transform.position);
            sqrDistance = Vector3.SqrMagnitude(closestPoint - attacker.transform.position);
            if (sqrDistance < closestDistance && sqrDistance < attackReach) {
                closest = targets[i].GetComponentInParent<FlockAgent>();
                closestDistance = sqrDistance;
            }
        }

        if (closest != null)
        {
            Vector3 heading = attacker.rb.velocity;
            Vector3 attackerPosition = attacker.rb.position;
            Vector3 closestPosition = closest.rb.position;

            float currentDistance = Vector3.Distance(attackerPosition, closestPosition);
            float nextDistance = Vector3.Distance(attackerPosition + heading, closestPosition);
            if (System.Math.Abs(currentDistance) > System.Math.Abs(nextDistance))
            {
                closest.unit.TakeDamage(damage, closest);
                return true;
            }
        }
        return false;
    }


}
