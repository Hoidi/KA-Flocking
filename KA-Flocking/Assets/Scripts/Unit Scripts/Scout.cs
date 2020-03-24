using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Units/Scout")]
public class Scout : Unit
{

    public ContextFilter attackFilter;
    [Range(1f,1000f)]
    public float health = 100f;
    // The amount of damage this unit deals in one Time unit
    [Range(0f,1000f)]
    public float damage = 2f;
    // The reach of the unit depending on the neighbourradius
    [Range(0f,100f)]
    public float attackReach = 1f;

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
            sqrDistance = Vector3.SqrMagnitude(targets[i].position - attacker.transform.position);
            if (sqrDistance < closestDistance && sqrDistance < attackReach) {
                closest = targets[i].GetComponentInParent<FlockAgent>();
                closestDistance = sqrDistance;
            }
        }
        if (closest != null) closest.unit.TakeDamage(damage, closest);
    }


}
/*
OBSERVATIONS ABOUT SCOUTS. TRYING TO GET A FEW SCOUTS TO BE ABLE TO LEAD MOST OF AN ARMY TO LONELY ENEMY TARGETS 
With a low neighbour radius ~3.5f the infantry will not follow the scouts enough.
Increasing the weight of the scout to 10x the amount barely helped (This will ofc )
Increasing the neighbour radius to 8f (and decreasing avoidance multiplier by half) helped a bit together with the increase in weight
    Seems that the infantry are not inclined enough to change direction to that of scouts, especially if there are external factors (altitude of map or enemies in conflicting directions)
    This might be because of the increased neighbour radius
Scouts tend to flock together mostly.
Current hypothesis. The most common reason for the flock to split of is because the scouts get too far away from the infantry and as such the infantry no longer follows them
This could perhaps be solved by increasing the cohesion factor of the scouts, further increase to the weight of the scouts, increasing the weight for infantry to align with scouts significantly
    Increased cohesion
        did however notice that it worked better with only 1 scout rather than 3, probably because scouts also had higher weights to go with scouts
        Trippling the alignment factor for scouts made it such that groups of ~5 infantry diverged from the army
    Increased alignment for infantry (1->3)
        made the groups larger ~10. Which is still insufficient
    Increasing the alignment for infantry to align with scouts, but not infantry and remove the weight of the added weight to scouts:
        By making the scouts have the same value as 20 infantry we got about 4 times as large groups (half of the entire army) to move away, however half of those eventually got too far away from the scouts
        Test to do the same thing with cohesion

*/