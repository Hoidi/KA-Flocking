﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : ScriptableObject
{
    public FlockBehaviour behaviour;
    public abstract bool Attack(List<Transform> targets, FlockAgent attacker, Flock flock);

    public abstract void TakeDamage(float amount, FlockAgent agent);
}
