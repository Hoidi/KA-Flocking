using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Units/Infantry")]
public class Infantry : Unit
{
    public override void Initialize()
    {
        attackMode = "meleeAttack";
        attackTime = 1.4f;
    }
}
