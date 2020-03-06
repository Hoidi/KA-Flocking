using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : ScriptableObject
{
    public abstract void Attack(List<Transform> targets, FlockAgent attacker, Flock flock);

    public abstract void TakeDamage(float amount, FlockAgent agent);
}
