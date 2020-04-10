﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Units/Infantry")]
public class Infantry : Unit
{

    public ContextFilter attackFilter;
    [Range(1f,1000f)]
    public float health = 100f;
    // The amount of damage this unit deals in one Time unit
    [Range(0f,1000f)]
    public float damage = 2f;
    // The reach of the unit, still limited by the neighbourradius
    [Range(0f,50f)]
    public float attackReach = 1f;

    // Returns if the target dies
    public override bool TakeDamage(float amount, FlockAgent agent) {
        amount *= Time.deltaTime;
        if (amount < health) {
            health -= amount;
            return false;
        } else {
            health = 0;
            agent.tag = "Dead";
            agent.AgentFlock.RemoveUnit(agent);
            return true;
        }
    }

    // Deal damage to the closest target from another flock within the unit's reach
    public override void Attack(List<Transform> targets, FlockAgent attacker, Flock flock) {
        targets = (attackFilter == null) ? targets : attackFilter.Filter(attacker, targets);
        if (targets.Count == 0) {
            return;
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
        if (closest != null) {
            if (closest.unit.TakeDamage(damage, closest)) flock.moneyAmount += 50;
        }
    }


}
