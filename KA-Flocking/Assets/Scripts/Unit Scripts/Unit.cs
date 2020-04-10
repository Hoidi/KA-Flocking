using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : ScriptableObject
{
    public FlockBehaviour behaviour;
    public abstract void Attack(List<Transform> targets, FlockAgent attacker, Flock flock);

    public abstract bool TakeDamage(float amount, FlockAgent agent);
}
