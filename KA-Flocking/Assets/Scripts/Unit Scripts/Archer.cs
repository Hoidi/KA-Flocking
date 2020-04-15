using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Units/Archer")]
public class Archer : Unit
{
    public override void Initialize()
    {
        attackMode = "gunAttack";
        attackTime = 1f;
    }
}
